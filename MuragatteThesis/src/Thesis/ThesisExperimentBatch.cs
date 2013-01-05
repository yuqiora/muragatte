// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
//
// Copyright (C) 2013  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Environment.Agents;
using Muragatte.Core.Storage;
using Muragatte.Random;
using Muragatte.Research;
using Muragatte.Visual;
using Muragatte.Visual.Styles;

namespace Muragatte.Thesis
{
    public class ThesisExperimentBatch : ExperimentBatch
    {
        #region Constants

        private const double SNAPSHOT_SCALE = 10;
        private const byte SNAPSHOT_ALPHA = 64;

        private const double GUIDE_PART = 0.2;
        private const int INTRUDER_MAX = 1;

        private const double REPULSION_RANGE = 1.5;
        private const double SPEED = 1;

        private const double ASSERTIVENESS_LOW = 0.1;
        private const double ASSERTIVENESS_MEDIUM = 0.35;
        private const double ASSERTIVENESS_HIGH = 0.85;
        private const double CREDIBILITY_NORMAL = 1;
        private const double CREDIBILITY_HIGH = 2;
        private const double CREDIBILITY_VERYHIGH = 5;

        private static readonly Angle TURNING_ANGLE = new Angle(115);

        private static readonly double[] ASSERTIVENESS = { ASSERTIVENESS_LOW, ASSERTIVENESS_MEDIUM, ASSERTIVENESS_HIGH };
        private static readonly double[] CREDIBILITY = { CREDIBILITY_NORMAL, CREDIBILITY_HIGH, CREDIBILITY_VERYHIGH };

        #endregion

        #region Fields

        private double _dFovRange = 1;
        private Angle _fovAngle = new Angle(180);
        private readonly Dictionary<double, char> _assertivenessSymbol = new Dictionary<double, char>();
        private readonly Dictionary<double, char> _credibilitySymbol = new Dictionary<double, char>();

        #endregion

        #region Constructors

        public ThesisExperimentBatch(string path, int count, int runs, int length, double timePerStep,
            ObservableCollection<Style> styles, SpeciesCollection species, Scene scene,
            double fovRange, double fovAngle)
            : base(path, count, runs, length, timePerStep, styles, species, scene)
        {
            _dFovRange = fovRange;
            _fovAngle = new Angle(fovAngle);
            _assertivenessSymbol.Add(ASSERTIVENESS_LOW, 'l');
            _assertivenessSymbol.Add(ASSERTIVENESS_MEDIUM, 'm');
            _assertivenessSymbol.Add(ASSERTIVENESS_HIGH, 'h');
            _credibilitySymbol.Add(CREDIBILITY_NORMAL, 'n');
            _credibilitySymbol.Add(CREDIBILITY_HIGH, 'h');
            _credibilitySymbol.Add(CREDIBILITY_VERYHIGH, 'v');
        }

        #endregion

        #region Properties

        private SpawnSpot StartSpawn
        {
            get { return _scene.SpawnSpots[0]; }
        }

        private Goal GoalG
        {
            get { return (Goal)_scene.StationaryElements[0]; }
        }

        private Goal GoalI
        {
            get { return (Goal)_scene.StationaryElements[1]; }
        }

        #endregion

        #region Methods

        protected override void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //naive only reference (1)
            //guided reference (3-assertiveness)
            //no credibility (6 = ll + lm + lh + mm + mh + hh)
            //with credibility (24 = 6-noCred * 2-credGuide * 2-credIntruder)
            int batchSize = 1 + 3 + 6 * (2 * 2 + 1);
            ExperimentBatchProgress progress = new ExperimentBatchProgress(batchSize, _iRuns, _iLength);
            //naives only reference
            Experiment x = CreateExperiment(0, 0, 0, 0, 0, 0);
            RunExperimentAsync(sender, e, x, progress);
            for (int ag = 0; ag < ASSERTIVENESS.Length; ag++)
            {
                //guided reference
                x = CreateExperiment((int)(_iCount * GUIDE_PART), 0, ASSERTIVENESS[ag], 0, CREDIBILITY_NORMAL, 0);
                RunExperimentAsync(sender, e, x, progress);
                for (int ai = ag; ai < ASSERTIVENESS.Length; ai++)
                {
                    //no credibility
                    //with credibility
                    for (int cg = 0; cg < CREDIBILITY.Length - 1; cg++)
                    {
                        for (int ci = cg; ci < CREDIBILITY.Length; ci++)
                        {
                            x = CreateExperiment((int)(_iCount * GUIDE_PART), INTRUDER_MAX,
                                ASSERTIVENESS[ag], ASSERTIVENESS[ai], CREDIBILITY[cg], CREDIBILITY[ci]);
                            RunExperimentAsync(sender, e, x, progress);
                        }
                    }
                }
            }
        }

        private void RunExperimentAsync(object sender, DoWorkEventArgs e, Experiment x, ExperimentBatchProgress progress)
        {
            if (x.Status == ExperimentStatus.Canceled) x.Reset();
            if (x.Status == ExperimentStatus.Ready)
            {
                progress.Name = x.Name;
                _worker.ReportProgress(0, progress);
                x.PreProcessing();
                for (int i = 0; i < x.RepeatCount; i++)
                {
                    if (_worker.CancellationPending)
                    {
                        e.Cancel = true;
                        x.Cancel();
                        break;
                    }
                    x.Instances[i].RunAsync(_worker, progress);
                }
                x.PostProcessing();
                _toSave.Enqueue(x);
                //batch results
                progress.UpdateExperiment(progress.Experiment + 1);
                _worker.ReportProgress(0, progress);
            }
        }

        private Experiment CreateExperiment(int nG, int nI, double assertG, double assertI, double credG, double credI)
        {
            string name = string.Format("MTE_{0}_{1}-{2}-{3}_{4}", _iCount, _iCount - nG - nI, SubgroupInfo(nG, assertG, credG),
                SubgroupInfo(nI, assertI, credI), _fovAngle.DegreesI);
            InstanceDefinition idef = new InstanceDefinition(_dTimePerStep, _iLength, false, _scene, _species,
                StorageOptions.SimpleBruteForce, CreateAgents(nG, nI, assertG, assertI, credG, credI));
            Experiment e = new Experiment(name, _iRuns, idef, _styles, _random.UInt());
            e.ExtraSetting.Path = _sPathCompleted;
            //e.ExtraSetting.Compression = Ionic.Zlib.CompressionLevel.BestCompression;
            return e;
        }

        private string SubgroupInfo(int count, double assert, double cred)
        {
            return count == 0 ? count.ToString() : string.Format("{0}{1}{2}", count, _assertivenessSymbol[assert], _credibilitySymbol[cred]);
        }

        private IEnumerable<ObservedArchetype> CreateAgents(int nG, int nI, double assertG, double assertI, double credG, double credI)
        {
            List<ObservedArchetype> oas = new List<ObservedArchetype>();
            oas.Add(CreateAgents("Naives", _iCount - nG - nI, GetSpecies("Naive"), null, ASSERTIVENESS_LOW, CREDIBILITY_NORMAL));
            if (nG > 0) oas.Add(CreateAgents("Guides", nG, GetSpecies("Guide"), GoalG, assertG, credG));
            if (nI > 0) oas.Add(CreateAgents("Intruders", nI, GetSpecies("Intruder"), GoalI, assertI, credI));
            return oas;
        }

        private ObservedArchetype CreateAgents(string name, int count, Species species, Goal goal, double assertiveness, double credibility)
        {
            return new ObservedArchetype(new Vejmola2013AgentArchetype(name, count, StartSpawn,
                new NoisedDouble(Distribution.Uniform, -180, 180), new NoisedDouble(1),
                species, new Neighbourhood(_dFovRange, _fovAngle), TURNING_ANGLE,
                new Vejmola2013AgentArgs(goal, new Neighbourhood(REPULSION_RANGE, _fovAngle), assertiveness, credibility)));
        }

        private Species GetSpecies(string name)
        {
            return _species.DefaultForAgents.Children.FirstOrDefault(s => s.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        protected override void SaveNext(Experiment e)
        {
            base.SaveNext(e);
            File.Copy(Path.Combine(_sPathCompleted, e.Name, "Settings.xml"), Path.Combine(_sPathExperiments, e.Name + ".xml"));
        }

        protected override void TakeSnapshot(Experiment e)
        {
            //leaves something in memory upon closing, don't know what
            LayeredSnapshot ls = new LayeredSnapshot(new Visualization(e.Instances[0].Model, _scene.Region.Width, _scene.Region.Height, SNAPSHOT_SCALE, null, _styles));
            ls.Scale = SNAPSHOT_SCALE;
            ls.DrawFlipped = true;
            ls.UseCustomVisuals = true;
            ls.IsEnvironmentEnabled = true;
            ls.IsAgentsEnabled = true;
            ls.IsNeighbourhoodsEnabled = true;
            ls.IsTracksEnabled = true;
            ls.Tracks.Add(_species.DefaultForAgents.FullName);
            //async would be better, cannot be used right now (collectionview problem)
            //options:
            //- let it be as it is, GUI freezing
            //- find a workaround, too much time and effort?
            ls.Redraw(e.GetHistories(), _iLength, SNAPSHOT_ALPHA);
            ls.SetVisualization(null);
            _snapshots.Save(Path.Combine(_sPathSnapshots, e.Name + ".png"), ls.Image);
        }

        #endregion
    }
}

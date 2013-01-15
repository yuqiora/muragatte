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
using Muragatte.Core.Storage;
using Muragatte.IO;
using Muragatte.Research;
using Muragatte.Research.IO;
using Muragatte.Research.Results;
using Muragatte.Visual;
using Muragatte.Visual.Styles;

namespace Muragatte.Thesis
{
    public class ThesisExperimentBatch : ExperimentBatch
    {
        #region Constants

        private const double SNAPSHOT_SCALE = 5;
        private const byte SNAPSHOT_ALPHA = 96;

        private const int GUIDE_COUNT = 5;
        private const int INTRUDER_COUNT = 1;

        private const double ASSERTIVENESS_LOW = 0.1;
        private const double ASSERTIVENESS_MEDIUM = 0.5;
        private const double ASSERTIVENESS_HIGH = 1;
        private const double ASSERTIVENESS_VERYHIGH = 2;
        private const double CREDIBILITY_NORMAL = 1;
        private const double CREDIBILITY_HIGH = 2;
        private const double CREDIBILITY_VERYHIGH = 5;
        private const double CREDIBILITY_EXTRAHIGH = 20;

        private static readonly double[] ASSERTIVENESS = { ASSERTIVENESS_MEDIUM, ASSERTIVENESS_HIGH, ASSERTIVENESS_VERYHIGH };
        private static readonly double[] CREDIBILITY = { CREDIBILITY_NORMAL, CREDIBILITY_HIGH, CREDIBILITY_VERYHIGH, CREDIBILITY_EXTRAHIGH };

        #endregion

        #region Fields

        private double _dFovRange = 1;
        private Angle _fovAngle = new Angle(180);
        private readonly Dictionary<double, char> _assertivenessSymbol = new Dictionary<double, char>();
        private readonly Dictionary<double, char> _credibilitySymbol = new Dictionary<double, char>();

        private XmlLoadSave<XmlExperimentRoot> _xml = new XmlLoadSave<XmlExperimentRoot>();

        #endregion

        #region Constructors

        public ThesisExperimentBatch(string path, int count, int runs, int length, double timePerStep,
            ObservableCollection<Style> styles, SpeciesCollection species, Scene scene,
            double fovRange, double fovAngle)
            : base(path, count, runs, length, timePerStep, styles, species, scene)
        {
            _dFovRange = fovRange;
            _fovAngle = new Angle(fovAngle);
            _assertivenessSymbol.Add(ASSERTIVENESS_LOW, 'L');
            _assertivenessSymbol.Add(ASSERTIVENESS_MEDIUM, 'M');
            _assertivenessSymbol.Add(ASSERTIVENESS_HIGH, 'H');
            _assertivenessSymbol.Add(ASSERTIVENESS_VERYHIGH, 'V');
            _credibilitySymbol.Add(CREDIBILITY_NORMAL, 'N');
            _credibilitySymbol.Add(CREDIBILITY_HIGH, 'H');
            _credibilitySymbol.Add(CREDIBILITY_VERYHIGH, 'V');
            _credibilitySymbol.Add(CREDIBILITY_EXTRAHIGH, 'X');
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
            PrepareWriter(!File.Exists(_sPath));
            //naive only reference (1)
            //guided reference (1)
            //with intruder (12 = 3-assertiveness * 4-credibility)
            int batchSize = 1 + 1 + 3 * 4;
            ExperimentBatchProgress progress = new ExperimentBatchProgress(batchSize, _iRuns, _iLength);
            //naives only reference
            RunExperimentAsync(sender, e, CreateExperiment(0, 0, 0, 0), progress);
            if (!_worker.CancellationPending)
            {
                //guided reference
                RunExperimentAsync(sender, e, CreateExperiment(GUIDE_COUNT, 0, 0, 0), progress);
                //with intruder
                for (int ai = 0; ai < ASSERTIVENESS.Length; ai++)
                {
                    for (int ci = 0; ci < CREDIBILITY.Length; ci++)
                    {
                        if (_worker.CancellationPending) break;
                        RunExperimentAsync(sender, e, CreateExperiment(GUIDE_COUNT, INTRUDER_COUNT, ASSERTIVENESS[ai], CREDIBILITY[ci]), progress);
                    }
                }
            }
            _writer.Close();
        }

        private void RunExperimentAsync(object sender, DoWorkEventArgs e, ThesisExperimentPack x, ExperimentBatchProgress progress)
        {
            progress.Name = x.Experiment.Name;
            _worker.ReportProgress(0, progress);
            x.Experiment.PreProcessing();
            for (int i = 0; i < _iRuns; i++)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    x.Cancel(i);
                    break;
                }
                x.Experiment.Instances[i].RunAsync(_worker, progress);
            }
            x.PostProcessing();
            _toSave.Enqueue(x.Experiment);
            ProcessResults(x);
            progress.UpdateExperiment(progress.Experiment + 1);
            _worker.ReportProgress(0, progress);
        }

        private ThesisExperimentPack CreateExperiment(int nG, int nI, double assertI, double credI)
        {
            int nN = _iCount - nG - nI;
            string name = string.Format("MTE_{0}_{1}-{2}-{3}", _iCount, nN, nG, SubgroupInfo(nI, assertI, credI));
            return new ThesisExperimentPack(name, _iRuns, _iLength, _dTimePerStep, _scene, _species, _styles, _random.UInt(), _sPathCompleted,
                nN, nG, nI, ASSERTIVENESS_LOW, ASSERTIVENESS_MEDIUM, assertI, CREDIBILITY_NORMAL, CREDIBILITY_NORMAL, credI, StartSpawn,
                GoalG, GoalI, GetSpecies("Naive"), GetSpecies("Guide"), GetSpecies("Intruder"), _dFovRange, _fovAngle);
        }

        private string SubgroupInfo(int count, double assert, double cred)
        {
            return count == 0 ? count.ToString() : string.Format("{0}{1}{2}", count, _assertivenessSymbol[assert], _credibilitySymbol[cred].ToString());
        }

        private Species GetSpecies(string name)
        {
            return _species.DefaultForAgents.Children.FirstOrDefault(s => s.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        protected override void SaveNext(Experiment e)
        {
            //base.SaveNext(e);
            //File.Copy(Path.Combine(_sPathCompleted, e.Name, Research.IO.CompletedExperimentArchiver.SETTINGS_FILENAME), Path.Combine(_sPathExperiments, e.Name + ".xml"));
            //_sTempExpName = e.Name;
            _xml.Save(Path.Combine(_sPathExperiments, e.Name + ".xml"), new XmlExperimentRoot(e));
            TakeSnapshot(e);
        }

        protected override void TakeSnapshot(Experiment e)
        {
            //leaves something in memory upon closing, don't know what
            LayeredSnapshot ls = new LayeredSnapshot(new Visualization(e.Instances[0].Model, _scene.Region.Width, _scene.Region.Height, SNAPSHOT_SCALE, null, _styles));
            ls.Scale = SNAPSHOT_SCALE;
            ls.DrawFlipped = false;
            ls.UseCustomVisuals = true;
            ls.IsEnvironmentEnabled = true;
            ls.IsAgentsEnabled = true;
            ls.IsNeighbourhoodsEnabled = true;
            ls.IsTracksEnabled = true;
            ls.Tracks.Add(_species.DefaultForAgents.FullName);
            ls.IsTrailsEnabled = false;
            //async would be better, cannot be used right now (collectionview problem)
            //options:
            //- let it be as it is, GUI freezing
            //- find a workaround, too much time and effort?
            //Async version would need more work and time to do right, current implementation wrong and not sufficient
            ls.Redraw(e.GetHistories(), _iLength, SNAPSHOT_ALPHA);
            ls.SetVisualization(null);
            _snapshots.Save(Path.Combine(_sPathSnapshots, e.Name + ".png"), ls.Image);
        }

        private void ProcessResults(ThesisExperimentPack e)
        {
            foreach (Instance i in e.Experiment.Instances)
            {
                if (!i.IsComplete) continue;
                double ToTG = 0;
                double ToTI = 0;
                int sizeG = 0;
                int sizeI = 0;
                double distI = 0;
                InstanceResults r = i.Results;
                if (e.GuideCount > 0)
                {
                    sizeG = r.Observed[0].MajorityGroupSizeEnd;
                    if (e.IntruderCount > 0)
                    {
                        sizeI = r.Observed[1].MajorityGroupSizeEnd;
                        distI = NormalizeDistance(r.Observed[1].GoalEndDistanceMinimum.Value);
                    }
                }
                ProcessResultsGoals(e, r, i.Model.History.Last, ref ToTG, ref ToTI);
                _writer.WriteLine(string.Join("\t", i.Number, _iCount, e.NaiveCount, e.GuideCount, e.IntruderCount,
                    e.AssertivenessIntruder, e.CredibilityIntruder, r.GroupCountEnd, r.StrayCountEnd,
                    r.MainGroupSizeEnd, sizeG, sizeI, ToTG.ToString("F3"), ToTI.ToString("F3"), distI.ToString("F3")));
            }
        }

        private double NormalizeDistance(double value)
        {
            value = value - GoalG.Radius - _dFovRange;
            return value < 0 ? 0 : value;
        }

        private void ProcessResultsGoals(ThesisExperimentPack e, InstanceResults r, HistoryRecord h, ref double ToTG, ref double ToTI)
        {
            double minDistG = double.MaxValue;
            double minDistI = double.MaxValue;
            foreach (Agent a in r.StepDetails.Last().MainGroup.Members)
            {
                double distG = Vector2.Distance(GoalG.Position, h[a.ID].Position);
                if (distG < minDistG) minDistG = distG;
                double distI = Vector2.Distance(GoalI.Position, h[a.ID].Position);
                if (distI < minDistI) minDistI = distI;
            }
            ToTG = NormalizeDistance(minDistG);
            ToTI = NormalizeDistance(minDistI);
        }

        protected override void WriteBatchResultsHeader()
        {
            /*
             * Run - run in experiment
             * N - # of all agents
             * N.n - # of naive agents
             * N.g - # of guides
             * N.i - # of intruders
             * Assert - assertiveness of intruders
             * Cred - credibility of intruders
             * Groups - # of groups
             * Strays - # of stray agents
             * Size - end size of main group
             * Size.g - end size of group with most guides
             * Size.i - end size of group with intruder
             * ToTG - min distance between main group and guide goal
             * ToTI - min distance between main group and intruder goal
             * ToTI.i - distance between intruder and its goal
             */
            _writer.WriteLine(string.Join("\t", "Run", "N", "N.n", "N.g", "N.i", "Assert", "Cred",
                "Groups", "Strays", "Size", "Size.g", "Size.i", "ToTG", "ToTI", "ToTI.i"));
        }

        #endregion
    }
}

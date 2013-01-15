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

        private const double GUIDE_PART = 0.2;
        private const int GUIDE_COUNT = 5;
        private const int INTRUDER_COUNT = 1;

        private const double ASSERTIVENESS_LOW = 0.1;
        private const double ASSERTIVENESS_MEDIUM = 0.35;
        private const double ASSERTIVENESS_HIGH = 0.85;
        private const double ASSERTIVENESS_VERYHIGH = 2;
        private const double CREDIBILITY_NORMAL = 1;
        private const double CREDIBILITY_HIGH = 2;
        private const double CREDIBILITY_VERYHIGH = 5;
        private const double CREDIBILITY_EXTRAHIGH = 20;

        private static readonly double[] ASSERTIVENESS = { ASSERTIVENESS_LOW, ASSERTIVENESS_MEDIUM, ASSERTIVENESS_HIGH, ASSERTIVENESS_VERYHIGH };
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

        private int GuideCount
        {
            get { return (int)(_iCount * GUIDE_PART); }
        }

        private int NaiveCount
        {
            get { return _iCount - GuideCount - INTRUDER_COUNT; }
        }

        #endregion

        #region Methods

        protected override void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            PrepareWriter(!File.Exists(_sPath));
            #region MODE 2
            ////naive only reference (1)
            ////guided reference (3-assertiveness)
            ////no credibility [assert_g <= assert_i] (9 = LL + LM + LH + LV + MM + MH + MV + HH + HV)
            ////with credibility [cred_g = const; cred_i = H | V | X] (27 = 9-noCred * 3)
            //int batchSize = 1 + 3 + (4 + 3 + 2) * 4;
            //ExperimentBatchProgress progress = new ExperimentBatchProgress(batchSize, _iRuns, _iLength);
            ////naives only reference
            //RunExperimentAsync(sender, e, CreateExperiment(0, 0, 0, 0, 0, 0), progress);
            //for (int ag = 0; ag < ASSERTIVENESS.Length - 1; ag++)
            //{
            //    if (_worker.CancellationPending) break;
            //    //guided reference
            //    RunExperimentAsync(sender, e, CreateExperiment(GuideCount, 0, ASSERTIVENESS[ag], 0, CREDIBILITY_NORMAL, 0), progress);
            //    for (int ai = ag; ai < ASSERTIVENESS.Length; ai++)
            //    {
            //        //no credibility
            //        //with credibility
            //        for (int ci = 0; ci < CREDIBILITY.Length; ci++)
            //        {
            //            if (_worker.CancellationPending) break;
            //            RunExperimentAsync(sender, e, CreateExperiment(GuideCount, INTRUDER_COUNT,
            //                ASSERTIVENESS[ag], ASSERTIVENESS[ai], CREDIBILITY_NORMAL, CREDIBILITY[ci]), progress);
            //            //first three runs only
            //            //_worker.CancelAsync();
            //        }
            //    }
            //}
            #endregion
            #region MODE 3
            ////naive only reference (1)
            ////guided reference (1)
            ////with intruder (12 = 3-assertiveness * 4-credibility)
            //int batchSize = 1 + 1 + 3 * 4;
            //ExperimentBatchProgress progress = new ExperimentBatchProgress(batchSize, _iRuns, _iLength);
            ////naives only reference
            //RunExperimentAsync(sender, e, CreateExperiment(0, 0, 0, 0, 0, 0), progress);
            //if (!_worker.CancellationPending)
            //{
            //    //guided reference
            //    RunExperimentAsync(sender, e, CreateExperiment(GuideCount, 0, ASSERTIVENESS_MEDIUM, 0, CREDIBILITY_NORMAL, 0), progress);
            //    //with intruder
            //    for (int ai = 1; ai < ASSERTIVENESS.Length; ai++)
            //    {
            //        for (int ci = 0; ci < CREDIBILITY.Length; ci++)
            //        {
            //            if (_worker.CancellationPending) break;
            //            RunExperimentAsync(sender, e, CreateExperiment(GuideCount, INTRUDER_COUNT,
            //                ASSERTIVENESS_MEDIUM, ASSERTIVENESS[ai], CREDIBILITY_NORMAL, CREDIBILITY[ci]), progress);
            //        }
            //    }
            //}
            #endregion
            #region MODE 4
            //naive only reference (1)
            //guided reference (1)
            //with intruder (12 = 3-assertiveness * 4-credibility)
            int batchSize = 1 + 1 + 3 * 4;
            ExperimentBatchProgress progress = new ExperimentBatchProgress(batchSize, _iRuns, _iLength);
            //naives only reference
            RunExperimentAsync(sender, e, CreateExperiment(0, 0, 0, 0, 0, 0), progress);
            if (!_worker.CancellationPending)
            {
                //guided reference
                RunExperimentAsync(sender, e, CreateExperiment(GUIDE_COUNT, 0, ASSERTIVENESS_MEDIUM, 0, CREDIBILITY_NORMAL, 0), progress);
                //with intruder
                for (int ai = 1; ai < ASSERTIVENESS.Length; ai++)
                {
                    for (int ci = 0; ci < CREDIBILITY.Length; ci++)
                    {
                        if (_worker.CancellationPending) break;
                        RunExperimentAsync(sender, e, CreateExperiment(GUIDE_COUNT, INTRUDER_COUNT,
                            ASSERTIVENESS_MEDIUM, ASSERTIVENESS[ai], CREDIBILITY_NORMAL, CREDIBILITY[ci]), progress);
                    }
                }
            }
            #endregion
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

        private ThesisExperimentPack CreateExperiment(int nG, int nI, double assertG, double assertI, double credG, double credI)
        {
            int nN = _iCount - nG - nI;
            string name = string.Format("MTE_{0}_{1}-{2}-{3}", _iCount, nN, SubgroupInfo(nG, assertG, credG, false), SubgroupInfo(nI, assertI, credI, true));
            return new ThesisExperimentPack(name, _iRuns, _iLength, _dTimePerStep, _scene, _species, _styles, _random.UInt(), _sPathCompleted,
                nN, nG, nI, ASSERTIVENESS_LOW, assertG, assertI, CREDIBILITY_NORMAL, credG, credI, StartSpawn, GoalG, GoalI,
                GetSpecies("Naive"), GetSpecies("Guide"), GetSpecies("Intruder"), _dFovRange, _fovAngle);
        }

        private string SubgroupInfo(int count, double assert, double cred, bool withCred)
        {
            return count == 0 ? count.ToString() : string.Format("{0}{1}{2}", count, _assertivenessSymbol[assert], withCred ? _credibilitySymbol[cred].ToString() : string.Empty);
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
            int frag = 0;
            int fragG = 0;
            int fragI = 0;
            int loneI = 0;
            int noGoal = 0;
            int atGg = 0;
            int atGi = 0;
            int nearGg = 0;
            int nearGi = 0;
            double size = 0;
            double sizeG = 0;
            for (int i = 0; i < e.Runs; i++)
            {
                if (e.Experiment.Status == ExperimentStatus.Canceled) continue;
                InstanceResults results = e.Experiment.Instances[i].Results;
                if (results.GroupCountEnd > 1 || results.StrayCountEnd > 0) frag++;
                if (!results.HasMainGroupGoalEnd) noGoal++;
                ProcessResultsGoals(e, results, e.Experiment.Instances[i].Model.History.Last, ref atGg, ref atGi, ref nearGg, ref nearGi);
                size += results.MainGroupSizeEnd;
                ProcessResultsObserved(e, results, ref sizeG, ref fragG, ref fragI, ref loneI);
            }
            if (e.Runs > 0)
            {
                size /= e.Runs;
                sizeG /= e.Runs;
            }
            _writer.WriteLine(string.Join("\t", _iCount, e.NaiveCount, e.GuideCount, e.IntruderCount,
                e.AssertivenessGuide, e.AssertivenessIntruder, e.CredibilityGuide, e.CredibilityIntruder,
                e.Runs, frag, fragG, fragI, loneI, noGoal, atGg, atGi, nearGg, nearGi, size, sizeG));
        }

        private void ProcessResultsGoals(ThesisExperimentPack e, InstanceResults results, HistoryRecord hist, ref int atGg, ref int atGi, ref int nearGg, ref int nearGi)
        {
            double minDistG = double.MaxValue;
            double minDistI = double.MaxValue;
            foreach (Agent a in results.StepDetails.Last().MainGroup.Members)
            {
                double distG = Vector2.Distance(GoalG.Position, hist[a.ID].Position);
                if (distG < minDistG) minDistG = distG;
                double distI = Vector2.Distance(GoalI.Position, hist[a.ID].Position);
                if (distI < minDistI) minDistI = distI;
            }
            if (minDistG < GoalG.Radius + _dFovRange)
            {
                atGg++;
                return;
            }
            if (minDistI < GoalI.Radius + _dFovRange)
            {
                atGi++;
                return;
            }
            double halfGoalDist = Vector2.Distance(GoalG.Position, GoalI.Position) / 2;
            //ElementStatus centroid = hist[results.StepDetails.Last().MainGroup.ID];
            //Angle angleG = Vector2.AngleBetween(centroid.Direction, GoalG.Position - centroid.Position);
            //Angle angleI = Vector2.AngleBetween(centroid.Direction, GoalI.Position - centroid.Position);
            if (minDistG < halfGoalDist && minDistG < minDistI)
            {
                nearGg++;
                return;
            }
            if (minDistI < halfGoalDist && minDistI < minDistG)
            {
                nearGi++;
            }
        }

        private void ProcessResultsObserved(ThesisExperimentPack e, InstanceResults results, ref double sizeG, ref int fragG, ref int fragI, ref int loneI)
        {
            if (e.GuideCount > 0)
            {
                sizeG += results.Observed[0].MajorityGroupSizeEnd;
                if ((results.GroupCountEnd > 0 || results.StrayGoalCountEnd > 0)
                    && results.Observed[0].InMainGroupCountEnd < results.Observed[0].Count) fragG++;
                if (e.IntruderCount > 0)
                {
                    if (results.Observed[1].MajorityGroupSizeEnd == 1)
                    {
                        loneI++;
                    }
                    else
                    {
                        if (results.GroupCountEnd > 1 && results.Observed[1].MajorityGroupSizeEnd > 1
                            && results.Observed[1].InMainGroupCountEnd == 0) fragI++;
                    }
                }
            }
        }

        protected override void WriteBatchResultsHeader()
        {
            /*
             * N - # of all agents
             * Nn - # of naive agents
             * Ng - # of guides
             * Ni - # of intruders
             * Ag - assertiveness of guides
             * Ai - assertiveness of intruders
             * Cg - credibility of guides
             * Ci - credibility of intruders
             * runs - runs completed (0:skipped, max:ok, <max:canceled) - not yet ready to work
             * frag - group fragmented
             * frag_g - a minority of agents split from initial group containing guides
             * frag_i - a minority of agents split from initial group containing intruder
             * lone_i - intruder alone
             * noGoal - main group ended without any guides or intruders
             * at_Gg - main group ended at goal for guides
             * at_Gi - main group ended at goal for intruders
             * near_Gg - main group ended near goal for guides and headed there
             * near_Gi - main group ended near goal for intruders and headed there
             * size - average end size of main group
             * size_g - average end size of group with most guides
             */
            _writer.WriteLine(string.Join("\t", "N", "Nn", "Ng", "Ni", "Ag", "Ai", "Cg", "Ci", "runs", "frag",
                "frag_g", "frag_i", "lone_i", "noGoal", "at_Gg", "at_Gi", "near_Gg", "near_Gi", "size", "size_g"));
        }

        #endregion
    }
}

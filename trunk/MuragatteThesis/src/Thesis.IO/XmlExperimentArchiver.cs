// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
//
// Copyright (C) 2012  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Muragatte.Thesis.GUI;
using Muragatte.Visual.IO;

using System.ComponentModel;
using System.IO;
using Ionic.Zip;
using Muragatte.IO;
using Muragatte.Core.Storage;

namespace Muragatte.Thesis.IO
{
    public class XmlExperimentArchiver : XmlBaseArchiver<XmlExperimentRoot, ThesisExperimentEditorWindow>
    {
        #region Constructors

        public XmlExperimentArchiver(ThesisExperimentEditorWindow owner) : base("Experiment", owner) { }

        #endregion

        #region Methods

        protected override void LoadPostProcessing(XmlExperimentRoot item)
        {
            _owner.LoadExperiment(item.Experiment);
        }

        #endregion
    }

    public class ExperimentArchiver
    {
        #region Fields

        private XmlLoadSave<XmlExperimentRoot> _xml = new XmlLoadSave<XmlExperimentRoot>();
        private BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Constructors

        public ExperimentArchiver()
        {
            InitializeWorker();
        }

        #endregion

        #region Properties

        public BackgroundWorker Worker
        {
            get { return _worker; }
        }

        #endregion

        #region Methods

        public void Save(Experiment experiment)
        {
            if (experiment != null && experiment.IsComplete && !_worker.IsBusy)
            {
                _worker.RunWorkerAsync(experiment);
            }
        }

        private void SaveAsync(Experiment experiment)
        {
            //if (Directory.Exists(_experiment.Name))
            //{
            //    foreach (string f in Directory.GetFiles(_experiment.Name))
            //    {
            //        File.Delete(f);
            //    }
            //    Directory.Delete(_experiment.Name, true);
            //}
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), experiment.Name));
            _xml.Save(Path.Combine(di.FullName, "Settings.xml"), new XmlExperimentRoot(experiment));

            #region Results
            DirectoryInfo diR = di.CreateSubdirectory(@"Results");
            #region General
            using (StreamWriter writer = File.CreateText(Path.Combine(diR.FullName, "General.txt")))
            {
                writer.WriteLine("Count {0}{3} {0}{4} {0}{5} {1}{3} {1}{4} {1}{5} {2}{3} {2}{4} {2}{5}", "Start", "Overall", "End", "Min", "Avg", "Max");
                writer.WriteLine("GroupCount {0}", experiment.Results.GroupCount);
                writer.WriteLine("StrayTotal {0}", experiment.Results.StrayCount);
                writer.WriteLine("StrayGoal {0}", experiment.Results.StrayGoalCount);
                writer.WriteLine("StrayWander {0}", experiment.Results.StrayWanderCount);
            }
            #endregion
            #region Main Group
            using (StreamWriter writer = File.CreateText(Path.Combine(diR.FullName, "MainGroup.txt")))
            {
                writer.WriteLine("SIZE");
                writer.WriteLine("Time Min Avg Max");
                writer.WriteLine("Start {0}", experiment.Results.MainGroupSize.Start);
                writer.WriteLine("Overall {0}", experiment.Results.MainGroupSize.Overall);
                writer.WriteLine("End {0}", experiment.Results.MainGroupSize.End);
                writer.WriteLine();
                if (experiment.Results.IsEndGoalDefined)
                {
                    writer.WriteLine("GOAL END DISTANCE");
                    writer.WriteLine("At Min Avg Max");
                    writer.WriteLine("Minimum {0}", experiment.Results.MainGroupGoalEndDistanceMinimum);
                    writer.WriteLine("Average {0}", experiment.Results.MainGroupGoalEndDistanceAverage);
                    writer.WriteLine("Maximum {0}", experiment.Results.MainGroupGoalEndDistanceMaximum);
                    writer.WriteLine("Centroid {0}", experiment.Results.MainGroupGoalEndDistanceCentroid);
                    writer.WriteLine();
                }
                writer.WriteLine("MAJORITY GOAL");
                writer.WriteLine("Goal Start Overall End");
                foreach (Results.GoalExperimentPercentage gep in experiment.Results.MainGroupGoalPercentage)
                {
                    writer.WriteLine(gep);
                }
            }
            #endregion
            #region Observed
            foreach (Results.ObservedArchetypeExperimentSummary os in experiment.Results.Observed)
            {
                using (StreamWriter writer = File.CreateText(Path.Combine(diR.FullName, string.Format("Observed_{0}.txt", os.Name.Replace(" ", string.Empty)))))
                {
                    writer.WriteLine("GENERAL");
                    writer.WriteLine("Name {0}", os.Name);
                    writer.WriteLine("Count {0}", os.Count);
                    writer.WriteLine("Goal {0}", os.Goal == null ? "None" : os.Goal.Name);
                    writer.WriteLine();
                    writer.WriteLine("COUNT AND SIZE");
                    writer.WriteLine("For {0}{3} {0}{4} {0}{5} {1}{3} {1}{4} {1}{5} {2}{3} {2}{4} {2}{5}", "Start", "Overall", "End", "Min", "Avg", "Max");
                    writer.WriteLine("Groups {0}", os.GroupCount);
                    writer.WriteLine("Strays {0}", os.StrayCount);
                    writer.WriteLine("InMainGroup {0}", os.InMainGroupCount);
                    writer.WriteLine("SharedGoal {0}", os.SharedGroupGoalCount);
                    writer.WriteLine("MajorityGroup {0}", os.MajorityGroupSize);
                    writer.WriteLine();
                    writer.WriteLine("ONE GROUP");
                    writer.WriteLine("Start Overall End");
                    writer.WriteLine("{0} {1} {2}", os.AllInOneGroupStart, os.AllInOneGroup, os.AllInOneGroupEnd);
                    if (os.HasGoal)
                    {
                        writer.WriteLine();
                        writer.WriteLine("GOAL END DISTANCE");
                        writer.WriteLine("At Min Avg Max");
                        writer.WriteLine("Minimum {0}", os.GoalEndDistanceMinimum);
                        writer.WriteLine("Average {0}", os.GoalEndDistanceAverage);
                        writer.WriteLine("Maximum {0}", os.GoalEndDistanceMaximum);
                    }
                }
            }
            #endregion
            #endregion

            #region History
            DirectoryInfo diH = di.CreateSubdirectory(@"History");
            string instanceFileNameFormat = "Instance_{0:D" + experiment.RepeatCount.ToString().Length + "}.zip";
            string stepFileNameFormat = "Step_{0:D" + experiment.Definition.Length.ToString().Length + "}.txt";
            int records = experiment.Instances.Last().Model.History.Count;
            for (int i = 0; i < experiment.RepeatCount; i++)
            {
                int rc = 0;
                using (ZipFile zip = new ZipFile())
                {
                    foreach (HistoryRecord r in experiment.Instances[i].Model.History)
                    {
                        string filePath = Path.Combine(diH.FullName, string.Format(stepFileNameFormat, r.Step));
                        using (StreamWriter writer = File.CreateText(filePath))
                        {
                            writer.WriteLine(ElementStatus.Header);
                            foreach (ElementStatus es in r)
                            {
                                writer.WriteLine(es);
                            }
                        }
                        zip.AddFile(filePath, "");
                        rc++;
                        _worker.ReportProgress(0, 100d * (i * records + rc) / (records * experiment.RepeatCount));
                    }
                    zip.Save(System.IO.Path.Combine(diH.FullName, string.Format(instanceFileNameFormat, i)));
                    foreach (string f in Directory.GetFiles(diH.FullName, "*.txt"))
                    {
                        File.Delete(f);
                    }
                }
                _worker.ReportProgress(0, 100d * (i + 1) / experiment.RepeatCount);
            }
            #endregion

            //snapshots?
        }

        private void DeleteExisting(string directoryPath)
        {
            foreach (string d in Directory.GetDirectories(directoryPath))
            {
                DeleteExisting(d);
            }
            foreach (string f in Directory.GetFiles(directoryPath))
            {
                File.Delete(f);
            }
            Directory.Delete(directoryPath);
        }

        private void InitializeWorker()
        {
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SaveAsync((Experiment)e.Argument);
        }

        #endregion
    }
}

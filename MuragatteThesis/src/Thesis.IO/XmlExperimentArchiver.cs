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
using Ionic.Zlib;
using Muragatte.IO;
using Muragatte.Core.Storage;
using Microsoft.Win32;

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

    public enum ArchiverMode
    {
        Load,
        Save
    }

    public class CompletedExperimentArchiver
    {
        #region Constants

        public const string LOAD_INFO = "Loading in progress...";
        public const string SAVE_INFO = "Saving in progress...";

        private const string SETTINGS_FILENAME = @"Settings.xml";
        private const string RESULTS_DIRECTORY = @"Results";
        private const string HISTORY_DIRECTORY = @"History";
        private const string INSTANCE_FILENAME_BASE = "Instance_";
        private const string INSTANCE_FILES = INSTANCE_FILENAME_BASE + "*.zip";
        private const string STEP_FILENAME_BASE = "Step_";

        #endregion

        #region Fields

        private Experiment _experiment = null;
        private CompressionLevel _compression = CompressionLevel.Default;
        private string _sPath = string.Empty;

        private OpenFileDialog _loadDlg = new OpenFileDialog();
        private SaveFileDialog _saveDlg = new SaveFileDialog();
        private XmlLoadSave<XmlExperimentRoot> _xml = new XmlLoadSave<XmlExperimentRoot>();
        private BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Constructors

        public CompletedExperimentArchiver()
        {
            InitializeLoadSaveDialogs();
            InitializeWorker();
        }

        #endregion

        #region Properties

        public BackgroundWorker Worker
        {
            get { return _worker; }
        }

        public Experiment Experiment
        {
            get { return _experiment; }
        }

        #endregion

        #region Methods

        public bool Load(out Experiment experiment)
        {
            experiment = null;
            bool? result = _loadDlg.ShowDialog();
            if (result == true)
            {
                _sPath = Path.GetDirectoryName(_loadDlg.FileName);
                string histDir = Path.Combine(_sPath, HISTORY_DIRECTORY);
                if (Directory.Exists(histDir) && Directory.GetFiles(histDir, INSTANCE_FILES).Length > 0)
                {
                    _experiment = _xml.Load(Path.Combine(_sPath, SETTINGS_FILENAME)).Experiment.ToExperiment();
                    _experiment.PreProcessing();
                    experiment = _experiment;
                    _worker.RunWorkerAsync(ArchiverMode.Load);
                    return true;
                }
            }
            return false;
        }

        private void LoadAsync()
        {
            string histDir = Path.Combine(_sPath, HISTORY_DIRECTORY);
            string tempDir = Path.Combine(histDir, "temp");
            foreach (string f in Directory.GetFiles(histDir, INSTANCE_FILES))
            {
                using (ZipFile zip = ZipFile.Read(f))
                {
                    int instance = int.Parse(Path.GetFileNameWithoutExtension(f).Substring(INSTANCE_FILENAME_BASE.Length));
                    foreach (ZipEntry e in zip)
                    {
                        e.Extract(tempDir, ExtractExistingFileAction.OverwriteSilently);
                        int step = int.Parse(Path.GetFileNameWithoutExtension(e.FileName).Substring(STEP_FILENAME_BASE.Length));
                        HistoryRecord record = new HistoryRecord(step);
                        using (StreamReader reader = new StreamReader(Path.Combine(tempDir, e.FileName)))
                        {
                            reader.ReadLine();
                            while (!reader.EndOfStream)
                            {
                                record.Add(ElementStatus.FromString(reader.ReadLine()));
                            }
                            record.CreateGroupsAndStrays(_experiment.Instances[instance].Model.Elements.Agents);
                        }
                        _experiment.Instances[instance].Model.History.Add(record);
                        _worker.ReportProgress(0, 100d * (instance * zip.Count + step) / (zip.Count * _experiment.RepeatCount));
                    }
                    //zip.ExtractAll(tempDir, ExtractExistingFileAction.OverwriteSilently);
                    //foreach (string txt in Directory.GetFiles(tempDir))
                    //{
                    //    int step = int.Parse(Path.GetFileNameWithoutExtension(txt).Substring(STEP_FILENAME_BASE.Length));
                    //    HistoryRecord record = new HistoryRecord(step);
                    //    using (StreamReader reader = new StreamReader(txt))
                    //    {
                    //        reader.ReadLine();
                    //        while (!reader.EndOfStream)
                    //        {
                    //            record.Add(ElementStatus.FromString(reader.ReadLine()));
                    //        }
                    //    }
                    //    _experiment.Instances[instance].Model.History.Add(record);
                    //    _worker.ReportProgress(0, 100d * (instance * zip.Count + step) / (zip.Count * _experiment.RepeatCount));
                    //}
                }
                DeleteFiles(tempDir);
            }
            Directory.Delete(tempDir);
            _experiment.FinishLoading();
        }

        public void SaveAt(Experiment experiment, CompressionLevel compression = CompressionLevel.Default)
        {
            if (experiment != null && experiment.IsComplete && !_worker.IsBusy)
            {
                _saveDlg.FileName = experiment.Name;
                bool? result = _saveDlg.ShowDialog();
                if (result == true)
                {
                    Save(experiment, Path.GetDirectoryName(_saveDlg.FileName), compression);
                }
            }
        }

        public void Save(Experiment experiment, CompressionLevel compression = CompressionLevel.Default)
        {
            if (experiment != null && experiment.IsComplete && !_worker.IsBusy)
            {
                Save(experiment, Directory.GetCurrentDirectory(), compression);
            }
        }

        private void Save(Experiment experiment, string path, CompressionLevel compression)
        {
            _compression = compression;
            _experiment = experiment;
            _sPath = path;
            _worker.RunWorkerAsync(ArchiverMode.Save);
        }

        private void SaveAsync()
        {
            //if (Directory.Exists(_experiment.Name))
            //{
            //    foreach (string f in Directory.GetFiles(_experiment.Name))
            //    {
            //        File.Delete(f);
            //    }
            //    Directory.Delete(_experiment.Name, true);
            //}
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(_sPath, _experiment.Name));
            _xml.Save(Path.Combine(di.FullName, SETTINGS_FILENAME), new XmlExperimentRoot(_experiment));

            #region Results
            DirectoryInfo diR = di.CreateSubdirectory(RESULTS_DIRECTORY);
            #region General
            using (StreamWriter writer = File.CreateText(Path.Combine(diR.FullName, "General.txt")))
            {
                writer.WriteLine("Count {0}{3} {0}{4} {0}{5} {1}{3} {1}{4} {1}{5} {2}{3} {2}{4} {2}{5}", "Start", "Overall", "End", "Min", "Avg", "Max");
                writer.WriteLine("GroupCount {0}", _experiment.Results.GroupCount);
                writer.WriteLine("StrayTotal {0}", _experiment.Results.StrayCount);
                writer.WriteLine("StrayGoal {0}", _experiment.Results.StrayGoalCount);
                writer.WriteLine("StrayWander {0}", _experiment.Results.StrayWanderCount);
            }
            #endregion
            #region Main Group
            using (StreamWriter writer = File.CreateText(Path.Combine(diR.FullName, "MainGroup.txt")))
            {
                writer.WriteLine("SIZE");
                writer.WriteLine("Time Min Avg Max");
                writer.WriteLine("Start {0}", _experiment.Results.MainGroupSize.Start);
                writer.WriteLine("Overall {0}", _experiment.Results.MainGroupSize.Overall);
                writer.WriteLine("End {0}", _experiment.Results.MainGroupSize.End);
                writer.WriteLine();
                if (_experiment.Results.IsEndGoalDefined)
                {
                    writer.WriteLine("GOAL END DISTANCE");
                    writer.WriteLine("At Min Avg Max");
                    writer.WriteLine("Minimum {0}", _experiment.Results.MainGroupGoalEndDistanceMinimum);
                    writer.WriteLine("Average {0}", _experiment.Results.MainGroupGoalEndDistanceAverage);
                    writer.WriteLine("Maximum {0}", _experiment.Results.MainGroupGoalEndDistanceMaximum);
                    writer.WriteLine("Centroid {0}", _experiment.Results.MainGroupGoalEndDistanceCentroid);
                    writer.WriteLine();
                }
                writer.WriteLine("MAJORITY GOAL");
                writer.WriteLine("Goal Start Overall End");
                foreach (Results.GoalExperimentPercentage gep in _experiment.Results.MainGroupGoalPercentage)
                {
                    writer.WriteLine(gep);
                }
            }
            #endregion
            #region Observed
            foreach (Results.ObservedArchetypeExperimentSummary os in _experiment.Results.Observed)
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
            DirectoryInfo diH = di.CreateSubdirectory(HISTORY_DIRECTORY);
            string instanceFileNameFormat = INSTANCE_FILENAME_BASE + "{0:D" + _experiment.RepeatCount.ToString().Length + "}.zip";
            string stepFileNameFormat = STEP_FILENAME_BASE + "{0:D" + _experiment.Definition.Length.ToString().Length + "}.txt";
            int records = _experiment.Instances.Last().Model.History.Count;
            for (int i = 0; i < _experiment.RepeatCount; i++)
            {
                int rc = 0;
                using (ZipFile zip = new ZipFile())
                {
                    zip.CompressionLevel = _compression;
                    foreach (HistoryRecord r in _experiment.Instances[i].Model.History)
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
                        _worker.ReportProgress(0, 100d * (i * records + rc) / (records * _experiment.RepeatCount));
                    }
                    zip.Save(System.IO.Path.Combine(diH.FullName, string.Format(instanceFileNameFormat, i)));
                    foreach (string f in Directory.GetFiles(diH.FullName, "*.txt"))
                    {
                        File.Delete(f);
                    }
                }
                _worker.ReportProgress(0, 100d * (i + 1) / _experiment.RepeatCount);
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
            DeleteFiles(directoryPath);
            Directory.Delete(directoryPath);
        }

        private void DeleteFiles(string directoryPath)
        {
            foreach (string f in Directory.GetFiles(directoryPath))
            {
                File.Delete(f);
            }
        }

        private void InitializeLoadSaveDialogs()
        {
            _loadDlg.Title = "Open Completed Experiment";
            _loadDlg.FileName = SETTINGS_FILENAME;
            _loadDlg.DefaultExt = ".xml";
            _loadDlg.Filter = "XML Files (.xml)|*.xml";

            _saveDlg.Title = "Save Completed Experiment";
            _saveDlg.DefaultExt = ".xml";
            _saveDlg.Filter = "XML Files (.xml)|*.xml";
        }

        private void InitializeWorker()
        {
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(Archiver_LoadSave);
        }

        private void Archiver_LoadSave(object sender, DoWorkEventArgs e)
        {
            ArchiverMode mode = (ArchiverMode)e.Argument;
            switch (mode)
            {
                case ArchiverMode.Load:
                    LoadAsync();
                    break;
                case ArchiverMode.Save:
                    SaveAsync();
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}

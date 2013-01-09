// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
//
// Copyright (C) 2012-2013  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Ionic.Zip;
using Ionic.Zlib;
using Muragatte.IO;
using Muragatte.Core.Storage;

using Muragatte.Research.Results;

namespace Muragatte.Research.IO
{
    public class CompletedExperimentArchiver
    {
        #region Enum

        private enum ArchiverMode
        {
            Load,
            Save
        }

        #endregion

        #region Constants

        public const string LOAD_INFO = "Loading in progress...";
        public const string SAVE_INFO = "Saving in progress...";

        public const string SETTINGS_FILENAME = @"Settings.xml";
        
        private const string RESULTS_DIRECTORY_NAME = @"Results";
        private const string RESULTS_GENERAL_FILENAME = @"General.txt";
        private const string RESULTS_MAINGROUP_FILENAME = @"MainGroup.txt";
        private const string RESULTS_OBSERVED_FILENAME_BASE = "Observed_";

        private const string HISTORY_DIRECTORY_NAME = @"History";
        private const string HISTORY_TEMP_DIRECTORY_NAME = @"temp";
        private const string HISTORY_INSTANCE_FILENAME_BASE = "Instance_";
        private const string HISTORY_INSTANCE_FILES = HISTORY_INSTANCE_FILENAME_BASE + "*.zip";
        private const string HISTORY_STEP_FILENAME_BASE = "Step_";

        #endregion

        #region Fields

        private readonly string ResultsHeaderSOEMAM = string.Format("{0}{3} {0}{4} {0}{5} {1}{3} {1}{4} {1}{5} {2}{3} {2}{4} {2}{5}", "Start", "Overall", "End", "Min", "Avg", "Max");
        private Style _saveConflictMsgStyle = null;

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
            _saveConflictMsgStyle = new Style(typeof(Xceed.Wpf.Toolkit.MessageBox));
            _saveConflictMsgStyle.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.YesButtonContentProperty, "Browse"));
            _saveConflictMsgStyle.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.NoButtonContentProperty, "Delete"));
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
                string histDir = Path.Combine(_sPath, HISTORY_DIRECTORY_NAME);
                if (Directory.Exists(histDir) && Directory.GetFiles(histDir, HISTORY_INSTANCE_FILES).Length > 0)
                {
                    _experiment = _xml.Load(Path.Combine(_sPath, SETTINGS_FILENAME)).Experiment.ToExperiment();
                    _experiment.StartLoading();
                    experiment = _experiment;
                    _worker.RunWorkerAsync(ArchiverMode.Load);
                    return true;
                }
            }
            return false;
        }

        private void LoadAsync()
        {
            string histDir = Path.Combine(_sPath, HISTORY_DIRECTORY_NAME);
            string tempDir = Path.Combine(histDir, HISTORY_TEMP_DIRECTORY_NAME);
            foreach (string f in Directory.GetFiles(histDir, HISTORY_INSTANCE_FILES))
            {
                using (ZipFile zip = ZipFile.Read(f))
                {
                    int instance = int.Parse(Path.GetFileNameWithoutExtension(f).Substring(HISTORY_INSTANCE_FILENAME_BASE.Length));
                    foreach (ZipEntry e in zip)
                    {
                        e.Extract(tempDir, ExtractExistingFileAction.OverwriteSilently);
                        int step = int.Parse(Path.GetFileNameWithoutExtension(e.FileName).Substring(HISTORY_STEP_FILENAME_BASE.Length));
                        LoadHistoryRecord(step, instance, Path.Combine(tempDir, e.FileName));
                        _worker.ReportProgress(0, 100d * (instance * _experiment.Definition.Length + step) / (_experiment.Definition.Length * _experiment.RepeatCount));
                    }
                }
                DeleteFiles(tempDir);
            }
            Directory.Delete(tempDir);
            _experiment.FinishLoading();
        }

        private void LoadHistoryRecord(int step, int instance, string path)
        {
            HistoryRecord record = new HistoryRecord(step);
            using (StreamReader reader = new StreamReader(path))
            {
                reader.ReadLine();  //skip header
                while (!reader.EndOfStream)
                {
                    record.Add(ElementStatus.FromString(reader.ReadLine()));
                }
                record.CreateGroupsAndStrays(_experiment.Instances[instance].Model.Elements.Agents);
            }
            _experiment.Instances[instance].Model.History.Add(record);
        }

        public void SaveAt(Experiment experiment, CompressionLevel compression = CompressionLevel.Default)
        {
            if (experiment != null && experiment.IsComplete && !_worker.IsBusy)
            {
                if (experiment.ExtraSetting.Path != null) _saveDlg.InitialDirectory = experiment.ExtraSetting.Path;
                _saveDlg.FileName = experiment.Name;
                bool? result = _saveDlg.ShowDialog();
                if (result == true)
                {
                    SaveConflict(experiment, Path.Combine(Path.GetDirectoryName(_saveDlg.FileName), Path.GetFileNameWithoutExtension(_saveDlg.FileName)), compression);
                }
            }
        }

        public void Save(Experiment experiment, CompressionLevel compression = CompressionLevel.Default)
        {
            if (experiment != null && experiment.IsComplete && !_worker.IsBusy)
            {
                string savePath = experiment.ExtraSetting.Path ?? Directory.GetCurrentDirectory();
                SaveConflict(experiment, Path.Combine(savePath, experiment.Name), compression);
            }
        }

        private void SaveConflict(Experiment experiment, string path, CompressionLevel compression)
        {
            if (Directory.Exists(path) && Directory.GetFiles(path).Length > 0)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(
                    "Specified folder already exists and isn't empty.", "Conflict detected",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel, _saveConflictMsgStyle);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveAt(experiment, compression);
                        break;
                    case MessageBoxResult.No:
                        DeleteExisting(path);
                        Save(experiment, path, compression);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Save(experiment, path, compression);
            }
        }

        private void Save(Experiment experiment, string path, CompressionLevel compression)
        {
            _compression = compression;
            _experiment = experiment;
            _sPath = path;
            _worker.RunWorkerAsync(ArchiverMode.Save);
        }

        private void SaveResults(DirectoryInfo directory)
        {
            DirectoryInfo diR = directory.CreateSubdirectory(RESULTS_DIRECTORY_NAME);
            SaveResultsGeneral(diR.FullName);
            SaveResultsMainGroup(diR.FullName);
            SaveResultsObserved(diR.FullName);
        }

        private void SaveResultsGeneral(string directoryPath)
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(directoryPath, RESULTS_GENERAL_FILENAME)))
            {
                writer.WriteLine("Count {0}", ResultsHeaderSOEMAM);
                writer.WriteLine("GroupCount {0}", _experiment.Results.GroupCount);
                writer.WriteLine("StrayTotal {0}", _experiment.Results.StrayCount);
                writer.WriteLine("StrayGoal {0}", _experiment.Results.StrayGoalCount);
                writer.WriteLine("StrayWander {0}", _experiment.Results.StrayWanderCount);
            }
        }

        private void SaveResultsMainGroup(string directoryPath)
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(directoryPath, RESULTS_MAINGROUP_FILENAME)))
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
                foreach (GoalExperimentPercentage gep in _experiment.Results.MainGroupGoalPercentage)
                {
                    writer.WriteLine(gep);
                }
            }
        }

        private void SaveResultsObserved(string directoryPath)
        {
            foreach (ObservedArchetypeExperimentSummary os in _experiment.Results.Observed)
            {
                using (StreamWriter writer = File.CreateText(Path.Combine(directoryPath, string.Format("{0}{1}.txt", RESULTS_OBSERVED_FILENAME_BASE, os.Name.Replace(" ", string.Empty)))))
                {
                    writer.WriteLine("GENERAL");
                    writer.WriteLine("Name {0}", os.Name);
                    writer.WriteLine("Count {0}", os.Count);
                    writer.WriteLine("Goal {0}", os.Goal == null ? "None" : os.Goal.Name);
                    writer.WriteLine();
                    writer.WriteLine("COUNT AND SIZE");
                    writer.WriteLine("For {0}", ResultsHeaderSOEMAM);
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
        }

        private void SaveHistory(DirectoryInfo directory)
        {
            DirectoryInfo diH = directory.CreateSubdirectory(HISTORY_DIRECTORY_NAME);
            DirectoryInfo diTemp = diH.CreateSubdirectory(HISTORY_TEMP_DIRECTORY_NAME);
            string instanceFileNameFormat = HISTORY_INSTANCE_FILENAME_BASE + "{0:D" + _experiment.RepeatCount.ToString().Length + "}.zip";
            string stepFileNameFormat = HISTORY_STEP_FILENAME_BASE + "{0:D" + _experiment.Definition.Length.ToString().Length + "}.txt";
            int records = _experiment.Instances.Last().Model.History.Count;
            for (int i = 0; i < _experiment.RepeatCount; i++)
            {
                using (ZipFile zip = new ZipFile())
                {
                    int rc = 0;
                    zip.CompressionLevel = _compression;
                    foreach (HistoryRecord r in _experiment.Instances[i].Model.History)
                    {
                        string filePath = Path.Combine(diTemp.FullName, string.Format(stepFileNameFormat, r.Step));
                        SaveStep(r, filePath);
                        zip.AddFile(filePath, "");
                        rc++;
                        _worker.ReportProgress(0, 100d * (i * records + rc) / (records * _experiment.RepeatCount));
                    }
                    zip.Save(Path.Combine(diH.FullName, string.Format(instanceFileNameFormat, i)));
                    DeleteFiles(diTemp.FullName);
                }
                _worker.ReportProgress(0, 100d * (i + 1) / _experiment.RepeatCount);
            }
            Directory.Delete(diTemp.FullName);
        }

        private void SaveStep(HistoryRecord record, string path)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.WriteLine(ElementStatus.Header);
                foreach (ElementStatus es in record)
                {
                    writer.WriteLine(es);
                }
            }
        }

        private void SaveAsync()
        {
            DirectoryInfo di = Directory.Exists(_sPath) ? new DirectoryInfo(_sPath) : Directory.CreateDirectory(_sPath);
            _xml.Save(Path.Combine(di.FullName, SETTINGS_FILENAME), new XmlExperimentRoot(_experiment));
            SaveResults(di);
            SaveHistory(di);

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
            _saveDlg.DefaultExt = ".mre";
            _saveDlg.Filter = "Muragatte Research Experiment (.mre)|*.mre";
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

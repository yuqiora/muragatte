// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
//
// Copyright (C) 2012  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Muragatte.Random;
using Muragatte.Research.IO;

namespace Muragatte.Research.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        private RandomMT _random = new RandomMT();
        private Experiment _experiment = null;

        private CompletedExperimentArchiver _archiver = new CompletedExperimentArchiver();

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public MainWindow()
        {
            _archiver.Worker.ProgressChanged += new ProgressChangedEventHandler(ExperimentLoadSave_ProgressChanged);
            _archiver.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ExperimentLoadSave_Completed);

            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Properties

        public Experiment Experiment
        {
            get { return _experiment; }
            set
            {
                _experiment = value;
                if (_experiment != null)
                {
                    _experiment.Worker.ProgressChanged += new ProgressChangedEventHandler(ExperimentInProgress_ProgressChanged);
                    _experiment.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ExperimentInProgress_Completed);
                }
                NotifyPropertyChanged("Experiment");
            }
        }

        public RandomMT Random
        {
            get { return _random; }
        }

        #endregion

        #region Events

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            OpenDialogWindow(new AboutWindow());
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            OpenDialogWindow(new ExperimentEditorWindow(this));
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment != null)
            {
                if (_experiment.Status == ExperimentStatus.Canceled) _experiment.Reset();
                ShowExperimentRunProgress();
                _experiment.RunAsync();
            }
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment != null)
            {
                OpenDialogWindow(new ExperimentEditorWindow(this));
            }
        }

        private void btnResults_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment.IsComplete)
            {
                OpenDialogWindow(new ExperimentResultsWindow(_experiment));
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Experiment = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _experiment.CancelAsync();
        }

        private void ExperimentInProgress_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_experiment.IsComplete)
            {
                ShowLoadSaveProgress(CompletedExperimentArchiver.SAVE_INFO);
                if (_experiment.ExtraSetting.IsAutoSaved)
                    _archiver.Save(_experiment, _experiment.ExtraSetting.Compression);
                if (!_archiver.Worker.IsBusy) binProgress.IsBusy = false;
            }
            else
            {
                binProgress.IsBusy = false;
            }
        }

        private void ExperimentInProgress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ExperimentProgress progress = (ExperimentProgress)e.UserState;
            UpdateProgressInfo(progress);
        }

        private void ExperimentLoadSave_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            binProgress.IsBusy = false;
            prbLoadSave.Value = 0;
        }

        private void ExperimentLoadSave_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prbLoadSave.Value = (double)e.UserState;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ShowLoadSaveProgress(CompletedExperimentArchiver.SAVE_INFO);
            _archiver.SaveAt(_experiment, _experiment.ExtraSetting.Compression);
            if (!_archiver.Worker.IsBusy) binProgress.IsBusy = false;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Experiment openExp;
            ShowLoadSaveProgress(CompletedExperimentArchiver.LOAD_INFO);
            if (_archiver.Load(out openExp))
            {
                Experiment = openExp;
            }
            else
            {
                binProgress.IsBusy = false;
            }
        }

        #endregion

        #region Methods

        private void ShowExperimentRunProgress()
        {
            ShowProgress(System.Windows.Visibility.Visible, System.Windows.Visibility.Collapsed);
        }

        private void ShowLoadSaveProgress(string info)
        {
            txbLoadSaveInfo.Text = info;
            ShowProgress(System.Windows.Visibility.Collapsed, System.Windows.Visibility.Visible);
        }

        private void ShowProgress(System.Windows.Visibility runInfo, System.Windows.Visibility loadSave)
        {
            grdExperimentRunProgressInfo.Visibility = runInfo;
            txbLoadSaveInfo.Visibility = loadSave;
            prbLoadSave.Visibility = loadSave;
            binProgress.IsBusy = true;
        }

        private void UpdateProgressInfo(ExperimentProgress progress)
        {
            txbInstanceProgress.Text = string.Format("{0} / {1}", progress.Step, progress.Length);
            prbInstance.Value = progress.InstancePercent;
            txbExperimentProgress.Text = string.Format("{0} / {1}", progress.Instance, progress.Repeat);
            prbExperiment.Value = progress.ExperimentPercent;
        }

        private void OpenDialogWindow(Window window)
        {
            window.Owner = this;
            window.ShowDialog();
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

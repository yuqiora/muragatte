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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.Random;
using Muragatte.Visual;

namespace Muragatte.Thesis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private ObservableCollection<Visual.Styles.Style> _styles = new ObservableCollection<Visual.Styles.Style>();
        private SpeciesCollection _species = new SpeciesCollection();
        private Scene _scene = new Scene();
        private readonly List<double> _timePerStepOptions = new List<double>() { 0.1, 0.2, 0.25, 0.5, 1 };

        private RandomMT _random = new RandomMT();

        ExperimentBatch batch = null;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Properties

        public ObservableCollection<Visual.Styles.Style> GetStyles
        {
            get { return _styles; }
        }

        public Scene GetScene
        {
            get { return _scene; }
        }

        public SpeciesCollection GetSpecies
        {
            get { return _species; }
        }

        public List<double> GetTimePerStepOptions
        {
            get { return _timePerStepOptions; }
        }

        #endregion

        #region Events

        private void spbStyles_Click(object sender, RoutedEventArgs e)
        {
            Visual.GUI.OptionsWindow.StyleEditorDialog(this, _styles);
        }

        private void spbSpecies_Click(object sender, RoutedEventArgs e)
        {
            Window editor = new Research.GUI.SpeciesEditorWindow(_species);
            editor.Owner = this;
            editor.ShowDialog();
        }

        private void spbScene_Click(object sender, RoutedEventArgs e)
        {
            Window editor = new Research.GUI.SceneEditorWindow(_scene, _species);
            editor.Owner = this;
            editor.ShowDialog();
        }
        
        private void btnDataFilePath_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _saveDlg = new SaveFileDialog();
            _saveDlg.Title = "Save Data File";
            _saveDlg.FileName = "mte_data.dat";
            _saveDlg.DefaultExt = ".dat";
            _saveDlg.Filter = "DAT Files (.dat)|*.dat";
            bool? result = _saveDlg.ShowDialog(this);
            if (result == true)
            {
                txtDataFilePath.Text = _saveDlg.FileName;
            }
        }

        private void btnThesisRun_Click(object sender, RoutedEventArgs e)
        {
            if (CheckThesisRunConditions())
            {
                batch = new ThesisExperimentBatch(txtDataFilePath.Text, iudCount.Value.Value,
                    iudRuns.Value.Value, iudLength.Value.Value, (double)cmbTimePerStep.SelectedValue,
                    _styles, _species, _scene, dudThesisFOVRange.Value.Value, dudThesisNeighAngle.Value.Value);
                batch.Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
                batch.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
                batch.Archiver.Worker.ProgressChanged += new ProgressChangedEventHandler(Archiver_ProgressChanged);
                batch.Archiver.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Archiver_RunWorkerCompleted);
                binProgress.IsBusy = true;
                batch.RunAsync();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!batch.Archiver.Worker.IsBusy) binProgress.IsBusy = false;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!batch.Archiver.Worker.IsBusy) batch.SaveNext();
            ExperimentBatchProgress progress = (ExperimentBatchProgress)e.UserState;
            txbProgressLabel.Text = progress.Name + " in progress...";
            txbInstanceProgress.Text = string.Format("{0} / {1}", progress.Step, progress.Length);
            prbInstance.Value = progress.InstancePercent;
            txbExperimentProgress.Text = string.Format("{0} / {1}", progress.Instance, progress.Repeat);
            prbExperiment.Value = progress.ExperimentPercent;
            txbBatchProgress.Text = string.Format("{0} / {1}", progress.Experiment, progress.BatchSize);
            prbBatch.Value = progress.BatchPercent;
        }

        private void Archiver_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (batch.IsAllSaved)
            {
                if (!batch.Worker.IsBusy) binProgress.IsBusy = false;
                txbSaving.Text = "";
            }
            else
            {
                batch.SaveNext();
            }
        }

        private void Archiver_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is double)
                prbSaving.Value = (double)e.UserState / (double)(batch.InQueueToSave + 1);
            else
                txbSaving.Text = "Saving " + e.UserState.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            batch.CancelAsync();
        }

        #endregion

        #region Methods

        private bool CheckRunConditions()
        {
            return !string.IsNullOrWhiteSpace(txtDataFilePath.Text) && iudCount.Value.HasValue
                && iudRuns.Value.HasValue && iudLength.Value.HasValue && cmbTimePerStep.SelectedValue != null
                && _styles.Count > 0 && _species.Count > 0;
        }

        private bool CheckThesisRunConditions()
        {
            return CheckRunConditions() && _scene.StationaryElements.OfType<Goal>().Count() == 2
                && dudThesisFOVRange.Value.HasValue && dudThesisNeighAngle.Value.HasValue;
        }

        #endregion
    }
}

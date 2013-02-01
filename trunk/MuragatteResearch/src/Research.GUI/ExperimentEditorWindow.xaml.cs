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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.Research.IO;

namespace Muragatte.Research.GUI
{
    /// <summary>
    /// Interaction logic for ExperimentEditorWindow.xaml
    /// </summary>
    public partial class ExperimentEditorWindow : Window
    {
        #region Fields

        private MainWindow _wndThesis;
        private ObservableCollection<Visual.Styles.Style> _styles = null;
        private Scene _scene = null;
        private SpeciesCollection _species = null;
        private ObservableCollection<ObservedArchetype> _archetypes = null;
        private readonly List<StorageOptions> _storageOptions = new List<StorageOptions>() { StorageOptions.SimpleBruteForce };
        private StorageOptions _storage = StorageOptions.SimpleBruteForce;
        private readonly List<double> _timePerStepOptions = new List<double>() { 0.1, 0.2, 0.25, 0.5, 1 };
        private bool _bAutoSave = false;

        private XmlExperimentArchiver _xml = null;

        #endregion

        #region Constructors

        public ExperimentEditorWindow(MainWindow main)
        {
            InitializeComponent();
            DataContext = this;

            _wndThesis = main;
            if (GetExperiment == null)
            {
                _styles = new ObservableCollection<Visual.Styles.Style>();
                _scene = new Scene(new Region(100, true));
                _species = new SpeciesCollection();
                _archetypes = new ObservableCollection<ObservedArchetype>();
            }
            else
            {
                btnLoad.Visibility = System.Windows.Visibility.Collapsed;
                btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            }

            _xml = new XmlExperimentArchiver(this);
        }

        #endregion

        #region Properties

        public uint MaxSeedValue
        {
            get { return _wndThesis.Random.RandMax; }
        }

        public Experiment GetExperiment
        {
            get { return _wndThesis.Experiment; }
        }

        public ObservableCollection<Visual.Styles.Style> GetStyles
        {
            get { return GetExperiment == null ? _styles : GetExperiment.Styles; }
        }

        public Scene GetScene
        {
            get { return GetExperiment == null ? _scene : GetExperiment.Definition.Scene; }
        }

        public SpeciesCollection GetSpecies
        {
            get { return GetExperiment == null ? _species : GetExperiment.Definition.Species; }
        }

        public ObservableCollection<ObservedArchetype> GetArchetypes
        {
            get { return GetExperiment == null ? _archetypes : GetExperiment.Definition.Archetypes; }
        }

        public List<StorageOptions> GetStorageOptions
        {
            get { return _storageOptions; }
        }

        public bool IsAutoSaved
        {
            get { return GetExperiment == null ? _bAutoSave : GetExperiment.ExtraSetting.IsAutoSaved; }
            set
            {
                if (GetExperiment == null) _bAutoSave = value;
                else GetExperiment.ExtraSetting.IsAutoSaved = value;
            }
        }

        public StorageOptions SelectedStorage
        {
            get { return GetExperiment == null ? _storage : GetExperiment.Definition.Storage; }
            set
            {
                if (GetExperiment == null)
                {
                    _storage = value;
                }
                else
                {
                    GetExperiment.Definition.Storage = value;
                }
            }
        }

        public List<double> GetTimePerStepOptions
        {
            get { return _timePerStepOptions; }
        }

        #endregion

        #region Events

        private void btnRandomSeed_Click(object sender, RoutedEventArgs e)
        {
            dudSeed.Value = _wndThesis.Random.UInt();
        }

        private void spbStyles_Click(object sender, RoutedEventArgs e)
        {
            Visual.DefaultValues.Scale = 5;
            Visual.GUI.OptionsWindow.StyleEditorDialog(this, GetStyles);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (GetExperiment == null)
            {
                _wndThesis.Experiment = NewExperiment();
            }
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void spbSpecies_Click(object sender, RoutedEventArgs e)
        {
            OpenEditorDialog(new SpeciesEditorWindow(GetSpecies));
        }

        private void spbScene_Click(object sender, RoutedEventArgs e)
        {
            OpenEditorDialog(new SceneEditorWindow(GetScene, GetSpecies));
        }

        private void spbArchetypes_Click(object sender, RoutedEventArgs e)
        {
            OpenEditorDialog(new ArchetypesEditorWindow(GetArchetypes, GetScene, GetSpecies));
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            _xml.Load();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _xml.Save(new XmlExperimentRoot(GetExperiment ?? NewExperiment()));
        }

        #endregion

        #region Methods

        private Experiment NewExperiment()
        {
            Experiment e = new Experiment(txtName.Text, iudRepeat.Value.Value,
                new InstanceDefinition((double)cmbTimePerStep.SelectedItem, iudLength.Value.Value,
                    chbKeepSubsteps.IsChecked.Value, _scene, _species, _storage, _archetypes),
                _styles, (uint)dudSeed.Value.Value);
            e.ExtraSetting.IsAutoSaved = _bAutoSave;
            return e;
        }

        private void OpenEditorDialog(Window editor)
        {
            editor.Owner = this;
            editor.ShowDialog();
        }

        public void LoadExperiment(XmlExperiment xe)
        {
            txtName.Text = xe.Name;
            iudRepeat.Value = xe.Repeat;
            iudLength.Value = xe.Length;
            cmbTimePerStep.SelectedItem = xe.TimePerStep;
            chbKeepSubsteps.IsChecked = xe.KeepSubsteps;
            dudSeed.Value = xe.Seed;
            _storage = xe.Storage;
            _styles.Clear();
            xe.ApplyToStyles(_styles);
            _species.Clear();
            xe.KnownSpecies.ApplyToCollection(_species);
            _scene.Load(xe.Scene);
            _archetypes.Clear();
            xe.ApplyToArchetypes(_archetypes);
        }

        #endregion
    }
}

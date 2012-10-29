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

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisExperimentEditorWindow.xaml
    /// </summary>
    public partial class ThesisExperimentEditorWindow : Window
    {
        #region Fields

        private ThesisMainWindow _wndThesis;
        private ObservableCollection<Visual.Styles.Style> _styles = null;
        private Scene _scene = null;
        private SpeciesCollection _species = null;
        private ObservableCollection<ObservedArchetype> _archetypes = null;

        #endregion

        #region Constructors

        public ThesisExperimentEditorWindow(ThesisMainWindow main)
        {
            InitializeComponent();
            DataContext = this;

            _wndThesis = main;
            if (GetExperiment == null)
            {
                _styles = new ObservableCollection<Visual.Styles.Style>();
                _scene = new Scene(new Region(100, true));
                _species = new SpeciesCollection(true);
                _archetypes = new ObservableCollection<ObservedArchetype>();
            }
            else
            {
                btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            }
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

        #endregion

        #region Events

        private void btnRandomSeed_Click(object sender, RoutedEventArgs e)
        {
            dudSeed.Value = _wndThesis.Random.UInt();
        }

        private void spbStyles_Click(object sender, RoutedEventArgs e)
        {
            Visual.DefaultValues.Scale = 5;
            Visual.GUI.VisualOptionsWindow.StyleEditorDialog(this, GetStyles);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (GetExperiment == null)
            {
                _wndThesis.Experiment = new Experiment(txtName.Text, txtPath.Text, iudRepeat.Value.Value,
                    new InstanceDefinition(dudTimePerStep.Value.Value, iudLength.Value.Value, _scene, _species, _archetypes),
                    _styles, (uint)dudSeed.Value.Value);
            }
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void spbSpecies_Click(object sender, RoutedEventArgs e)
        {
            OpenEditorDialog(new ThesisSpeciesEditorWindow(GetSpecies));
        }

        private void spbScene_Click(object sender, RoutedEventArgs e)
        {
            OpenEditorDialog(new ThesisSceneEditorWindow(GetScene, GetSpecies));
        }

        private void spbArchetypes_Click(object sender, RoutedEventArgs e)
        {
            OpenEditorDialog(new ThesisArchetypesEditorWindow(GetArchetypes, GetScene, GetSpecies));
        }

        #endregion

        #region Methods

        private void OpenEditorDialog(Window editor)
        {
            editor.Owner = this;
            editor.ShowDialog();
        }

        #endregion
    }
}

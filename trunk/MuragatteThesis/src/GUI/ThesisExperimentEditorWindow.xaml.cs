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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisExperimentEditorWindow.xaml
    /// </summary>
    public partial class ThesisExperimentEditorWindow : Window
    {
        #region Fields

        private ThesisMainWindow _wndThesis;
        private List<Visual.Styles.Style> _styles = null;
        private Core.Scene _scene = null;
        private Core.Storage.SpeciesCollection _species = null;
        private List<Core.Environment.AgentArchetype> _archetypes = null;

        #endregion

        #region Constructors

        public ThesisExperimentEditorWindow(ThesisMainWindow main)
        {
            InitializeComponent();
            DataContext = this;

            _wndThesis = main;
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

        #endregion

        #region Events

        private void btnRandomSeed_Click(object sender, RoutedEventArgs e)
        {
            dudSeed.Value = _wndThesis.Random.UInt();
        }

        private void btnStyleEditor_Click(object sender, RoutedEventArgs e)
        {
            Visual.DefaultValues.Scale = 5;
            _styles = new List<Visual.Styles.Style>(Visual.GUI.VisualOptionsWindow.StyleEditorDialog(GetExperiment == null ? null : GetExperiment.Styles));
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _wndThesis.Experiment = new Experiment(txtName.Text, txtPath.Text, iudRepeat.Value.Value,
                new InstanceDefinition(dudTimePerStep.Value.Value, iudLength.Value.Value, _scene, _species, _archetypes),
                _styles, (uint)dudSeed.Value.Value);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}

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
using Muragatte.Visual;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisExperimentResultsWindow.xaml
    /// </summary>
    public partial class ThesisExperimentResultsWindow : Window
    {
        private Experiment _experiment = null;
        private Visualization _visual = null;

        public ThesisExperimentResultsWindow(Experiment experiment)
        {
            InitializeComponent();
            DataContext = this;

            _experiment = experiment;
        }

        public Experiment GetExperiment
        {
            get { return _experiment; }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnReplay_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInstance.SelectedIndex >= 0)
            {
                if (_visual != null) _visual.Close();
                Instance instance = (Instance)cmbInstance.SelectedItem;
                _visual = new Visualization(instance.Model, instance.Model.Region.Width, instance.Model.Region.Height, dudScale.Value.Value, this, _experiment.Styles);
                _visual.Initialize();
            }
        }
    }
}

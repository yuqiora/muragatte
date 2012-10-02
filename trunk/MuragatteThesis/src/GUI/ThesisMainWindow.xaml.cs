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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Muragatte.Random;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisMainWindow.xaml
    /// </summary>
    public partial class ThesisMainWindow : Window
    {
        private RandomMT _random = new RandomMT();
        private Experiment _experiment = null;

        public ThesisMainWindow()
        {
            InitializeComponent();
        }

        public Experiment Experiment
        {
            get { return _experiment; }
            set { _experiment = value; }
        }

        public RandomMT Random
        {
            get { return _random; }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            ThesisAboutWindow about = new ThesisAboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ThesisExperimentEditorWindow editor = new ThesisExperimentEditorWindow(this);
            editor.Show();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (_experiment != null)
            {
                binProgress.IsBusy = true;
                _experiment.Run();
                binProgress.IsBusy = false;
            }
        }
    }
}

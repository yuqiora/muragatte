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
using Muragatte.Visual;

namespace Muragatte.Research.GUI
{
    /// <summary>
    /// Interaction logic for ExperimentResultsWindow.xaml
    /// </summary>
    public partial class ExperimentResultsWindow : Window
    {
        #region Fields

        private Experiment _experiment = null;
        private Visualization _visual = null;

        #endregion

        #region Constructors

        public ExperimentResultsWindow(Experiment experiment)
        {
            _experiment = experiment;

            InitializeComponent();
            DataContext = _experiment;
        }

        #endregion

        #region Events

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnStepNavInstanceFirst_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavInstance, 0);
        }

        private void btnStepNavInstancePrev_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavInstance, cmbStepNavInstance.SelectedIndex - 1);
        }

        private void btnStepNavInstanceNext_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavInstance, cmbStepNavInstance.SelectedIndex + 1);
        }

        private void btnStepNavInstanceLast_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavInstance, cmbStepNavInstance.Items.Count - 1);
        }

        private void btnStepNavStepFirst_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavStep, 0);
        }

        private void btnStepNavStepPrev_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavStep, cmbStepNavStep.SelectedIndex - 1);
        }

        private void btnStepNavStepNext_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavStep, cmbStepNavStep.SelectedIndex + 1);
        }

        private void btnStepNavStepLast_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbStepNavStep, cmbStepNavStep.Items.Count - 1);
        }

        private void btnStepGroupsMain_Click(object sender, RoutedEventArgs e)
        {
            cmbStepGroups.SelectedItem = btnStepGroupsMain.Tag;
        }

        private void btnInstanceNavFirst_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbInstanceNav, 0);
        }

        private void btnInstanceNavPrev_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbInstanceNav, cmbInstanceNav.SelectedIndex - 1);
        }

        private void btnInstanceNavNext_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbInstanceNav, cmbInstanceNav.SelectedIndex + 1);
        }

        private void btnInstanceNavLast_Click(object sender, RoutedEventArgs e)
        {
            ChangeComboBoxSelectedIndex(cmbInstanceNav, cmbInstanceNav.Items.Count - 1);
        }

        private void btnInstanceVisualize_Click(object sender, RoutedEventArgs e)
        {
            if (_visual != null) _visual.Close();
            MultiAgentSystem mas = _experiment.Instances[(int)btnInstanceVisualize.Tag].Model;
            double scaleWidth = Math.Floor(SystemParameters.WorkArea.Width / mas.Region.Width);
            double scaleHeight = Math.Floor(SystemParameters.WorkArea.Height / mas.Region.Height);
            _visual = new Visualization(mas, Math.Min(scaleWidth, scaleHeight), this, _experiment.Styles);
            _visual.Initialize();
        }

        #endregion

        #region Methods

        private void ChangeComboBoxSelectedIndex(ComboBox comboBox, int index)
        {
            if (index >= 0 && index < comboBox.Items.Count) comboBox.SelectedIndex = index;
        }

        #endregion
    }
}

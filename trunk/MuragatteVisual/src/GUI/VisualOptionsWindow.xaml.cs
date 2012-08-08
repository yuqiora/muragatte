// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace Muragatte.GUI
{
    /// <summary>
    /// Interaction logic for VisualOptionsWindow.xaml
    /// </summary>
    public partial class VisualOptionsWindow : Window
    {
        #region Fields

        private Visualization _visualization;

        private readonly Color _defaultBackgroundColor = Colors.White;

        #endregion

        #region Constructors

        public VisualOptionsWindow(Visualization visualization)
        {
            InitializeComponent();
            _visualization = visualization;
            Binding bindVisPlay = new Binding("IsPlaying");
            bindVisPlay.Source = _visualization.GetPlayback;
            bindVisPlay.Converter = new InverseBoolConverter();
            titGroups.SetBinding(TabItem.IsEnabledProperty, bindVisPlay);
            titSnapshot.SetBinding(TabItem.IsEnabledProperty, bindVisPlay);
            ccBackgroundColor.SelectedColor = _defaultBackgroundColor;
            BindVisualizationSelection();
        }

        #endregion

        #region Events

        private void btnDefaultBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            ccBackgroundColor.SelectedColor = _defaultBackgroundColor;
        }

        #endregion

        #region Methods

        private void BindVisualizationSelection()
        {
            BindVisualizationSelection(chbGeneralEnvironment, "IsEnvironmentEnabled");
            BindVisualizationSelection(chbGeneralNeighbourhoods, "IsNeighbourhoodsEnabled");
            BindVisualizationSelection(chbGeneralTracks, "IsTracksEnabled");
            BindVisualizationSelection(chbGeneralTrails, "IsTrailsEnabled");
            BindVisualizationSelection(chbGeneralAgents, "IsAgentsEnabled");
            BindVisualizationSelection(chbGeneralCentroids, "IsCentroidsEnabled");
            chbGeneralEnvironment.IsChecked = true;
            chbGeneralAgents.IsChecked = true;
        }

        private void BindVisualizationSelection(CheckBox target, string propertyName)
        {
            Binding bind = new Binding(propertyName);
            bind.Mode = BindingMode.OneWayToSource;
            bind.Source = _visualization.GetCanvas;
            BindingOperations.SetBinding(target, CheckBox.IsCheckedProperty, bind);
        }

        #endregion
    }
}

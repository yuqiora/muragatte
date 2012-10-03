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
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.Thesis.GUI
{
    /// <summary>
    /// Interaction logic for ThesisSceneEditorWindow.xaml
    /// </summary>
    public partial class ThesisSceneEditorWindow : Window
    {
        #region Fields

        private Scene _scene = null;
        private SpeciesCollection _species = null;

        #endregion

        #region Constructors

        public ThesisSceneEditorWindow(Scene scene, SpeciesCollection species)
        {
            InitializeComponent();
            DataContext = this;

            _scene = scene ?? new Scene(new Region());
            _species = species;

            iudRegionWidth.Value = _scene.Region.Width;
            iudRegionHeight.Value = _scene.Region.Height;
            chbHorizontalBorders.IsChecked = _scene.Region.IsBorderedHorizontally;
            chbVerticalBorders.IsChecked = _scene.Region.IsBorderedVertically;
        }

        #endregion

        #region Properties

        public Scene GetScene
        {
            get { return _scene; }
        }

        public SpeciesCollection GetSpecies
        {
            get { return _species; }
        }

        #endregion

        #region Events

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSpawnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lboSpawn.SelectedIndex >= 0)
            {
                _scene.SpawnSpots.RemoveAt(lboSpawn.SelectedIndex);
            }
        }

        private void btnSpawnNewPoint_Click(object sender, RoutedEventArgs e)
        {
            NewSpawnSpot(new PointSpawnSpot(Vector2.Zero));
        }

        private void btnSpawnNewEllipse_Click(object sender, RoutedEventArgs e)
        {
            NewSpawnSpot(new EllipseSpawnSpot(Vector2.Zero, 5, 5));
        }

        private void btnSpawnNewRectangle_Click(object sender, RoutedEventArgs e)
        {
            NewSpawnSpot(new RectangleSpawnSpot(Vector2.Zero, 5, 5));
        }

        private void btnStationaryDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lboStationary.SelectedIndex >= 0)
            {
                _scene.StationaryElements.RemoveAt(lboStationary.SelectedIndex);
            }
        }

        private void btnStationaryNewGoalPosition_Click(object sender, RoutedEventArgs e)
        {
            _scene.StationaryElements.Add(new PositionGoal(null, Vector2.Zero, null));
            lboStationary.SelectedIndex = lboStationary.Items.Count - 1;
        }

        private void btnStationaryNewObstacleEllipse_Click(object sender, RoutedEventArgs e)
        {
            _scene.StationaryElements.Add(new EllipseObstacle(null, Vector2.Zero, null, 5, 5));
            lboStationary.SelectedIndex = lboStationary.Items.Count - 1;
        }

        private void btnRegionApply_Click(object sender, RoutedEventArgs e)
        {
            _scene.Region.Width = iudRegionWidth.Value.Value;
            _scene.Region.Height = iudRegionHeight.Value.Value;
            _scene.Region.IsBorderedHorizontally = chbHorizontalBorders.IsChecked.Value;
            _scene.Region.IsBorderedVertically = chbVerticalBorders.IsChecked.Value;
        }

        #endregion

        #region Methods

        private void NewSpawnSpot(SpawnSpot s)
        {
            _scene.SpawnSpots.Add(s);
            lboSpawn.SelectedIndex = lboSpawn.Items.Count - 1;
        }

        private void NewStationaryElement(Element e)
        {
            _scene.StationaryElements.Add(e);
            lboStationary.SelectedIndex = lboStationary.Items.Count - 1;
        }

        #endregion
    }
}

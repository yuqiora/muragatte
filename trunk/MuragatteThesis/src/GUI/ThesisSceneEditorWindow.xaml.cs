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
using Muragatte.Visual;

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
        private ScenePreview _preview;

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
            CreatePreview(_scene.Region.Width, _scene.Region.Height);
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

        private Vector2 PreviewCenter
        {
            get { return new Vector2(_preview.UnitWidth / 2, _preview.UnitHeight / 2); }
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
            NewSpawnSpot(new PointSpawnSpot(PreviewCenter));
        }

        private void btnSpawnNewEllipse_Click(object sender, RoutedEventArgs e)
        {
            NewSpawnSpot(new EllipseSpawnSpot(PreviewCenter, 5, 5));
        }

        private void btnSpawnNewRectangle_Click(object sender, RoutedEventArgs e)
        {
            NewSpawnSpot(new RectangleSpawnSpot(PreviewCenter, 5, 5));
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
            NewStationaryElement(new PositionGoal(NewStationaryElementID(), null, PreviewCenter, null));
        }

        private void btnStationaryNewGoalArea_Click(object sender, RoutedEventArgs e)
        {
            NewStationaryElement(new AreaGoal(NewStationaryElementID(), null, PreviewCenter, null, 5, 5));
        }

        private void btnStationaryNewObstacleEllipse_Click(object sender, RoutedEventArgs e)
        {
            NewStationaryElement(new EllipseObstacle(NewStationaryElementID(), null, PreviewCenter, null, 5, 5));
        }

        private void btnStationaryNewObstacleRectangle_Click(object sender, RoutedEventArgs e)
        {
            NewStationaryElement(new RectangleObstacle(NewStationaryElementID(), null, PreviewCenter, null, 5, 5));
        }

        private void btnStationaryNewAttractSpot_Click(object sender, RoutedEventArgs e)
        {
            NewStationaryElement(new AttractSpot(NewStationaryElementID(), null, PreviewCenter, null));
        }

        private void btnStationaryNewRepelSpot_Click(object sender, RoutedEventArgs e)
        {
            NewStationaryElement(new RepelSpot(NewStationaryElementID(), null, PreviewCenter, null));
        }

        private void btnStationaryNewGuidepost_Click(object sender, RoutedEventArgs e)
        {
            NewStationaryElement(new Guidepost(NewStationaryElementID(), null, PreviewCenter, Vector2.X0Y1, null));
        }

        private void btnRegionApply_Click(object sender, RoutedEventArgs e)
        {
            _scene.Region.Width = iudRegionWidth.Value.Value;
            _scene.Region.Height = iudRegionHeight.Value.Value;
            _scene.Region.IsBorderedHorizontally = chbHorizontalBorders.IsChecked.Value;
            _scene.Region.IsBorderedVertically = chbVerticalBorders.IsChecked.Value;
            CreatePreview(iudRegionWidth.Value.Value, iudRegionHeight.Value.Value);
        }

        private void btnRescale_Click(object sender, RoutedEventArgs e)
        {
            _preview.Rescale(dudScale.Value.Value);
            SetPreviewImage();
            RedrawPreview();
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RedrawPreview();
        }

        #endregion

        #region Methods

        private void NewSpawnSpot(SpawnSpot s)
        {
            ddbSpawnNew.IsOpen = false;
            _scene.SpawnSpots.Add(s);
            lboSpawn.SelectedIndex = lboSpawn.Items.Count - 1;
            RedrawPreview();
        }

        private void NewStationaryElement(Element e)
        {
            ddbStationaryNew.IsOpen = false;
            _scene.StationaryElements.Add(e);
            lboStationary.SelectedIndex = lboStationary.Items.Count - 1;
            RedrawPreview();
        }

        private int NewStationaryElementID()
        {
            return _scene.StationaryElements.Count > 0 ? _scene.StationaryElements.Last().ID + 1 : 0;
        }

        private void CreatePreview(int width, int height)
        {
            _preview = new ScenePreview(width, height);
            SetPreviewImage();
            RedrawPreview();
        }

        private void SetPreviewImage()
        {
            imgPreview.Width = _preview.PixelWidth;
            imgPreview.Height = _preview.PixelHeight;
            imgPreview.Source = _preview.Image;
        }

        private void RedrawPreview()
        {
            _preview.Clear();
            _preview.DrawStationaryElement(_scene.StationaryElements);
            _preview.DrawSpawnSpot(_scene.SpawnSpots);
        }

        #endregion
    }
}

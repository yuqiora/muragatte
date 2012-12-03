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
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.IO;
using Muragatte.Research.IO;

namespace Muragatte.Research.GUI
{
    /// <summary>
    /// Interaction logic for SceneEditorWindow.xaml
    /// </summary>
    public partial class SceneEditorWindow : Window
    {
        #region Fields

        private Scene _scene = null;
        private SpeciesCollection _species = null;
        private ScenePreview _preview;

        private XmlSceneArchiver _xml = null;

        #endregion

        #region Constructors

        public SceneEditorWindow(Scene scene, SpeciesCollection species)
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

            _xml = new XmlSceneArchiver(this);
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
            NewSpawnSpot(new PointSpawnSpot("Point", PreviewCenter));
        }

        private void btnSpawnNewEllipse_Click(object sender, RoutedEventArgs e)
        {
            NewSpawnSpot(new EllipseSpawnSpot("Ellipse", PreviewCenter, 5, 5));
        }

        private void btnSpawnNewRectangle_Click(object sender, RoutedEventArgs e)
        {
            NewSpawnSpot(new RectangleSpawnSpot("Rectangle", PreviewCenter, 5, 5));
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
            NewStationaryElement(new Guidepost(NewStationaryElementID(), null, PreviewCenter, Vector2.X1Y0, null));
        }

        private void RegionSize_LostFocus(object sender, RoutedEventArgs e)
        {
            CreatePreview(_scene.Region.Width, _scene.Region.Height);
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

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            _xml.Load();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _xml.Save(new XmlSceneRoot(_scene));
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

        public void LoadScene(XmlScene xs)
        {
            _scene.Load(xs.Region, xs.SpawnSpots, xs.StationaryElements);
            CreatePreview(_scene.Region.Width, _scene.Region.Height);
        }

        #endregion
    }
}

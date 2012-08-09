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

        private Visualization _visual;

        private readonly Color _defaultBackgroundColor = Colors.White;
        private readonly int _iUnitWidth;
        private readonly int _iUnitHeight;

        #endregion

        #region Constructors

        public VisualOptionsWindow(Visualization visualization)
        {
            InitializeComponent();
            _visual = visualization;
            Binding bindVisPlay = new Binding("IsPlaying");
            bindVisPlay.Source = _visual.GetPlayback;
            bindVisPlay.Converter = new InverseBoolConverter();
            titGroups.SetBinding(TabItem.IsEnabledProperty, bindVisPlay);
            titSnapshot.SetBinding(TabItem.IsEnabledProperty, bindVisPlay);
            ccBackgroundColor.SelectedColor = _defaultBackgroundColor;
            BindVisualizationSelection();
            _iUnitWidth = _visual.GetCanvas.UnitWidth;
            _iUnitHeight = _visual.GetCanvas.UnitHeight;
            FillWidthHeight(lblUnitSize, _iUnitWidth, _iUnitHeight);
            dudScale.Value = _visual.GetCanvas.Scale;
            dudScale.Tag = _visual.GetCanvas.Scale;

            TestingShapes();
        }

        #endregion

        #region Events

        private void btnDefaultBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            ccBackgroundColor.SelectedColor = _defaultBackgroundColor;
        }

        private void dudScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FillWidthHeight(lblPixelSize, (int)(dudScale.Value * _iUnitWidth), (int)(dudScale.Value * _iUnitHeight));
        }

        private void btnRescaleApply_Click(object sender, RoutedEventArgs e)
        {
            //rescaling & redraw
            dudScale.Tag = dudScale.Value;
        }

        private void btnRescaleCancel_Click(object sender, RoutedEventArgs e)
        {
            RevertScale();
        }

        private void tabOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RevertScale();
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
            bind.Source = _visual.GetCanvas;
            target.SetBinding(CheckBox.IsCheckedProperty, bind);
        }

        private void FillWidthHeight(Label label, int width, int height)
        {
            label.Content = string.Format("{0} x {1}", width, height);
        }

        private void RevertScale()
        {
            dudScale.Value = (double)dudScale.Tag;
        }

        #endregion

        #region TEST - SHAPES

        private WriteableBitmap _wb;

        private void TestingShapes()
        {
            CreateShapeList();
            _wb = BitmapFactory.New(200, 200);
            imgShapePreview.Source = _wb;
        }

        private void CreateShapeList()
        {
            cobShape.Items.Add(PixelShape.Instance());
            cobShape.Items.Add(QuadPixelShape.Instance());
            cobShape.Items.Add(EllipseShape.Instance());
            cobShape.Items.Add(RectangleShape.Instance());
            cobShape.Items.Add(TriangleShape.Instance());
            cobShape.Items.Add(PointingCircleShape.Instance());
        }

        private void PreviewShape()
        {
            if (cobShape.HasItems && cobShape.SelectedItem != null)
            {
                _wb.Clear();
                ((Visual.Shape)cobShape.SelectedItem).Draw(
                    _wb, new Common.Vector2(100, 100), new Common.Angle(sldAngle.Value),
                    cpiPrimaryColor.SelectedColor, cpiSecondaryColor.SelectedColor,
                    iudShapeWidth.Value.Value, iudShapeHeight.Value.Value);
            }
        }

        private void cobShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PreviewShape();
        }

        private void sldAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PreviewShape();
        }

        private void cpiPrimaryColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            PreviewShape();
        }

        private void cpiSecondaryColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            PreviewShape();
        }

        private void iudShapeWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PreviewShape();
        }

        private void iudShapeHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PreviewShape();
        }

        #endregion
    }
}

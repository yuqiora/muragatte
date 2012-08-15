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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Muragatte.Core.Environment;
using Muragatte.Visual;
using Muragatte.Visual.Shapes;
using Muragatte.Visual.Styles;
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
        private ObservableCollection<Appearance> _appearances = new ObservableCollection<Appearance>();
        private ObservableCollection<Visual.Styles.Style> _styles = new ObservableCollection<Visual.Styles.Style>();
        private CollectionView _appearancesEnvironmentView = null;
        private CollectionView _appearancesAgentsView = null;

        #region Default Values

        private readonly Color _defaultBackgroundColor = Colors.White;
        private readonly Color _defaultAgentColor = Colors.Black;
        private readonly Color _defaultObstacleColor = Colors.Gray;
        private readonly Color _defaultGoalColor = Colors.Red;
        private readonly Color _defaultNeighbourhoodColor = Colors.LightGreen;
        private readonly Color _defaultCentroidColor = Colors.Silver;
        private readonly Color _defaultHighlightColor = Colors.LightYellow;
        
        private readonly int _iDefaultTrailLength = 10;
        private readonly Visual.Shapes.Shape _defaultShape = EllipseShape.Instance();
        private readonly Visual.Shapes.Shape _defaultAgentShape = PointingCircleShape.Instance();

        #endregion

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
            clbSpeciesList.ItemsSource = _visual.GetModel.Species;

            CreateShapeList(cobAgentsShape);

            TestingShapes();

            CreateDefaultStyles();
            _visual.GetModel.Elements.CollectionChanged += ModelElementStorageUpdated;

            _appearancesEnvironmentView = new CollectionView(_appearances);
            _appearancesEnvironmentView.Filter += new Predicate<object>(FilterIsStationary);
            clbEnvironmentEnabled.ItemsSource = _appearancesEnvironmentView;
            _appearancesAgentsView = new CollectionView(_appearances);
            _appearancesAgentsView.Filter += new Predicate<object>(FilterIsAgent);
            clbAgentsEnabled.ItemsSource = _appearancesAgentsView;
            clbNeighbourhoodsEnabled.ItemsSource = _appearancesAgentsView;
            clbTracksEnabled.ItemsSource = _appearancesAgentsView;
            clbTrailsEnabled.ItemsSource = _appearancesAgentsView;
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
            if ((double)dudScale.Tag != dudScale.Value.Value)
            {
                foreach (Visual.Styles.Style s in _styles)
                {
                    s.Rescale(dudScale.Value.Value);
                }
                _visual.GetCanvas.Rescale(dudScale.Value.Value);
                dudScale.Tag = dudScale.Value.Value;
            }
        }

        private void btnRescaleCancel_Click(object sender, RoutedEventArgs e)
        {
            RevertScale();
        }

        private void tabOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RevertScale();
        }

        private void chbAgentsEnabledAll_Checked(object sender, RoutedEventArgs e)
        {
            //clbAgentsEnabled.SelectedItems = clbAgentsEnabled.Items;
        }

        private void chbAgentsEnabledAll_Unchecked(object sender, RoutedEventArgs e)
        {
            //foreach
            //clbAgentsEnabled.SelectedItems.Clear();
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
            CreateShapeList(cobShape);
            _wb = BitmapFactory.New(200, 200);
            imgShapePreview.Source = _wb;
        }

        private void CreateShapeList(ComboBox comboBox)
        {
            comboBox.Items.Add(PixelShape.Instance());
            comboBox.Items.Add(QuadPixelShape.Instance());
            comboBox.Items.Add(EllipseShape.Instance());
            comboBox.Items.Add(RectangleShape.Instance());
            comboBox.Items.Add(TriangleShape.Instance());
            comboBox.Items.Add(PointingCircleShape.Instance());
            comboBox.Items.Add(ArcShape.Instance());
        }

        private void PreviewShape()
        {
            if (cobShape.HasItems && cobShape.SelectedItem != null)
            {
                _wb.Clear(ccBackgroundColor.SelectedColor);
                ((Visual.Shapes.Shape)cobShape.SelectedItem).Draw(
                    _wb, new Common.Vector2(100, 100), new Common.Angle(sldAngle.Value),
                    cpiPrimaryColor.SelectedColor, cpiSecondaryColor.SelectedColor,
                    iudShapeWidth.Value.Value, iudShapeHeight.Value.Value, new Common.Angle(sldArc.Value));
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

        private void sldArc_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PreviewShape();
        }

        #endregion

        private void lboAgentsStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ModelElementStorageUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _appearances.Clear();
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    _appearances.Add(ElementToAppearance(item as Element));
                }
            }
        }

        private void CreateDefaultStyles()
        {
            _styles.Add(new Visual.Styles.Style(PointingCircleShape.Instance(), "Agents", 1, 1, _visual.GetCanvas.Scale, Colors.Transparent, Colors.Black,
                new NeighbourhoodStyle(ArcShape.Instance(), _defaultNeighbourhoodColor, Colors.Transparent, 5, new Angle(135), _visual.GetCanvas.Scale),
                new TrackStyle(Colors.Black), new TrailStyle(Colors.Black, _iDefaultTrailLength)));
            _styles.Add(new Visual.Styles.Style(EllipseShape.Instance(), "Obstacles", 1, 1, _visual.GetCanvas.Scale, _defaultObstacleColor, Colors.Transparent,
                null, null, null));
            _styles.Add(new Visual.Styles.Style(RectangleShape.Instance(), "Goals", 1, 1, _visual.GetCanvas.Scale, _defaultGoalColor, Colors.Transparent,
                null, null, null));
            _styles.Add(new Visual.Styles.Style(TriangleShape.Instance(), "Centroids", 1, 1, _visual.GetCanvas.Scale, Colors.Transparent, _defaultCentroidColor,
                null, null, null));
        }

        private Appearance ElementToAppearance(Element e)
        {
            Visual.Styles.Style style = null;
            if (e is Agent) style = _styles[0];
            if (e is Obstacle) style = _styles[1];
            if (e is Goal) style = _styles[2];
            if (e is Centroid) style = _styles[3];
            return new Appearance(e, style, _visual.GetCanvas.Scale);
        }

        private bool FilterIsStationary(object o)
        {
            Appearance a = o as Appearance;
            return a.IsStationary;
        }

        private bool FilterIsAgent(object o)
        {
            Appearance a = o as Appearance;
            return a.IsAgent;
        }
    }
}

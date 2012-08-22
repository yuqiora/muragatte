﻿// ------------------------------------------------------------------------
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
using System.ComponentModel;
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
        private CollectionViewSource _environmentView = new CollectionViewSource();
        private CollectionViewSource _agentsView = new CollectionViewSource();
        private CollectionViewSource _centroidsView = new CollectionViewSource();
        private CollectionViewSource _enabledElementsView = new CollectionViewSource();

        private WriteableBitmap _stylePreview = null;

        private readonly List<Visual.Shapes.Shape> _shapes = null;

        #endregion

        #region Constructors

        public VisualOptionsWindow(Visualization visualization)
        {
            InitializeComponent();
            DataContext = this;
            _visual = visualization;
            
            Binding bindVisPlay = new Binding("IsPlaying");
            bindVisPlay.Source = _visual.GetPlayback;
            bindVisPlay.Converter = new InverseBoolConverter();
            titGroups.SetBinding(TabItem.IsEnabledProperty, bindVisPlay);
            titSnapshot.SetBinding(TabItem.IsEnabledProperty, bindVisPlay);
            BindVisualizationSelection();
            FillWidthHeight(lblUnitSize, _visual.GetCanvas.UnitWidth, _visual.GetCanvas.UnitHeight);
            dudScale.Value = _visual.GetCanvas.Scale;
            dudScale.Tag = _visual.GetCanvas.Scale;
            Binding bindBackgroundColor = new Binding("BackgroundColor");
            bindBackgroundColor.Source = _visual.GetCanvas;
            ccBackgroundColor.SetBinding(ColorCanvas.SelectedColorProperty, bindBackgroundColor);

            //clbSpeciesList.ItemsSource = _visual.GetModel.Species;

            _shapes = CreateShapeList();

            //_defaultNeighbourhoodColor.A = 64;

            CreateDefaultStyles();
            _visual.GetModel.Elements.CollectionChanged += ModelElementStorageUpdated;

            SetViews();

            _stylePreview = BitmapFactory.New((int)imgStylesPreview.Width, (int)imgStylesPreview.Height);
            imgStylesPreview.Source = _stylePreview;
        }

        #endregion

        #region Properties

        public ICollectionView EnvironmentView
        {
            get { return _environmentView.View; }
        }

        public ICollectionView AgentsView
        {
            get { return _agentsView.View; }
        }

        public ICollectionView CentroidsView
        {
            get { return _centroidsView.View; }
        }

        public ICollectionView EnabledElementsView
        {
            get { return _enabledElementsView.View; }
        }

        public List<Visual.Shapes.Shape> GetShapes
        {
            get { return _shapes; }
        }

        public ObservableCollection<Visual.Styles.Style> GetStyles
        {
            get { return _styles; }
        }

        #endregion

        #region Events

        private void btnDefaultBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            ccBackgroundColor.SelectedColor = DefaultValues.BACKGROUND_COLOR;
        }

        private void dudScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FillWidthHeight(lblPixelSize, (int)(dudScale.Value * _visual.GetCanvas.UnitWidth), (int)(dudScale.Value * _visual.GetCanvas.UnitHeight));
        }

        private void btnRescaleApply_Click(object sender, RoutedEventArgs e)
        {
            if ((double)dudScale.Tag != dudScale.Value.Value)
            {
                DefaultValues.Scale = dudScale.Value.Value;
                foreach (Visual.Styles.Style s in _styles)
                {
                    s.Rescale(dudScale.Value.Value);
                }
                foreach (Appearance a in _appearances)
                {
                    a.Rescale(dudScale.Value.Value);
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

        private void btnStylesDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedStyle();
        }

        private void btnStylesNew_Click(object sender, RoutedEventArgs e)
        {
            _styles.Add(new Visual.Styles.Style());
            lboStyleEditorList.SelectedIndex = lboStyleEditorList.Items.Count - 1;
        }

        private void btnStylesCopy_Click(object sender, RoutedEventArgs e)
        {
            _styles.Add(new Visual.Styles.Style(lboStyleEditorList.SelectedItem as Visual.Styles.Style));
            lboStyleEditorList.SelectedIndex = lboStyleEditorList.Items.Count - 1;
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

        private void SetViews()
        {
            _environmentView.Source = _appearances;
            EnvironmentView.Filter += new Predicate<object>(FilterIsStationary);
            _agentsView.Source = _appearances;
            AgentsView.Filter += new Predicate<object>(FilterIsAgent);
            _centroidsView.Source = _appearances;
            CentroidsView.Filter += new Predicate<object>(FilterIsType<Centroid>);
            _enabledElementsView.Source = _appearances;
            EnabledElementsView.Filter += new Predicate<object>(FilterIsEnabled);
        }

        private void CreateDefaultStyles()
        {
            _styles.Add(new Visual.Styles.Style(
                PointingCircleShape.Instance(), "Agent", 1, 1,
                Colors.Transparent, DefaultValues.AGENT_COLOR,
                new NeighbourhoodStyle(
                    ArcShape.Instance(),
                    Colors.Transparent, DefaultValues.NEIGHBOURHOOD_COLOR,
                    5, new Angle(DefaultValues.NEIGHBOURHOOD_ANGLE_DEGREES), _visual.GetCanvas.Scale),
                new TrackStyle(DefaultValues.AGENT_COLOR),
                new TrailStyle(DefaultValues.AGENT_COLOR, DefaultValues.TRAIL_LENGTH)));
            _styles.Add(new Visual.Styles.Style(
                EllipseShape.Instance(), "Obstacle", 1, 1,
                DefaultValues.OBSTACLE_COLOR, Colors.Transparent,
                null, null, null));
            _styles.Add(new Visual.Styles.Style(
                RectangleShape.Instance(), "Goal", 1, 1,
                DefaultValues.GOAL_COLOR, Colors.Transparent,
                null, null, null));
            _styles.Add(new Visual.Styles.Style(
                TriangleShape.Instance(), "Centroid", 1, 1,
                DefaultValues.CENTROID_COLOR, Colors.Transparent,
                null, new TrackStyle(DefaultValues.CENTROID_COLOR),
                new TrailStyle(DefaultValues.CENTROID_COLOR, DefaultValues.TRAIL_LENGTH)));
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

        private bool FilterIsType<T>(object o) where T : Element
        {
            Appearance a = o as Appearance;
            return a.IsType<T>();
        }

        private bool FilterIsEnabled(object o)
        {
            Appearance a = o as Appearance;
            return a.IsEnabled;
        }

        private List<Visual.Shapes.Shape> CreateShapeList()
        {
            List<Visual.Shapes.Shape> items = new List<Visual.Shapes.Shape>();
            items.Add(PixelShape.Instance());
            items.Add(QuadPixelShape.Instance());
            items.Add(EllipseShape.Instance());
            items.Add(RectangleShape.Instance());
            items.Add(TriangleShape.Instance());
            items.Add(PointingCircleShape.Instance());
            items.Add(ArcShape.Instance());
            return items;
        }

        private void DeleteSelectedStyle()
        {
            int index = lboStyleEditorList.SelectedIndex;
            if (index >= 0)
            {
                _styles.RemoveAt(index);
            }
        }

        #endregion

        private void RedrawStylePreview()
        {

        }
    }
}

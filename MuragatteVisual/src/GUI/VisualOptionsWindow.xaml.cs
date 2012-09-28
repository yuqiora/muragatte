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
using Muragatte.Visual.Shapes;
using Muragatte.Visual.Styles;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace Muragatte.Visual.GUI
{
    /// <summary>
    /// Interaction logic for VisualOptionsWindow.xaml
    /// </summary>
    public partial class VisualOptionsWindow : Window
    {
        #region Fields

        private Visualization _visual;
        private ObservableCollection<Appearance> _appearances = new ObservableCollection<Appearance>();
        private ObservableCollection<Styles.Style> _styles = new ObservableCollection<Styles.Style>();
        private CollectionViewSource _environmentView = new CollectionViewSource();
        private CollectionViewSource _agentsView = new CollectionViewSource();
        private CollectionViewSource _neighbourhoodsView = new CollectionViewSource();
        private CollectionViewSource _tracksView = new CollectionViewSource();
        private CollectionViewSource _trailsView = new CollectionViewSource();
        private CollectionViewSource _centroidsView = new CollectionViewSource();
        private CollectionViewSource _enabledElementsView = new CollectionViewSource();

        private List<ICollectionView> _views = new List<ICollectionView>();

        private WriteableBitmap _wbStylePreview = null;
        private Vector2 _stylePreviewCenter = Vector2.Zero;

        private HistoryViewer _historyViewer = null;

        private readonly List<Shapes.Shape> _shapes = null;

        #endregion

        #region Constructors

        public VisualOptionsWindow(Visualization visualization)
        {
            InitializeComponent();
            DataContext = this;
            _visual = visualization;
            _historyViewer = new HistoryViewer(_visual.GetModel.History, _visual.GetPlayback);
            
            FillWidthHeight(lblUnitSize, _visual.GetCanvas.UnitWidth, _visual.GetCanvas.UnitHeight);
            dudScale.Value = _visual.GetCanvas.Scale;
            dudScale.Tag = _visual.GetCanvas.Scale;

            _shapes = CreateShapeList();

            CreateDefaultStyles();
            _visual.GetModel.Elements.CollectionChanged += ModelElementStorageUpdated;
            SetViews();

            _wbStylePreview = BitmapFactory.New((int)imgStylesPreview.Width, (int)imgStylesPreview.Height);
            imgStylesPreview.Source = _wbStylePreview;
            _stylePreviewCenter = new Vector2(_wbStylePreview.PixelWidth / 2, _wbStylePreview.PixelHeight / 2);
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

        public ICollectionView NeighbourhoodsView
        {
            get { return _neighbourhoodsView.View; }
        }

        public ICollectionView TracksView
        {
            get { return _tracksView.View; }
        }

        public ICollectionView TrailsView
        {
            get { return _trailsView.View; }
        }

        public ICollectionView CentroidsView
        {
            get { return _centroidsView.View; }
        }

        public ICollectionView EnabledElementsView
        {
            get { return _enabledElementsView.View; }
        }

        public List<Shapes.Shape> GetShapes
        {
            get { return _shapes; }
        }

        public ObservableCollection<Styles.Style> GetStyles
        {
            get { return _styles; }
        }

        public Visualization GetVisual
        {
            get { return _visual; }
        }

        #endregion

        #region Events

        private void btnDefaultBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            ccBackgroundColor.SelectedColor = DefaultValues.BACKGROUND_COLOR;
        }

        private void btnHighlightColorDefault_Click(object sender, RoutedEventArgs e)
        {
            ccHighlightColor.SelectedColor = DefaultValues.HIGHLIGHT_COLOR;
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
                foreach (Styles.Style s in _styles)
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
            if (tabOptions == e.OriginalSource)
            {
                if (e.RemovedItems.Contains(tabOptions.Items[0])) RevertScale();
                if (tabOptions.SelectedIndex > 0 && tabOptions.SelectedIndex < _views.Count)
                {
                    _views[tabOptions.SelectedIndex].Refresh();
                }
            }
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
            _styles.Add(new Styles.Style());
            lboStyleEditorList.SelectedIndex = lboStyleEditorList.Items.Count - 1;
        }

        private void btnStylesCopy_Click(object sender, RoutedEventArgs e)
        {
            _styles.Add(new Styles.Style(lboStyleEditorList.SelectedItem as Styles.Style));
            lboStyleEditorList.SelectedIndex = lboStyleEditorList.Items.Count - 1;
        }

        private void StyleEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RedrawStylePreview();
        }

        private void StyleEditor_NumericValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RedrawStylePreview();
        }

        private void StyleEditor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            RedrawStylePreview();
        }

        private void StyleEditor_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            RedrawStylePreview();
        }

        private void VisualItemList_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            RedrawCurrentIfStopped();
        }

        private void VisualItemsEnabled_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            RedrawCurrentIfStopped();
        }

        #endregion

        #region Methods

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
            _neighbourhoodsView.Source = _appearances;
            NeighbourhoodsView.Filter += new Predicate<object>(FilterHasNeighbourhood);
            _tracksView.Source = _appearances;
            TracksView.Filter += new Predicate<object>(FilterHasTrack);
            _trailsView.Source = _appearances;
            TrailsView.Filter += new Predicate<object>(FilterHasTrail);
            _centroidsView.Source = _appearances;
            CentroidsView.Filter += new Predicate<object>(FilterIsType<Centroid>);
            _enabledElementsView.Source = _appearances;
            EnabledElementsView.Filter += new Predicate<object>(FilterIsEnabled);
            FillViews();
        }

        private void CreateDefaultStyles()
        {
            _styles.Add(new Styles.Style(
                PointingCircleShape.Instance, "Agent", 1, 1,
                Colors.Transparent, DefaultValues.AGENT_COLOR,
                new NeighbourhoodStyle(
                    ArcShape.Instance,
                    Colors.Transparent, DefaultValues.NEIGHBOURHOOD_COLOR,
                    5, new Angle(DefaultValues.NEIGHBOURHOOD_ANGLE_DEGREES), _visual.GetCanvas.Scale),
                new TrackStyle(DefaultValues.AGENT_COLOR),
                new TrailStyle(DefaultValues.AGENT_COLOR, DefaultValues.TRAIL_LENGTH)));
            _styles.Add(new Styles.Style(
                EllipseShape.Instance, "Obstacle", 1, 1,
                DefaultValues.OBSTACLE_COLOR, Colors.Transparent,
                null, null, null));
            _styles.Add(new Styles.Style(
                RectangleShape.Instance, "Goal", 1, 1,
                DefaultValues.GOAL_COLOR, Colors.Transparent,
                null, null, null));
            _styles.Add(new Styles.Style(
                TriangleShape.Instance, "Centroid", 1, 1,
                DefaultValues.CENTROID_COLOR, Colors.Transparent,
                null, new TrackStyle(DefaultValues.CENTROID_COLOR),
                new TrailStyle(DefaultValues.CENTROID_COLOR, DefaultValues.TRAIL_LENGTH)));
        }

        private Appearance ElementToAppearance(Element e)
        {
            Styles.Style style = _styles.FirstOrDefault(s => e.Species != null && (s.Name == e.Species.Name || s.Name == e.Species.FullName));
            if (style == null)
            {
                if (e is Agent) style = _styles[0];
                if (e is Obstacle) style = _styles[1];
                if (e is Goal) style = _styles[2];
                if (e is Centroid) style = _styles[3];
            }
            return new Appearance(e, style, _visual.GetCanvas.Scale, _historyViewer);
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
            return (a.IsEnabled && !a.IsType<Centroid>()) || a.IsRepresentativeCentroid;
        }

        private bool FilterHasNeighbourhood(object o)
        {
            Appearance a = o as Appearance;
            return a.Style.HasNeighbourhood;
        }

        private bool FilterHasTrack(object o)
        {
            Appearance a = o as Appearance;
            return a.Style.HasTrack;
        }

        private bool FilterHasTrail(object o)
        {
            Appearance a = o as Appearance;
            return a.Style.HasTrail;
        }

        private List<Visual.Shapes.Shape> CreateShapeList()
        {
            List<Shapes.Shape> items = new List<Shapes.Shape>();
            items.Add(PixelShape.Instance);
            items.Add(QuadPixelShape.Instance);
            items.Add(EllipseShape.Instance);
            items.Add(RectangleShape.Instance);
            items.Add(TriangleShape.Instance);
            items.Add(PointingCircleShape.Instance);
            items.Add(ArcShape.Instance);
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

        private void RedrawStylePreview()
        {
            _wbStylePreview.Clear(_visual.GetCanvas.BackgroundColor);
            Styles.Style selectedStyle = (Styles.Style)lboStyleEditorList.SelectedItem;
            if (selectedStyle != null)
            {
                if (selectedStyle.HasNeighbourhood)
                    selectedStyle.Neighbourhood.Draw(_wbStylePreview, _stylePreviewCenter, Vector2.X0Y1);
                selectedStyle.Draw(_wbStylePreview, _stylePreviewCenter, Vector2.X0Y1);
            }
        }

        private void RedrawCurrentIfStopped()
        {
            if (!_visual.GetPlayback.IsPlaying)
            {
                _visual.Redraw();
            }
        }

        private void FillViews()
        {
            _views.Add(null);
            _views.Add(EnvironmentView);
            _views.Add(AgentsView);
            _views.Add(NeighbourhoodsView);
            _views.Add(TracksView);
            _views.Add(TrailsView);
            _views.Add(CentroidsView);
            _views.Add(EnabledElementsView);
        }

        #endregion
    }
}

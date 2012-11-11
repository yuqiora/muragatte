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
using System.Linq;
using System.Text;
using System.Windows;
using Muragatte.Core;

using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Muragatte.Visual
{
    public class Visualization
    {
        #region Fields

        private MultiAgentSystem _model;
        private Canvas _canvas;
        private GUI.VisualCanvasWindow _wndCanvas;
        private GUI.VisualPlaybackWindow _wndPlayback;
        private GUI.VisualOptionsWindow _wndOptions;

        #endregion

        #region Constructors

        public Visualization(MultiAgentSystem model, double scale, Window owner)
            : this(model, model.Region.Width, model.Region.Height, scale, owner, null) { }

        public Visualization(MultiAgentSystem model, int width, int height, double scale, Window owner)
            : this(model, width, height, scale, owner, null) { }

        public Visualization(MultiAgentSystem model, double scale, Window owner, ObservableCollection<Styles.Style> styles)
            : this(model, model.Region.Width, model.Region.Height, scale, owner, styles) { }

        public Visualization(MultiAgentSystem model, int width, int height, double scale, Window owner, ObservableCollection<Styles.Style> styles)
        {
            DefaultValues.Scale = scale;
            _model = model;
            _canvas = new Canvas(width, height, scale, this);
            _wndCanvas = new GUI.VisualCanvasWindow(this, _canvas);
            _wndCanvas.Owner = owner;
            _wndPlayback = new GUI.VisualPlaybackWindow(this);
            _wndPlayback.Owner = owner;
            _wndOptions = new GUI.VisualOptionsWindow(this, styles);
            _wndOptions.Owner = owner;
        }

        #endregion

        #region Properties

        public MultiAgentSystem GetModel
        {
            get { return _model; }
        }

        public Canvas GetCanvas
        {
            get { return _canvas; }
        }

        public GUI.VisualCanvasWindow GetWindow
        {
            get { return _wndCanvas; }
        }

        public GUI.VisualPlaybackWindow GetPlayback
        {
            get { return _wndPlayback; }
        }

        public GUI.VisualOptionsWindow GetOptions
        {
            get { return _wndOptions; }
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            _canvas.Clear();
            ShowAll();
        }

        public void ShowAll()
        {
            _wndCanvas.Show();
            _wndPlayback.Show();
            _wndOptions.Show();
        }

        public void HideAll()
        {
            _wndCanvas.Hide();
            _wndPlayback.Hide();
            _wndOptions.Hide();
        }

        public void Redraw()
        {
            _canvas.Redraw(_model.History, (int)_wndPlayback.sldFrame.Value);
        }

        public void Redraw(int frame)
        {
            _canvas.Redraw(_model.History, frame);
        }

        public void Close()
        {
            CloseWindow(_wndCanvas);
            CloseWindow(_wndPlayback);
            CloseWindow(_wndOptions);
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        #endregion
    }

    public class Snapshot : Canvas
    {
        #region Fields

        private int _iStep = 0;
        private bool _bFlip = false;
        private bool _bCustom = false;
        private HashSet<string> _stationary = new HashSet<string>();
        private HashSet<string> _agents = new HashSet<string>();
        private HashSet<string> _neighbourhoods = new HashSet<string>();
        private HashSet<string> _tracks = new HashSet<string>();
        private HashSet<string> _trails = new HashSet<string>();

        #endregion

        #region Constructors

        public Snapshot(int width, int height, Visualization visualization) : base(width, height, 1, visualization) { }

        #endregion

        #region Properties

        public int Step
        {
            get { return _iStep; }
            set
            {
                _iStep = value;
                NotifyPropertyChanged("Step");
            }
        }

        public bool DrawFlipped
        {
            get { return _bFlip; }
            set
            {
                _bFlip = value;
                NotifyPropertyChanged("DrawFlipped");
            }
        }

        public bool UseCustomVisuals
        {
            get { return _bCustom; }
            set
            {
                _bCustom = value;
                NotifyPropertyChanged("UseCustomVisuals");
            }
        }

        public new double Scale
        {
            get { return _dScale; }
            set
            {
                _dScale = value;
                NotifyPropertyChanged("Scale");
            }
        }

        public override WriteableBitmap Image
        {
            get { return _bFlip ? _wb.Flip(WriteableBitmapExtensions.FlipMode.Vertical) : _wb; }
        }

        public HashSet<string> Stationary
        {
            get { return _stationary; }
        }

        public HashSet<string> Agents
        {
            get { return _agents; }
        }

        public HashSet<string> Neighbourhoods
        {
            get { return _neighbourhoods; }
        }

        public HashSet<string> Tracks
        {
            get { return _tracks; }
        }

        public HashSet<string> Trails
        {
            get { return _trails; }
        }

        #endregion

        #region Methods

        private void ScaleBack()
        {
            if (_dScale != DefaultValues.Scale)
            {
                _visual.GetOptions.RescaleStyles(DefaultValues.Scale);
                _visual.GetOptions.RescaleAppearances(DefaultValues.Scale);
            }
        }

        private void Customize()
        {
            if (_dScale != DefaultValues.Scale) _visual.GetOptions.RescaleStyles(_dScale);
            foreach (Appearance a in _visual.GetOptions.GetAppearances)
            {
                if (_dScale != DefaultValues.Scale) a.Rescale(_dScale);
                if (a.IsStationary) a.IsEnabled = IsSelected(a, _bEnvironment, _stationary);
                if (a.IsAgent) a.IsEnabled = IsSelected(a, _bAgents, _agents);
                if (a.Style.HasNeighbourhood) a.IsNeighbourhoodEnabled = IsSelected(a, _bNeighbourhoods, _neighbourhoods);
                if (a.Style.HasTrack) a.IsTrackEnabled = IsSelected(a, _bTracks, _tracks);
                if (a.Style.HasTrail) a.IsTrailEnabled = IsSelected(a, _bTrails, _trails);
                if (a.IsType<Core.Environment.Centroid>()) a.IsEnabled = _bCentroids;
            }
        }

        private bool IsSelected(Appearance a, bool enabled, HashSet<string> species)
        {
            return enabled && (species.Count == 0 || species.Any(s => a.Species != null || a.Species.StartsWith(s)));
        }

        private void Rescale()
        {
            if (_dScale != DefaultValues.Scale)
                _wb = BitmapFactory.New((int)(_iUnitWidth * _dScale), (int)(_iUnitHeight * _dScale));
        }

        public override void Redraw(Core.Storage.History history)
        {
            Redraw(history, _iStep);
        }

        public override void Redraw(Core.Storage.History history, int step)
        {
            if (history.Count > 0 && step >= 0 && step < history.Count)
            {
                Rescale();
                if (_bCustom) Customize();
                Clear();
                DrawNeighbourhoods(history[step]);
                DrawEnvironment(history[step]);
                DrawTracks(history, step);
                DrawTrails(history, step);
                DrawAgents(history[step]);
                DrawCentroids(history[step]);
            }
        }

        #endregion
    }
}

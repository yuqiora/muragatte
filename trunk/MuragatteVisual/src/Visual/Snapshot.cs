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
using System.Windows.Media.Imaging;
using Muragatte.Core.Storage;

using System.ComponentModel;
using System.Windows.Media;

namespace Muragatte.Visual
{
    public class Snapshot : Canvas
    {
        #region Constants

        private const string DEFAULT_SEPARATOR = ",";

        #endregion

        #region Fields

        private int _iStep = 0;
        private bool _bFlip = false;
        private bool _bCustom = false;
        private HashSet<string> _stationary = new HashSet<string>();
        private HashSet<string> _agents = new HashSet<string>();
        private HashSet<string> _neighbourhoods = new HashSet<string>();
        private HashSet<string> _tracks = new HashSet<string>();
        private HashSet<string> _trails = new HashSet<string>();
        private readonly string[] _separators = { DEFAULT_SEPARATOR };

        #endregion

        #region Constructors

        public Snapshot(int width, int height) : this(width, height, null) { }

        public Snapshot(int width, int height, Visualization visualization)
            : base(width, height, 1, visualization)
        {
            _bHighlighting = false;
        }

        public Snapshot(Visualization visualization)
            : this(visualization.GetCanvas.UnitWidth, visualization.GetCanvas.UnitHeight, visualization) { }

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
            get { return _bFlip ? _wb.Flip(WriteableBitmapExtensions.FlipMode.Horizontal) : _wb; }
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

        public string StationaryString
        {
            get { return SelectionToString(_stationary); }
            set { UpdateSelection(_stationary, value, "StationaryString"); }
        }

        public string AgentsString
        {
            get { return SelectionToString(_agents); }
            set { UpdateSelection(_agents, value, "AgentsString"); }
        }

        public string NeighbourhoodsString
        {
            get { return SelectionToString(_neighbourhoods); }
            set { UpdateSelection(_neighbourhoods, value, "NeighbourhoodsString"); }
        }

        public string TracksString
        {
            get { return SelectionToString(_tracks); }
            set { UpdateSelection(_tracks, value, "TracksString"); }
        }

        public string TrailsString
        {
            get { return SelectionToString(_trails); }
            set { UpdateSelection(_trails, value, "TrailsString"); }
        }

        #endregion

        #region Methods

        protected void ScaleBack()
        {
            RescaleStylesAndAppearances(DefaultValues.Scale);
        }

        private bool IsSelected(Appearance a, bool enabled, HashSet<string> species)
        {
            return enabled && (species.Count == 0 || species.Any(s => a.Species != null && a.Species.StartsWith(s)));
        }

        protected WriteableBitmap CreateBitmap()
        {
            return BitmapFactory.New((int)(_iUnitWidth * _dScale), (int)(_iUnitHeight * _dScale)); ;
        }

        protected virtual void Rescale()
        {
            _wb = CreateBitmap();
            RescaleStylesAndAppearances(_dScale);
        }

        private void RescaleStylesAndAppearances(double value)
        {
            _visual.GetOptions.RescaleStyles(value);
            _visual.GetOptions.RescaleAppearances(value);
        }

        public override void Redraw()
        {
            Redraw(_visual.GetModel.History, _iStep);
        }

        public override void Redraw(History history)
        {
            Redraw(history, _iStep);
        }

        public override void Redraw(History history, int step)
        {
            if (history.Count > 0)
            {
                Rescale();
                RedrawLayers(history, step);
                ScaleBack();
            }
        }

        protected override bool IsSpecifiedElementEnabled(Appearance a)
        {
            if (_bCustom)
            {
                if (a.IsStationary) return IsSelected(a, _bEnvironment, _stationary);
                if (a.IsAgent) return IsSelected(a, _bAgents, _agents);
                if (a.IsType<Core.Environment.Centroid>()) return _bCentroids;
            }
            return a.IsEnabled;
        }

        protected override bool IsSpecifiedNeighbourhoodEnabled(Appearance a)
        {
            return _bCustom ? IsSelected(a, _bNeighbourhoods, _neighbourhoods) : a.IsNeighbourhoodEnabled;
        }

        protected override bool IsSpecifiedTrackEnabled(Appearance a)
        {
            return _bCustom ? IsSelected(a, _bTracks, _tracks) : a.IsTrackEnabled;
        }

        protected override bool IsSpecifiedTrailEnabled(Appearance a)
        {
            return _bCustom ? IsSelected(a, _bTrails, _trails) : a.IsTrailEnabled;
        }

        private string SelectionToString(HashSet<string> collection)
        {
            return string.Join(DEFAULT_SEPARATOR, collection);
        }

        private void UpdateSelection(HashSet<string> collection, string value, string propertyName)
        {
            string[] items = value.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            collection.Clear();
            collection.UnionWith(items);
            NotifyPropertyChanged(propertyName);
        }

        public void SetVisualization(Visualization visual)
        {
            if (visual != null) _visual = visual;
        }

        #endregion
    }

    public class LayeredSnapshot : Snapshot
    {
        #region Fields

        private byte _alpha = byte.MaxValue;
        private WriteableBitmap _wbL = null;

        private BackgroundWorker _worker = new BackgroundWorker();

        #endregion

        #region Constructors

        public LayeredSnapshot(int width, int height, Visualization visual)
            : base(width, height, visual)
        {
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
        }

        public LayeredSnapshot(Visualization visual)
            : this(visual.GetCanvas.UnitWidth, visual.GetCanvas.UnitHeight, visual) { }

        #endregion

        #region Properties

        public byte Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                NotifyPropertyChanged("Alpha");
            }
        }

        public BackgroundWorker Worker
        {
            get { return _worker; }
        }

        #endregion

        #region Methods

        protected override void Rescale()
        {
            _wbL = CreateBitmap();
            base.Rescale();
        }

        private void CombineLayers(byte alpha)
        {
            ApplyAlpha(_wb, alpha);
            _wbL.Blit(_wb);
            _wb = CreateBitmap();
        }

        private void ApplyAlpha(WriteableBitmap wb, byte alpha)
        {
            wb.ForEach((x, y, color) => color.WithA(alpha));
        }

        public void Redraw(IEnumerable<History> histories)
        {
            if (histories.Count() > 0)
            {
                Rescale();
                _wbL.Clear(_backgroundColor);
                foreach (History h in histories)
                {
                    RedrawLayers(h, Step);
                    CombineLayers(_alpha);
                }
                ScaleBack();
                _wb = _wbL;
            }
        }

        public void Redraw(IEnumerable<History> histories, byte alpha)
        {
            Alpha = alpha;
            Redraw(histories);
        }

        public void Redraw(IEnumerable<History> histories, int step, byte alpha)
        {
            Step = step;
            Redraw(histories, alpha);
        }

        public void RedrawAsync(IEnumerable<History> histories)
        {
            if (histories.Count() > 0) _worker.RunWorkerAsync(histories);
        }

        public void RedrawAsync(IEnumerable<History> histories, byte alpha)
        {
            Alpha = alpha;
            RedrawAsync(histories);
        }

        public void RedrawAsync(IEnumerable<History> histories, int step, byte alpha)
        {
            Step = step;
            RedrawAsync(histories, alpha);
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IEnumerable<History> histories = (IEnumerable<History>)e.Argument;
            double progress = 0;
            double progressInc = 100d / histories.Count();
            Rescale();
            _wbL.Clear(_backgroundColor);
            foreach (History h in histories)
            {
                RedrawLayers(h, Step);
                CombineLayers(_alpha);
                progress += progressInc;
                _worker.ReportProgress(0, progress);
            }
            ScaleBack();
            _wb = _wbL;
        }

        #endregion
    }
}

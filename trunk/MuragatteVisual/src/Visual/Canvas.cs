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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.Core.Storage.ONG;

namespace Muragatte.Visual
{
    public class Canvas : INotifyPropertyChanged
    {
        #region Fields

        protected WriteableBitmap _wb = null;
        protected int _iUnitWidth = 1;
        protected int _iUnitHeight = 1;
        protected double _dScale = 1;
        protected bool _bEnvironment = true;
        protected bool _bNeighbourhoods = false;
        protected bool _bTracks = false;
        protected bool _bTrails = false;
        protected bool _bAgents = true;
        protected bool _bCentroids = false;
        protected bool _bHighlighting = true;
        protected Visualization _visual = null;
        protected Color _backgroundColor = DefaultValues.BACKGROUND_COLOR;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Canvas(int width, int height, Visualization visualization)
            : this(width, height, 1, visualization) { }

        public Canvas(int width, int height, double scale, Visualization visualization)
        {
            _dScale = scale;
            _iUnitWidth = width;
            _iUnitHeight = height;
            _wb = BitmapFactory.New((int)(width * scale), (int)(height * scale));
            _visual = visualization;
        }

        #endregion

        #region Properties

        public virtual WriteableBitmap Image
        {
            get { return _wb; }
        }

        public int UnitWidth
        {
            get { return _iUnitWidth; }
        }

        public int PixelWidth
        {
            get { return _wb.PixelWidth; }
        }

        public int UnitHeight
        {
            get { return _iUnitHeight; ; }
        }

        public int PixelHeight
        {
            get { return _wb.PixelHeight; }
        }

        public double Scale
        {
            get { return _dScale; }
        }

        public bool IsEnvironmentEnabled
        {
            get { return _bEnvironment; }
            set
            {
                _bEnvironment = value;
                NotifyPropertyChanged("IsEnvironmentEnabled");
            }
        }

        public bool IsNeighbourhoodsEnabled
        {
            get { return _bNeighbourhoods; }
            set
            {
                _bNeighbourhoods = value;
                NotifyPropertyChanged("IsNeighbourhoodsEnabled");
            }
        }

        public bool IsTracksEnabled
        {
            get { return _bTracks; }
            set
            {
                _bTracks = value;
                NotifyPropertyChanged("IsTracksEnabled");
            }
        }

        public bool IsTrailsEnabled
        {
            get { return _bTrails; }
            set
            {
                _bTrails = value;
                NotifyPropertyChanged("IsTrailsEnabled");
            }
        }

        public bool IsAgentsEnabled
        {
            get { return _bAgents; }
            set
            {
                _bAgents = value;
                NotifyPropertyChanged("IsAgentsEnabled");
            }
        }

        public bool IsCentroidsEnabled
        {
            get { return _bCentroids; }
            set
            {
                _bCentroids = value;
                NotifyPropertyChanged("IsCentroidsEnabled");
            }
        }

        public bool IsHighlightingEnabled
        {
            get { return _bHighlighting; }
            set
            {
                _bHighlighting = value;
                NotifyPropertyChanged("IsHighlightingEnabled");
            }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        #endregion

        #region Methods

        public void Rescale(double value)
        {
            if (value != _dScale)
            {
                _wb = BitmapFactory.New((int)(_iUnitWidth * value), (int)(_iUnitHeight * value));
                if (_visual != null) _visual.GetWindow.Initialize(this);
                _dScale = value;
            }
        }

        public void Clear()
        {
            _wb.Clear(_backgroundColor);
        }

        public virtual void Redraw()
        {
            HistoryRecord record = CurrentSituation();
            Clear();
            DrawNeighbourhoods(record);
            DrawEnvironment(record);
            DrawAgents(record);
            //DrawONG();
        }

        public virtual void Redraw(int step)
        {
            Redraw(_visual.GetModel.History, step);
        }

        public virtual void Redraw(History history)
        {
            Redraw(history, history.Count - 1);
        }

        public virtual void Redraw(History history, int step)
        {
            if (step >= history.Count)
            {
                return;
            }
            if (history.Count == 0)
            {
                Redraw();
                return;
            }
            RedrawLayers(history, step);
            //DrawONG();
        }

        protected void RedrawLayers(History history, int step)
        {
            Clear();
            DrawNeighbourhoods(history[step]);
            DrawEnvironment(history[step]);
            DrawTracks(history, step);
            DrawTrails(history, step);
            DrawAgents(history[step]);
            DrawCentroids(history[step]);
        }

        private void DrawElements(HistoryRecord record, bool enabled, ICollectionView items)
        {
            if (enabled)
            {
                foreach (Appearance a in items)
                {
                    ElementStatus es = record[a.ID];
                    if (es.IsEnabled && IsSpecifiedElementEnabled(a))
                    {
                        if (a.IsHighlighted)
                        {
                            DrawHighlight(a, es);
                        }
                        a.Draw(_wb, es.Position * _dScale, es.Direction);
                    }
                }
            }
        }

        private void DrawNeighbourhoods(HistoryRecord record)
        {
            if (_bNeighbourhoods)
            {
                foreach (Appearance a in _visual.GetOptions.NeighbourhoodsView)
                {
                    ElementStatus es = record[a.ID];
                    if (es.IsEnabled && IsSpecifiedNeighbourhoodEnabled(a))
                    {
                        a.DrawNeighbourhood(_wb, es.Position * _dScale, es.Direction);
                    }
                }
            }
        }

        private void DrawEnvironment(HistoryRecord record)
        {
            DrawElements(record, _bEnvironment, _visual.GetOptions.EnvironmentView);
        }

        private void DrawTracks(History history, int step)
        {
            if (_bTracks)
            {
                int time = Math.Min(step, history.Count);
                foreach (Appearance a in _visual.GetOptions.TracksView)
                {
                    if (IsSpecifiedTrackEnabled(a))
                    {
                        List<int[]> segments = TrackLinePoints(history.GetElementPositions(a.ID, time));
                        foreach (int[] segment in segments)
                        {
                            _wb.DrawPolyline(segment, a.Style.Track.Color);
                        }
                    }
                }
            }
        }

        private void DrawTrails(History history, int step)
        {
            if (_bTrails)
            {
                int substep = _visual.GetModel.Substeps;
                foreach (Appearance a in _visual.GetOptions.TrailsView)
                {
                    if (IsSpecifiedTrailEnabled(a))
                    {
                        byte alphaInc = (byte)(byte.MaxValue / (a.Style.Trail.Length + 1));
                        byte alpha = alphaInc;
                        for (int i = Math.Max(0, step - substep * a.Style.Trail.Length); i < step; i += substep)
                        {
                            ElementStatus es = history[i][a.ID];
                            a.DrawTrail(_wb, es.Position * _dScale, es.Direction, alpha);
                            alpha += alphaInc;
                        }
                    }
                }
            }
        }

        private void DrawAgents(HistoryRecord record)
        {
            DrawElements(record, _bAgents, _visual.GetOptions.AgentsView);
        }

        private void DrawCentroids(HistoryRecord record)
        {
            DrawElements(record, _bCentroids, _visual.GetOptions.CentroidsView);
        }

        private void DrawHighlight(Appearance a, ElementStatus es)
        {
            if (_bHighlighting)
            {
                Shapes.EllipseShape.Instance.Draw(_wb, es.Position * _dScale, es.Direction.Angle,
                    _visual.GetOptions.cpiHighlightColor.SelectedColor, Colors.Transparent,
                    (int)Math.Ceiling(a.Width * 1.5), (int)Math.Ceiling(a.Height * 1.5));
            }
        }

        protected virtual bool IsSpecifiedElementEnabled(Appearance a)
        {
            return a.IsEnabled;
        }

        protected virtual bool IsSpecifiedNeighbourhoodEnabled(Appearance a)
        {
            return a.IsNeighbourhoodEnabled;
        }

        protected virtual bool IsSpecifiedTrackEnabled(Appearance a)
        {
            return a.IsTrackEnabled;
        }

        protected virtual bool IsSpecifiedTrailEnabled(Appearance a)
        {
            return a.IsTrailEnabled;
        }

        private List<int[]> TrackLinePoints(List<Vector2> positions)
        {
            //int limit = Math.Min(UnitWidth, UnitHeight) / 3;
            int limit = (int)(_dScale * 3);
            List<int[]> points = new List<int[]>();
            if (positions.Count > 0)
            {
                List<int> part = new List<int>();
                part.Add(Scaled(positions[0].X));
                part.Add(Scaled(positions[0].Y));
                for (int i = 1; i < positions.Count; i++)
                {
                    if (IsOutside(positions[i - 1], positions[i], limit))
                    {
                        points.Add(part.ToArray());
                        part.Clear();
                    }
                    part.Add(Scaled(positions[i].X));
                    part.Add(Scaled(positions[i].Y));
                }
                points.Add(part.ToArray());
            }
            return points;
        }

        private bool IsOutside(Vector2 a, Vector2 b, int limit)
        {
            return (a.X < limit && b.X > UnitWidth - limit) ||
                (a.X > UnitWidth - limit && b.X < limit) ||
                (a.Y < limit && b.Y > UnitHeight - limit) ||
                (a.Y > UnitHeight - limit && b.Y < limit);
        }

        protected int Scaled(double value)
        {
            return (int)(value * _dScale);
        }

        private HistoryRecord CurrentSituation()
        {
            HistoryRecord record = new HistoryRecord(0);
            foreach (Element e in _visual.GetModel.Elements)
            {
                record.Add(e.ReportStatus());
            }
            foreach (Element e in _visual.GetModel.Elements.Centroids)
            {
                record.Add(e.ReportStatus());
            }
            return record;
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void DrawONG()
        {
            if (_visual.GetModel.Elements is OrthantNeighbourhoodGraphStorage)
            {
                List<Quadrant> quadrants = new List<Quadrant>() { Quadrant.NorthEast, Quadrant.SouthEast, Quadrant.SouthWest, Quadrant.NorthWest };
                foreach (Vertex v in ((OrthantNeighbourhoodGraphStorage)_visual.GetModel.Elements).Vertices)
                {
                    foreach (Quadrant q in quadrants)
                    {
                        if (v[q] != null) _wb.DrawLine(v.Position * _dScale, v[q].Position * _dScale, v == v[q][q.Opposite()] ? Colors.DarkGoldenrod : Colors.PaleGoldenrod);
                    }
                }
            }
        }

        #endregion
    }
}

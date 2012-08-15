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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.Visual
{
    public class Canvas : INotifyPropertyChanged
    {
        #region Fields

        private WriteableBitmap _wb = null;
        private int _iUnitWidth = 1;
        private int _iUnitHeight = 1;
        private double _dScale = 1;
        //private System.Windows.Rect _canvasArea;    //probably not needed
        private bool _bEnvironment = true;
        private bool _bNeighbourhoods = false;
        private bool _bTracks = false;
        private bool _bTrails = false;
        private bool _bAgents = true;
        private bool _bCentroids = false;
        private int _iTrailLength = 10;
        private Visualization _visual = null;

        #endregion

        #region Constructors

        public Canvas(int width, int height, Visualization visualization)
        {
            //_canvasArea = new System.Windows.Rect(0, 0, width, height);
            _iUnitWidth = width;
            _iUnitHeight = height;
            _wb = BitmapFactory.New(width, height);
            _visual = visualization;
        }

        public Canvas(int width, int height, double scale, Visualization visualization)
            : this((int)(width * scale), (int)(height * scale), visualization)
        {
            _dScale = scale;
            _iUnitWidth = width;
            _iUnitHeight = height;
        }

        #endregion

        #region Properties

        public WriteableBitmap Image
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

        public int TrailLength
        {
            get { return _iTrailLength; }
            set { _iTrailLength = value; }
        }

        #endregion

        #region Methods

        public void Rescale(double value)
        {
            if (value != _dScale)
            {
                _wb = BitmapFactory.New((int)(_iUnitWidth * value), (int)(_iUnitHeight * value));
                _visual.GetWindow.Initialize(this);
                _dScale = value;
            }
        }

        public void Clear()
        {
            _wb.Clear(_visual.GetOptions.ccBackgroundColor.SelectedColor);
        }

        //public void Redraw()
        //{
        //    Clear();
        //    IEnumerable<Element> stationary = _visual.GetModel.Elements.Stationary;
        //    IEnumerable<Agent> agents = _visual.GetModel.Elements.Agents;
        //    DrawNeighbourhoods(agents);
        //    DrawEnvironment(stationary);
        //    DrawAgents(agents);
        //}

        //public void DrawEnvironment(IEnumerable<Element> items)
        //{
        //    if (_bEnvironment)
        //    {
        //        DrawItems(items);
        //    }
        //}
        
        //public void DrawCentroids(IEnumerable<Centroid> items)
        //{ }

        //public void DrawAgents(IEnumerable<Agent> items)
        //{
        //    if (_bAgents)
        //    {
        //        DrawItems(items);
        //    }
        //}

        //public void DrawNeighbourhoods(IEnumerable<Agent> items)
        //{
        //    if (_bNeighbourhoods)
        //    {
        //        Vector2 up = Vector2.X0Y1;
        //        foreach (Agent a in items)
        //        {
        //            DrawParticle(a.FieldOfView.GetItemAs<Particle>(), a.Position, up);
        //            //DrawParticle(a.FieldOfView.GetItemAs<Particle>(), a.Position, a.Direction);
        //        }
        //    }
        //}

        //public void DrawTracks(IEnumerable<Agent> items)
        //{ }

        //public void DrawTrails(IEnumerable<Agent> items)
        //{ }

        //private void DrawItems<T>(IEnumerable<T> items) where T : Element
        //{
        //    foreach (T e in items)
        //    {
        //        DrawParticle(e.GetItemAs<Particle>(), e.Position, e.Direction);
        //    }
        //}

        //private void DrawParticle(Particle particle, Vector2 position, Vector2 direction, float alpha = 1)
        //{
        //    particle.DrawInto(_wb, position * _dScale, direction, alpha);
        //}

        //public void DrawEnvironment(IEnumerable<Element> items, HistoryRecord record)
        //{
        //    if (_bEnvironment)
        //    {
        //        DrawItems(items, record);
        //    }
        //}

        //public void DrawAgents(IEnumerable<Agent> items, HistoryRecord record, float alpha = 1)
        //{
        //    if (_bAgents)
        //    {
        //        DrawItems(items, record, alpha);
        //    }
        //}

        //public void DrawNeighbourhoods(IEnumerable<Agent> items, HistoryRecord record)
        //{
        //    if (_bNeighbourhoods)
        //    {
        //        Vector2 up = Vector2.X0Y1;
        //        foreach (Agent a in items)
        //        {
        //            ElementStatus es = record[a.ID];
        //            if (es.IsEnabled)
        //            {
        //                DrawParticle(a.FieldOfView.GetItemAs<Particle>(), es.Position, up);
        //                //DrawParticle(a.FieldOfView.GetItemAs<Particle>(), es.Position, es.Direction);
        //            }
        //        }
        //    }
        //}

        //public void DrawTracks(IEnumerable<Element> items, History history, int step)
        //{
        //    if (_bTracks)
        //    {
        //        int time = Math.Min(step, history.Count);
        //        foreach (Element a in items)
        //        {
        //            List<int[]> segments = TrackLinePoints(history.GetElementPositions(a.ID, time));
        //            foreach (int[] segment in segments)
        //            {
        //                _wb.DrawPolyline(segment, a.GetItemAs<Particle>().Color);
        //            }
        //        }
        //    }
        //}

        //public void DrawTrails(IEnumerable<Element> items, History history, int step)
        //{
        //    if (_bTrails)
        //    {
        //        int substep = _visual.GetModel.Substeps;
        //        float alphaInc = 1.0f / (_iTrailLength + 1);
        //        float alpha = alphaInc;
        //        for (int i = Math.Max(0, step - substep * _iTrailLength); i < step; i += substep)
        //        {
        //            DrawItems(items, history[i], alpha);
        //            alpha += alphaInc;
        //        }
        //    }
        //}

        //public void DrawTrails2(IEnumerable<Element> items, History history, int step)
        //{
        //    if (_bTrails)
        //    {
        //        int substep = _visual.GetModel.Substeps;
        //        //float alphaInc = 1.0f / (_iTrailLength + 1);
        //        //float alpha = alphaInc;
        //        //for (int i = Math.Max(0, step - substep * _iTrailLength); i < step; i += substep)
        //        //{
        //        //    DrawItems(items, history[i], alpha);
        //        //    alpha += alphaInc;
        //        //}
        //        int time = Math.Min(substep * _iTrailLength, history.Count);
        //        foreach (Element a in items)
        //        {
        //            List<int[]> segments = TrackLinePoints(history.GetElementPositions(a.ID, Math.Max(0, step - substep * _iTrailLength), time));
        //            foreach (int[] segment in segments)
        //            {
        //                _wb.DrawPolyline(segment, a.GetItemAs<Particle>().Color);
        //            }
        //        }
        //    }
        //}

        //public void DrawCentroids(IEnumerable<Centroid> items, HistoryRecord record, float alpha = 1)
        //{
        //    if (_bCentroids)
        //    {
        //        DrawItems(items, record, alpha);
        //    }
        //}

        //private void DrawItems<T>(IEnumerable<T> items, HistoryRecord record, float alpha = 1) where T : Element
        //{
        //    foreach (T e in items)
        //    {
        //        ElementStatus es = record[e.ID];
        //        if (es.IsEnabled)
        //        {
        //            DrawParticle(e.GetItemAs<Particle>(), es.Position, es.Direction, alpha);
        //        }
        //    }
        //}

        //public void Redraw(History history, int step)
        //{
        //    if (step >= history.Count)
        //    {
        //        return;
        //    }
        //    Clear();
        //    IEnumerable<Element> stationary = _visual.GetModel.Elements.Stationary;
        //    IEnumerable<Agent> agents = _visual.GetModel.Elements.Agents;
        //    IEnumerable<Centroid> centroids = _visual.GetModel.Elements.Centroids;
        //    DrawNeighbourhoods(agents, history[step]);
        //    DrawEnvironment(stationary, history[step]);
        //    if (_bAgents)
        //    {
        //        DrawTracks(agents, history, step);
        //        //DrawTracks(agents, history, history.Count);
        //        DrawTrails(agents, history, step);
        //    }
        //    if (_bCentroids)
        //    {
        //        DrawTracks(centroids, history, step);
        //        DrawTrails(centroids, history, step);
        //    }
        //    DrawAgents(agents, history[step]);
        //    DrawCentroids(centroids, history[step]);
        //    _wb.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
        //}

        public void Redraw()
        {
            HistoryRecord record = CurrentSituation();
            Clear();
            DrawNeighbourhoods(record);
            DrawEnvironment(record);
            DrawAgents(record);
            _wb.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
        }

        public void Redraw(History history)
        {
            Redraw(history, history.Count - 1);
        }

        public void Redraw(History history, int step)
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
            Clear();
            DrawNeighbourhoods(history[step]);
            DrawEnvironment(history[step]);
            DrawTracks(history, step);
            DrawTrails(history, step);
            DrawAgents(history[step]);
            DrawCentroids(history[step]);
            _wb.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
        }

        private void DrawElements(HistoryRecord record, bool enabled, ICollectionView items)
        {
            if (enabled)
            {
                foreach (Appearance a in items)
                {
                    ElementStatus es = record[a.ID];
                    if (es.IsEnabled && a.IsEnabled)
                    {
                        a.Draw(_wb, es.Position * _dScale, es.Direction);
                    }
                }
            }
        }

        private void DrawNeighbourhoods(HistoryRecord record)
        {
            if (_bNeighbourhoods)
            {
                foreach (Appearance a in _visual.GetOptions.AgentsView)
                {
                    ElementStatus es = record[a.ID];
                    if (es.IsEnabled && a.IsNeighbourhoodEnabled)
                    {
                        a.Style.Neighbourhood.Draw(_wb, es.Position * _dScale, es.Direction);
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
                foreach (Appearance a in _visual.GetOptions.AgentsView)
                {
                    if (history[step][a.ID].IsEnabled && a.IsEnabled && a.IsTrackEnabled)
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
            //        int substep = _visual.GetModel.Substeps;
            //        float alphaInc = 1.0f / (_iTrailLength + 1);
            //        float alpha = alphaInc;
            //        for (int i = Math.Max(0, step - substep * _iTrailLength); i < step; i += substep)
            //        {
            //            DrawItems(items, history[i], alpha);
            //            alpha += alphaInc;
            //        }

            if (_bTrails)
            {
                int substep = _visual.GetModel.Substeps;
                foreach (Appearance a in _visual.GetOptions.AgentsView)
                {
                    if (history[step][a.ID].IsEnabled && a.IsEnabled && a.IsTrailEnabled)
                    {
                        //byte alphaInc = a.Style.Trail.Color.A / (a.Style.Trail.Length + 1);
                        //trail compute
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

        private List<int[]> TrackLinePoints(List<Vector2> positions)
        {
            int limit = Math.Min(UnitWidth, UnitHeight) / 3;
            List<int[]> points = new List<int[]>();
            if (positions.Count > 0)
            {
                List<int> part = new List<int>();
                part.Add(Scaled(positions[0].X));
                //part.Add(_wb.PixelHeight - Scaled(positions[0].Y) - 1);
                part.Add(Scaled(positions[0].Y));
                for (int i = 1; i < positions.Count; i++)
                {
                    if (IsOutside(positions[i - 1], positions[i], limit))
                    {
                        points.Add(part.ToArray());
                        part.Clear();
                    }
                    part.Add(Scaled(positions[i].X));
                    //part.Add(_wb.PixelHeight - Scaled(positions[i].Y) - 1);
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

        private int Scaled(double value)
        {
            return (int)(value * _dScale);
        }

        private HistoryRecord CurrentSituation()
        {
            HistoryRecord record = new HistoryRecord();
            foreach (Element e in _visual.GetModel.Elements)
            {
                record.Add(e.ReportStatus());
            }
            return record;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
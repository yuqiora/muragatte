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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.Visual
{
    public class Canvas
    {
        #region Fields

        private WriteableBitmap _wb = null;
        private double _dScale = 1;
        private System.Windows.Rect _canvasArea;    //probably not needed
        private bool _bEnvironment = true;
        private bool _bNeighbourhoods = false;
        private bool _bTracks = false;
        private bool _bTrails = false;
        private bool _bAgents = true;
        private bool _bCentroids = false;
        private int _iTrailLength = 10;
        private MultiAgentSystem _model = null;

        #endregion

        #region Constructors

        public Canvas(int width, int height)
        {
            _canvasArea = new System.Windows.Rect(0, 0, width, height);
            _wb = BitmapFactory.New(width, height);
        }

        public Canvas(int width, int height, double scale)
            : this((int)(width * scale), (int)(height * scale))
        {
            _dScale = scale;
        }

        #endregion

        #region Properties

        public WriteableBitmap Image
        {
            get { return _wb; }
        }

        public int UnitWidth
        {
            get { return (int)(_wb.PixelWidth / _dScale); }
        }

        public int PixelWidth
        {
            get { return _wb.PixelWidth; }
        }

        public int UnitHeight
        {
            get { return (int)(_wb.PixelHeight / _dScale); }
        }

        public int PixelHeight
        {
            get { return _wb.PixelHeight; }
        }

        public double Scale
        {
            get { return _dScale; }
            //set { _dScale = value; }
        }

        public bool IsEnvironmentEnabled
        {
            get { return _bEnvironment; }
            set { _bEnvironment = value; }
        }

        public bool IsNeighbourhoodsEnabled
        {
            get { return _bNeighbourhoods; }
            set { _bNeighbourhoods = value; }
        }

        public bool IsTracksEnabled
        {
            get { return _bTracks; }
            set { _bTracks = value; }
        }

        public bool IsTrailsEnabled
        {
            get { return _bTrails; }
            set { _bTrails = value; }
        }

        public bool IsAgentsEnabled
        {
            get { return _bAgents; }
            set { _bAgents = value; }
        }

        public bool IsCentroidsEnabled
        {
            get { return _bCentroids; }
            set { _bCentroids = value; }
        }

        public int TrailLength
        {
            get { return _iTrailLength; }
            set { _iTrailLength = value; }
        }

        public MultiAgentSystem Model
        {
            get { return _model; }
            set { _model = value; }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            _wb.Clear();
        }

        public void Initialize()
        {
            if (_model != null)
            {
                Redraw();
            }
        }

        public void Initialize(MultiAgentSystem model)
        {
            _model = model;
            Redraw();
        }

        public void Redraw()
        {
            _wb.Clear();
            IEnumerable<Element> stationary = _model.Elements.Stationary;
            IEnumerable<Agent> agents = _model.Elements.Agents;
            DrawNeighbourhoods(agents);
            DrawEnvironment(stationary);
            DrawAgents(agents);
        }

        public void DrawEnvironment(IEnumerable<Element> items)
        {
            if (_bEnvironment)
            {
                DrawItems(items);
            }
        }
        
        public void DrawCentroids(IEnumerable<Centroid> items)
        { }

        public void DrawAgents(IEnumerable<Agent> items)
        {
            if (_bAgents)
            {
                DrawItems(items);
            }
        }

        public void DrawNeighbourhoods(IEnumerable<Agent> items)
        {
            if (_bNeighbourhoods)
            {
                Vector2 up = Vector2.X0Y1();
                foreach (Agent a in items)
                {
                    DrawParticle(a.FieldOfView.GetItemAs<Particle>(), a.Position, up);
                    //DrawParticle(a.FieldOfView.GetItemAs<Particle>(), a.Position, a.Direction);
                }
            }
        }

        public void DrawTracks(IEnumerable<Agent> items)
        { }

        public void DrawTrails(IEnumerable<Agent> items)
        { }

        private void DrawItems<T>(IEnumerable<T> items) where T : Element
        {
            foreach (T e in items)
            {
                DrawParticle(e.GetItemAs<Particle>(), e.Position, e.Direction);
            }
        }

        private void DrawParticle(Particle particle, Vector2 position, Vector2 direction, float alpha = 1)
        {
            particle.DrawInto(_wb, position * _dScale, direction, alpha);
        }

        #region From History

        public void DrawEnvironment(IEnumerable<Element> items, HistoryRecord record)
        {
            if (_bEnvironment)
            {
                DrawItems(items, record);
            }
        }

        public void DrawAgents(IEnumerable<Agent> items, HistoryRecord record, float alpha = 1)
        {
            if (_bAgents)
            {
                DrawItems(items, record, alpha);
            }
        }

        public void DrawNeighbourhoods(IEnumerable<Agent> items, HistoryRecord record)
        {
            if (_bNeighbourhoods)
            {
                Vector2 up = Vector2.X0Y1();
                foreach (Agent a in items)
                {
                    ElementStatus es = record[a.ID];
                    if (es.IsEnabled)
                    {
                        DrawParticle(a.FieldOfView.GetItemAs<Particle>(), es.Position, up);
                        //DrawParticle(a.FieldOfView.GetItemAs<Particle>(), es.Position, es.Direction);
                    }
                }
            }
        }

        public void DrawTracks(IEnumerable<Agent> items, History history, int step)
        {
            if (_bTracks)
            {
                int time = Math.Min(step, history.Count);
                foreach (Agent a in items)
                {
                    List<int[]> segments = TrackLinePoints(history.GetElementPositions(a.ID, time));
                    foreach (int[] segment in segments)
                    {
                        _wb.DrawPolyline(segment, a.GetItemAs<Particle>().Color);
                    }
                }
            }
        }

        public void DrawTrails(IEnumerable<Agent> items, History history, int step)
        {
            if (_bTrails)
            {
                float alphaInc = 1.0f / (_iTrailLength + 1);
                float alpha = alphaInc;
                for (int i = Math.Max(0, step - _iTrailLength); i < step; i++)
                {
                    DrawItems(items, history[i], alpha);
                    alpha += alphaInc;
                }
            }
        }

        private void DrawItems<T>(IEnumerable<T> items, HistoryRecord record, float alpha = 1) where T : Element
        {
            foreach (T e in items)
            {
                ElementStatus es = record[e.ID];
                if (es.IsEnabled)
                {
                    DrawParticle(e.GetItemAs<Particle>(), es.Position, es.Direction, alpha);
                }
            }
        }

        public void Redraw(History history, int step)
        {
            _wb.Clear();
            IEnumerable<Element> stationary = _model.Elements.Stationary;
            IEnumerable<Agent> agents = _model.Elements.Agents;
            DrawNeighbourhoods(agents, history[step]);
            DrawEnvironment(stationary, history[step]);
            DrawTracks(agents, history, step);
            //DrawTracks(agents, history, history.Count);
            DrawTrails(agents, history, step);
            DrawAgents(agents, history[step]);
        }

        public void Redraw(History history)
        {
            Redraw(history, history.Count - 1);
        }

        private List<int[]> TrackLinePoints(List<Vector2> positions)
        {
            int limit = Math.Min(UnitWidth, UnitHeight) / 3;
            List<int[]> points = new List<int[]>();
            if (positions.Count > 0)
            {
                List<int> part = new List<int>();
                part.Add(Scaled(positions[0].X));
                part.Add(_wb.PixelHeight - Scaled(positions[0].Y) - 1);
                for (int i = 1; i < positions.Count; i++)
                {
                    if (IsOutside(positions[i - 1], positions[i], limit))
                    {
                        points.Add(part.ToArray());
                        part.Clear();
                    }
                    part.Add(Scaled(positions[i].X));
                    part.Add(_wb.PixelHeight - Scaled(positions[i].Y) - 1);
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

        #endregion

        #endregion
    }
}

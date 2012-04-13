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

namespace Muragatte.Visual
{
    public enum VisualizationMode { Replay, Simulation }

    public class Canvas
    {
        #region Fields

        private WriteableBitmap _wb = null;
        private double _dScale = 1;
        private System.Windows.Rect _canvasArea;
        private bool _bEnvironment = true;
        private bool _bNeighbourhoods = false;
        private bool _bTracks = false;
        private bool _bTrails = false;
        private bool _bAgents = true;
        private bool _bCentroids = false;
        private MultiAgentSystem _model = null;
        private VisualizationMode _visMode = VisualizationMode.Simulation;

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

        public MultiAgentSystem Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public VisualizationMode VisMode
        {
            get { return _visMode; }
            set { _visMode = value; }
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
            //no tracks/trails in simulation, only in replay when history available

            _wb.Clear();
            if (_visMode == VisualizationMode.Simulation)
            {
                IEnumerable<Element> stationary = _model.Elements.Stationary;
                IEnumerable<Agent> agents = _model.Elements.Agents;
                DrawNeighbourhoods(agents);
                DrawEnvironment(stationary);
                DrawAgents(agents);
            }
            else
            {
                //draw from history
            }
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

        //private int Scaled(double value)
        //{
        //    return (int)(value * _dScale);
        //}

        private void DrawItems<T>(IEnumerable<T> items) where T : Element
        {
            foreach (T e in items)
            {
                DrawParticle(e.GetItemAs<Particle>(), e.Position, e.Direction);
            }
        }

        private void DrawParticle(Particle particle, Vector2 position, Vector2 direction)
        {
            particle.DrawInto(_wb, position * _dScale, direction);
        }

        #endregion
    }
}

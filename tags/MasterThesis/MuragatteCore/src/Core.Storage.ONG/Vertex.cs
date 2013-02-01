// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Core Library
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
using Muragatte.Common;
using Muragatte.Core.Environment;

namespace Muragatte.Core.Storage.ONG
{
    public class Vertex : IEnumerable<Vertex>
    {
        #region Fields

        private Vector2 _position = Vector2.Zero;
        private Element _item = null;
        private Dictionary<Quadrant, Vertex> _neighbours = new Dictionary<Quadrant, Vertex>();
        private bool _bFlagged = false;

        #endregion

        #region Constructors

        public Vertex(Element item)
        {
            _item = item;
            _position = _item.Position;
            InitializeNeighbours();
        }

        #endregion

        #region Properties

        public Element Value
        {
            get { return _item; }
        }

        public Vertex this[Quadrant q]
        {
            get { return _neighbours[q]; }
            set { _neighbours[q] = value; }
        }

        public bool IsFlagged
        {
            get { return _bFlagged; }
            set { _bFlagged = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
        }

        public bool Moved
        {
            get { return _position != _item.Position; }
        }

        #endregion

        #region Methods

        private void InitializeNeighbours()
        {
            if (_neighbours == null)
            {
                _neighbours = new Dictionary<Quadrant, Vertex>();
            }
            else
            {
                _neighbours.Clear();
            }
            _neighbours.Add(Quadrant.NorthEast, null);
            _neighbours.Add(Quadrant.SouthEast, null);
            _neighbours.Add(Quadrant.SouthWest, null);
            _neighbours.Add(Quadrant.NorthWest, null);
        }

        public Quadrant QuadrantOf(Vertex vertex)
        {
            return QuadrantOf(_position, vertex);
        }

        public Quadrant QuadrantOf(Vector2 origin)
        {
            return QuadrantOf(origin, this);
        }

        private Quadrant QuadrantOf(Vector2 origin, Vertex vertex)
        {
            if (vertex.Position.Y > origin.Y)
            {
                return vertex.Position.X > origin.X ? Quadrant.NorthEast : Quadrant.NorthWest;
            }
            else
            {
                if (vertex.Position.X >= origin.X)
                {
                    return Quadrant.SouthEast;
                }
                else
                {
                    return vertex.Position.Y == origin.Y ? Quadrant.NorthWest : Quadrant.SouthWest;
                }
            }
        }

        //public Vertex GetNeighbour()
        //{
        //    foreach (Vertex v in _neighbours.Values)
        //    {
        //        if (v != null) return v;
        //    }
        //    return null;
        //}

        public void Move()
        {
            _position = _item.Position;
        }

        public Vertex NearestNeighbour(Metric metric)
        {
            double d = double.PositiveInfinity;
            Vertex nearest = null;
            foreach (Vertex v in _neighbours.Values)
            {
                if (v != null)
                {
                    double dist = metric.Distance(_position, v.Position, v.Value.Radius);
                    if (dist < d)
                    {
                        d = dist;
                        nearest = v;
                    }
                }
            }
            return nearest;
        }

        public void Clear()
        {
            _neighbours[Quadrant.NorthEast] = null;
            _neighbours[Quadrant.SouthEast] = null;
            _neighbours[Quadrant.SouthWest] = null;
            _neighbours[Quadrant.NorthWest] = null;
        }

        public IEnumerator<Vertex> GetEnumerator()
        {
            return _neighbours.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        #endregion
    }
}

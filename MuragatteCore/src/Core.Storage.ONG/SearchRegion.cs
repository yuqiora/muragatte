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

namespace Muragatte.Core.Storage.ONG
{
    public class SearchRegion
    {
        #region Fields

        private double _dTop;
        private double _dBottom;
        private double _dLeft;
        private double _dRight;

        #endregion

        #region Constructors

        public SearchRegion() : this(double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity) { }

        public SearchRegion(double top, double bottom, double left, double right)
        {
            _dTop = top;
            _dBottom = bottom;
            _dLeft = left;
            _dRight = right;
        }

        public SearchRegion(Vector2 origin, double distance)
            : this(origin.Y + distance, origin.Y - distance, origin.X - distance, origin.X + distance) { }

        public SearchRegion(Vector2 origin, double distance, Vector2 limit, Quadrant q)
        {
            _dTop = q == Quadrant.SouthEast || q == Quadrant.SouthWest ? limit.Y : origin.Y + distance;
            _dBottom = q == Quadrant.NorthEast || q == Quadrant.NorthWest ? limit.Y : origin.Y - distance;
            _dLeft = q == Quadrant.NorthEast || q == Quadrant.SouthEast ? limit.X : origin.X - distance;
            _dRight = q == Quadrant.NorthWest || q == Quadrant.SouthWest ? limit.X : origin.X + distance;
        }

        #endregion

        #region Methods

        public bool Covers(Vertex v)
        {
            return v.Position.X >= _dLeft && v.Position.X <= _dRight && v.Position.Y <= _dTop && v.Position.Y >= _dBottom;
        }

        public bool Cuts(Vertex vertex, Quadrant q)
        {
            if (vertex == null) return false;
            if (vertex.Position.X >= _dLeft && vertex.Position.X <= _dRight && vertex.Position.Y <= _dTop && vertex.Position.Y >= _dBottom)
            {
                return true;
            }
            return (q == Quadrant.NorthEast && (vertex.Position.X > _dRight || vertex.Position.Y > _dTop)) ||
                   (q == Quadrant.SouthEast && (vertex.Position.X > _dRight || vertex.Position.Y < _dBottom)) ||
                   (q == Quadrant.SouthWest && (vertex.Position.X < _dLeft || vertex.Position.Y < _dBottom)) ||
                   (q == Quadrant.NorthWest && (vertex.Position.X < _dLeft || vertex.Position.Y > _dTop))
                   ? false : true;
        }

        public void Clip(Vector2 limit, int signX, int signY)
        {
            ClipX(limit, signX);
            ClipY(limit, signY);
        }

        public void ClipX(Vector2 limit, int sign)
        {
            if (sign > 0)
            {
                _dRight = limit.X;
            }
            else
            {
                _dLeft = limit.X;
            }
        }

        public void ClipY(Vector2 limit, int sign)
        {
            if (sign > 0)
            {
                _dTop = limit.Y;
            }
            else
            {
                _dBottom = limit.Y;
            }
        }

        public static SearchRegion CoverQuadrant(Vertex origin, Quadrant q)
        {
            SearchRegion r = new SearchRegion();
            r.Clip(origin.Position, -q.SignX(), -q.SignY());
            return r;
        }

        public static Dictionary<Quadrant, SearchRegion> CoverQuadrants(Vertex origin)
        {
            Dictionary<Quadrant, SearchRegion> regions = new Dictionary<Quadrant, SearchRegion>();
            regions.Add(Quadrant.NorthEast, new SearchRegion(double.PositiveInfinity, origin.Position.Y, origin.Position.X, double.PositiveInfinity));
            regions.Add(Quadrant.SouthEast, new SearchRegion(origin.Position.Y, double.NegativeInfinity, origin.Position.X, double.PositiveInfinity));
            regions.Add(Quadrant.SouthWest, new SearchRegion(origin.Position.Y, double.NegativeInfinity, double.NegativeInfinity, origin.Position.X));
            regions.Add(Quadrant.NorthWest, new SearchRegion(double.PositiveInfinity, origin.Position.Y, double.NegativeInfinity, origin.Position.X));
            return regions;
        }

        #endregion
    }
}

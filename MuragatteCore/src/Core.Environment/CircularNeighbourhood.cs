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

namespace Muragatte.Core.Environment
{
    //euklid
    public class CircularNeighbourhood : Neighbourhood
    {
        #region Fields

        protected Angle _dAngle = Angle.Deg180();

        #endregion

        #region Constructors

        public CircularNeighbourhood(double range)
            : base(range) { }

        public CircularNeighbourhood(double range, Angle angle)
            : base(range)
        {
            _dAngle = angle;
        }

        public CircularNeighbourhood(Agent source, double range)
            : base(source, range) { }

        public CircularNeighbourhood(Agent source, double range, Angle angle)
            : base(source, range)
        {
            _dAngle = angle;
        }
        
        #endregion

        #region Properties
        
        public Angle Angle
        {
            get { return _dAngle; }
            set { _dAngle = value; }
        }
        
        #endregion

        #region Methods
        
        protected IEnumerable<T> WithinFull<T>(IEnumerable<T> elements) where T : Environment.Element
        {
            List<T> result = new List<T>();
            foreach (T e in elements)
            {
                if (Vector2.Distance(_source.Position, e.Position) < _dRange)
                {
                    result.Add(e);
                }
            }
            return result;
        }

        protected IEnumerable<T> WithinPartial<T>(IEnumerable<T> elements) where T : Environment.Element
        {
            List<T> result = new List<T>();
            foreach (T e in elements)
            {
                if (Vector2.Distance(_source.Position, e.Position) < _dRange &&
                    Vector2.AngleBetween(_source.Direction, e.Position - _source.Position) <= _dAngle)
                {
                    result.Add(e);
                }
            }
            return result;
        }

        public override IEnumerable<T> Within<T>(IEnumerable<T> elements)
        {
            if (_dAngle.Degrees == Angle.MaxDegree)
            {
                return WithinFull(elements);
            }
            else
            {
                return WithinPartial(elements);
            }
        }

        #endregion
    }
}

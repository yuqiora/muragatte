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

        protected double _dAngle = Agent.MaxAngle;

        #endregion

        #region Constructors

        public CircularNeighbourhood(double range, double angle)
            : base(range)
        {
            _dAngle = angle;
        }

        public CircularNeighbourhood(Agent source, double range, double angle)
            : base(source, range)
        {
            _dAngle = angle;
        }
        
        #endregion

        #region Properties
        
        public double Angle
        {
            get { return _dAngle; }
            set { _dAngle = Agent.ProperAngle(value); }
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
                    Math.Abs(Vector2.AngleBetween(_source.Direction, e.Direction)) <= _dAngle)
                {
                    result.Add(e);
                }
            }
            return result;
        }

        public override IEnumerable<T> Within<T>(IEnumerable<T> elements)
        {
            if (_dAngle == Agent.MaxAngle)
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

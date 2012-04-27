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

        protected Angle _angle = Angle.Deg180();

        #endregion

        #region Constructors

        public CircularNeighbourhood(double range)
            : base(range) { }

        public CircularNeighbourhood(double range, Angle angle)
            : base(range)
        {
            _angle = angle;
        }

        public CircularNeighbourhood(Agent source, double range)
            : base(source, range) { }

        public CircularNeighbourhood(Agent source, double range, Angle angle)
            : base(source, range)
        {
            _angle = angle;
        }
        
        #endregion

        #region Properties
        
        public Angle Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }
        
        #endregion

        #region Methods
        
        protected IEnumerable<T> WithinFull<T>(IEnumerable<T> elements) where T : Environment.Element
        {
            List<T> result = new List<T>();
            foreach (T e in elements)
            {
                if (Vector2.Distance(_source.Position, e.Position) - e.Radius < _dRange)
                {
                    result.Add(e);
                }
            }
            return result;
        }

        protected IEnumerable<T> WithinPartial<T>(IEnumerable<T> elements, Angle angle) where T : Environment.Element
        {
            List<T> result = new List<T>();
            foreach (T e in elements)
            {
                if (Covers(e, angle))
                {
                    result.Add(e);
                }
            }
            return result;
        }

        public override IEnumerable<T> Within<T>(IEnumerable<T> elements)
        {
            return Within(elements, _angle);
        }

        public override IEnumerable<T> Within<T>(IEnumerable<T> elements, Angle angle)
        {
            if (angle.Degrees == Angle.MaxDegree)
            {
                return WithinFull(elements);
            }
            else
            {
                return WithinPartial(elements, angle);
            }
        }

        public override bool Covers(Element e)
        {
            return Covers(e, _angle);
        }

        public override bool Covers(Element e, Angle angle)
        {
            return Vector2.Distance(_source.Position, e.Position) - e.Radius < _dRange &&
                Vector2.AngleBetween(_source.Direction, e.Position - _source.Position) <= angle;
        }

        #endregion
    }
}

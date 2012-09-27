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
    public class Neighbourhood
    {
        #region Fields

        protected Agent _source = null;
        protected double _dRange = 0;
        protected Angle _angle = Angle.Deg180;
        protected Metric _metric = Metric.Euclidean;

        #endregion

        #region Constructors

        public Neighbourhood(double range, Metric metric = Metric.Euclidean)
        {
            _dRange = range;
        }

        public Neighbourhood(double range, Angle angle, Metric metric = Metric.Euclidean)
            : this(range, metric)
        {
            _angle = angle;
        }

        public Neighbourhood(Agent source, double range, Metric metric = Metric.Euclidean)
            : this(range, metric)
        {
            _source = source;
        }

        public Neighbourhood(Agent source, double range, Angle angle, Metric metric = Metric.Euclidean)
            : this(range, angle, metric)
        {
            _source = source;
        }

        #endregion

        #region Properties
        
        public Agent Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public double Range
        {
            get { return _dRange; }
        }

        public Angle Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        #endregion

        #region Methods

        protected IEnumerable<T> WithinFull<T>(IEnumerable<T> elements) where T : Element
        {
            List<T> result = new List<T>();
            foreach (T e in elements)
            {
                if (InRange(e))
                {
                    result.Add(e);
                }
            }
            return result;
        }

        protected IEnumerable<T> WithinPartial<T>(IEnumerable<T> elements, Angle angle) where T : Element
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

        protected bool InRange(Element e)
        {
            return _metric.Distance(_source.Position, e.Position, e.Radius) < _dRange;
        }

        public IEnumerable<T> Within<T>(IEnumerable<T> elements) where T : Element
        {
            return Within(elements, _angle);
        }

        public IEnumerable<T> Within<T>(IEnumerable<T> elements, Angle angle) where T : Element
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

        public bool Covers(Element e)
        {
            return Covers(e, _angle);
        }

        public bool Covers(Element e, Angle angle)
        {
            return InRange(e) && Vector2.AngleBetween(_source.Direction, e.Position - _source.Position) <= angle;
        }

        public Neighbourhood Clone(bool withSource = false)
        {
            return new Neighbourhood(withSource ? _source : null, _dRange, _angle, _metric);
        }

        #endregion

    }
}

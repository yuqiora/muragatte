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

namespace Muragatte.Core.Environment.SteeringUtils
{
    public abstract class Steering
    {
        #region Fields

        protected Element _element;
        protected double _dWeight;

        #endregion

        #region Constructors

        public Steering(Element element, double weight)
        {
            _element = element;
            _dWeight = weight;
        }

        #endregion

        #region Properties

        public Element Element
        {
            get { return _element; }
        }

        public double Weight
        {
            get { return _dWeight; }
            set
            {
                if (!double.IsNaN(value) && !double.IsInfinity(value))
                {
                    _dWeight = value;
                }
            }
        }

        public abstract string Name { get; }

        #endregion

        #region Methods

        public virtual Vector2 Steer()
        {
            return Steer(_dWeight);
        }

        public virtual Vector2 Steer(Element other)
        {
            return Steer(other, _dWeight);
        }

        public virtual Vector2 Steer(IEnumerable<Element> others)
        {
            return Steer(others, _dWeight);
        }

        public abstract Vector2 Steer(double weight);

        public abstract Vector2 Steer(Element other, double weight);

        public abstract Vector2 Steer(IEnumerable<Element> others, double weight);

        #endregion
    }
}

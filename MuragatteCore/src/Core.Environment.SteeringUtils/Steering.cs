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

        protected virtual Vector2 DefaultSteer
        {
            get { return Vector2.Zero; }
        }

        public abstract string Name { get; }

        #endregion

        #region Methods

        public virtual Vector2 Steer(bool normalize = false)
        {
            return Steer(_dWeight, normalize);
        }

        public virtual Vector2 Steer(Element other, bool normalize = false)
        {
            return Steer(other, _dWeight, normalize);
        }

        public virtual Vector2 Steer(IEnumerable<Element> others, bool average = false)
        {
            return Steer(others, _dWeight, average);
        }

        public abstract Vector2 Steer(double weight, bool normalize = false);

        public abstract Vector2 Steer(Element other, double weight, bool normalize = false);

        public abstract Vector2 Steer(IEnumerable<Element> others, double weight, bool average = false);

        #endregion
    }
}

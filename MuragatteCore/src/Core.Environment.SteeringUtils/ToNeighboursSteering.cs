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
    public abstract class ToNeighboursSteering : Steering
    {
        #region Constructors

        public ToNeighboursSteering(Element element, double weight) : base(element, weight) { }

        #endregion

        #region Methods

        public override Vector2 Steer(double weight)
        {
            return Vector2.Zero;
        }

        public override Vector2 Steer(Element other, double weight)
        {
            if (other == null)
            {
                return Vector2.Zero;
            }
            else
            {
                List<Element> others = new List<Element>();
                others.Add(other);
                return SteerToOthers(others, weight);
            }
        }

        public override Vector2 Steer(IEnumerable<Element> others, double weight)
        {
            return others == null || others.Count() == 0 ? Vector2.Zero : SteerToOthers(others, weight);
        }

        protected abstract Vector2 SteerToOthers(IEnumerable<Element> others, double weight);

        #endregion
    }
}

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

        public override Vector2 Steer(double weight, bool normalize = false)
        {
            return DefaultSteer;
        }

        public override Vector2 Steer(Element other, double weight, bool normalize = false)
        {
            if (other == null || weight == 0)
            {
                return DefaultSteer;
            }
            else
            {
                List<Element> others = new List<Element>();
                others.Add(other);
                return SteerToOthers(others, weight, normalize);
            }
        }

        public override Vector2 Steer(IEnumerable<Element> others, double weight, bool average = false)
        {
            return others == null || others.Count() == 0 || weight == 0 ? DefaultSteer : SteerToOthers(others, weight, average);
        }

        protected abstract Vector2 SteerToOthers(IEnumerable<Element> others, double weight, bool average);

        #endregion
    }
}

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
    public abstract class ToNearestSteering : Steering
    {
        #region Constructors

        public ToNearestSteering(Element element, double weight) : base(element, weight) { }

        #endregion

        #region Methods

        public override Vector2 Steer(double weight, bool normalzie = false)
        {
            return DefaultSteer;
        }

        public override Vector2 Steer(Element other, double weight, bool normalize = false)
        {
            return other == null || weight == 0 ? DefaultSteer: SteerToOther(other, weight, normalize);
        }

        public override Vector2 Steer(IEnumerable<Element> others, double weight, bool average = false)
        {
            if (others == null || others.Count() == 0 || weight == 0)
            {
                return DefaultSteer;
            }
            else
            {
                Element nearest = null;
                double distance = double.MaxValue;
                foreach (Element e in others)
                {
                    if ((_element.Position - e.Position).Length < distance)
                    {
                        distance = (_element.Position - e.Position).Length;
                        nearest = e;
                    }
                }
                return SteerToOther(nearest, weight, average);
            }
        }

        protected abstract Vector2 SteerToOther(Element other, double weight, bool normalize);

        #endregion
    }
}

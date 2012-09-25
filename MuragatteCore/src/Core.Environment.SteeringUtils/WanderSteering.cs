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
    public class WanderSteering : Steering
    {
        #region Constants

        public const string LABEL = "Wander";

        #endregion

        #region Constructors

        public WanderSteering(Element element, double weight) : base(element, weight) { }

        #endregion

        #region Properties

        public override string Name
        {
            get { return LABEL; }
        }

        #endregion

        #region Methods

        public override Vector2 Steer(double weight)
        {
            return _element.Direction + Angle.Random(weight);
        }

        public override Vector2 Steer(Element other, double weight)
        {
            return Steer(weight);
        }

        public override Vector2 Steer(IEnumerable<Element> others, double weight)
        {
            return Steer(weight);
        }

        #endregion
    }
}

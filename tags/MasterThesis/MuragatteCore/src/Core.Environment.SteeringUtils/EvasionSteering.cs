﻿// ------------------------------------------------------------------------
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
    public class EvasionSteering : ToNearestSteering
    {
        #region Constants

        public const string LABEL = "Evasion";

        #endregion

        #region Constructors

        public EvasionSteering(Element element, double weight) : base(element, weight) { }

        #endregion

        #region Properties

        public override string Name
        {
            get { return LABEL; }
        }

        #endregion

        #region Methods

        protected override Vector2 SteerToOther(Element other, double weight, bool normalize)
        {
            Vector2 v = _element.Position - other.PredictPositionAfter();
            if (normalize) v.Normalize();
            return weight * v;
        }

        #endregion
    }
}

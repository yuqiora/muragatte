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
    public class AlignmentSteering : ToNeighboursSteering
    {
        #region Constants

        public const string LABEL = "Alignment";

        #endregion

        #region Constructors

        public AlignmentSteering(Element element, double weight) : base(element, weight) { }

        #endregion

        #region Properties

        public override string Name
        {
            get { return LABEL; }
        }

        protected override Vector2 DefaultSteer
        {
            get { return _element.Direction; }
        }

        #endregion

        #region Methods

        protected override Vector2 SteerToOthers(IEnumerable<Element> others, double weight, bool average)
        {
            Vector2 x = _element.Direction;
            foreach (Element e in others)
            {
                x += e.GetDirection();
            }
            if (average) x /= others.Count() + 1;
            return weight * x;
        }

        #endregion
    }
}

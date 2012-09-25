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
    public class CohesionSteering : ToNeighboursSteering
    {
        #region Constants

        public const string LABEL = "Cohesion";

        #endregion

        #region Constructors

        public CohesionSteering(Element element, double weight) : base(element, weight) { }

        #endregion

        #region Properties

        public override string Name
        {
            get { return LABEL; }
        }

        #endregion

        #region Methods

        protected override Vector2 SteerToOthers(IEnumerable<Element> others, double weight)
        {
            Vector2 x = Vector2.Zero;
            foreach (Element e in others)
            {
                x += Vector2.Normalized(e.GetPosition() - _element.Position);
            }
            return weight * x / others.Count();
        }

        #endregion
    }
}

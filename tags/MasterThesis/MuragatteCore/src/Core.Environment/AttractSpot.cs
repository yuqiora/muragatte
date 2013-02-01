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
    public class AttractSpot : Extras
    {
        #region Constructors

        public AttractSpot() : base() { }

        public AttractSpot(int id, MultiAgentSystem model, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model, species, radius) { }

        public AttractSpot(int id, MultiAgentSystem model, Vector2 position, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model, position, species, radius) { }

        protected AttractSpot(AttractSpot other, MultiAgentSystem model)
            : base(other, model) { }

        #endregion

        #region Properties

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Companion; }
        }

        public override string Name
        {
            get { return CreateName("Ea"); }
        }

        #endregion

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new AttractSpot(this, model);
        }

        #endregion
    }
}

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
    public class RepelSpot : Extras
    {
        #region Constructors

        public RepelSpot(MultiAgentSystem model, Species species, double radius = DEFAULT_RADIUS)
            : base(model, species, radius) { }

        public RepelSpot(MultiAgentSystem model, Vector2 position, Species species, double radius = DEFAULT_RADIUS)
            : base(model, position, species, radius) { }

        public RepelSpot(int id, MultiAgentSystem model, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model, species, radius) { }

        public RepelSpot(int id, MultiAgentSystem model, Vector2 position, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model, position, species, radius) { }

        protected RepelSpot(RepelSpot other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Properties

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Threat; }
        }

        public override string Name
        {
            get { return CreateName("Er"); }
        }

        #endregion

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new RepelSpot(this, model);
        }

        #endregion
    }
}

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
    public class PositionGoal : Goal
    {
        #region Constructors

        public PositionGoal(MultiAgentSystem model, Species species)
            : base(model, species) { }

        public PositionGoal(MultiAgentSystem model, Vector2 position, Species species)
            : base(model, position, species) { }

        public PositionGoal(int id, MultiAgentSystem model, Species species)
            : base(id, model, species) { }

        public PositionGoal(int id, MultiAgentSystem model, Vector2 position, Species species)
            : base(id, model, position, species) { }

        #endregion

        #region Properties

        public override double Width
        {
            get { return 1; }
        }

        public override double Height
        {
            get { return 1; }
        }

        public override double Radius
        {
            get { return DEFAULT_RADIUS; }
        }

        public override string Name
        {
            get { return CreateName("Gp"); }
        }

        #endregion
    }
}

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

        public PositionGoal(MultiAgentSystem model)
            : base(model) { }

        public PositionGoal(MultiAgentSystem model, Vector2 position)
            : base(model, position) { }

        public PositionGoal(int id, MultiAgentSystem model)
            : base(id, model) { }

        public PositionGoal(int id, MultiAgentSystem model, Vector2 position)
            : base(id, model, position) { }

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

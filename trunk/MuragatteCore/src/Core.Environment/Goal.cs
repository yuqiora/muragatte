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
    public abstract class Goal : Element
    {
        #region Constructors

        public Goal(MultiAgentSystem model)
            : base(model) { }

        public Goal(MultiAgentSystem model, Vector2 position)
            : base(model, position) { }

        public Goal(int id, MultiAgentSystem model)
            : base(id, model) { }

        public Goal(int id, MultiAgentSystem model, Vector2 position)
            : base(id, model, position) { }

        #endregion

        #region Properties

        public override Vector2 Direction
        {
            get { return new Vector2(0, 0); }
            set { }
        }

        public override double Speed
        {
            get { return 0; }
            set { }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Goal; }
        }

        public override string Name
        {
            get { return CreateName("G"); }
        }

        public override bool IsStationary
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public override void Update() { }

        public override void ConfirmUpdate() { }

        public override ElementNature RelationshipWith(Element e)
        {
            return ElementNature.Ignored;
        }

        public override string ToString()
        {
            return ToString("G");
        }

        #endregion
    }
}

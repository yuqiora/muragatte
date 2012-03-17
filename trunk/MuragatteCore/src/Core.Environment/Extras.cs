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
    public abstract class Extras : Element
    {
        #region Constructors

        public Extras(int id, MultiAgentSystem model)
            : base(id, model) { }

        public Extras(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed)
            : base(id, model, position, direction, speed) { }

        #endregion

        #region Properties

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Ignored; }
        }

        #endregion

        #region Methods

        public override void Update() { }

        public override void AfterUpdate() { }

        public override ElementNature RelationshipWith(Element e)
        {
            return ElementNature.Ignored;
        }

        #endregion
    }
}

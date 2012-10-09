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
    public abstract class StationaryElement : Element
    {
        #region Constructors

        public StationaryElement(MultiAgentSystem model) : base(model) { }

        public StationaryElement(MultiAgentSystem model, Vector2 position) : base(model, position) { }

        public StationaryElement(int id, MultiAgentSystem model) : base(id, model) { }

        public StationaryElement(int id, MultiAgentSystem model, Vector2 position) : base(id, model, position) { }

        protected StationaryElement(Element other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Properties

        public override Vector2 Direction
        {
            get { return Vector2.Zero; }
            set { }
        }

        public override double Speed
        {
            get { return 0; }
            set { }
        }

        public override bool IsStationary
        {
            get { return true; }
        }

        public override bool IsDirectable
        {
            get { return false; }
        }

        public override bool IsResizeable
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

        #endregion
    }
}

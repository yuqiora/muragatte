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
    public abstract class Obstacle : Element
    {
        //shapes
        //size

        public Obstacle(int id, MultiAgentSystem model)
            : base(id, model) { }

        public Obstacle(int id, MultiAgentSystem model, Vector2 position, Vector2 direction)
            : base(id, model, position, direction, 0) { }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Obstacle; }
        }

        public override void Update() { }

        public override void AfterUpdate() { }

        public override ElementNature RelationshipWith(Element e)
        {
            return ElementNature.Ignored;
        }

        public override string ToString()
        {
            return "O-" + base.ToString();
        }

    }
}

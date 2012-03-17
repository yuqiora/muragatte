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
    public class Centroid : Extras
    {
        //group

        public Centroid(int id, MultiAgentSystem model)
            : base(id, model)
        {
            _bStationary = false;
        }

        public Centroid(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed)
            : base(id, model, position, direction, speed)
        {
            _bStationary = false;
        }

        //update fields according to group members
        public override void AfterUpdate() { }

        public override double Size
        {
            get { return 1; }
        }

        public override string ToString()
        {
            return "C-" + base.ToString();
        }
    }
}

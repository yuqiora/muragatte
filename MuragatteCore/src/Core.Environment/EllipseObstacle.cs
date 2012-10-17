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
    public class EllipseObstacle : Obstacle
    {
        #region Constructors

        public EllipseObstacle(int id, MultiAgentSystem model, Species species, double size)
            : base(id, model, species, size, size) { }

        public EllipseObstacle(int id, MultiAgentSystem model, Species species, double width, double height)
            : base(id, model, species, width, height) { }

        public EllipseObstacle(int id, MultiAgentSystem model, Vector2 position, Species species, double size)
            : base(id, model, position, species, size, size) { }

        public EllipseObstacle(int id, MultiAgentSystem model, Vector2 position, Species species, double width, double height)
            : base(id, model, position, species, width, height) { }

        protected EllipseObstacle(EllipseObstacle other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Properties

        public override double Radius
        {
            get { return Math.Max(_dWidth, _dHeight) / 2.0; }
        }

        public override string Name
        {
            get { return CreateName("Oe"); }
        }

        #endregion

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new EllipseObstacle(this, model);
        }

        #endregion
    }
}

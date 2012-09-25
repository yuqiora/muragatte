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
        #region Fields

        protected double _dWidth = 1;
        protected double _dHeight = 1;

        #endregion

        #region Constructors

        public Obstacle(MultiAgentSystem model, double width, double height)
            : base(model)
        {
            _dWidth = width;
            _dHeight = height;
        }

        public Obstacle(MultiAgentSystem model, Vector2 position, double width, double height)
            : base(model, position)
        {
            _dWidth = width;
            _dHeight = height;
        }

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

        public override double Width
        {
            get { return _dWidth; }
        }

        public override double Height
        {
            get { return _dHeight; }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Obstacle; }
        }

        public override string Name
        {
            get { return CreateName("O"); }
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
            return ToString("O");
        }

        #endregion
    }

    public class EllipseObstacle : Obstacle
    {
        #region Constructors

        public EllipseObstacle(MultiAgentSystem model, double size)
            : base(model, size, size) { }

        public EllipseObstacle(MultiAgentSystem model, double width, double height)
            : base(model, width, height) { }

        public EllipseObstacle(MultiAgentSystem model, Vector2 position, double size)
            : base(model, position, size, size) { }

        public EllipseObstacle(MultiAgentSystem model, Vector2 position, double width, double height)
            : base(model, position, width, height) { }

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
    }

    public class RectangleObstacle : Obstacle
    {
        #region Constructors

        public RectangleObstacle(MultiAgentSystem model, double size)
            : base(model, size, size) { }

        public RectangleObstacle(MultiAgentSystem model, double width, double height)
            : base(model, width, height) { }

        public RectangleObstacle(MultiAgentSystem model, Vector2 position, double size)
            : base(model, position, size, size) { }

        public RectangleObstacle(MultiAgentSystem model, Vector2 position, double width, double height)
            : base(model, position, width, height) { }

        #endregion

        #region Properties

        public override double Radius
        {
            get { return Math.Sqrt(_dWidth * _dWidth + _dHeight * _dHeight) / 2.0; }
        }

        public override string Name
        {
            get { return CreateName("Or"); }
        }

        #endregion
    }
}

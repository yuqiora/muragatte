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

        public Obstacle(MultiAgentSystem model, Species species, double width, double height)
            : this(IdCounter.Next(), model, species, width, height) { }

        public Obstacle(MultiAgentSystem model, Vector2 position, Species species, double width, double height)
            : this(IdCounter.Next(), model, position, species, width, height) { }

        public Obstacle(int id, MultiAgentSystem model, Species species, double width, double height)
            : base(id, model)
        {
            Construct(species, width, height);
        }

        public Obstacle(int id, MultiAgentSystem model, Vector2 position, Species species, double width, double height)
            : base(id, model, position)
        {
            Construct(species, width, height);
        }

        protected Obstacle(Obstacle other, MultiAgentSystem model)
            : base(other, model)
        {
            _species = other._species;
            _dWidth = other._dWidth;
            _dHeight = other._dHeight;
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
            set { _dWidth = value; }
        }

        public override double Height
        {
            get { return _dHeight; }
            set { _dHeight = value; }
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

        protected void Construct(Species species, double width, double height)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_OBSTACLES_LABEL);
            _dWidth = width;
            _dHeight = height;
        }

        #endregion
    }

    public class EllipseObstacle : Obstacle
    {
        #region Constructors

        public EllipseObstacle(MultiAgentSystem model, Species species, double size)
            : base(model, species, size, size) { }

        public EllipseObstacle(MultiAgentSystem model, Species species, double width, double height)
            : base(model, species, width, height) { }

        public EllipseObstacle(MultiAgentSystem model, Vector2 position, Species species, double size)
            : base(model, position, species, size, size) { }

        public EllipseObstacle(MultiAgentSystem model, Vector2 position, Species species, double width, double height)
            : base(model, position, species, width, height) { }

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

    public class RectangleObstacle : Obstacle
    {
        #region Constructors

        public RectangleObstacle(MultiAgentSystem model, Species species, double size)
            : base(model, species, size, size) { }

        public RectangleObstacle(MultiAgentSystem model, Species species, double width, double height)
            : base(model, species, width, height) { }

        public RectangleObstacle(MultiAgentSystem model, Vector2 position, Species species, double size)
            : base(model, position, species, size, size) { }

        public RectangleObstacle(MultiAgentSystem model, Vector2 position, Species species, double width, double height)
            : base(model, position, species, width, height) { }

        public RectangleObstacle(int id, MultiAgentSystem model, Species species, double size)
            : base(id, model, species, size, size) { }

        public RectangleObstacle(int id, MultiAgentSystem model, Species species, double width, double height)
            : base(id, model, species, width, height) { }

        public RectangleObstacle(int id, MultiAgentSystem model, Vector2 position, Species species, double size)
            : base(id, model, position, species, size, size) { }

        public RectangleObstacle(int id, MultiAgentSystem model, Vector2 position, Species species, double width, double height)
            : base(id, model, position, species, width, height) { }

        protected RectangleObstacle(RectangleObstacle other, MultiAgentSystem model) : base(other, model) { }

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

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new RectangleObstacle(this, model);
        }

        #endregion
    }
}

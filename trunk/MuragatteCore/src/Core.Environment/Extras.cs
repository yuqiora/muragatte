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

        public Extras(MultiAgentSystem model, bool stationary)
           : base(model)
        {
            _bStationary = stationary;
        }

        public Extras(MultiAgentSystem model, Vector2 position, bool stationary)
            : base(model, position)
        {
            _bStationary = stationary;
        }

        #endregion

        #region Properties

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Ignored; }
        }

        public override string Name
        {
            get { return CreateName("E"); }
        }

        #endregion

        #region Methods

        public override ElementNature RelationshipWith(Element e)
        {
            return ElementNature.Ignored;
        }

        public override string ToString()
        {
            return ToString("E");
        }

        #endregion
    }

    public class AttractSpot : Extras
    {
        #region Fields

        protected double _dRadius;

        #endregion

        #region Constructors

        public AttractSpot(MultiAgentSystem model, double radius = DEFAULT_RADIUS)
            : base(model, true)
        {
            _dRadius = radius;
        }

        public AttractSpot(MultiAgentSystem model, Vector2 position, double radius = DEFAULT_RADIUS)
            : base(model, position, true)
        {
            _dRadius = radius;
        }

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

        public override double Width
        {
            get { return 2 * _dRadius; }
        }

        public override double Height
        {
            get { return 2 * _dRadius; }
        }

        public override double Radius
        {
            get { return _dRadius; }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Companion; }
        }

        public override string Name
        {
            get { return CreateName("Ea"); }
        }

        #endregion

        #region Methods

        public override void Update() { }

        public override void ConfirmUpdate() { }

        #endregion
    }

    public class RepelSpot : AttractSpot
    {
        #region Constructors

        public RepelSpot(MultiAgentSystem model, double radius = DEFAULT_RADIUS)
            : base(model, radius) { }

        public RepelSpot(MultiAgentSystem model, Vector2 position, double radius = DEFAULT_RADIUS)
            : base(model, position, radius) { }

        #endregion

        #region Properties

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Threat; }
        }

        public override string Name
        {
            get { return CreateName("Er"); }
        }

        #endregion
    }

    public class Guidepost : AttractSpot
    {
        #region Fields

        private Vector2 _direction;

        #endregion

        #region Constructors

        public Guidepost(MultiAgentSystem model, Vector2 direction)
            : base(model)
        {
            _direction = direction;
        }

        public Guidepost(MultiAgentSystem model, Vector2 position, Vector2 direction)
            : base(model, position)
        {
            _direction = direction;
        }

        #endregion

        #region Properties

        public override Vector2 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public override string Name
        {
            get { return CreateName("Eg"); }
        }

        #endregion
    }
}

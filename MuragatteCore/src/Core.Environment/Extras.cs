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

        public Extras(MultiAgentSystem model, Species species)
            : base(model)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_EXTRAS_LABEL);
        }

        public Extras(MultiAgentSystem model, Vector2 position, Species species)
            : base(model, position)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_EXTRAS_LABEL);
        }

        public Extras(int id, MultiAgentSystem model, Species species)
            : base(id, model)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_EXTRAS_LABEL);
        }

        public Extras(int id, MultiAgentSystem model, Vector2 position, Species species)
            : base(id, model, position)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_EXTRAS_LABEL);
        }

        protected Extras(Extras other, MultiAgentSystem model)
            : base(other, model)
        {
            _species = other._species;
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

        public override bool IsStationary
        {
            get { return true; }
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

        public AttractSpot(MultiAgentSystem model, Species species, double radius = DEFAULT_RADIUS)
            : base(model, species)
        {
            _dRadius = radius;
        }

        public AttractSpot(MultiAgentSystem model, Vector2 position, Species species, double radius = DEFAULT_RADIUS)
            : base(model, position, species)
        {
            _dRadius = radius;
        }

        protected AttractSpot(AttractSpot other, MultiAgentSystem model)
            : base(other, model)
        {
            _dRadius = other._dRadius;
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
            set { }
        }

        public override double Height
        {
            get { return 2 * _dRadius; }
            set { }
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

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new AttractSpot(this, model);
        }

        #endregion
    }

    public class RepelSpot : AttractSpot
    {
        #region Constructors

        public RepelSpot(MultiAgentSystem model, Species species, double radius = DEFAULT_RADIUS)
            : base(model, species, radius) { }

        public RepelSpot(MultiAgentSystem model, Vector2 position, Species species, double radius = DEFAULT_RADIUS)
            : base(model, position, species, radius) { }

        protected RepelSpot(RepelSpot other, MultiAgentSystem model) : base(other, model) { }

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

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new RepelSpot(this, model);
        }

        #endregion
    }

    public class Guidepost : AttractSpot
    {
        #region Fields

        private Vector2 _direction;

        #endregion

        #region Constructors

        public Guidepost(MultiAgentSystem model, Vector2 direction, Species species)
            : base(model, species)
        {
            _direction = direction;
        }

        public Guidepost(MultiAgentSystem model, Vector2 position, Vector2 direction, Species species)
            : base(model, position, species)
        {
            _direction = direction;
        }

        protected Guidepost(Guidepost other, MultiAgentSystem model)
            : base(other, model)
        {
            _direction = other._direction;
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

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new Guidepost(this, model);
        }

        #endregion
    }
}

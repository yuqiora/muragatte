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

        public Goal(MultiAgentSystem model, Species species)
            : base(model)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_GOALS_LABEL);
        }

        public Goal(MultiAgentSystem model, Vector2 position, Species species)
            : base(model, position)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_GOALS_LABEL);
        }

        public Goal(int id, MultiAgentSystem model, Species species)
            : base(id, model)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_GOALS_LABEL);
        }

        public Goal(int id, MultiAgentSystem model, Vector2 position, Species species)
            : base(id, model, position)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_GOALS_LABEL);
        }

        protected Goal(Goal other, MultiAgentSystem model)
            : base(other, model)
        {
            _species = other._species;
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

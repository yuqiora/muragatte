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
    public abstract class Goal : StationaryElement
    {
        #region Constructors

        public Goal() : base() { }

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

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Goal; }
        }

        public override string Name
        {
            get { return CreateName("G"); }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return ToString("G");
        }

        #endregion
    }
}

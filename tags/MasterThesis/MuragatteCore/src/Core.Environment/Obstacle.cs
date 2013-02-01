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
using System.Xml.Serialization;
using Muragatte.Common;

namespace Muragatte.Core.Environment
{
    public abstract class Obstacle : StationaryElement
    {
        #region Fields

        protected double _dWidth = 1;
        protected double _dHeight = 1;

        #endregion

        #region Constructors

        public Obstacle() : base() { }

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

        [XmlElement]
        public override double Width
        {
            get { return _dWidth; }
            set
            {
                _dWidth = value;
                NotifyPropertyChanged("Width");
            }
        }

        [XmlElement]
        public override double Height
        {
            get { return _dHeight; }
            set
            {
                _dHeight = value;
                NotifyPropertyChanged("Height");
            }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Obstacle; }
        }

        public override string Name
        {
            get { return CreateName("O"); }
        }

        #endregion

        #region Methods

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
}

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
    public abstract class Extras : StationaryElement
    {
        #region Fields

        protected double _dRadius;

        #endregion

        #region Constructors

        public Extras() : base() { _dRadius = DEFAULT_RADIUS; }

        public Extras(int id, MultiAgentSystem model, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model)
        {
            _dRadius = radius;
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_EXTRAS_LABEL);
        }

        public Extras(int id, MultiAgentSystem model, Vector2 position, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model, position)
        {
            _dRadius = radius;
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_EXTRAS_LABEL);
        }

        protected Extras(Extras other, MultiAgentSystem model)
            : base(other, model)
        {
            _dRadius = other._dRadius;
            _species = other._species;
        }

        #endregion

        #region Properties

        [XmlElement]
        public override double Width
        {
            get { return 2 * _dRadius; }
            set
            {
                _dRadius = value / 2;
                NotifySizeChanged();
            }
        }

        [XmlElement]
        public override double Height
        {
            get { return 2 * _dRadius; }
            set
            {
                _dRadius = value / 2;
                NotifySizeChanged();
            }
        }

        public override double Radius
        {
            get { return _dRadius; }
        }

        public override string Name
        {
            get { return CreateName("E"); }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return ToString("E");
        }

        protected void NotifySizeChanged()
        {
            NotifyPropertyChanged("Width");
            NotifyPropertyChanged("Height");
        }

        #endregion
    }
}

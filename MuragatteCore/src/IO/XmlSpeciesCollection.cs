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
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.IO
{
    public class XmlSpeciesCollection
    {
        #region Fields

        private Species[] _items = null;
        private XmlSpeciesCollectionDefaults _defaults = null;

        #endregion

        #region Constructors

        public XmlSpeciesCollection() { }

        public XmlSpeciesCollection(SpeciesCollection collection)
        {
            _items = collection.Progenitors.ToArray();
            _defaults = new XmlSpeciesCollectionDefaults(collection);
        }

        #endregion

        #region Properties

        [XmlElement(ElementName = "Species", Type = typeof(XmlSpecies))]
        public Species[] Items
        {
            get { return _items; }
            set
            {
                _items = value;
                XmlSpeciesReference.KnownSpecies = new SpeciesCollection(value);
            }
        }

        [XmlElement(IsNullable = false)]
        public XmlSpeciesCollectionDefaults Defaults
        {
            get { return _defaults.AnySpecified ? _defaults : null; }
            set { _defaults = value; }
        }

        #endregion

        #region Methods

        public void ApplyToCollection(SpeciesCollection collection)
        {
            collection.Add(_items);
            _defaults.SetDefaultsToCollection(collection);
        }

        public SpeciesCollection ToSpeciesCollection()
        {
            SpeciesCollection sc = new SpeciesCollection();
            ApplyToCollection(sc);
            return sc;
        }

        #endregion

        #region Operators

        public static implicit operator SpeciesCollection(XmlSpeciesCollection x)
        {
            return x.ToSpeciesCollection();
        }

        public static implicit operator XmlSpeciesCollection(SpeciesCollection s)
        {
            return new XmlSpeciesCollection(s);
        }

        #endregion
    }
}

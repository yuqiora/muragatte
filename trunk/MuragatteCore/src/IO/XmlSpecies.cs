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
    public class XmlSpecies
    {
        #region Fields

        [XmlAttribute]
        public string Name = null;

        private Species[] _subspecies = null;
        private XmlSpeciesRelation[] _relations = null;

        #endregion

        #region Constructors

        public XmlSpecies() { }

        public XmlSpecies(Species species)
        {
            Name = species.Name;
            _subspecies = species.Children.ToArray();
            _relations = ConvertRelations(species.Relationships);
        }

        #endregion

        #region Properties

        [XmlArray(IsNullable = false)]
        [XmlArrayItem(ElementName = "Species", Type = typeof(XmlSpecies))]
        public Species[] Subspecies
        {
            get { return _subspecies == null || _subspecies.Length == 0 ? null : _subspecies; }
            set { _subspecies = value; }
        }

        [XmlArray(IsNullable = false)]
        [XmlArrayItem(ElementName = "Relation")]
        public XmlSpeciesRelation[] Relationships
        {
            get { return _relations == null || _relations.Length == 0 ? null : _relations; }
            set { _relations = value; }
        }

        #endregion

        #region Methods

        private XmlSpeciesRelation[] ConvertRelations(Dictionary<string, ElementNature> relations)
        {
            List<XmlSpeciesRelation> result = new List<XmlSpeciesRelation>();
            foreach (KeyValuePair<string, ElementNature> x in relations)
            {
                result.Add(new XmlSpeciesRelation(x));
            }
            return result.ToArray();
        }

        public Species ToSpecies()
        {
            Species s = new Species(Name, null, _subspecies);
            if (_relations != null)
            {
                foreach (XmlSpeciesRelation r in _relations)
                {
                    s.Relationships.Add(r.With, r.Nature);
                }
            }
            return s;
        }

        #endregion

        #region Operators

        public static implicit operator Species(XmlSpecies x)
        {
            return x.ToSpecies();
        }

        public static implicit operator XmlSpecies(Species s)
        {
            return new XmlSpecies(s);
        }

        #endregion
    }
}

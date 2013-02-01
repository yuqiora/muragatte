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
    public class XmlSpeciesReference
    {
        #region Fields

        [XmlIgnore]
        public static SpeciesCollection KnownSpecies = null;

        [XmlText]
        public string Name = null;

        #endregion

        #region Constructors

        public XmlSpeciesReference() { }

        public XmlSpeciesReference(Species s)
        {
            if (s != null) Name = s.FullName;
        }

        #endregion

        #region Methods

        public Species ToSpecies()
        {
            return Name == null || KnownSpecies == null ? null : KnownSpecies[Name];
        }

        #endregion

        #region Operators

        public static implicit operator Species(XmlSpeciesReference x)
        {
            return x.ToSpecies();
        }

        public static implicit operator XmlSpeciesReference(Species s)
        {
            return new XmlSpeciesReference(s);
        }

        #endregion
    }
}

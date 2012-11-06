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
    public class XmlSpeciesRelation
    {
        #region Fields

        [XmlAttribute]
        public string With = null;
        
        [XmlAttribute]
        public ElementNature Nature = ElementNature.Unknown;

        #endregion

        #region Constructors

        public XmlSpeciesRelation() { }

        public XmlSpeciesRelation(KeyValuePair<string, ElementNature> pair) : this(pair.Key, pair.Value) { }

        public XmlSpeciesRelation(Species with, ElementNature nature) : this(with.FullName, nature) { }

        public XmlSpeciesRelation(string with, ElementNature nature)
        {
            With = with;
            Nature = nature;
        }

        #endregion
    }
}

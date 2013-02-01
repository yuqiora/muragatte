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
using Muragatte.Core.Storage;

namespace Muragatte.IO
{
    [XmlRoot(ElementName = "Muragatte", Namespace = "Muragatte/Species")]
    public class XmlSpeciesCollectionRoot
    {
        #region Fields

        public XmlSpeciesCollection KnownSpecies = null;

        #endregion

        #region Constructors

        public XmlSpeciesCollectionRoot() { }

        public XmlSpeciesCollectionRoot(SpeciesCollection collection)
        {
            KnownSpecies = new XmlSpeciesCollection(collection);
        }

        #endregion
    }
}

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

namespace Muragatte.IO
{
    public class XmlSpawnSpotReference
    {
        #region Fields

        [XmlIgnore]
        public static IEnumerable<SpawnSpot> KnownSpawnSpots = null;

        [XmlText]
        public string Name = null;

        #endregion

        #region Constructors

        public XmlSpawnSpotReference() { }

        public XmlSpawnSpotReference(SpawnSpot s)
        {
            if (s != null) Name = s.Name;
        }

        #endregion

        #region Methods

        public SpawnSpot ToSpawnSpot()
        {
            return Name == null || KnownSpawnSpots == null ? null : KnownSpawnSpots.FirstOrDefault(s => s.Name == Name);
        }

        #endregion

        #region Operators

        public static implicit operator SpawnSpot(XmlSpawnSpotReference x)
        {
            return x.ToSpawnSpot();
        }

        public static implicit operator XmlSpawnSpotReference(SpawnSpot s)
        {
            return new XmlSpawnSpotReference(s);
        }

        #endregion
    }
}

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
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public class PointSpawnSpot : SpawnSpot
    {
        #region Constructors

        public PointSpawnSpot() : base() { }

        public PointSpawnSpot(string name, Vector2 position) : base(name, position, 1, 1) { }

        #endregion

        #region Properties

        [XmlIgnore]
        public override double Width
        {
            get { return 1; }
            set { }
        }

        [XmlIgnore]
        public override double Height
        {
            get { return 1; }
            set { }
        }

        #endregion

        #region Methods

        public override Vector2 Respawn(RandomMT random)
        {
            return Respawn();
        }

        public override string ToString()
        {
            return string.Format(". {0} .", base.ToString());
        }

        #endregion
    }
}

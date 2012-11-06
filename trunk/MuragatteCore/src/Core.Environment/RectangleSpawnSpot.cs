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
    public class RectangleSpawnSpot : SpawnSpot
    {
        #region Constructors

        public RectangleSpawnSpot() : base() { }

        public RectangleSpawnSpot(string name, Vector2 position, double width, double height) : base(name, position, width, height) { }

        public RectangleSpawnSpot(string name, Vector2 position, double size) : this(name, position, size, size) { }

        #endregion

        #region Properties

        [XmlAttribute]
        public override double Width
        {
            get { return base.Width; }
            set { base.Width = value; }
        }

        [XmlAttribute]
        public override double Height
        {
            get { return base.Height; }
            set { base.Height = value; }
        }

        #endregion

        #region Methods

        public override Vector2 Respawn(RandomMT random)
        {
            return random.UniformVector(_position, _dWidth, _dHeight);
        }

        public override string ToString()
        {
            return string.Format("[ {0} ]", base.ToString());
        }

        #endregion
    }
}

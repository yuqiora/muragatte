﻿// ------------------------------------------------------------------------
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
using Muragatte.Common;
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public class EllipseSpawnSpot : SpawnSpot
    {
        #region Constructors

        public EllipseSpawnSpot(Vector2 position, double width, double height) : base(position, width, height) { }

        #endregion

        #region Methods

        public override Vector2 Respawn(RandomMT random)
        {
            return random.Disk(_position, _dWidth, _dHeight);
        }

        public override string ToString()
        {
            return string.Format("( {0} )", base.ToString());
        }

        #endregion
    }
}
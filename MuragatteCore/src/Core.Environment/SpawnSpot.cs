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
using Muragatte.Common;
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public abstract class SpawnSpot
    {
        #region Fields

        protected Vector2 _position;
        protected double _dWidth;
        protected double _dHeight;

        #endregion

        #region Constructors

        public SpawnSpot(Vector2 position, double width, double height)
        {
            _position = position;
            _dWidth = width;
            _dHeight = height;
        }

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public virtual double Width
        {
            get { return _dWidth; }
            set { _dWidth = value; }
        }

        public virtual double Heigth
        {
            get { return _dHeight; }
            set { _dHeight = value; }
        }

        #endregion

        #region Methods

        public Vector2 Respawn()
        {
            return _position;
        }

        public abstract Vector2 Respawn(RandomMT random);

        #endregion
    }
}

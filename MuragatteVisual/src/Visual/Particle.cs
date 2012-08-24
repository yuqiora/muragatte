// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
//
// Copyright (C) 2012  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

/*  O B S O L E T E  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;
using SysWin = System.Windows;

namespace Muragatte.Visual
{
    public abstract class Particle
    {
        #region Fields

        protected Color _color = Colors.Black;

        #endregion

        #region Constructors

        public Particle(Color color)
        {
            _color = color;
        }

        #endregion

        #region Properties

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        #endregion

        #region Abstract Properties

        public abstract int XC { get; }

        public abstract int YC { get; }

        #endregion

        #region Abstract Methods

        public abstract void DrawInto(WriteableBitmap wb, Vector2 position, Vector2 direction, float alpha = 1);

        #endregion
    }
}

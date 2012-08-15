// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;

namespace Muragatte.Visual.Shapes
{
    public abstract class Shape
    {
        #region Constructors

        protected Shape() { }

        #endregion

        #region Methods

        public abstract void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null);

        public abstract void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates);

        public abstract List<Coordinates> CreateCoordinates(int width, int height, object other = null);

        protected List<Coordinates> ListOfOne(Coordinates item)
        {
            List<Coordinates> list = new List<Coordinates>();
            list.Add(item);
            return list;
        }

        #endregion
    }
}

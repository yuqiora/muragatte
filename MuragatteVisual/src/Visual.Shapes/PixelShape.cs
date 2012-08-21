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
    public class PixelShape : Shape
    {
        #region Fields

        private static PixelShape _shape = new PixelShape();

        #endregion

        #region Constructors

        private PixelShape() { }

        #endregion

        #region Properties

        public override string Symbol
        {
            get { return "."; }
        }

        #endregion

        #region Methods

        public static PixelShape Instance()
        {
            return _shape;
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            Draw(target, position, primaryColor, secondaryColor);
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            Draw(target, position, primaryColor, secondaryColor);
        }

        private void Draw(WriteableBitmap target, Vector2 position, Color primaryColor, Color secondaryColor)
        {
            target.SetPixel(position, primaryColor.NotTransparent() ? primaryColor : secondaryColor);
        }

        public override List<Coordinates> CreateCoordinates(int width, int height, object other = null)
        {
            return null;
        }

        public override string ToString()
        {
            return "Pixel";
        }

        #endregion
    }
}

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

        public static PixelShape Instance
        {
            get { return _shape; }
        }

        #endregion

        #region Methods

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            Draw(target, position, angle, primaryColor, secondaryColor, null);
        }

        protected override void BlitDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            Color s = primaryColor.NotTransparent() ? primaryColor : secondaryColor;
            Color d = target.GetPixel(position.Xi, position.Yi);
            //based on Blit code from WriteableBitmapEx library
            int destPixel = ((((s.A * s.A) + ((255 - s.A) * d.A)) >> 8) << 24) +
                            ((((s.A * s.R) + ((255 - s.A) * d.R)) >> 8) << 16) +
                            ((((s.A * s.G) + ((255 - s.A) * d.G)) >> 8) << 8) +
                            (((s.A * s.B) + ((255 - s.A) * d.B)) >> 8);
            target.SetPixel(position.Xi, position.Yi, destPixel);
        }

        protected override void FullDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
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

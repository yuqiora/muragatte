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
    public class QuadPixelShape : Shape
    {
        #region Fields

        private static QuadPixelShape _shape = new QuadPixelShape();

        #endregion

        #region Constructors

        private QuadPixelShape() { }

        #endregion

        #region Properties

        public override string Symbol
        {
            get { return "::"; }
        }

        public override ShapeLabel Label
        {
            get { return ShapeLabel.QuadPixel; }
        }

        public static QuadPixelShape Instance
        {
            get { return _shape; }
        }

        #endregion

        #region Methods

        protected override void BlitDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            coordinates[0].Bitmap.Clear(primaryColor.NotTransparent() ? primaryColor : secondaryColor);
            target.Blit(position, coordinates[0].Bitmap);
        }

        protected override void FullDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            if (position.Xi == target.PixelWidth - 1) position.X--;
            if (position.Yi == target.PixelHeight - 1) position.Y--;
            target.FillRectangle(position, position + new Vector2(1, 1), primaryColor.NotTransparent() ? primaryColor : secondaryColor);
        }

        public override List<Coordinates> CreateCoordinates(int width, int height, object other = null)
        {
            return ListOfOne(new Coordinates(2, 2, BitmapFactory.New(2, 2)));
        }

        public override string ToString()
        {
            return "QuadPixel";
        }

        #endregion
    }
}

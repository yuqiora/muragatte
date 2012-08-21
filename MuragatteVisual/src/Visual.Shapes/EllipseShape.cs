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
    public class EllipseShape : Shape
    {
        #region Fields

        private static EllipseShape _shape = new EllipseShape();

        #endregion

        #region Constructors

        private EllipseShape() { }

        #endregion

        #region Properties

        public override string Symbol
        {
            get { return "()"; }
        }

        #endregion

        #region Methods

        public static EllipseShape Instance()
        {
            return _shape;
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            if (width == 1 && height == 1)
            {
                PixelShape.Instance().Draw(target, position, angle, primaryColor, secondaryColor, width, height, other);
            }
            else
            {
                if (primaryColor.NotTransparent())
                {
                    target.FillEllipseCentered(position, width / 2, height / 2, primaryColor);
                }
                if (primaryColor != secondaryColor && secondaryColor.NotTransparent())
                {
                    target.DrawEllipseCentered(position, width / 2, height / 2, secondaryColor);
                }
            }
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            Draw(target, position, angle, primaryColor, secondaryColor, coordinates[0].P1.Xi, coordinates[0].P1.Yi);
        }

        public override List<Coordinates> CreateCoordinates(int width, int height, object other = null)
        {
            return ListOfOne(new Coordinates(width, height));
        }

        public override string ToString()
        {
            return "Ellipse";
        }

        #endregion
    }
}

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
    public class TriangleShape : Shape
    {
        #region Fields

        private static TriangleShape _shape = new TriangleShape();

        #endregion

        #region Constructors

        private TriangleShape() { }

        #endregion

        #region Properties

        public override string Symbol
        {
            get { return "|>"; }
        }

        public override ShapeLabel Label
        {
            get { return ShapeLabel.Triangle; }
        }

        public static TriangleShape Instance
        {
            get { return _shape; }
        }

        #endregion

        #region Methods

        protected override void FullDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            Coordinates points = coordinates[0].Move(position).Rotate(position, angle);
            if (primaryColor.NotTransparent())
            {
                target.FillTriangle(points.P1, points.P2, points.P3, primaryColor);
            }
            if (primaryColor != secondaryColor && secondaryColor.NotTransparent())
            {
                target.DrawTriangle(points.P1, points.P2, points.P3, secondaryColor);
            }
        }

        public override List<Coordinates> CreateCoordinates(int width, int height, object other = null)
        {
            int x1 = width / 2;
            int y1 = 0;
            int x2 = x1 - width + 1;
            int y2 = y1 - height / 3;
            int y3 = y1 + height / 3;
            return ListOfOne(new Coordinates(x1, y1, x2, y2, x2, y3, BitmapFactory.New(width * 2, height * 2)));
        }

        public override string ToString()
        {
            return "Triangle";
        }

        #endregion
    }
}

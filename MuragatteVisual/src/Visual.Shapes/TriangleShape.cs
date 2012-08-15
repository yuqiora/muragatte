﻿// ------------------------------------------------------------------------
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

        #region Methods

        public static TriangleShape Instance()
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
                Draw(target, position, angle, primaryColor, secondaryColor, CreateCoordinates(width, height));
            }
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
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
            int x1 = 0;
            int y1 = height / 2;
            int x2 = x1 - (width / 3);
            int y2 = y1 - height + 1;
            int x3 = x1 + (width / 3);
            return ListOfOne(new Coordinates(x1, y1, x2, y2, x3, y2));
        }

        public override string ToString()
        {
            return "Triangle";
        }

        #endregion
    }
}
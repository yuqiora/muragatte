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

        public override ShapeLabel Label
        {
            get { return ShapeLabel.Ellipse; }
        }

        public static EllipseShape Instance
        {
            get { return _shape; }
        }

        #endregion

        #region Methods

        protected override void FullDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            Coordinates points = coordinates[0];
            if (primaryColor.NotTransparent())
            {
                target.FillEllipseCentered(position, points.P1.Xi / 2, points.P1.Yi / 2, primaryColor);
            }
            if (primaryColor != secondaryColor && secondaryColor.NotTransparent())
            {
                target.DrawEllipseCentered(position, points.P1.Xi / 2, points.P1.Yi / 2, secondaryColor);
            }
        }

        public override List<Coordinates> CreateCoordinates(int width, int height, object other = null)
        {
            return ListOfOne(new Coordinates(width, height, BitmapFactory.New(width + 2, height + 2)));
        }

        public override string ToString()
        {
            return "Ellipse";
        }

        #endregion
    }
}

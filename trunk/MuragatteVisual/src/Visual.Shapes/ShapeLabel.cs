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

namespace Muragatte.Visual.Shapes
{
    public enum ShapeLabel
    {
        Pixel,
        QuadPixel,
        Ellipse,
        Rectangle,
        Triangle,
        PointingCircle,
        Arc
    }

    public static class ShapeLabelExtensions
    {
        public static Shape ToShape(this ShapeLabel shape)
        {
            switch (shape)
            {
                case ShapeLabel.Pixel:
                    return PixelShape.Instance;
                case ShapeLabel.QuadPixel:
                    return QuadPixelShape.Instance;
                case ShapeLabel.Ellipse:
                    return EllipseShape.Instance;
                case ShapeLabel.Rectangle:
                    return RectangleShape.Instance;
                case ShapeLabel.Triangle:
                    return TriangleShape.Instance;
                case ShapeLabel.PointingCircle:
                    return PointingCircleShape.Instance;
                case ShapeLabel.Arc:
                    return ArcShape.Instance;
                default:
                    return null;
            }
        }
    }
}

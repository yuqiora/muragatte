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


    public abstract class Shape
    {
        #region Constructors

        protected Shape() { }

        #endregion

        #region Methods

        public abstract void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null);

        protected Vector2 Rotate(Vector2 point, Vector2 origin, Angle angle)
        {
            if (angle.IsZero)
            {
                return point;
            }
            else
            {
                return point - origin + angle + origin;
            }
        }

        #endregion
    }

    public class PixelShape : Shape
    {
        #region Fields

        private static PixelShape _shape = new PixelShape();

        #endregion

        #region Constructors

        private PixelShape() { }

        #endregion

        #region Methods

        public static PixelShape Instance()
        {
            return _shape;
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            target.SetPixel(position.Xi, position.Yi, primaryColor);
        }

        #endregion
    }

    public class QuadPixelShape : Shape
    {
        #region Fields

        private static QuadPixelShape _shape = new QuadPixelShape();

        #endregion

        #region Constructors

        private QuadPixelShape() { }

        #endregion

        #region Methods

        public static QuadPixelShape Instance()
        {
            return _shape;
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            target.FillRectangle(position.Xi, position.Yi, position.Xi + 1, position.Yi + 1, primaryColor);
        }

        #endregion
    }

    public class EllipseShape : Shape
    {
        #region Fields

        private static EllipseShape _shape = new EllipseShape();

        #endregion

        #region Constructors

        private EllipseShape() { }

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
                int x1 = position.Xi - (width / 2);
                int y1 = target.PixelHeight - position.Yi - (height / 2) - 1;
                int x2 = x1 + width - 1;
                int y2 = y1 + height - 1;
                target.FillEllipse(x1, y1, x2, y2, primaryColor);
                if (primaryColor != secondaryColor)
                {
                    target.DrawEllipse(x1, y1, x2, y2, secondaryColor);
                }
            }
        }

        #endregion
    }

    public class RectangleShape : Shape
    {
        #region Fields

        private static RectangleShape _shape = new RectangleShape();

        #endregion

        #region Constructors

        private RectangleShape() { }

        #endregion

        #region Methods

        public static RectangleShape Instance()
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
                int x1 = position.Xi - (width / 2);
                int y1 = target.PixelHeight - position.Yi - (height / 2) - 1;
                int x2 = x1 + width - 1;
                int y2 = y1 + height - 1;
                if (angle.IsZero)
                {
                    target.FillRectangle(x1, y1, x2, y2, primaryColor);
                    if (primaryColor != secondaryColor)
                    {
                        target.DrawRectangle(x1, y1, x2, y2, secondaryColor);
                    }
                }
                else
                {
                    Vector2 p1 = Rotate(new Vector2(x1, y1), position, angle);
                    Vector2 p2 = Rotate(new Vector2(x2, y1), position, angle);
                    Vector2 p3 = Rotate(new Vector2(x2, y2), position, angle);
                    Vector2 p4 = Rotate(new Vector2(x1, y2), position, angle);
                    target.FillQuad(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, p4.Xi, p4.Yi, primaryColor);
                    if (primaryColor != secondaryColor)
                    {
                        target.DrawQuad(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, p4.Xi, p4.Yi, secondaryColor);
                    }
                }
            }
        }

        #endregion
    }

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
                int x1 = position.Xi;
                int y1 = target.PixelHeight - position.Yi - (height / 2) - 1;
                int x2 = x1 - (width / 2) + 1;
                int y2 = y1 + height - 1;
                int x3 = x1 + (width / 2) - 2;
                Vector2 p1 = Rotate(new Vector2(x1, y1), position, angle);
                Vector2 p2 = Rotate(new Vector2(x2, y2), position, angle);
                Vector2 p3 = Rotate(new Vector2(x3, y2), position, angle);
                target.FillTriangle(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, primaryColor);
                if (primaryColor != secondaryColor)
                {
                    target.DrawTriangle(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, secondaryColor);
                }
            }
        }

        #endregion
    }

    public class PointingCircleShape : Shape
    {
        #region Fields

        private static PointingCircleShape _shape = new PointingCircleShape();

        #endregion

        #region Constructors

        private PointingCircleShape() { }

        #endregion

        #region Methods

        public static PointingCircleShape Instance()
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
                target.FillEllipseCentered(position.Xi, position.Yi, width / 2, height / 2, primaryColor);
                target.DrawEllipseCentered(position.Xi, position.Yi, width / 2, height / 2, secondaryColor);
                Vector2 p = Rotate(position - new Vector2(0, height), position, angle);
                target.DrawLine(p.Xi, p.Yi, position.Xi, position.Yi, secondaryColor);
            }
        }

        #endregion
    }

    //later
    //public class ArcShape : Shape
    //{
    //    #region Constants

    //    private const int DEFAULT_ARC_DEGREES = 60;

    //    #endregion

    //    #region Fields

    //    private static ArcShape _shape = new ArcShape();

    //    #endregion

    //    #region Constructors

    //    private ArcShape() { }

    //    #endregion

    //    #region Methods

    //    public static ArcShape Instance()
    //    {
    //        return _shape;
    //    }

    //    public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
    //    {
    //        //bezier, see http://hansmuller-flex.blogspot.cz/2011/04/approximating-circular-arc-with-cubic.html
    //        if (width == 1 && height == 1)
    //        {
    //            PixelShape.Instance().Draw(target, position, angle, primaryColor, secondaryColor, width, height, other);
    //        }
    //        else
    //        {
    //            Angle arc = new Angle(DEFAULT_ARC_DEGREES);
    //            if (other is Angle)
    //            {
    //                arc = (Angle)other;
    //            }
    //        }
    //    }

    //    private void DrawPart() { }

    //    #endregion
    //}
}

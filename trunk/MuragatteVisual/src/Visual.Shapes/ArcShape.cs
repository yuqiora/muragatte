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
    //based on http://hansmuller-flex.blogspot.com/2011/10/more-about-approximating-circular-arcs.html
    public class ArcShape : Shape
    {
        #region Constants

        private const double EPSILON = 0.00001;

        #endregion

        #region Fields

        private static ArcShape _shape = new ArcShape();

        private readonly Angle _deg90 = new Angle(-90);

        #endregion

        #region Constructors

        private ArcShape() { }

        #endregion

        #region Properties

        public override string Symbol
        {
            get { return "(<"; }
        }

        public static ArcShape Instance
        {
            get { return _shape; }
        }

        #endregion

        #region Methods

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            if (width == 1 && height == 1)
            {
                PixelShape.Instance.Draw(target, position, angle, primaryColor, secondaryColor, width, height, other);
            }
            else
            {
                Draw(target, position, angle, primaryColor, secondaryColor, CreateCoordinates(width, height, other));
                //Angle arc = new Angle(DEFAULT_ARC_DEGREES);
                //if (other != null && other is Angle)
                //{
                //    arc = (Angle)other;
                //}
                //List<Coordinates> points = CreateArc(width, -arc.Radians, arc.Radians);
                //foreach (Coordinates c in points)
                //{
                //    c.Move(position);
                //    c.Rotate(position, angle - _deg90);
                //}
                //if (primaryColor.NotTransparent())
                //{
                //    foreach (Coordinates ap in points)
                //    {
                //        target.FillBeziers(ap.FillBeziersCoordinates(position), primaryColor);
                //        //target.FillTriangle(cp.P1, cp.P4, position, primaryColor);
                //    }
                //}
                //if (primaryColor != secondaryColor && secondaryColor.NotTransparent())
                //{
                //    foreach (Coordinates ap in points)
                //    {
                //        target.DrawBezier(ap.P1, ap.P2, ap.P3, ap.P4, secondaryColor);
                //    }
                //    target.DrawLine(points[0].P1, position, secondaryColor);
                //    target.DrawLine(points[points.Count - 1].P4, position, secondaryColor);
                //}
            }
        }

        public override void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            List<Coordinates> points = new List<Coordinates>();
            foreach (Coordinates cp in coordinates)
            {
                points.Add(cp.Move(position).Rotate(position, angle));
            }
            if (primaryColor.NotTransparent())
            {
                foreach (Coordinates cp in points)
                {
                    target.FillBeziers(cp.FillBeziersCoordinates(position), primaryColor);
                    target.DrawLine(cp.P1, position, primaryColor);
                }
            }
            if (primaryColor != secondaryColor && secondaryColor.NotTransparent())
            {
                foreach (Coordinates cp in points)
                {
                    target.DrawBezier(cp.P1, cp.P2, cp.P3, cp.P4, secondaryColor);
                }
                target.DrawLine(points[0].P1, position, secondaryColor);
                target.DrawLine(points[points.Count - 1].P4, position, secondaryColor);
            }
        }

        public override List<Coordinates> CreateCoordinates(int width, int height, object other = null)
        {
            Angle arc = new Angle(DefaultValues.NEIGHBOURHOOD_ANGLE_DEGREES);
            if (other != null && other is Angle)
            {
                arc = (Angle)other;
            }
            return CreateArc((width + height) / 2, -arc.Radians, arc.Radians);
        }

        private List<Coordinates> CreateArc(int radius, double startAngle, double endAngle)
        {
            List<Coordinates> points = new List<Coordinates>();

            double twoPI = Math.PI * 2;
            //double startAngleN = startAngle % twoPI;
            //double endAngleN = endAngle % twoPI;

            double piOverTwo = Math.PI / 2.0;
            double sgn = (startAngle < endAngle) ? +1 : -1;

            double a1 = startAngle;
            //double totalAngle = Math.Min(twoPI, Math.Abs(endAngleN - startAngleN));
            double totalAngle = Math.Min(twoPI, Math.Abs(endAngle - startAngle));
            while (totalAngle > EPSILON)
            {
                double a2 = a1 + sgn * Math.Min(totalAngle, piOverTwo);
                points.Add(CreateSmallArc(radius, a1, a2));
                totalAngle -= Math.Abs(a2 - a1);
                a1 = a2;
            }

            return points;
        }

        private Coordinates CreateSmallArc(int r, double a1, double a2)
        {
            double a = (a2 - a1) / 2;

            double x4 = r * Math.Cos(a);
            double y4 = r * Math.Sin(a);
            double x1 = x4;
            double y1 = -y4;

            double q1 = x1 * x1 + y1 * y1;
            double q2 = q1 + x1 * x4 + y1 * y4;
            double k2 = 4 / 3 * (Math.Sqrt(2 * q1 * q2) - q2) / (x1 * y4 - y1 * x4);

            double x2 = x1 - k2 * y1;
            double y2 = y1 + k2 * x1;
            double x3 = x2;
            double y3 = -y2;

            double ar = a + a1;
            double cos_ar = Math.Cos(ar);
            double sin_ar = Math.Sin(ar);

            return new Coordinates(
                r * Math.Cos(a1),
                r * Math.Sin(a1),
                x2 * cos_ar - y2 * sin_ar,
                x2 * sin_ar + y2 * cos_ar,
                x3 * cos_ar - y3 * sin_ar,
                x3 * sin_ar + y3 * cos_ar,
                r * Math.Cos(a2),
                r * Math.Sin(a2)).Rotate(Vector2.Zero, _deg90);
        }

        public override string ToString()
        {
            return "Arc";
        }

        #endregion
    }
}

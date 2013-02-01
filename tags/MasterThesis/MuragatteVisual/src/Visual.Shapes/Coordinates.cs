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
using System.Windows.Media.Imaging;
using Muragatte.Common;

namespace Muragatte.Visual.Shapes
{
    public class Coordinates
    {
        #region Fields

        private Vector2 _p1;
        private Vector2 _p2;
        private Vector2 _p3;
        private Vector2 _p4;
        private WriteableBitmap _wb;

        #endregion

        #region Constructors

        public Coordinates(Vector2 p1, WriteableBitmap wb = null)
            : this(p1, Vector2.Zero, Vector2.Zero, Vector2.Zero, wb) { }

        public Coordinates(Vector2 p1, Vector2 p2, WriteableBitmap wb = null)
            : this(p1, p2, Vector2.Zero, Vector2.Zero, wb) { }

        public Coordinates(Vector2 p1, Vector2 p2, Vector2 p3, WriteableBitmap wb = null)
            : this(p1, p2, p3, Vector2.Zero, wb) { }

        public Coordinates(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, WriteableBitmap wb = null)
        {
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
            _p4 = p4;
            _wb = wb;
        }

        public Coordinates(double x1, double y1, WriteableBitmap wb = null)
            : this(x1, y1, 0, 0, 0, 0, 0, 0, wb) { }

        public Coordinates(double x1, double y1, double x2, double y2, WriteableBitmap wb = null)
            : this(x1, y1, x2, y2, 0, 0, 0, 0, wb) { }

        public Coordinates(double x1, double y1, double x2, double y2, double x3, double y3, WriteableBitmap wb = null)
            : this(x1, y1, x2, y2, x3, y3, 0, 0, wb) { }

        public Coordinates(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, WriteableBitmap wb = null)
            : this(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x3, y3), new Vector2(x4, y4), wb) { }

        #endregion

        #region Properties

        public Vector2 P1
        {
            get { return _p1; }
            set { _p1 = value; }
        }

        public Vector2 P2
        {
            get { return _p2; }
            set { _p2 = value; }
        }

        public Vector2 P3
        {
            get { return _p3; }
            set { _p3 = value; }
        }

        public Vector2 P4
        {
            get { return _p4; }
            set { _p4 = value; }
        }

        public WriteableBitmap Bitmap
        {
            get { return _wb; }
            set { _wb = value; }
        }

        #endregion

        #region Methods

        public List<double> ToList()
        {
            List<double> l = new List<double>();
            l.Add(_p1.X);
            l.Add(_p1.Y);
            l.Add(_p2.X);
            l.Add(_p2.Y);
            l.Add(_p3.X);
            l.Add(_p3.Y);
            l.Add(_p4.X);
            l.Add(_p4.Y);
            return l;
        }

        public int[] ToIntArray()
        {
            return new int[]
                {
                    _p1.Xi, _p1.Yi,
                    _p2.Xi, _p2.Yi,
                    _p3.Xi, _p3.Yi,
                    _p4.Xi, _p4.Yi
                };
        }

        public int[] FillBeziersCoordinates(Vector2 point)
        {
            return new int[]
                {
                    point.Xi, point.Yi,
                    point.Xi, point.Yi,
                    point.Xi, point.Yi,
                    _p1.Xi, _p1.Yi,
                    _p2.Xi, _p2.Yi,
                    _p3.Xi, _p3.Yi,
                    _p4.Xi, _p4.Yi,
                    point.Xi, point.Yi,
                    point.Xi, point.Yi,
                    point.Xi, point.Yi
                };
        }

        public Coordinates Move(Vector2 position)
        {
            return new Coordinates(
                _p1 + position,
                _p2 + position,
                _p3 + position,
                _p4 + position);
        }

        public Coordinates Move(Vector2 position, Vector2 origin)
        {
            return Move(position - origin);
        }

        public Coordinates Rotate(Vector2 origin, Angle angle)
        {
            if (angle.IsZero)
            {
                return this;
            }
            else
            {
                return new Coordinates(
                    Rotate(_p1, origin, angle),
                    Rotate(_p2, origin, angle),
                    Rotate(_p3, origin, angle),
                    Rotate(_p4, origin, angle));
            }
        }

        public static Vector2 Rotate(Vector2 point, Vector2 origin, Angle angle)
        {
            //return angle.IsZero ? point : point - origin - angle + origin;
            return point - origin + angle + origin;
        }

        #endregion
    }
}

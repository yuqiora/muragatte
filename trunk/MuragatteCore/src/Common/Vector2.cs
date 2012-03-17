// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Core Library
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

namespace Muragatte.Common
{
    public struct Vector2
    {
        #region Fields

        private double _dX;
        private double _dY;

        #endregion

        #region Constructors

        public Vector2(double x, double y)
        {
            _dX = x;
            _dY = y;
        }

        #endregion

        #region Properties

        public double X
        {
            get { return _dX; }
            set { _dX = value; }
        }

        public double Y
        {
            get { return _dY; }
            set { _dY = value; }
        }

        public double Length
        {
            get { return Math.Sqrt(_dX * _dX + _dY * _dY); }
        }

        public double LengthSquared
        {
            get { return _dX * _dX + _dY * _dY; }
        }

        public int Xi
        {
            get { return (int)_dX; }
        }

        public int Yi
        {
            get { return (int)_dY; }
        }

        public bool IsZero
        {
            get { return _dX == 0 && _dY == 0; }
        }

        #endregion

        #region Methods

        public void Negate()
        {
            _dX = -_dX;
            _dY = -_dY;
        }

        public void Normalize()
        {
            _dX /= Length;
            _dY /= Length;
        }

        public Vector2 Normalized()
        {
            Vector2 v = new Vector2(_dX, _dY);
            v.Normalize();
            return v;
        }

        public bool Equals(Vector2 v)
        {
            return _dX == v.X && _dY == v.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Vector2)
            {
                Vector2 v = (Vector2)obj;
                return _dX == v.X && _dY == v.Y;
            }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return _dX.GetHashCode() ^ _dY.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", _dX, _dY);
        }
        
        #endregion

        #region Static Methods
        
        public static Vector2 Add(Vector2 a, Vector2 b) { return a + b; }

        public static double AngleBetween(Vector2 a, Vector2 b)
        {
            return Math.Acos((a * b) / (a.Length * b.Length)) * (180 / Math.PI);
        }

        public static Vector2 Divide(Vector2 vector, double scalar) { return vector / scalar; }

        public static bool Equals(Vector2 a, Vector2 b) { return a == b; }

        public static Vector2 Multiply(double scalar, Vector2 vector) { return scalar * vector; }

        public static Vector2 Multiply(Vector2 vector, double scalar) { return vector * scalar; }

        public static double Multiply(Vector2 a, Vector2 b) { return a * b; }

        public static Vector2 Parse(string s)
        {
            string[] sTmp = s.Split(' ');
            if (sTmp.Length != 2)
            {
                return new Vector2(0, 0);
            }
            else
            {
                double dx = 0;
                double dy = 0;
                if (!double.TryParse(sTmp[0], out dx)) { dx = 0; }
                if (!double.TryParse(sTmp[1], out dy)) { dy = 0; }
                return new Vector2(dx, dy);
            }
        }

        public static Vector2 Subtract(Vector2 a, Vector2 b) { return a - b; }

        public static double Distance(Vector2 a, Vector2 b) { return (a - b).Length; }

        public static double DistanceX(Vector2 a, Vector2 b) { return Math.Abs(a.X - b.X); }

        public static double DistanceY(Vector2 a, Vector2 b) { return Math.Abs(a.Y - b.Y); }

        public static Vector2 Zero() { return new Vector2(0, 0); }

        public static Vector2 UpOne() { return new Vector2(0, 1); }
        
        #endregion

        #region Operators

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator /(Vector2 vector, double scalar)
        {
            return new Vector2(vector.X / scalar, vector.Y / scalar);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Vector2 operator *(double scalar, Vector2 vector)
        {
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        public static Vector2 operator *(Vector2 vector, double scalar)
        {
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        public static double operator *(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator -(Vector2 vector)
        {
            return new Vector2(-vector.X, -vector.Y);
        }
        
        #endregion
    }
}

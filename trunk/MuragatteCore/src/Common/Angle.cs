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
    //values between -180 and 180
    public struct Angle
    {
        #region Constants

        public const double MaxDegree = 180;

        #endregion

        #region Fields

        private double _dDegrees;

        #endregion

        #region Constructors

        public Angle(double degrees)
        {
            _dDegrees = degrees;
            Normalize();
        }

        public Angle(Vector2 vector)
        {
            _dDegrees = Math.Atan2(vector.X, vector.Y) * (180 / Math.PI);
        }

        #endregion

        #region Properties

        public double Degrees
        {
            get { return _dDegrees; }
            set
            {
                _dDegrees = value;
                Normalize();
            }
        }

        public int DegreesI
        {
            get { return (int)_dDegrees; }
            set
            {
                _dDegrees = value;
                Normalize();
            }
        }

        public double Radians
        {
            get { return _dDegrees * (Math.PI / 180.0); }
            set
            {
                _dDegrees = value * (180 / Math.PI);
                Normalize();
            }
        }

        public bool IsZero
        {
            get { return _dDegrees == 0; }
        }

        #endregion

        #region Static Properties

        public static Angle Zero
        {
            get { return new Angle(0); }
        }

        public static Angle Deg180
        {
            get { return new Angle(180); }
        }

        #endregion

        #region Methods

        public void Negate()
        {
            _dDegrees = -_dDegrees;
        }

        public void Normalize()
        {
            _dDegrees %= 360;
            if (_dDegrees > MaxDegree)
            {
                _dDegrees -= 360;
            }
            if (_dDegrees <= -MaxDegree)
            {
                _dDegrees += 360;
            }
        }

        public Angle Abs()
        {
            return new Angle(Math.Abs(_dDegrees));
        }

        public int Sign()
        {
            return Math.Sign(_dDegrees);
        }

        public bool Equals(Angle a)
        {
            return _dDegrees == a._dDegrees;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Angle)
            {
                Angle a = (Angle)obj;
                return _dDegrees == a._dDegrees;
            }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return _dDegrees.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}°", _dDegrees.ToString());
        }

        #endregion

        #region Static Methods

        public static Angle FromRadians(double radians)
        {
            return new Angle(radians * 180 / Math.PI);
        }

        public static Angle Parse(string s)
        {
            double d;
            if (!double.TryParse(s, out d)) { d = 0; }
            return new Angle(d);
        }

        public static Angle Add(Angle a, Angle b)
        {
            return a + b;
        }

        public static Angle Subtract(Angle a, Angle b)
        {
            return a - b;
        }

        public static Angle Multiply(Angle angle, double value)
        {
            return angle * value;
        }

        public static Angle Multiply(double value, Angle angle)
        {
            return value * angle;
        }

        public static Angle Divide(Angle angle, double value)
        {
            return angle / value;
        }

        public static bool Equals(Angle a, Angle b)
        {
            return a == b;
        }

        //will be modified accordingly when Muragatte.Random is done
        //or more likely moved under Random

        public static Angle Random()
        {
            return new Angle(Core.Environment.RNGs.Ran2.Gauss());
        }

        public static Angle Random(double deviation)
        {
            return new Angle(Core.Environment.RNGs.Ran2.Gauss(0, deviation));
        }

        #endregion

        #region Operators

        public static Angle operator +(Angle a, Angle b)
        {
            return new Angle(a._dDegrees + b._dDegrees);
        }

        public static Angle operator -(Angle a, Angle b)
        {
            return new Angle(a._dDegrees - b._dDegrees);
        }

        public static Angle operator -(Angle angle)
        {
            return new Angle(-angle._dDegrees);
        }

        public static Angle operator *(Angle angle, double value)
        {
            return new Angle(angle._dDegrees * value);
        }

        public static Angle operator *(double value, Angle angle)
        {
            return new Angle(value * angle._dDegrees);
        }

        public static Angle operator /(Angle angle, double value)
        {
            return new Angle(angle._dDegrees / value);
        }

        public static Angle operator %(Angle angle, double value)
        {
            return new Angle(angle._dDegrees % value);
        }

        public static bool operator ==(Angle a, Angle b)
        {
            return a._dDegrees == b._dDegrees;
        }

        public static bool operator !=(Angle a, Angle b)
        {
            return a._dDegrees != b._dDegrees;
        }

        public static bool operator >(Angle a, Angle b)
        {
            return Math.Abs(a._dDegrees) > Math.Abs(b._dDegrees);
        }

        public static bool operator <(Angle a, Angle b)
        {
            return Math.Abs(a._dDegrees) < Math.Abs(b._dDegrees);
        }

        public static bool operator >=(Angle a, Angle b)
        {
            return Math.Abs(a._dDegrees) >= Math.Abs(b._dDegrees);
        }

        public static bool operator <=(Angle a, Angle b)
        {
            return Math.Abs(a._dDegrees) <= Math.Abs(b._dDegrees);
        }

        #endregion
    }
}

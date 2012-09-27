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
using Muragatte.Common;
using RandomOps;

namespace Muragatte.Random
{
    public class RandomMT
    {
        #region Fields

        protected MersenneTwister _random;

        #endregion

        #region Constructors

        public RandomMT()
        {
            _random = new MersenneTwister();
        }

        public RandomMT(uint seed)
        {
            _random = new MersenneTwister(seed);
        }

        #endregion

        #region Methods

        public double Uniform()
        {
            return _random.Uniform();
        }

        public double Uniform(double low, double high)
        {
            return _random.Uniform(low, high);
        }

        public double Gauss()
        {
            return _random.Gauss();
        }

        public double Gauss(double mean, double deviation)
        {
            return _random.Gauss(mean, deviation);
        }

        public uint UInt()
        {
            return _random.Rand();
        }

        public Angle UniformAngle()
        {
            return UniformAngle(-Angle.MaxDegree, Angle.MaxDegree);
        }

        public Angle UniformAngle(double low, double high)
        {
            return new Angle(Uniform(low, high));
        }

        public Angle GaussAngle(double deviation)
        {
            return GaussAngle(0, deviation);
        }

        public Angle GaussAngle(double mean, double deviation)
        {
            return new Angle(Gauss(mean, deviation));
        }

        public void Disk(out double x, out double y, out double sumSquares)
        {
            _random.Disk(out x, out y, out sumSquares);
        }

        public Vector2 Disk()
        {
            return Disk(Vector2.Zero, 2, 2);
        }

        public Vector2 Disk(double width, double height)
        {
            return Disk(Vector2.Zero, width, height);
        }

        public Vector2 Disk(Vector2 origin, double width, double height)
        {
            double x, y, ss;
            Disk(out x, out y, out ss);
            return new Vector2(x * width / 2, y * height / 2) + origin;
        }

        public void Circle(out double x, out double y)
        {
            _random.Circle(out x, out y);
        }

        public Vector2 Circle()
        {
            return Circle(Vector2.Zero, 2, 2);
        }

        public Vector2 Circle(double width, double height)
        {
            return Circle(Vector2.Zero, width, height);
        }

        public Vector2 Circle(Vector2 origin, double width, double height)
        {
            double x, y;
            Circle(out x, out y);
            return new Vector2(x * width / 2, y * height / 2) + origin;
        }

        public Vector2 NormalizedVector()
        {
            double x, y;
            Circle(out x, out y);
            return new Vector2(x, y, true);
        }

        public Vector2 UniformVector(double xLow, double xHigh, double yLow, double yHigh)
        {
            return new Vector2(Uniform(xLow, xHigh), Uniform(yLow, yHigh));
        }

        public Vector2 UniformVector(Vector2 origin, double width, double height)
        {
            return UniformVector(origin.X - width / 2, origin.X + width / 2, origin.Y - height / 2, origin.Y + height / 2);
        }

        public Vector2 GaussVector(double xMean, double xDeviation, double yMean, double yDeviation)
        {
            return new Vector2(Gauss(xMean, xDeviation), Gauss(yMean, yDeviation));
        }

        public Vector2 GaussVector(Vector2 origin, double xDeviation, double yDeviation)
        {
            return GaussVector(origin.X, xDeviation, origin.Y, yDeviation);
        }

        public Noise Noise(double a, double b, Distribution distribution)
        {
            return distribution.Noise(this, a, b);
        }

        #endregion
    }
}

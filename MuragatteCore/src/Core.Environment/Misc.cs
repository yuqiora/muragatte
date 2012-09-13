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

namespace Muragatte.Core.Environment
{
    public enum ElementNature
    {
        Unknown,
        Companion,
        Goal,
        Obstacle,
        Threat,
        Ignored
    }

    //temporary
    public static class RNGs
    {
        private static RandomOps.KISS _kiss = new RandomOps.KISS();
        private static RandomOps.MersenneTwister _mersenneTwister = new RandomOps.MersenneTwister();
        private static RandomOps.Ran2 _ran2 = new RandomOps.Ran2();

        public static RandomOps.KISS KISS
        {
            get { return _kiss; }
        }

        public static RandomOps.MersenneTwister MersenneTwister
        {
            get { return _mersenneTwister; }
        }

        public static RandomOps.Ran2 Ran2
        {
            get { return _ran2; }
        }

        public static double Uniform()
        {
            return _ran2.Uniform();
        }

        public static double Uniform(double high)
        {
            return _ran2.Uniform(0, high);
        }

        public static double Uniform(double low, double high)
        {
            return _ran2.Uniform(low, high);
        }

        public static double Gauss()
        {
            return _ran2.Gauss();
        }

        public static double Gauss(double deviation)
        {
            return _ran2.Gauss(0, deviation);
        }

        public static double Gauss(double mean, double deviation)
        {
            return _ran2.Gauss(mean, deviation);
        }
    }

    public abstract class SpawnSpot
    {
        protected Vector2 _position;

        public SpawnSpot(Vector2 position)
        {
            _position = position;
        }

        public Vector2 Position
        {
            get { return _position; }
        }

        public abstract double Width { get; set; }

        public abstract double Heigth { get; set; }

        public abstract Vector2 Respawn();
    }

    public class SpawnPoint : SpawnSpot
    {
        public SpawnPoint(Vector2 position) : base(position) { }

        public override double Width
        {
            get { return 1; }
            set { }
        }

        public override double Heigth
        {
            get { return 1; }
            set { }
        }

        public override Vector2 Respawn()
        {
            return _position;
        }
    }

    public class SpawnCircle : SpawnSpot
    {
        protected double _dRadius;

        public SpawnCircle(Vector2 position, double radius)
            : base(position)
        {
            _dRadius = radius;
        }

        public override double Width
        {
            get { return _dRadius * 2; }
            set { _dRadius = value / 2; }
        }

        public override double Heigth
        {
            get { return _dRadius * 2; }
            set { _dRadius = value / 2; }
        }

        public override Vector2 Respawn()
        {
            //temporary, to be completed after random
            double x, y, ss;
            RNGs.Ran2.Disk(out x, out y, out ss);
            return new Vector2(x, y) + _position;
        }
    }

    public class SpawnRectangle : SpawnSpot
    {
        protected double _dWidth;
        protected double _dHeight;

        public SpawnRectangle(Vector2 position, double width, double height)
            : base(position)
        {
            _dWidth = width;
            _dHeight = height;
        }

        public SpawnRectangle(Vector2 position, double size) : this(position, size, size) { }

        public override double Width
        {
            get { return _dWidth; }
            set { _dWidth = value; }
        }

        public override double Heigth
        {
            get { return _dHeight; }
            set { _dHeight = value; }
        }

        public override Vector2 Respawn()
        {
            //temporary, to be completed after random
            double w = _dWidth / 2;
            double h = _dHeight / 2;
            return Vector2.RandomUniform(_position.X - w, _position.X + w, _position.Y - h, _position.Y + h);
        }
    }
}

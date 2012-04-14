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

    public class HistoryRecord
    {
    }

    //additional changes expected
    public class ElementStatus
    {
        #region Fields

        private int _iElementID;
        private Vector2 _position;
        private Vector2 _direction;
        private double _dSpeed;
        private bool _bEnabled;
        private int _iSpeciesID;
        //group?
        private double[] _dModifiers = null;

        #endregion

        #region Constructors

        public ElementStatus(int elementID, Vector2 position, Vector2 direction,
            double speed, bool enabled, int speciesID)
        {
            _iElementID = elementID;
            _position = position;
            _direction = direction;
            _dSpeed = speed;
            _bEnabled = enabled;
            _iSpeciesID = speciesID;
        }

        public ElementStatus(int elementID, Vector2 position, Vector2 direction,
            double speed, bool enabled, int speciesID, params double[] modifiers)
            : this(elementID, position, direction, speed, enabled, speciesID)
        {
            _dModifiers = modifiers;
        }

        #endregion

        #region Properties

        public int ElementID
        {
            get { return _iElementID; }
        }

        public Vector2 Position
        {
            get { return _position; }
        }

        public Vector2 Direction
        {
            get { return _direction; }
        }

        public double Speed
        {
            get { return _dSpeed; }
        }

        public bool IsEnabled
        {
            get { return _bEnabled; }
        }

        public int SpeciesID
        {
            get { return _iSpeciesID; }
        }

        public double[] Modifiers
        {
            get { return _dModifiers; }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}",
                _iElementID, _position, _direction, _dSpeed, _bEnabled, _iSpeciesID, _dModifiers);
        }

        #endregion
    }
}

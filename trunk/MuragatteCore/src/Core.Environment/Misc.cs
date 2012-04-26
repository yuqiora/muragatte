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

    public class Group
    {
        #region Fields

        private Centroid _representative = null;

        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
    }
}

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
    }
}

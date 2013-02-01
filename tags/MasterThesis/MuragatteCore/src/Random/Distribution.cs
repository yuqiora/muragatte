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

namespace Muragatte.Random
{
    public enum Distribution
    {
        None,
        Uniform,
        Gaussian
    }

    public static class DistributionExtension
    {
        public static Noise Noise(this Distribution d, RandomMT random, double a, double b)
        {
            switch (d)
            {
                case Distribution.Uniform:
                    return new UniformNoise(random, a, b);
                case Distribution.Gaussian:
                    return new GaussianNoise(random, a, b);
                default:
                    return new ZeroNoise();
            }
        }

        public static Noise Noise(this Distribution d, double a, double b)
        {
            return Noise(d, null, a, b);
        }
    }
}

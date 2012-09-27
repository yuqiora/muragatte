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
    public class GaussianNoise : Noise
    {
        #region Constructors

        public GaussianNoise(RandomMT random, double mean, double deviation) : base(random, mean, deviation) { }

        #endregion

        #region Properties

        public double Mean
        {
            get { return _dA; }
            set { _dA = value; }
        }

        public double Deviation
        {
            get { return _dB; }
            set { _dB = value; }
        }

        public override Distribution Distribution
        {
            get { return Distribution.Gaussian; }
        }

        #endregion

        #region Methods

        protected override double SafeApply(RandomMT random)
        {
            return random.Gauss(_dA, _dB);
        }

        #endregion
    }
}

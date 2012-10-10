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
    public class NoisedDouble
    {
        #region Fields

        private double _dBaseValue;
        private Noise _noise;

        #endregion

        #region Constructors

        public NoisedDouble(double baseValue) : this(baseValue, new ZeroNoise()) { }

        public NoisedDouble(double baseValue, Noise noise)
        {
            _dBaseValue = baseValue;
            _noise = noise;
        }

        public NoisedDouble(double baseValue, Distribution distribution, double a, double b)
            : this(baseValue, distribution, null, a, b) { }

        public NoisedDouble(double baseValue, Distribution distribution, RandomMT random, double a, double b)
            : this(baseValue, distribution.Noise(random, a, b)) { }

        public NoisedDouble(Noise noise) : this(0, noise) { }

        public NoisedDouble(Distribution distribution, double a, double b)
            : this(0, distribution, null, a, b) { }

        public NoisedDouble(Distribution distribution, RandomMT random, double a, double b)
            : this(0, distribution, random, a, b) { }

        #endregion

        #region Properties

        public double BaseValue
        {
            get { return _dBaseValue; }
            set { _dBaseValue = value; }
        }

        public Noise Noise
        {
            get { return _noise; }
            set { _noise = value; }
        }

        #endregion

        #region Methods

        public double GetValue()
        {
            return _dBaseValue + _noise.Apply();
        }

        public double GetValue(RandomMT random)
        {
            return _dBaseValue + _noise.Apply(random);
        }

        public void ChangeNoiseDistribution(Distribution d)
        {
            _noise = d.Noise(_noise.Random, _noise.A, _noise.B);
        }

        #endregion
    }
}

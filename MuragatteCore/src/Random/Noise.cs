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

namespace Muragatte.Random
{
    public abstract class Noise
    {
        #region Fields

        protected RandomMT _random;
        protected double _dA;
        protected double _dB;

        #endregion

        #region Constructors

        public Noise(RandomMT random, double a, double b)
        {
            _random = random;
            _dA = a;
            _dB = b;
        }

        #endregion

        #region Properties

        public double A
        {
            get { return _dA; }
            set { _dA = value; }
        }

        public double B
        {
            get { return _dB; }
            set { _dB = value; }
        }

        public RandomMT Random
        {
            get { return _random; }
        }

        public abstract Distribution Distribution { get; }

        #endregion

        #region Methods

        public double Apply()
        {
            return Apply(_random);
        }

        public double Apply(RandomMT random)
        {
            return random == null ? 0 : SafeApply(random);
        }

        public Angle ApplyAngle()
        {
            return new Angle(Apply());
        }

        public Angle ApplyAngle(RandomMT random)
        {
            return new Angle(Apply(random));
        }

        protected abstract double SafeApply(RandomMT random);

        #endregion
    }
}

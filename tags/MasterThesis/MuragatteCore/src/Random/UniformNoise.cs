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
using System.Xml.Serialization;

namespace Muragatte.Random
{
    public class UniformNoise : Noise
    {
        #region Constructors

        public UniformNoise() : base() { }

        public UniformNoise(RandomMT random, double low, double high) : base(random, low, high) { }

        #endregion

        #region Properties

        [XmlAttribute]
        public double Low
        {
            get { return _dA; }
            set { _dA = value; }
        }

        [XmlAttribute]
        public double High
        {
            get { return _dB; }
            set { _dB = value; }
        }

        public override Distribution Distribution
        {
            get { return Distribution.Uniform; }
        }

        #endregion

        #region Methods

        protected override double SafeApply(RandomMT random)
        {
            return random.Uniform(_dA, _dB);
        }

        #endregion
    }
}

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
    public class GaussianNoise : Noise
    {
        #region Constructors

        public GaussianNoise() : base() { }

        public GaussianNoise(RandomMT random, double deviation, double limit) : base(random, deviation, limit) { }

        #endregion

        #region Properties

        [XmlAttribute]
        public double Deviation
        {
            get { return _dA; }
            set { _dA = value; }
        }

        [XmlAttribute]
        public double Limit
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
            return random.GaussLimited(_dA, _dB);
        }

        #endregion
    }
}

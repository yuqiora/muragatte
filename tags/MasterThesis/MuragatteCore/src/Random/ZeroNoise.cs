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
    public class ZeroNoise : Noise
    {
        #region Constructors

        public ZeroNoise() : base(null, 0, 0) { }

        #endregion

        #region Properties

        public override Distribution Distribution
        {
            get { return Distribution.None; }
        }

        #endregion

        #region Methods

        protected override double SafeApply(RandomMT random)
        {
            return 0;
        }

        #endregion
    }
}

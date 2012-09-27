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
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public abstract class AgentArgs
    {
        #region Fields

        protected Dictionary<string, double> _modifiers = new Dictionary<string, double>();
        protected Distribution _distribution;
        protected double _dNoiseArgA;
        protected double _dNoiseArgB;

        #endregion

        #region Constructors

        public AgentArgs(Distribution distribution, double noiseA, double noiseB)
        {
            _distribution = distribution;
            _dNoiseArgA = noiseA;
            _dNoiseArgB = noiseB;
        }

        #endregion

        #region Properties

        public abstract Goal Goal { get; set; }

        public abstract Dictionary<string, Neighbourhood> Neighbourhoods { get; }

        public Dictionary<string, double> Modifiers
        {
            get { return _modifiers; }
        }

        public Distribution Distribution
        {
            get { return _distribution; }
        }

        public double NoiseArgA
        {
            get { return _dNoiseArgA; }
        }

        public double NoiseArgB
        {
            get { return _dNoiseArgB; }
        }

        #endregion

        #region Methods

        public void SetNeighbourhoodOwner(Agent owner)
        {
            if (Neighbourhoods != null)
            {
                foreach (Neighbourhood n in Neighbourhoods.Values)
                {
                    n.Source = owner;
                }
            }
        }

        #endregion
    }
}

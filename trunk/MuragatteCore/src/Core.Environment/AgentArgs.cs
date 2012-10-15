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
        #region Constants

        protected const double DEFAULT_DEVIATION = 0.01 * 180 / Math.PI;    //0.01 radians
        protected const double DEFAULT_LIMIT = 5;

        #endregion

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

        protected AgentArgs(AgentArgs args)
            : this(args._distribution, args._dNoiseArgA, args._dNoiseArgB)
        {
            _modifiers = new Dictionary<string, double>(args._modifiers);
        }

        #endregion

        #region Properties

        public abstract bool HasGoal { get; }

        public abstract bool HasNeighbourhoods { get; }

        public abstract Goal Goal { get; set; }

        public abstract Dictionary<string, Neighbourhood> Neighbourhoods { get; }

        public Dictionary<string, double> Modifiers
        {
            get { return _modifiers; }
        }

        public Distribution Distribution
        {
            get { return _distribution; }
            set { _distribution = value; }
        }

        public double NoiseArgA
        {
            get { return _dNoiseArgA; }
            set { _dNoiseArgA = value; }
        }

        public double NoiseArgB
        {
            get { return _dNoiseArgB; }
            set { _dNoiseArgB = value; }
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

        protected Dictionary<string, Neighbourhood> GetNeigbourhoodClones(Dictionary<string, Neighbourhood> neighbourhoods)
        {
            Dictionary<string, Neighbourhood> clones = new Dictionary<string, Neighbourhood>();
            foreach (KeyValuePair<string, Neighbourhood> n in neighbourhoods)
            {
                clones.Add(n.Key, n.Value.Clone());
            }
            return clones;
        }

        public abstract AgentArgs Clone();

        #endregion
    }
}

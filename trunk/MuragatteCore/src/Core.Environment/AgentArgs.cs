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
using Muragatte.IO;
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
        protected Distribution _distribution = Distribution.None;
        protected double _dNoiseArgA = 0;
        protected double _dNoiseArgB = 0;

        #endregion

        #region Constructors

        public AgentArgs() { }

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

        public abstract bool HasGoal { get; }

        public abstract bool HasNeighbourhoods { get; }

        [XmlIgnore]
        public abstract Goal Goal { get; set; }

        [XmlIgnore]
        public abstract Dictionary<string, Neighbourhood> Neighbourhoods { get; }

        [XmlIgnore]
        public Dictionary<string, double> Modifiers
        {
            get { return _modifiers; }
        }

        [XmlArray("Neighbourhoods", IsNullable = false)]
        [XmlArrayItem(ElementName = "Neighbourhood", Type = typeof(XmlAgentArgsNeighbourhood))]
        public KeyValuePair<string, Neighbourhood>[] XmlNeighbourhoods
        {
            get { return Neighbourhoods == null ? null : Neighbourhoods.ToArray(); }
            set
            {
                if (Neighbourhoods != null)
                {
                    foreach (KeyValuePair<string, Neighbourhood> n in value)
                    {
                        if (Neighbourhoods.ContainsKey(n.Key)) Neighbourhoods[n.Key] = n.Value;
                    }
                }
            }
        }

        [XmlArray("Modifiers")]
        [XmlArrayItem(ElementName = "Modifier", Type = typeof(XmlAgentArgsModifier))]
        public KeyValuePair<string, double>[] XmlModifiers
        {
            get { return _modifiers.ToArray(); }
            set
            {
                foreach (KeyValuePair<string, double> m in value)
                {
                    if (_modifiers.ContainsKey(m.Key)) _modifiers[m.Key] = m.Value;
                }
            }
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

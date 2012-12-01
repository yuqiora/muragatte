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
using Muragatte.Core.Environment.SteeringUtils;
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class SimpleBoidAgentArgs : AgentArgs
    {
        #region Constructors

        public SimpleBoidAgentArgs() : this(1, 1, 1, Distribution.Gaussian, DEFAULT_DEVIATION, DEFAULT_LIMIT) { }

        public SimpleBoidAgentArgs(double separation, double cohesion, double alignment, Distribution distribution, double noiseA, double noiseB)
            : base(distribution, noiseA, noiseB)
        {
            _modifiers.Add(SeparationSteering.LABEL, separation);
            _modifiers.Add(CohesionSteering.LABEL, cohesion);
            _modifiers.Add(AlignmentSteering.LABEL, alignment);
        }

        protected SimpleBoidAgentArgs(SimpleBoidAgentArgs args) : base(args) { }

        #endregion

        #region Properties

        public override bool HasGoal
        {
            get { return false; }
        }

        public override bool HasNeighbourhoods
        {
            get { return false; }
        }

        [XmlIgnore]
        public override Goal Goal
        {
            get { return null; }
            set { }
        }

        [XmlIgnore]
        public override Dictionary<string, Neighbourhood> Neighbourhoods
        {
            get { return null; }
        }

        #endregion

        #region Methods

        public override AgentArgs Clone(MultiAgentSystem model)
        {
            return new SimpleBoidAgentArgs(this);
        }

        #endregion
    }
}

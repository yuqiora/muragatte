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
    public class LoneWandererAgentArgs : AgentArgs
    {
        #region Constructors

        public LoneWandererAgentArgs() : this(10, 1) { }

        public LoneWandererAgentArgs(double wander, double obstacleAvoidance)
            : base(Distribution.None, 0, 0)
        {
            _modifiers.Add(WanderSteering.LABEL, wander);
            _modifiers.Add(ObstacleAvoidanceSteering.LABEL, obstacleAvoidance);
        }

        protected LoneWandererAgentArgs(LoneWandererAgentArgs args) : base(args) { }

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

        public override AgentArgs Clone()
        {
            return new LoneWandererAgentArgs(this);
        }

        #endregion
    }
}

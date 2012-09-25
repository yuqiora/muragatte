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
using Muragatte.Core.Environment.SteeringUtils;

namespace Muragatte.Core.Environment.Agents
{
    public class LoneWandererAgentArgs : AgentArgs
    {
        #region Constructors

        public LoneWandererAgentArgs() : this(10, 1) { }

        public LoneWandererAgentArgs(double wander, double obstacleAvoidance)
        {
            _modifiers.Add(WanderSteering.LABEL, wander);
            _modifiers.Add(ObstacleAvoidanceSteering.LABEL, obstacleAvoidance);
        }

        #endregion

        #region Properties

        public override Goal Goal
        {
            get { return null; }
            set { }
        }

        public override Dictionary<string, Neighbourhood> Neighbourhoods
        {
            get { return null; }
        }

        #endregion
    }
}

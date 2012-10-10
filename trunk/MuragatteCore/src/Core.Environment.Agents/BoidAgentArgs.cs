﻿// ------------------------------------------------------------------------
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
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class BoidAgentArgs : AgentArgs
    {
        #region Constructors

        public BoidAgentArgs() : this(1, 1, 1, Distribution.Gaussian, 0, 2) { }

        public BoidAgentArgs(double separation, double cohesion, double alignment, Distribution distribution, double noiseA, double noiseB)
            : base(distribution, noiseA, noiseB)
        {
            _modifiers.Add(SeparationSteering.LABEL, separation);
            _modifiers.Add(CohesionSteering.LABEL, cohesion);
            _modifiers.Add(AlignmentSteering.LABEL, alignment);
        }

        protected BoidAgentArgs(BoidAgentArgs args) : base(args) { }

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

        #region Methods

        public override AgentArgs Clone()
        {
            return new BoidAgentArgs(this);
        }

        #endregion
    }

    public class AdvancedBoidAgentArgs : BoidAgentArgs
    {
        #region Constants

        public const string MOD_ASSERTIVITY = "Assertivity";
        public const string NEIGH_PERSONAL_AREA = "Personal Area";

        #endregion

        #region Fields

        private Goal _goal;
        private Dictionary<string, Neighbourhood> _neighbourhoods = new Dictionary<string, Neighbourhood>();

        #endregion

        #region Constructors

        public AdvancedBoidAgentArgs(Goal goal, Neighbourhood personalArea) : this(goal, personalArea, 1) { }

        public AdvancedBoidAgentArgs(Goal goal, Neighbourhood personalArea, double assertivity) :
            this(goal, personalArea, assertivity, 1, 1, 1, 1, 10, Distribution.Gaussian, 0, 2) { }

        public AdvancedBoidAgentArgs(Goal goal, Neighbourhood personalArea, double assertivity, double separation, double cohesion,
            double alignment, double obstacleAvoidance, double wander, Distribution distribution, double noiseA, double noiseB)
            : base(separation, cohesion, alignment, distribution, noiseA, noiseB)
        {
            _goal = goal;
            _neighbourhoods.Add(NEIGH_PERSONAL_AREA, personalArea);
            _modifiers.Add(ObstacleAvoidanceSteering.LABEL, obstacleAvoidance);
            _modifiers.Add(WanderSteering.LABEL, wander);
            _modifiers.Add(MOD_ASSERTIVITY, assertivity);
        }

        protected AdvancedBoidAgentArgs(AdvancedBoidAgentArgs args)
            : base(args)
        {
            _goal = args._goal;
            _neighbourhoods = GetNeigbourhoodClones(args._neighbourhoods);
        }

        #endregion

        #region Properties

        public override bool HasGoal
        {
            get { return true; }
        }

        public override bool HasNeighbourhoods
        {
            get { return true; }
        }

        public override Goal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public override Dictionary<string, Neighbourhood> Neighbourhoods
        {
            get { return _neighbourhoods; }
        }

        #endregion

        #region Methods

        public override AgentArgs Clone()
        {
            return new AdvancedBoidAgentArgs(this);
        }

        #endregion
    }
}

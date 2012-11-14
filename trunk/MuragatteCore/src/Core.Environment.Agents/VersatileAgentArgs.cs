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
using Muragatte.IO;
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class VersatileAgentArgs : AgentArgs
    {
        #region Constants

        public const string MOD_ASSERTIVITY = "Assertivity";
        public const string MOD_CREDIBILITY = "Credibility";
        public const string NEIGH_PERSONAL_AREA = "Personal Area";

        #endregion

        #region Fields

        private Goal _goal;
        private Dictionary<string, Neighbourhood> _neighbourhoods = new Dictionary<string, Neighbourhood>();

        #endregion

        #region Constructors

        public VersatileAgentArgs() : this(null, new Neighbourhood()) { }

        public VersatileAgentArgs(Goal goal, Neighbourhood personalArea) : this(goal, personalArea, 1, 1) { }

        public VersatileAgentArgs(Goal goal, Neighbourhood personalArea, double assertivity, double credibility)
            : this(goal, personalArea, assertivity, credibility, 1, 1, 1, 1, 1, 1, 1, 1, 10, Distribution.Gaussian, DEFAULT_DEVIATION, DEFAULT_LIMIT) { }

        public VersatileAgentArgs(Goal goal, Neighbourhood personalArea, double assertivity, double credibility,
            double separation, double cohesion, double alignment, double obstacleAvoidance, double seek, double flee,
            double pursuit, double evasion, double wander, Distribution distribution, double noiseA, double noiseB)
            : base(distribution, noiseA, noiseB)
        {
            _goal = goal;
            _neighbourhoods.Add(NEIGH_PERSONAL_AREA, personalArea);
            _modifiers.Add(MOD_ASSERTIVITY, assertivity);
            _modifiers.Add(MOD_CREDIBILITY, credibility);
            _modifiers.Add(SeparationSteering.LABEL, separation);
            _modifiers.Add(CohesionSteering.LABEL, cohesion);
            _modifiers.Add(AlignmentSteering.LABEL, alignment);
            _modifiers.Add(ObstacleAvoidanceSteering.LABEL, obstacleAvoidance);
            _modifiers.Add(SeekSteering.LABEL, seek);
            _modifiers.Add(FleeSteering.LABEL, flee);
            _modifiers.Add(PursuitSteering.LABEL, pursuit);
            _modifiers.Add(EvasionSteering.LABEL, evasion);
            _modifiers.Add(WanderSteering.LABEL, wander);
        }

        protected VersatileAgentArgs(VersatileAgentArgs args, MultiAgentSystem model)
            : base(args)
        {
            _goal = GetProperGoal(_goal, model);
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

        [XmlElement(Type = typeof(XmlGoalReference), IsNullable = false)]
        public override Goal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        [XmlIgnore]
        public override Dictionary<string, Neighbourhood> Neighbourhoods
        {
            get { return _neighbourhoods; }
        }

        #endregion

        #region Methods

        public override AgentArgs Clone(MultiAgentSystem model)
        {
            return new VersatileAgentArgs(this, model);
        }

        #endregion
    }
}

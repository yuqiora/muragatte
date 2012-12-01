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
    public class FlockAndSeekBaseAgentArgs : AgentArgs
    {
        #region Constants

        public const string MOD_ASSERTIVENESS = "Assertiveness";
        public const string NEIGH_PERSONAL_AREA = "Personal Area";

        #endregion

        #region Fields

        protected Goal _goal;
        protected Dictionary<string, Neighbourhood> _neighbourhoods = new Dictionary<string, Neighbourhood>();

        #endregion

        #region Constructors

        public FlockAndSeekBaseAgentArgs() : this(null, new Neighbourhood()) { }
        
        public FlockAndSeekBaseAgentArgs(Goal goal, Neighbourhood personalArea) : this(goal, personalArea, 1) { }

        public FlockAndSeekBaseAgentArgs(Goal goal, Neighbourhood personalArea, double assertiveness)
            : this(goal, personalArea, assertiveness, 1, 1, 1, 1, Distribution.Gaussian, DEFAULT_DEVIATION, DEFAULT_LIMIT) { }

        public FlockAndSeekBaseAgentArgs(Goal goal, Neighbourhood personalArea, double assertiveness,
            double separation, double cohesion, double alignment, double seek,
            Distribution distribution, double noiseA, double noiseB)
            : base(distribution, noiseA, noiseB)
        {
            _goal = goal;
            _neighbourhoods.Add(NEIGH_PERSONAL_AREA, personalArea);
            _modifiers.Add(MOD_ASSERTIVENESS, assertiveness);
            _modifiers.Add(SeparationSteering.LABEL, separation);
            _modifiers.Add(CohesionSteering.LABEL, cohesion);
            _modifiers.Add(AlignmentSteering.LABEL, alignment);
            _modifiers.Add(SeekSteering.LABEL, seek);
        }

        protected FlockAndSeekBaseAgentArgs(FlockAndSeekBaseAgentArgs args, MultiAgentSystem model)
            : base(args)
        {
            _goal = GetProperGoal(args._goal, model);
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
            return new FlockAndSeekBaseAgentArgs(this, model);
        }

        #endregion
    }
}

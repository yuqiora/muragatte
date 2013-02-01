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
using Muragatte.Random;

namespace Muragatte.Core.Environment.Agents
{
    public class Vejmola2013AgentArgs : FlockAndSeekBaseAgentArgs
    {
        #region Constants

        public const string MOD_CREDIBILITY = "Credibility";

        #endregion

        #region Constructors

        public Vejmola2013AgentArgs() : this(null, new Neighbourhood()) { }
        
        public Vejmola2013AgentArgs(Goal goal, Neighbourhood personalArea) : this(goal, personalArea, 1, 1) { }

        public Vejmola2013AgentArgs(Goal goal, Neighbourhood personalArea, double assertiveness, double credibility)
            : this(goal, personalArea, assertiveness, credibility, 1, 1, 1, 1, 10, Distribution.Gaussian, DEFAULT_DEVIATION, DEFAULT_LIMIT) { }

        public Vejmola2013AgentArgs(Goal goal, Neighbourhood personalArea, double assertiveness, double credibility,
            double separation, double cohesion, double alignment, double seek, double wander,
            Distribution distribution, double noiseA, double noiseB)
            : base(goal, personalArea, assertiveness, separation, cohesion, alignment, seek, distribution, noiseA, noiseB)
        {
            _modifiers.Add(WanderSteering.LABEL, wander);
            _modifiers.Add(MOD_CREDIBILITY, credibility);
        }

        protected Vejmola2013AgentArgs(Vejmola2013AgentArgs args, MultiAgentSystem model) : base(args, model) { }

        #endregion

        #region Methods

        public override AgentArgs Clone(MultiAgentSystem model)
        {
            return new Vejmola2013AgentArgs(this, model);
        }

        #endregion
    }
}

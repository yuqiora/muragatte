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
    public class AdvancedBoidAgentArgs : ClassicBoidAgentArgs
    {
        #region Constructors

        public AdvancedBoidAgentArgs() : this(new Neighbourhood()) { }

        public AdvancedBoidAgentArgs(Neighbourhood fieldOfView) : this(fieldOfView, fieldOfView, fieldOfView) { }

        public AdvancedBoidAgentArgs(Neighbourhood separationArea, Neighbourhood cohesionArea, Neighbourhood alignmentArea)
            : this(separationArea, cohesionArea, alignmentArea, 1, 1, 1, 1, 1, 10,
            Distribution.Gaussian, DEFAULT_DEVIATION, DEFAULT_LIMIT) { }

        public AdvancedBoidAgentArgs(Neighbourhood separationArea, Neighbourhood cohesionArea, Neighbourhood alignmentArea,
            double separation, double cohesion, double alignment, double obstacleAvoidance, double seek, double wander,
            Distribution distribution, double noiseA, double noiseB)
            : base(separationArea, cohesionArea, alignmentArea, separation, cohesion, alignment, distribution, noiseA, noiseB)
        {
            _modifiers.Add(ObstacleAvoidanceSteering.LABEL, obstacleAvoidance);
            _modifiers.Add(SeekSteering.LABEL, seek);
            _modifiers.Add(WanderSteering.LABEL, wander);
        }

        protected AdvancedBoidAgentArgs(AdvancedBoidAgentArgs args, MultiAgentSystem model) : base(args) { }

        #endregion

        #region Methods

        public override AgentArgs Clone(MultiAgentSystem model)
        {
            return new AdvancedBoidAgentArgs(this, model);
        }

        #endregion
    }
}

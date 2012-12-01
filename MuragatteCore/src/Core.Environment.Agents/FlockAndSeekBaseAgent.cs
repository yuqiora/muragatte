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
using Muragatte.Common;
using Muragatte.Core.Environment.SteeringUtils;

namespace Muragatte.Core.Environment.Agents
{
    public abstract class FlockAndSeekBaseAgent : Agent
    {
        #region Constructors

        public FlockAndSeekBaseAgent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, FlockAndSeekBaseAgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        public FlockAndSeekBaseAgent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, FlockAndSeekBaseAgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        protected FlockAndSeekBaseAgent(FlockAndSeekBaseAgent other, MultiAgentSystem model)
            : base(other, model)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        #endregion

        #region Properties

        public Neighbourhood PersonalArea
        {
            get { return _args.Neighbourhoods[FlockAndSeekBaseAgentArgs.NEIGH_PERSONAL_AREA]; }
        }

        public double Assertiveness
        {
            get { return _args.Modifiers[FlockAndSeekBaseAgentArgs.MOD_ASSERTIVENESS]; }
            set { _args.Modifiers[FlockAndSeekBaseAgentArgs.MOD_ASSERTIVENESS] = value; }
        }

        public double SeparationWeight
        {
            get { return Separation.Weight; }
            set
            {
                Separation.Weight = value;
                _args.Modifiers[SeparationSteering.LABEL] = value;
            }
        }

        public double CohesionWeight
        {
            get { return Cohesion.Weight; }
            set
            {
                Cohesion.Weight = value;
                _args.Modifiers[CohesionSteering.LABEL] = value;
            }
        }

        public double AlignmentWeight
        {
            get { return Alignment.Weight; }
            set
            {
                Alignment.Weight = value;
                _args.Modifiers[AlignmentSteering.LABEL] = value;
            }
        }

        public double SeekWeight
        {
            get { return Seek.Weight; }
            set
            {
                Seek.Weight = value;
                _args.Modifiers[SeekSteering.LABEL] = value;
            }
        }

        protected Steering Separation
        {
            get { return _steering[SeparationSteering.LABEL]; }
        }

        protected Steering Cohesion
        {
            get { return _steering[CohesionSteering.LABEL]; }
        }

        protected Steering Alignment
        {
            get { return _steering[AlignmentSteering.LABEL]; }
        }

        protected Steering Seek
        {
            get { return _steering[SeekSteering.LABEL]; }
        }

        #endregion

        #region Methods

        protected override void EnableSteering()
        {
            AddSteering(new SeparationSteering(this, _args.Modifiers[SeparationSteering.LABEL]));
            AddSteering(new CohesionSteering(this, _args.Modifiers[CohesionSteering.LABEL]));
            AddSteering(new AlignmentSteering(this, _args.Modifiers[AlignmentSteering.LABEL]));
            AddSteering(new SeekSteering(this, _args.Modifiers[SeekSteering.LABEL]));
        }

        #endregion
    }
}

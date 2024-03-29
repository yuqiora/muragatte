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
using Muragatte.Common;
using Muragatte.Core.Environment.SteeringUtils;

namespace Muragatte.Core.Environment.Agents
{
    public class VersatileAgent : Agent
    {
        #region Constructors

        public VersatileAgent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, VersatileAgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        public VersatileAgent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, VersatileAgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        protected VersatileAgent(VersatileAgent other, MultiAgentSystem model)
            : base(other, model)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        #endregion

        #region Properties

        public Neighbourhood PersonalArea
        {
            get { return _args.Neighbourhoods[VersatileAgentArgs.NEIGH_PERSONAL_AREA]; }
        }

        public double Assertiveness
        {
            get { return _args.Modifiers[VersatileAgentArgs.MOD_ASSERTIVENESS]; }
            set { _args.Modifiers[VersatileAgentArgs.MOD_ASSERTIVENESS] = value; }
        }

        public double Credibility
        {
            get { return _args.Modifiers[VersatileAgentArgs.MOD_CREDIBILITY]; }
            set { _args.Modifiers[VersatileAgentArgs.MOD_CREDIBILITY] = value; }
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

        public double ObstacleAvoidanceWeight
        {
            get { return Avoid.Weight; }
            set
            {
                Avoid.Weight = value;
                _args.Modifiers[ObstacleAvoidanceSteering.LABEL] = value;
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

        public double FleeWeight
        {
            get { return Flee.Weight; }
            set
            {
                Flee.Weight = value;
                _args.Modifiers[FleeSteering.LABEL] = value;
            }
        }

        public double PursuitWeight
        {
            get { return Pursuit.Weight; }
            set
            {
                Pursuit.Weight = value;
                _args.Modifiers[PursuitSteering.LABEL] = value;
            }
        }

        public double EvasionWeight
        {
            get { return Evasion.Weight; }
            set
            {
                Evasion.Weight = value;
                _args.Modifiers[EvasionSteering.LABEL] = value;
            }
        }

        public double WanderWeight
        {
            get { return Wander.Weight; }
            set
            {
                Wander.Weight = value;
                _args.Modifiers[WanderSteering.LABEL] = value;
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

        protected Steering Avoid
        {
            get { return _steering[ObstacleAvoidanceSteering.LABEL]; }
        }

        protected Steering Seek
        {
            get { return _steering[SeekSteering.LABEL]; }
        }

        protected Steering Flee
        {
            get { return _steering[FleeSteering.LABEL]; }
        }

        protected Steering Pursuit
        {
            get { return _steering[PursuitSteering.LABEL]; }
        }

        protected Steering Evasion
        {
            get { return _steering[EvasionSteering.LABEL]; }
        }

        protected Steering Wander
        {
            get { return _steering[WanderSteering.LABEL]; }
        }

        #endregion

        #region Methods

        public override Vector2 GetDirection()
        {
            return Credibility * _direction;
        }

        protected override Vector2 ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Vector2.Zero;
            List<Element> companions = new List<Element>();
            List<Element> goals = new List<Element>();
            List<Element> obstacles = new List<Element>();
            List<Element> threats = new List<Element>();
            foreach (Element e in locals)
            {
                switch (RelationshipWith(e))
                {
                    case ElementNature.Companion:
                        companions.Add(e);
                        break;
                    case ElementNature.Goal:
                        goals.Add(e);
                        break;
                    case ElementNature.Obstacle:
                        obstacles.Add(e);
                        break;
                    case ElementNature.Threat:
                        threats.Add(e);
                        break;
                    default:
                        continue;
                }
            }
            IEnumerable<Element> tooClose = PersonalArea.Within(companions);
            if (threats.Count > 0)
            {
                dirDelta = Evasion.Steer(threats);
            }
            else
            {
                Vector2 avoid = Avoid.Steer(obstacles);
                if (avoid.IsZero)
                {
                    if (tooClose.Count() > 0)
                    {
                        dirDelta = Separation.Steer(tooClose);
                    }
                    else
                    {
                        if (companions.Count + goals.Count > 0)
                        {
                            dirDelta = Cohesion.Steer(companions, true) + Alignment.Steer(companions, true) + Pursuit.Steer(goals, true);
                            if (Goal != null)
                            {
                                dirDelta += Assertiveness * Seek.Steer(Goal, true);
                            }
                        }
                        else
                        {
                            dirDelta = Goal == null ? Wander.Steer() : Assertiveness * Seek.Steer(Goal);
                        }
                    }
                }
                else
                {
                    dirDelta = avoid;
                }
            }
            return dirDelta;
        }

        protected override void EnableSteering()
        {
            AddSteering(new SeparationSteering(this, _args.Modifiers[SeparationSteering.LABEL]));
            AddSteering(new CohesionSteering(this, _args.Modifiers[CohesionSteering.LABEL]));
            AddSteering(new AlignmentSteering(this, _args.Modifiers[AlignmentSteering.LABEL]));
            AddSteering(new ObstacleAvoidanceSteering(this, _args.Modifiers[ObstacleAvoidanceSteering.LABEL], VisibleRange));
            AddSteering(new SeekSteering(this, _args.Modifiers[SeekSteering.LABEL]));
            AddSteering(new FleeSteering(this, _args.Modifiers[FleeSteering.LABEL]));
            AddSteering(new PursuitSteering(this, _args.Modifiers[PursuitSteering.LABEL]));
            AddSteering(new EvasionSteering(this, _args.Modifiers[EvasionSteering.LABEL]));
            AddSteering(new WanderSteering(this, _args.Modifiers[WanderSteering.LABEL], _model.Random));
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new VersatileAgent(this, model);
        }

        #endregion
    }
}

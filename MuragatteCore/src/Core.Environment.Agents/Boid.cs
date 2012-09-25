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
    public class BoidAgent : Agent
    {
        #region Constructors

        public BoidAgent(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle, BoidAgentArgs args)
            : base(model, fieldOfView, turningAngle, args) { }

        public BoidAgent(MultiAgentSystem model, Vector2 position, Vector2 direction,
            double speed, Neighbourhood fieldOfView, Angle turningAngle, BoidAgentArgs args)
            : base(model, position, direction, speed, fieldOfView, turningAngle, args) { }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        public override void Update()
        {
            IEnumerable<Agent> locals = _model.Elements.RangeSearch<Agent>(this, VisibleRange);
            IEnumerable<Agent> fov = _fieldOfView.Within(locals);
            ApplyRules(fov);
        }

        public override void ConfirmUpdate()
        {
            Position = _model.Region.Outside(_altPosition);
            _direction = _altDirection;
        }

        protected override void ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Separation.Steer(locals) + Cohesion.Steer(locals) + Alignment.Steer(locals);
            //noise temporary, might need further work
            _altDirection = Vector2.Normalized(_direction + dirDelta + Angle.Random(2));
            ProperDirection();
            _altPosition = _position + _dSpeed * _model.TimePerStep * _altDirection;
        }

        protected override void EnableSteering()
        {
            AddSteering(new SeparationSteering(this, _args.Modifiers[SeparationSteering.LABEL]));
            AddSteering(new CohesionSteering(this, _args.Modifiers[CohesionSteering.LABEL]));
            AddSteering(new AlignmentSteering(this, _args.Modifiers[AlignmentSteering.LABEL]));
        }

        #endregion
    }

    //temporary
    //base for the one used in thesis experiments
    public class AdvancedBoidAgent : BoidAgent
    {
        #region Constructors

        public AdvancedBoidAgent(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle, AdvancedBoidAgentArgs args)
            : base(model, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        public AdvancedBoidAgent(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Neighbourhood fieldOfView, Angle turningAngle, AdvancedBoidAgentArgs args)
            : base(model, position, direction, speed, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        #endregion

        #region Properties

        public Goal Goal
        {
            get { return _args.Goal; }
            set { _args.Goal = value; }
        }

        public Neighbourhood PersonalArea
        {
            get { return _args.Neighbourhoods[AdvancedBoidAgentArgs.NEIGH_PERSONAL_AREA]; }
        }

        public double Assertivity
        {
            get { return _args.Modifiers[AdvancedBoidAgentArgs.MOD_ASSERTIVITY]; }
            set
            {
                Seek.Weight = value;
                _args.Modifiers[AdvancedBoidAgentArgs.MOD_ASSERTIVITY] = value;
            }
        }

        public double AvoidWeight
        {
            get { return Avoid.Weight; }
            set
            {
                Avoid.Weight = value;
                _args.Modifiers[ObstacleAvoidanceSteering.LABEL] = value;
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

        protected Steering Avoid
        {
            get { return _steering[ObstacleAvoidanceSteering.LABEL]; }
        }

        protected Steering Seek
        {
            get { return _steering[SeekSteering.LABEL]; }
        }

        protected Steering Wander
        {
            get { return _steering[WanderSteering.LABEL]; }
        }

        #endregion

        #region Methods

        public override void Update()
        {
            IEnumerable<Element> locals = _model.Elements.RangeSearch(this, VisibleRange);
            IEnumerable<Element> fov = _fieldOfView.Within(locals);
            ApplyRules(fov);
        }

        protected override void ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Vector2.Zero;
            IEnumerable<Element> companions = locals.Where(e => RelationshipWith(e) == ElementNature.Companion);
            IEnumerable<Element> obstacles = locals.Where(e => RelationshipWith(e) == ElementNature.Obstacle);
            IEnumerable<Element> tooClose = PersonalArea.Within(companions);
            Vector2 avoid = Avoid.Steer(obstacles);
            if (!avoid.IsZero)
            {
                dirDelta = avoid;
            }
            else
            {
                if (tooClose.Count() > 0)
                {
                    dirDelta = Separation.Steer(tooClose);
                }
                else
                {
                    if (companions.Count() > 0)
                    {
                        dirDelta = Cohesion.Steer(companions) + Alignment.Steer(companions);
                        if (Goal != null)
                        {
                            dirDelta += Seek.Steer(Goal);
                        }
                    }
                    else
                    {
                        dirDelta = Goal == null ? Wander.Steer() : Seek.Steer(Goal);
                    }
                }
            }
            dirDelta.Normalize();
            //noise temporary, needs further work
            _altDirection = Vector2.Normalized(_direction + dirDelta + Angle.Random(1));
            ProperDirection();
            _altPosition = _position + _dSpeed * _model.TimePerStep * _altDirection;
        }

        protected override void EnableSteering()
        {
            base.EnableSteering();
            AddSteering(new ObstacleAvoidanceSteering(this, _args.Modifiers[ObstacleAvoidanceSteering.LABEL], VisibleRange));
            AddSteering(new SeekSteering(this, _args.Modifiers[AdvancedBoidAgentArgs.MOD_ASSERTIVITY]));
            AddSteering(new WanderSteering(this, _args.Modifiers[WanderSteering.LABEL]));
        }

        #endregion
    }
}

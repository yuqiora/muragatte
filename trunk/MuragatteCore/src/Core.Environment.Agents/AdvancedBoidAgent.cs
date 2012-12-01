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
    public class AdvancedBoidAgent : ClassicBoidAgent
    {
        #region Constructors

        public AdvancedBoidAgent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, AdvancedBoidAgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        public AdvancedBoidAgent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, AdvancedBoidAgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        protected AdvancedBoidAgent(AdvancedBoidAgent other, MultiAgentSystem model)
            : base(other, model)
        {
            _args.SetNeighbourhoodOwner(this);
        }

        #endregion

        #region Properties

        public double AvoidWeight
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

        protected override Vector2 ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Vector2.Zero;
            IEnumerable<Element> companions = locals.Where(e => RelationshipWith(e) == ElementNature.Companion);
            IEnumerable<Element> obstacles = locals.Where(e => RelationshipWith(e) == ElementNature.Obstacle);
            IEnumerable<Element> goals = locals.Where(e => RelationshipWith(e) == ElementNature.Goal);
            Vector2 avoid = Avoid.Steer(obstacles);
            if (!avoid.IsZero)
            {
                dirDelta = avoid;
            }
            else
            {
                IEnumerable<Element> tooClose = SeparationArea.Within(companions);
                if (tooClose.Count() > 0)
                {
                    dirDelta = Separation.Steer(tooClose);
                }
                else
                {
                    if (companions.Count() > 0)
                    {
                        dirDelta = Cohesion.Steer(CohesionArea.Within(companions), true)
                            + Alignment.Steer(AlignmentArea.Within(companions), true)
                            + Seek.Steer(goals, true);
                    }
                    else
                    {
                        dirDelta = Wander.Steer();
                    }
                }
            }
            return dirDelta;
        }

        protected override void EnableSteering()
        {
            base.EnableSteering();
            AddSteering(new ObstacleAvoidanceSteering(this, _args.Modifiers[ObstacleAvoidanceSteering.LABEL], VisibleRange));
            AddSteering(new SeekSteering(this, _args.Modifiers[SeekSteering.LABEL]));
            AddSteering(new WanderSteering(this, _args.Modifiers[WanderSteering.LABEL], _model.Random));
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new AdvancedBoidAgent(this, model);
        }

        #endregion
    }
}

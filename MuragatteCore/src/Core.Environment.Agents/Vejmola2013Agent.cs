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
    public class Vejmola2013Agent : FlockAndSeekBaseAgent
    {
        #region Constructors

        public Vejmola2013Agent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, Vejmola2013AgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args) { }

        public Vejmola2013Agent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, Vejmola2013AgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args) { }

        protected Vejmola2013Agent(Vejmola2013Agent other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Properties

        public double Credibility
        {
            get { return _args.Modifiers[Vejmola2013AgentArgs.MOD_CREDIBILITY]; }
            set { _args.Modifiers[Vejmola2013AgentArgs.MOD_CREDIBILITY] = value; }
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
            IEnumerable<Element> others = locals.Where(e => RelationshipWith(e) == ElementNature.Companion);
            IEnumerable<Element> tooClose = PersonalArea.Within(others);
            if (tooClose.Count() > 0)
            {
                dirDelta = Separation.Steer(tooClose);
            }
            else
            {
                if (others.Count() > 0)
                {
                    if (Goal == null)
                    {
                        dirDelta = Cohesion.Steer(others) + Alignment.Steer(others);
                    }
                    else
                    {
                        //dirDelta = 0.25 * Cohesion.Steer(others, true) + 0.25 * Alignment.Steer(others, true)
                        //    + Assertiveness * 0.5 * Seek.Steer(Goal, true);
                        dirDelta = Vector2.Normalized(Cohesion.Steer(others) + Alignment.Steer(others))
                            + Assertiveness * Seek.Steer(Goal, true);
                    }
                }
                else
                {
                    dirDelta = Goal == null ? Wander.Steer() : Seek.Steer(Goal);
                }
            }
            return dirDelta;
        }

        protected override void EnableSteering()
        {
            base.EnableSteering();
            AddSteering(new WanderSteering(this, _args.Modifiers[WanderSteering.LABEL], _model.Random));
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new Vejmola2013Agent(this, model);
        }

        #endregion
    }
}

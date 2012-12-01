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
    public class Conradt2009Agent : FlockAndSeekBaseAgent
    {
        #region Constructors

        public Conradt2009Agent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, FlockAndSeekBaseAgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args) { }

        public Conradt2009Agent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, FlockAndSeekBaseAgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args) { }

        protected Conradt2009Agent(Conradt2009Agent other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Methods

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
                    dirDelta = 0.25 * Cohesion.Steer(others, true) + 0.25 * Alignment.Steer(others, true)
                        + Assertiveness * 0.5 * Seek.Steer(Goal, true);
                }
                else
                {
                    dirDelta = Seek.Steer(Goal, false);
                }
            }
            return dirDelta;
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new Conradt2009Agent(this, model);
        }
        
        #endregion
    }
}

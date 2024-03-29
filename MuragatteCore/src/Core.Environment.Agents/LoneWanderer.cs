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
    public class LoneWandererAgent : Agent
    {
        #region Constructors

        public LoneWandererAgent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, LoneWandererAgentArgs args)
            : base(id, model, species, fieldOfView, turningAngle, args) { }

        public LoneWandererAgent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, LoneWandererAgentArgs args)
            : base(id, model, position, direction, speed, species, fieldOfView, turningAngle, args) { }

        protected LoneWandererAgent(LoneWandererAgent other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Properties

        public double WanderWeight
        {
            get { return Wander.Weight; }
            set
            {
                Wander.Weight = value;
                _args.Modifiers[WanderSteering.LABEL] = value;
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

        protected Steering Wander
        {
            get { return _steering[WanderSteering.LABEL]; }
        }

        protected Steering Avoid
        {
            get { return _steering[ObstacleAvoidanceSteering.LABEL]; }
        }

        #endregion

        #region Methods

        protected override Vector2 ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Avoid.Steer(locals);
            if (dirDelta.IsZero)
            {
                dirDelta = Wander.Steer();
            }
            return dirDelta;
        }

        protected override IEnumerable<Element> GetLocalNeighbours()
        {
            return base.GetLocalNeighbours().Where(e => NotIgnoredOrUnknown(RelationshipWith(e)));
        }

        protected bool NotIgnoredOrUnknown(ElementNature nature)
        {
            return nature != ElementNature.Ignored && nature != ElementNature.Unknown;
        }

        protected override void EnableSteering()
        {
            AddSteering(new WanderSteering(this, _args.Modifiers[WanderSteering.LABEL], _model.Random));
            AddSteering(new ObstacleAvoidanceSteering(this, _args.Modifiers[ObstacleAvoidanceSteering.LABEL], VisibleRange));
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new LoneWandererAgent(this, model);
        }

        #endregion
    }
}

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

namespace Muragatte.Core.Environment.Agents
{
    public class LoneWanderer : Agent
    {
        #region Fields

        private double _dWanderRate;
        private double _dAvoidWeight;

        #endregion

        #region Constructors

        public LoneWanderer(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle,
            double wanderRate, double avoidWeight)
            : base(model, fieldOfView, turningAngle)
        {
            _dWanderRate = wanderRate;
            _dAvoidWeight = avoidWeight;
        }

        public LoneWanderer(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Neighbourhood fieldOfView, Angle turningAngle, double wanderRate, double avoidWeight)
            : base(model, position, direction, speed, fieldOfView, turningAngle)
        {
            _dWanderRate = wanderRate;
            _dAvoidWeight = avoidWeight;
        }

        #endregion

        #region Properties

        public double WanderRate
        {
            get { return _dWanderRate; }
            set { _dWanderRate = value; }
        }

        public double AvoidWeight
        {
            get { return _dAvoidWeight; }
            set { _dAvoidWeight = value; }
        }

        #endregion

        #region Methods

        protected override void ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = SteeringAvoid(locals, VisibleRange, _dAvoidWeight);
            if (dirDelta.IsZero)
            {
                dirDelta = SteeringWander(_dWanderRate);
            }
            _altDirection = Vector2.Normalized(_direction + dirDelta);
            ProperDirection();
            _altPosition = _position + _dSpeed * _model.TimePerStep * _altDirection;
        }

        public override void Update()
        {
            IEnumerable<Element> locals = _model.Elements.RangeSearch(this, VisibleRange);
            IEnumerable<Element> fov = _fieldOfView.Within(locals);
            ApplyRules(fov.Where(e => NotIgnoredOrUnknown(RelationshipWith(e))));
        }

        public override void ConfirmUpdate()
        {
            _position = _model.Region.Outside(_altPosition);
            _direction = _altDirection;
        }

        public override void SetModifiers(params double[] values)
        {
            if (values.Length >= 2)
            {
                if (ChangeModifier(values[0]))
                {
                    _dWanderRate = values[0];
                }
                if (ChangeModifier(values[1]))
                {
                    _dAvoidWeight = values[1];
                }
            }
        }

        private bool NotIgnoredOrUnknown(ElementNature nature)
        {
            return nature != ElementNature.Ignored && nature != ElementNature.Unknown;
        }

        #endregion
    }
}

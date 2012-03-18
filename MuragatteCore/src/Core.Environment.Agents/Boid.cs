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
    public class Boid : Agent
    {
        #region Constructors

        public Boid(MultiAgentSystem model, Neighbourhood fieldOfView, double turningAngle)
            : base(model, fieldOfView, turningAngle) { }

        public Boid(MultiAgentSystem model, Vector2 position, Vector2 direction,
            double speed, Neighbourhood fieldOfView, double turningAngle)
            : base(model, fieldOfView, turningAngle)
        {
            _direction = direction;
            _dSpeed = speed;
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
            _position = _model.Region.Outside(_altPosition);
            _direction = _altDirection;
        }

        protected override void ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Separation(locals) + Cohesion(locals) + Alignment(locals);
            //noise temporary, needs further work
            //doesn't check if inside turning angles
            _altDirection = (_direction + dirDelta + Vector2.RandomGauss()).Normalized();
            _altPosition = _position + _altDirection * _dSpeed * _model.TimePerStep;
        }

        #endregion
    }
}

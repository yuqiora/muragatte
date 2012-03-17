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

        public Boid(int id, MultiAgentSystem model,
            Vector2 position, Vector2 direction, double speed,
            Neighbourhood personalSpace, Neighbourhood fieldOfView,
            double turningAngle)
            : base(id, model, personalSpace, fieldOfView, null, turningAngle)
        {
            _position = position;
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

        public override void AfterUpdate()
        {
            _position = _altPosition;
            _direction = _altDirection;
        }

        protected override void ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Separation(locals) + Cohesion(locals) + Alignment(locals);
            _altDirection = (_direction + dirDelta).Normalized();
            _altPosition = _position + _altDirection * _dSpeed * _model.TimePerStep;
        }

        #endregion
    }
}

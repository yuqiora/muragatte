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

        public Boid(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle)
            : base(model, fieldOfView, turningAngle) { }

        public Boid(MultiAgentSystem model, Vector2 position, Vector2 direction,
            double speed, Neighbourhood fieldOfView, Angle turningAngle)
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
            //_altDirection = (_direction + dirDelta + 0.3 * Vector2.RandomGauss().Normalized()).Normalized();
            _altDirection = (_direction + dirDelta + Angle.Random(2)).Normalized();
            DirectionInBounds();
            _altPosition = _position + _altDirection * _dSpeed * _model.TimePerStep;
        }

        #endregion
    }

    //temporary
    public class AdvancedBoid : Boid
    {
        private Goal _goal = null;
        private double _dAssertivity = 0.5;
        private Neighbourhood _personalArea = null;

        #region Constructors

        public AdvancedBoid(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle,
            Goal goal, double assertivity, Neighbourhood personalArea)
            : base(model, fieldOfView, turningAngle)
        {
            _goal = goal;
            _dAssertivity = assertivity;
            _personalArea = personalArea;
            _personalArea.Source = this;
        }

        public AdvancedBoid(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Neighbourhood fieldOfView, Angle turningAngle, Goal goal, double assertivity, Neighbourhood personalArea)
            : base(model, position, direction, speed, fieldOfView, turningAngle)
        {
            _goal = goal;
            _dAssertivity = assertivity;
            _personalArea = personalArea;
            _personalArea.Source = this;
        }

        #endregion

        public Goal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public double Assertivity
        {
            get { return _dAssertivity; }
            set { _dAssertivity = value; }
        }

        public override void Update()
        {
            IEnumerable<Element> locals = _model.Elements.RangeSearch(this, VisibleRange);
            IEnumerable<Element> fov = _fieldOfView.Within(locals);
            ApplyRules(fov);
        }

        //based on Couzinetal2005
        protected override void ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Vector2.Zero();
            IEnumerable<Element> tooClose = _personalArea.Within(locals);
            if (tooClose.Count() > 0)
            {
                dirDelta = Separation(tooClose).Normalized();    
            }
            else
            {
                dirDelta = (Cohesion(locals) + Alignment(locals)).Normalized();
            }
            if (_goal != null)
            {
                dirDelta += _dAssertivity * (_goal.Position - _position).Normalized();
                dirDelta.Normalize();
            }
            //noise temporary, needs further work
            //doesn't check if inside turning angles
            //_altDirection = (_direction + dirDelta + 0.3 * Vector2.RandomGauss().Normalized()).Normalized();
            _altDirection = (_direction + dirDelta + Angle.Random(1)).Normalized();
            DirectionInBounds();
            _altPosition = _position + _altDirection * _dSpeed * _model.TimePerStep;
        }

    }
}

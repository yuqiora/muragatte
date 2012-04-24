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
    public class Versatile : Agent
    {
        #region Fields

        protected double _dAltSpeed = 1;
        protected Goal _goal = null;
        protected Neighbourhood _personalArea = null;
        protected bool _bAdjustSpeed = true;
        protected double _dAssertivity = 0.5;
        protected double _dCredibility = 1;
        protected double _dWeightSeparation = 1;
        protected double _dWeightCohesion = 1;
        protected double _dWeightAlignment = 1;
        protected double _dWeightAvoid = 1;
        protected double _dWeightSeekPursuit = 1;
        protected double _dWeightFleeEvasion = 1;
        protected double _dWeightAdjustSpeed = 1;
        protected double _dWanderRate = 10;

        #endregion

        #region Constructors

        public Versatile(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle,
            Goal goal, Neighbourhood personalArea, bool adjustSpeed, double assertivity, double credibility)
            : base(model, fieldOfView, turningAngle)
        {
            _goal = goal;
            _personalArea = personalArea;
            _personalArea.Source = this;
            _bAdjustSpeed = adjustSpeed;
            _dAssertivity = assertivity;
            _dCredibility = credibility;
        }

        public Versatile(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Neighbourhood fieldOfView, Angle turningAngle, Goal goal, Neighbourhood personalArea,
            bool adjustSpeed, double assertivity, double credibility)
            : this(model, fieldOfView, turningAngle, goal, personalArea, adjustSpeed, assertivity, credibility)
        {
            _position = position;
            _direction = direction;
            _dSpeed = speed;
        }

        public Versatile(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle,
            Goal goal, Neighbourhood personalArea, bool adjustSpeed, double assertivity, double credibility,
            double wSeparation, double wCohesion, double wAlignment, double wAvoid, double wSeekPursuit,
            double wFleeEvasion, double wAdjustSpeed, double wanderRate)
            : this(model, fieldOfView, turningAngle, goal, personalArea, adjustSpeed, assertivity, credibility)
        {
            _dWeightSeparation = wSeparation;
            _dWeightCohesion = wCohesion;
            _dWeightAlignment = wAlignment;
            _dWeightAvoid = wAvoid;
            _dWeightSeekPursuit = wSeekPursuit;
            _dWeightFleeEvasion = wFleeEvasion;
            _dWeightAdjustSpeed = wAdjustSpeed;
            _dWanderRate = wanderRate;
        }

        public Versatile(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Neighbourhood fieldOfView, Angle turningAngle, Goal goal, Neighbourhood personalArea, bool adjustSpeed,
            double assertivity, double credibility, double wSeparation, double wCohesion, double wAlignment,
            double wAvoid, double wSeekPursuit, double wFleeEvasion, double wAdjustSpeed, double wanderRate)
            : this(model, fieldOfView, turningAngle, goal, personalArea, adjustSpeed, assertivity, credibility,
            wSeparation, wCohesion, wAlignment, wAvoid, wSeekPursuit, wFleeEvasion, wAdjustSpeed, wanderRate)
        {
            _position = position;
            _direction = direction;
            _dSpeed = speed;
        }
        
        #endregion

        #region Properties

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

        public double Credibility
        {
            get { return _dCredibility; }
            set { _dCredibility = value; }
        }

        public double WeightSeparation
        {
            get { return _dWeightSeparation; }
            set { _dWeightSeparation = value; }
        }

        public double WeightCohesion
        {
            get { return _dWeightCohesion; }
            set { _dWeightCohesion = value; }
        }

        public double WeightAlignment
        {
            get { return _dWeightAlignment; }
            set { _dWeightAlignment = value; }
        }

        public double WeightAvoid
        {
            get { return _dWeightAvoid; }
            set { _dWeightAvoid = value; }
        }

        public double WeightSeekPursuit
        {
            get { return _dWeightSeekPursuit; }
            set { _dWeightSeekPursuit = value; }
        }

        public double WeightFleeEvasion
        {
            get { return _dWeightFleeEvasion; }
            set { _dWeightFleeEvasion = value; }
        }

        public double WeightAdjustSpeed
        {
            get { return _dWeightAdjustSpeed; }
            set { _dWeightAdjustSpeed = value; }
        }

        public double WanderRate
        {
            get { return _dWanderRate; }
            set { _dWanderRate = value; }
        }

        public bool IsSpeedAdjustable
        {
            get { return _bAdjustSpeed; }
            set { _bAdjustSpeed = value; }
        }

        #endregion

        #region Methods

        public override Vector2 GetDirection()
        {
            return _dCredibility * _direction;
        }

        public override void SetModifiers(params double[] values)
        {
            if (values.Length >= 11)
            {
                if (ChangeModifier(values[0]))
                {
                    _dAssertivity = values[0];
                }
                if (ChangeModifier(values[1]))
                {
                    _dCredibility = values[1];
                }
                if (ChangeModifier(values[2]))
                {
                    _dWeightSeparation = values[2];
                }
                if (ChangeModifier(values[3]))
                {
                    _dWeightCohesion = values[3];
                }
                if (ChangeModifier(values[4]))
                {
                    _dWeightAlignment = values[4];
                }
                if (ChangeModifier(values[5]))
                {
                    _dWeightAvoid = values[5];
                }
                if (ChangeModifier(values[6]))
                {
                    _dWeightSeekPursuit = values[6];
                }
                if (ChangeModifier(values[7]))
                {
                    _dWeightFleeEvasion = values[7];
                }
                if (ChangeModifier(values[8]))
                {
                    _dWeightAdjustSpeed = values[8];
                }
                if (ChangeModifier(values[9]))
                {
                    _dWanderRate = values[9];
                }
                if (ChangeModifier(values[10]))
                {
                    _bAdjustSpeed = values[10] != 0;
                }
            }
        }

        protected override void ApplyRules(IEnumerable<Element> locals)
        {
            Vector2 dirDelta = Vector2.Zero();
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
            IEnumerable<Element> tooClose = _personalArea.Within(companions);
            if (threats.Count > 0)
            {
                dirDelta = SteeringFleeOrEvasion(threats, _dWeightFleeEvasion);
            }
            else
            {
                Vector2 avoid = SteeringAvoid(obstacles, VisibleRange, _dWeightAvoid);
                if (!avoid.IsZero)
                {
                    dirDelta = avoid;
                }
                else
                {
                    if (tooClose.Count() > 0)
                    {
                        dirDelta = SteeringSeparation(tooClose, _dWeightSeparation);
                    }
                    else
                    {
                        if (companions.Count + goals.Count > 0)
                        {
                            dirDelta = SteeringCohesion(companions, _dWeightCohesion) +
                                SteeringAlignment(companions, _dWeightAlignment) +
                                SteeringSeekOrPursuit(goals, _dWeightSeekPursuit);
                            if (_goal != null)
                            {
                                dirDelta += SteeringSeek(_goal, _dAssertivity);
                            }
                        }
                        else
                        {
                            dirDelta = _goal == null ? SteeringWander(_dWanderRate) : SteeringSeek(_goal, _dAssertivity);
                        }
                    }
                }
            }
            dirDelta.Normalize();
            _altDirection = Vector2.Normalized(_direction + dirDelta + Angle.Random(1));
            ProperDirection();
            _dAltSpeed = _bAdjustSpeed ? SteeringAdjustSpeed(companions, _dWeightAdjustSpeed) : _dSpeed;
            _altPosition = _position + _dAltSpeed * _model.TimePerStep * _altDirection;
        }

        public override void Update()
        {
            IEnumerable<Element> locals = _model.Elements.RangeSearch(this, VisibleRange);
            IEnumerable<Element> fov = _fieldOfView.Within(locals);
            ApplyRules(fov);
        }

        public override void ConfirmUpdate()
        {
            _position = _model.Region.Outside(_altPosition);
            _direction = _altDirection;
            _dSpeed = _dAltSpeed;
        }

        public override Storage.ElementStatus ReportStatus()
        {
            return ReportStatus(_bAdjustSpeed ? 1 : 0, _dAssertivity, _dCredibility,
                _dWeightSeparation, _dWeightCohesion, _dWeightAlignment, _dWeightAvoid,
                _dWeightSeekPursuit, _dWeightFleeEvasion, _dWeightAdjustSpeed, _dWanderRate);
        }

        #endregion
    }
}

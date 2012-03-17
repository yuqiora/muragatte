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
using Muragatte.Core.Environment;

namespace Muragatte.Core.Environment
{
    public abstract class Agent : Element
    {
        //will be simplified
        //currently has too much for base agent

        #region Constants

        //will probably make Angle struct/class

        public const double MinAngle = 1;
        public const double MaxAngle = 180;

        #endregion

        #region Fields
        
        protected Vector2 _altPosition = new Vector2();
        protected Vector2 _altDirection = new Vector2();
        //might be left for specific agents, not everyone will be changing speed
        protected double _dAltSpeed = 1;
        //might leave assertivity and credibility for concrete agents that will be using it
        //not needed in base agent class
        protected double _dAssertivity = 0.5;
        protected double _dCredibility = 1;
        //might remove personal space, leave only one neighbourhood, FOV, as default
        protected Neighbourhood _personalSpace = null;
        protected Neighbourhood _fieldOfView = null;
        //will probably be moved to specific agents
        protected Goal _goal = null;
        protected double _dTurningAngle = 180;
        //line of sight neighbourhood?
        
        #endregion

        #region Constructors

        public Agent(int id, MultiAgentSystem model,
            Vector2 position, Vector2 direction, double speed,
            double asseritvity, double credibility,
            Neighbourhood personalSpace, Neighbourhood fieldOfView,
            Goal goal, double turningAngle)
            : base(id, model, position, direction, speed)
        {
            _bStationary = false;
            _dAssertivity = asseritvity;
            _dCredibility = credibility;
            _personalSpace = personalSpace;
            _fieldOfView = fieldOfView;
            _goal = goal;
            _dTurningAngle = turningAngle;
        }

        public Agent(int id, MultiAgentSystem model,
            Neighbourhood personalSpace, Neighbourhood fieldOfView,
            Goal goal, double turningAngle)
            : base(id, model)
        {
            _bStationary = false;
            _personalSpace = personalSpace;
            _fieldOfView = fieldOfView;
            _goal = goal;
            TurningAngle = turningAngle;
        }
        
        #endregion

        #region Properties

        public Goal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public double TurningAngle
        {
            get { return _dTurningAngle; }
            set { _dTurningAngle = ProperAngle(value); }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Companion; }
        }

        public override double Size
        {
            get { return 1; }
        }

        #endregion

        #region Virtual Properties

        public virtual double VisibleRange
        {
            get { return _fieldOfView == null ? 0 : _fieldOfView.Range; }
        }

        #endregion

        #region Methods

        public void SetMovementInfo(Vector2 position, Vector2 direction)
        {
            _position = position;
            _direction = direction;
        }

        public void SetMovementInfo(Vector2 position, Vector2 direction, double speed)
        {
            _position = position;
            _direction = direction;
            _dSpeed = speed;
        }

        public void SetMovementInfo(Vector2 position, Vector2 direction, double speed, double turningAngle)
        {
            _position = position;
            _direction = direction;
            _dSpeed = speed;
            TurningAngle = turningAngle;
        }

        public override ElementNature RelationshipWith(Element e)
        {
            if (e.IsEnabled)
            {
                ElementNature en;
                if (_species != null && e.Species != null &&
                    _species.RelationshipWith(e.Species, out en) && en != ElementNature.Unknown)
                {
                    return en;
                }
                else
                {
                    return e.DefaultNature;
                }
            }
            else
            {
                return ElementNature.Ignored;
            }
        }

        public override string ToString()
        {
            return "A-" + base.ToString();
        }

        #endregion

        #region Virtual Methods (Steering)

        protected virtual Vector2 SeekPursue(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Seek(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Pursue(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 FleeEvade(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Flee(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Evade(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Avoid(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Wander()
        {
            return new Vector2();
        }

        protected virtual Vector2 Separation(IEnumerable<Element> elements)
        {
            Vector2 x = new Vector2();
            foreach (Element e in elements)
            {
                x -= (e.Position - _position).Normalized();
            }
            return x;
        }

        protected virtual Vector2 Cohesion(IEnumerable<Element> elements)
        {
            Vector2 x = new Vector2();
            foreach (Element e in elements)
            {
                x += (e.Position - _position).Normalized();
            }
            return x;
        }

        protected virtual Vector2 Alignment(IEnumerable<Element> elements)
        {
            Vector2 x = new Vector2();
            foreach (Element e in elements)
            {
                x += e.Direction;
            }
            return x;
        }
        
        #endregion

        #region Abstract Methods

        protected abstract void ApplyRules(IEnumerable<Element> locals);

        #endregion

        #region Static Methods

        public static double ProperAngle(double value)
        {
            if (value < MinAngle)
            {
                return MinAngle;
            }
            else
            {
                if (value > MaxAngle)
                {
                    return MaxAngle;
                }
                else
                {
                    return value;
                }
            }
        }

        #endregion
    }
}

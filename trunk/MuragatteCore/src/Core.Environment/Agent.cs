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

        //public const double MinAngle = 1;
        //public const double MaxAngle = 180;

        #endregion

        #region Fields

        protected Vector2 _direction = new Vector2(0, 1);
        protected double _dSpeed = 1;
        protected Vector2 _altPosition = new Vector2(0, 0);
        protected Vector2 _altDirection = new Vector2(0, 1);
        protected Neighbourhood _fieldOfView = null;
        protected Angle _dTurningAngle = Angle.Deg180();
        //might be left for specific agents, not everyone will be changing speed
        //protected double _dAltSpeed = 1;
        //might leave assertivity and credibility for concrete agents that will be using it
        //not needed in base agent class
        //protected double _dAssertivity = 0.5;
        //protected double _dCredibility = 1;
        //might remove personal space, leave only one neighbourhood, FOV, as default
        //protected Neighbourhood _personalSpace = null;
        //will probably be moved to specific agents
        //protected Goal _goal = null;
        //line of sight neighbourhood?
        
        #endregion

        #region Constructors

        public Agent(MultiAgentSystem model, Vector2 position, Vector2 direction,
            double speed, Neighbourhood fieldOfView, Angle turningAngle)
            : base(model, position)
        {
            _direction = direction;
            _dSpeed = speed;
            _bStationary = false;
            _fieldOfView = fieldOfView;
            _fieldOfView.Source = this;
            _dTurningAngle = turningAngle;
        }

        public Agent(MultiAgentSystem model, Neighbourhood fieldOfView, Angle turningAngle)
            : base(model)
        {
            _bStationary = false;
            _fieldOfView = fieldOfView;
            _fieldOfView.Source = this;
            _dTurningAngle = turningAngle;
        }
        
        #endregion

        #region Properties

        public override Vector2 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public override double Speed
        {
            get { return _dSpeed; }
            set { _dSpeed = value; }
        }

        public override double Width
        {
            get { return 1; }
        }

        public override double Height
        {
            get { return 1; }
        }

        public Neighbourhood FieldOfView
        {
            get { return _fieldOfView; }
        }

        public Angle TurningAngle
        {
            get { return _dTurningAngle; }
            set { _dTurningAngle = value; }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Companion; }
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

        public void SetMovementInfo(Vector2 position, Vector2 direction, double speed, Angle turningAngle)
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

        #region Protected Methods

        protected void DirectionInBounds()
        {
            if (Vector2.AngleBetween(_direction, _altDirection) > _dTurningAngle)
            {
                _altDirection = _direction - (new Angle(_altDirection) - new Angle(_direction)).Sign() * _dTurningAngle;
            }
        }

        #endregion

        #region Virtual Methods (Steering)

        //might need some simplification, a lot of these rules will be very similar

        protected virtual Vector2 SeekPursue(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Seek(IEnumerable<Element> elements)
        {
            //same as cohesion
            Vector2 x = new Vector2();
            foreach (Element e in elements)
            {
                x += (e.Position - _position).Normalized();
            }
            return x;
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
            Vector2 x = new Vector2(0, 0);
            foreach (Element e in elements)
            {
                x -= (e.Position - _position).Normalized();
            }
            return x;
        }

        protected virtual Vector2 Cohesion(IEnumerable<Element> elements)
        {
            Vector2 x = new Vector2(0, 0);
            foreach (Element e in elements)
            {
                x += (e.Position - _position).Normalized();
            }
            return x;
        }

        protected virtual Vector2 Alignment(IEnumerable<Element> elements)
        {
            Vector2 x = new Vector2(0, 0);
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

        //#region Static Methods

        //public static double ProperAngle(double value)
        //{
        //    if (value < MinAngle)
        //    {
        //        return MinAngle;
        //    }
        //    if (value > MaxAngle)
        //    {
        //        return MaxAngle;
        //    }
        //    return value;
        //}

        //#endregion
    }
}

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

        public override double Radius
        {
            get { return 0.5; }
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

        protected void ProperDirection()
        {
            ContainedInRegion();
            DirectionInBounds();
        }

        protected void ContainedInRegion()
        {
            _altDirection = _model.Region.Containment(_position, _altDirection, VisibleRange);
        }

        protected void DirectionInBounds()
        {
            if (Vector2.AngleBetween(_direction, _altDirection) > _dTurningAngle)
            {
                _altDirection = _direction - (new Angle(_altDirection) - new Angle(_direction)).Sign() * _dTurningAngle;
            }
        }

        #endregion

        #region Virtual Methods (Steering)

        protected virtual Vector2 SeekOrPursuit(IEnumerable<Element> elements, double weight = 1)
        {
            return new Vector2();
        }

        protected virtual Vector2 Seek(Element element, double weight = 1)
        {
            return weight * (element.Position - _position).Normalized();
        }

        protected virtual Vector2 Pursuit(IEnumerable<Element> elements, double weight = 1)
        {
            return new Vector2();
        }

        protected virtual Vector2 FleeOrEvasion(IEnumerable<Element> elements, double weight = 1)
        {
            return new Vector2();
        }

        protected virtual Vector2 Flee(Element element, double weight = 1)
        {
            return Seek(element, -weight);
        }

        protected virtual Vector2 Evasion(IEnumerable<Element> elements, double weight = 1)
        {
            return new Vector2();
        }

        protected virtual Vector2 Avoid(IEnumerable<Element> elements)
        {
            return new Vector2();
        }

        protected virtual Vector2 Wander(double weight)
        {
            return new Vector2();
        }

        protected virtual Vector2 Separation(IEnumerable<Element> elements, double weight = 1)
        {
            return Cohesion(elements, -weight);
        }

        protected virtual Vector2 Cohesion(IEnumerable<Element> elements, double weight = 1)
        {
            Vector2 x = new Vector2(0, 0);
            foreach (Element e in elements)
            {
                x += (e.Position - _position).Normalized();
            }
            return weight * SteerAverage(x, elements.Count());
        }

        protected virtual Vector2 Alignment(IEnumerable<Element> elements, double weight = 1)
        {
            Vector2 x = new Vector2(0, 0);
            foreach (Element e in elements)
            {
                x += e.Direction;
            }
            return weight * SteerAverage(x, elements.Count());
        }

        //not sure if really needed, vectors normalized
        protected virtual Vector2 SteerAverage(Vector2 vector, int count)
        {
            return vector;
            //return count > 0 ? vector / count : vector;
        }
        
        #endregion

        #region Abstract Methods

        protected abstract void ApplyRules(IEnumerable<Element> locals);

        #endregion
    }
}

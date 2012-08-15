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
        #region Fields

        protected Vector2 _direction = new Vector2(0, 1);
        protected double _dSpeed = 1;
        protected Vector2 _altPosition = new Vector2(0, 0);
        protected Vector2 _altDirection = new Vector2(0, 1);
        protected Neighbourhood _fieldOfView = null;
        protected Angle _dTurningAngle = Angle.Deg180;
        protected Centroid _representative = null;
        
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
            get { return DEFAULT_RADIUS; }
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

        public Centroid Representative
        {
            get { return _representative; }
        }

        public Group Group
        {
            get { return _representative == null ? null : _representative.Group; }
        }

        public override string Name
        {
            get { return CreateName("A"); }
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
            return ToString("A");
        }

        public void CreateRepresentative()
        {
            _representative = new Centroid(this);
        }

        protected bool IsGroupCandidate(Agent a)
        {
            return a.IsEnabled && !a.Representative.IsInGroup && (_fieldOfView.Covers(a) || a.FieldOfView.Covers(this));
        }

        public IEnumerable<Agent> GroupSearch()
        {
            _representative.IsInGroup = true;
            IEnumerable<Agent> candidates = _model.Elements.RangeSearch<Agent>(this, VisibleRange, e => IsGroupCandidate((Agent)e));
            List<Agent> members = new List<Agent>(candidates);
            foreach (Agent a in candidates)
            {
                members.AddRange(a.GroupSearch());
            }
            return members;
        }

        #endregion

        #region Protected Methods

        protected void ProperDirection()
        {
            ContainedInRegion();
            DirectionInBounds();
            _altDirection.Normalize();
        }

        protected void ContainedInRegion()
        {
            _altDirection = _model.Region.Containment(_position, _altDirection, VisibleRange);
        }

        protected void DirectionInBounds()
        {
            if (Vector2.AngleBetween(_direction, _altDirection) > _dTurningAngle * _model.TimePerStep)
            {
                _altDirection = _direction - (new Angle(_altDirection) - new Angle(_direction)).Sign() * _dTurningAngle * _model.TimePerStep;
            }
        }

        protected void ChangeModifier(ref double modifier, double value)
        {
            if (!double.IsNaN(value) && !double.IsInfinity(value))
            {
                modifier = value;
            }
        }

        #endregion

        #region Steering Virtual Methods

        protected virtual Vector2 SteeringSeekOrPursuit(Element element, double weight = 1)
        {
            return element.IsStationary ? SteeringSeek(element, weight) : SteeringPursuit(element, weight);
        }

        protected virtual Vector2 SteeringSeekOrPursuit(IEnumerable<Element> elements, double weight = 1)
        {
            Element target = null;
            double distance = double.MaxValue;
            foreach (Element e in elements)
            {
                Vector2 v = e.IsStationary ? e.GetPosition() : e.PredictPositionAfter();
                double x = Vector2.Distance(_position, v);
                if (x < distance)
                {
                    distance = x;
                    target = e;
                }
            }
            return target.IsStationary ? SteeringSeek(target, weight) : SteeringPursuit(target, weight);
        }

        protected virtual Vector2 SteeringSeek(Vector2 position, double weight = 1)
        {
            return weight * Vector2.Normalized(position - _position);
        }

        protected virtual Vector2 SteeringSeek(Element element, double weight = 1)
        {
            return element == null ? Vector2.Zero : weight * Vector2.Normalized(element.GetPosition() - _position);
        }

        protected virtual Vector2 SteeringSeek(IEnumerable<Element> elements, double weight = 1)
        {
            Element target = null;
            double distance = double.MaxValue;
            foreach (Element e in elements)
            {
                double x = Vector2.Distance(_position, e.GetPosition());
                if (x < distance)
                {
                    distance = x;
                    target = e;
                }
            }
            return SteeringSeek(target, weight);
        }

        //same as SteeringSeek
        protected virtual Vector2 SteeringPursuit(Vector2 position, double weight = 1)
        {
            return SteeringSeek(position, weight);
        }

        protected virtual Vector2 SteeringPursuit(Element element, double weight = 1)
        {
            return element == null ? Vector2.Zero : SteeringSeek(element.PredictPositionAfter(), weight);
        }

        protected virtual Vector2 SteeringPursuit(IEnumerable<Element> elements, double weight = 1)
        {
            Element target = null;
            double distance = double.MaxValue;
            foreach (Element e in elements)
            {
                double x = Vector2.Distance(_position, e.PredictPositionAfter());
                if (x < distance)
                {
                    distance = x;
                    target = e;
                }
            }
            return SteeringPursuit(target, weight);
        }

        protected virtual Vector2 SteeringFleeOrEvasion(Element element, double weight = 1)
        {
            return element.IsStationary ? SteeringFlee(element, weight) : SteeringEvasion(element, weight);
        }

        protected virtual Vector2 SteeringFleeOrEvasion(IEnumerable<Element> elements, double weight = 1)
        {
            //temporary until evasion is done
            return SteeringFlee(elements, weight);
        }

        protected virtual Vector2 SteeringFlee(Vector2 position, double weight = 1)
        {
            return SteeringSeek(position, -weight);
        }

        protected virtual Vector2 SteeringFlee(Element element, double weight = 1)
        {
            return SteeringSeek(element, -weight);
        }

        protected virtual Vector2 SteeringFlee(IEnumerable<Element> elements, double weight = 1)
        {
            Vector2 v = Vector2.Zero;
            int count = elements.Count();
            if (count == 0)
            {
                return v;
            }
            foreach (Element e in elements)
            {
                v += e.GetPosition();
            }
            return SteeringFlee(SteerAverage(v, count), weight);
        }

        //same as SteeringFlee
        protected virtual Vector2 SteeringEvasion(Vector2 position, double weight = 1)
        {
            return SteeringSeek(position, -weight);
        }

        protected virtual Vector2 SteeringEvasion(Element element, double weight = 1)
        {
            return SteeringPursuit(element, -weight);
        }

        protected virtual Vector2 SteeringEvasion(IEnumerable<Element> elements, double weight = 1)
        {
            Vector2 v = Vector2.Zero;
            int count = elements.Count();
            if (count == 0)
            {
                return v;
            }
            foreach (Element e in elements)
            {
                v += e.IsStationary ? e.GetPosition() : e.PredictPositionAfter();
            }
            return SteeringFlee(SteerAverage(v, count), weight);
        }

        protected virtual Vector2 SteeringAvoid(IEnumerable<Element> elements, double range, double weight = 1)
        {
            if (elements.Count() == 0)
            {
                return new Vector2(0, 0);
            }
            int ytox = 0;
            Vector2 lineOfSight = range * _direction;
            double nearest = lineOfSight.Length;
            Vector2 nearestPos = new Vector2(0, 0);
            Vector2 r1 = _position + Vector2.Perpendicular(_direction * Radius);
            Vector2 r2 = r1 + lineOfSight;
            Vector2 l1 = _position - Vector2.Perpendicular(_direction * Radius);
            Vector2 l2 = l1 + lineOfSight;
            foreach (Element e in elements)
            {
                if (Vector2.Distance(_position, e.GetPosition()) > e.Radius + _dSpeed)
                {
                    continue;
                }
                Vector2 ip;
                if (e.IntersectsWith(r1, r2, out ip))
                {
                    double dist = Vector2.Distance(_position, ip);
                    if (dist < nearest)
                    {
                        nearest = dist;
                        nearestPos = e.GetPosition();
                        ytox = -1;
                    }
                }
                if (e.IntersectsWith(l1, l2, out ip))
                {
                    double dist = Vector2.Distance(_position, ip);
                    if (dist < nearest)
                    {
                        nearest = dist;
                        nearestPos = e.GetPosition();
                        ytox = 1;
                    }
                }
            }
            return nearest < lineOfSight.Length ? weight * ytox * Vector2.Perpendicular(_position - nearestPos) : new Vector2(0, 0);
        }

        protected virtual Vector2 SteeringWander(double weight = 10)
        {
            return _direction + Angle.Random(weight);
        }

        protected virtual Vector2 SteeringSeparation(IEnumerable<Element> elements, double weight = 1)
        {
            return SteeringCohesion(elements, -weight);
        }

        protected virtual Vector2 SteeringCohesion(IEnumerable<Element> elements, double weight = 1)
        {
            Vector2 x = Vector2.Zero;
            foreach (Element e in elements)
            {
                x += Vector2.Normalized(e.GetPosition() - _position);
            }
            return weight * SteerAverage(x, elements.Count());
        }

        protected virtual Vector2 SteeringAlignment(IEnumerable<Element> elements, double weight = 1)
        {
            Vector2 x = Vector2.Zero;
            foreach (Element e in elements)
            {
                x += e.GetDirection();
            }
            return weight * SteerAverage(x, elements.Count());
        }

        protected virtual double SteeringAdjustSpeed(IEnumerable<Element> elements, double weight = 1)
        {
            int count = elements.Count();
            if (count == 0)
            {
                return _dSpeed;
            }
            double x = 0;
            foreach (Element e in elements)
            {
                x += e.GetSpeed();
            }
            return weight * x / count;
        }

        protected virtual Vector2 SteerAverage(Vector2 vector, int count)
        {
            //return vector;
            //return Vector2.Normalized(vector);
            return count > 0 ? vector / count : vector;
        }
        
        #endregion

        #region Abstract Methods

        public abstract void SetModifiers(params double[] values);

        protected abstract void ApplyRules(IEnumerable<Element> locals);

        #endregion
    }
}

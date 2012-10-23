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
using Muragatte.Core.Environment.SteeringUtils;
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public abstract class Agent : Element
    {
        #region Fields

        protected Vector2 _direction = Vector2.X0Y1;
        protected double _dSpeed = 1;
        protected Vector2 _altPosition = Vector2.Zero;
        protected Vector2 _altDirection = Vector2.X0Y1;
        protected Neighbourhood _fieldOfView = null;
        protected Angle _dTurningAngle = Angle.Deg180;
        protected Centroid _representative = null;
        protected Dictionary<string, Steering> _steering = new Dictionary<string, Steering>();
        protected AgentArgs _args = null;
        protected Noise _noise;
        
        #endregion

        #region Constructors

        public Agent(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed,
            Species species, Neighbourhood fieldOfView, Angle turningAngle, AgentArgs args)
            : base(id, model, position)
        {
            _direction = direction;
            _dSpeed = speed;
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_AGENTS_LABEL);
            FieldOfView = fieldOfView;
            _dTurningAngle = turningAngle;
            _args = args;
            _noise = _args.Distribution.Noise(_model.Random, _args.NoiseArgA, _args.NoiseArgB);
            EnableSteering();
        }

        public Agent(int id, MultiAgentSystem model, Species species, Neighbourhood fieldOfView, Angle turningAngle, AgentArgs args)
            : this(id, model, Vector2.Zero, Vector2.X0Y1, 1, species, fieldOfView, turningAngle, args) { }

        protected Agent(Agent other, MultiAgentSystem model)
            : this(other._iElementID, model, other._position, other._direction, other._dSpeed,
            other._species, other._fieldOfView.Clone(), other._dTurningAngle, other._args.Clone()) { }
        
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
            set { }
        }

        public override double Height
        {
            get { return 1; }
            set { }
        }

        public override double Radius
        {
            get { return DEFAULT_RADIUS; }
        }

        public Neighbourhood FieldOfView
        {
            get { return _fieldOfView; }
            protected set
            {
                _fieldOfView = value;
                if (_fieldOfView != null) _fieldOfView.Source = this;
            }
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

        public override bool IsStationary
        {
            get { return false; }
        }

        public Centroid Representative
        {
            get { return _representative; }
        }

        public override Group Group
        {
            get { return _representative == null ? null : _representative.Group; }
            set { }
        }

        public override string Name
        {
            get { return CreateName("A"); }
        }

        public virtual double VisibleRange
        {
            get { return _fieldOfView == null ? 0 : _fieldOfView.Range; }
        }

        public override bool IsDirectable
        {
            get { return true; }
        }

        public override bool IsResizeable
        {
            get { return false; }
        }

        public Goal Goal
        {
            get { return _args.Goal; }
            set { if (_args.HasGoal) _args.Goal = value; }
        }

        #endregion

        #region Methods

        public void SetMovementInfo(Vector2 position, Vector2 direction)
        {
            Position = position;
            _direction = direction;
        }

        public void SetMovementInfo(Vector2 position, Vector2 direction, double speed)
        {
            SetMovementInfo(position, direction);
            _dSpeed = speed;
        }

        public void SetMovementInfo(Vector2 position, Vector2 direction, double speed, Angle turningAngle)
        {
            SetMovementInfo(position, direction, speed);
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
            _representative = new Centroid(-_iElementID, this);
        }
        public void CreateRepresentative(int id)
        {

            _representative = new Centroid(id, this);
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

        //public override Storage.ElementStatus ReportStatus()
        //{
        //    return ReportStatus(_args.Modifiers.Values);
        //}

        //public override void LoadStatus(Storage.ElementStatus status)
        //{
        //    base.LoadStatus(status);
        //    //load modifiers
        //}

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

        protected void AddSteering(Steering steering)
        {
            _steering.Add(steering.Name, steering);
        }

        public override void ConfirmUpdate()
        {
            Position = _model.Region.Outside(_altPosition);
            _direction = _altDirection;
        }

        public override void Update()
        {
            UpdateMovement(ApplyRules(GetLocalNeighbours()));
        }

        protected virtual void UpdateMovement(Vector2 dirDelta)
        {
            _altDirection = Vector2.Normalized(_direction + dirDelta + _noise.ApplyAngle());
            ProperDirection();
            _altPosition = _position + _dSpeed * _model.TimePerStep * _altDirection;
        }

        protected virtual IEnumerable<Element> GetLocalNeighbours()
        {
            return _fieldOfView.Within(_model.Elements.RangeSearch(this, VisibleRange));
        }

        protected abstract Vector2 ApplyRules(IEnumerable<Element> locals);

        protected abstract void EnableSteering();

        #endregion
    }
}

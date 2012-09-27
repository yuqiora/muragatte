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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Common;

namespace Muragatte.Core.Environment
{
    public class Centroid : Element
    {
        #region Fields

        private Vector2 _direction = new Vector2(0, 1);
        private double _dSpeed = 1;
        private Group _group = null;
        private bool _bInGroup = false;

        #endregion

        #region Constructors

        public Centroid(MultiAgentSystem model, Species species)
            : this(IdCounter.Next(), model, species) { }

        public Centroid(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed, Species species)
            : this(IdCounter.Next(), model, position, direction, speed, species) { }

        public Centroid(int id, MultiAgentSystem model, Species species)
            : base(id, model)
        {
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_CENTROIDS_LABEL);
        }

        public Centroid(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed, Species species)
            : base(id, model, position)
        {
            _direction = direction;
            _dSpeed = speed;
            SetSpecies(species, Storage.SpeciesCollection.DEFAULT_CENTROIDS_LABEL);
        }

        public Centroid(Agent source) : this(source.Model, source.Position, source.Direction, source.Speed, null) { }

        public Centroid(int id, Agent source) : this(id, source.Model, source.Position, source.Direction, source.Speed, null) { }

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
            get { return 2 * Radius; }
        }

        public override double Height
        {
            get { return 2 * Radius; }
        }

        public override double Radius
        {
            get { return DEFAULT_RADIUS; }
        }

        public override bool IsStationary
        {
            get { return false; }
        }

        public override Group Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public bool IsInGroup
        {
            get { return _bInGroup; }
            set { _bInGroup = value; }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Ignored; }
        }

        public override string Name
        {
            get { return CreateName("C"); }
        }

        public bool IsGroupRepresentative
        {
            get { return _group != null && this == _group.FirstMember.Representative; }
        }

        #endregion

        #region Methods

        public override ElementNature RelationshipWith(Element e)
        {
            return ElementNature.Ignored;
        }

        public override void Update()
        {
            _bEnabled = false;
            _group = null;
            _bInGroup = false;
        }

        public override void ConfirmUpdate()
        {
            if (_group != null)
            {
                if (_bEnabled)
                {
                    _position = new Vector2(0, 0);
                    _direction = new Vector2(0, 0);
                    _dSpeed = 0;
                    foreach (Agent a in _group)
                    {
                        _position += a.Position;
                        _direction += a.Direction;
                        _dSpeed += a.Speed;
                    }
                    _position /= _group.Count;
                    _direction.Normalize();
                    _dSpeed /= _group.Count;
                }
                else
                {
                    _position = _group.FirstMember.Representative._position;
                    _direction = _group.FirstMember.Representative._direction;
                    _dSpeed = _group.FirstMember.Representative._dSpeed;
                }
            }
        }

        public override string ToString()
        {
            return ToString("C");
        }

        #endregion
    }
}

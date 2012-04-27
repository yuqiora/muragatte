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

namespace Muragatte.Core.Environment
{
    public class Centroid : Extras
    {
        #region Fields

        private Vector2 _direction = new Vector2(0, 1);
        private double _dSpeed = 1;
        private Group _group = null;
        private bool _bInGroup = false;

        #endregion

        #region Constructors

        public Centroid(MultiAgentSystem model)
            : base(model, false) { }

        public Centroid(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed)
            : base(model, position, false)
        {
            _direction = direction;
            _dSpeed = speed;
        }

        public Centroid(Agent source)
            : this(source.Model, source.Position, source.Direction, source.Speed) { }

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

        public Group Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public bool IsInGroup
        {
            get { return _bInGroup; }
            set { _bInGroup = value; }
        }

        #endregion

        #region Methods

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

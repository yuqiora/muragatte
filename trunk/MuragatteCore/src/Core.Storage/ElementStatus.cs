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

namespace Muragatte.Core.Storage
{
    public class ElementStatus
    {
        #region Fields

        private int _iElementID;
        private Vector2 _position;
        private Vector2 _direction;
        private double _dSpeed;
        private bool _bEnabled;
        private int _iSpeciesID;
        private int _iGroupID;
        private double[] _dModifiers = null;

        #endregion

        #region Constructors

        public ElementStatus(int elementID, Vector2 position, Vector2 direction,
            double speed, bool enabled, int speciesID, int groupID)
        {
            _iElementID = elementID;
            _position = position;
            _direction = direction;
            _dSpeed = speed;
            _bEnabled = enabled;
            _iSpeciesID = speciesID;
            _iGroupID = groupID;
        }

        public ElementStatus(int elementID, Vector2 position, Vector2 direction,
            double speed, bool enabled, int speciesID, int groupID, params double[] modifiers)
            : this(elementID, position, direction, speed, enabled, speciesID, groupID)
        {
            _dModifiers = modifiers;
        }

        #endregion

        #region Properties

        public int ElementID
        {
            get { return _iElementID; }
        }

        public Vector2 Position
        {
            get { return _position; }
        }

        public Vector2 Direction
        {
            get { return _direction; }
        }

        public double Speed
        {
            get { return _dSpeed; }
        }

        public bool IsEnabled
        {
            get { return _bEnabled; }
        }

        public int SpeciesID
        {
            get { return _iSpeciesID; }
        }

        public int GroupID
        {
            get { return _iGroupID; }
        }

        public double[] Modifiers
        {
            get { return _dModifiers; }
        }

        #endregion

        #region Methods

        private string ModifiersToString()
        {
            if (_dModifiers == null)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (double mod in _dModifiers)
            {
                sb.AppendFormat(" {0}", mod);
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}{7}",
                _iElementID, _position, _direction, _dSpeed, _bEnabled, _iSpeciesID, _iGroupID, ModifiersToString());
        }

        #endregion
    }
}

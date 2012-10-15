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
        private string _sSpeciesName;
        private int _iGroupID;
        private List<double> _modifiers = null;

        #endregion

        #region Constructors

        public ElementStatus(int elementID, Vector2 position, Vector2 direction, double speed, bool enabled, string speciesName, int groupID)
        {
            _iElementID = elementID;
            _position = position;
            _direction = direction;
            _dSpeed = speed;
            _bEnabled = enabled;
            _sSpeciesName = speciesName;
            _iGroupID = groupID;
        }

        public ElementStatus(int elementID, Vector2 position, Vector2 direction,
            double speed, bool enabled, string speciesName, int groupID, IEnumerable<double> modifiers)
            : this(elementID, position, direction, speed, enabled, speciesName, groupID)
        {
            _modifiers = modifiers == null ? null : new List<double>(modifiers);
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

        public string SpeciesName
        {
            get { return _sSpeciesName; }
        }

        public int GroupID
        {
            get { return _iGroupID; }
        }

        public List<double> Modifiers
        {
            get { return _modifiers; }
        }

        #endregion

        #region Methods

        private string ModifiersToString()
        {
            if (_modifiers == null)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (double mod in _modifiers)
            {
                sb.AppendFormat(" {0}", mod);
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}{7}",
                _iElementID, _position, _direction, _dSpeed, _bEnabled, _sSpeciesName, _iGroupID, ModifiersToString());
        }

        #endregion
    }
}

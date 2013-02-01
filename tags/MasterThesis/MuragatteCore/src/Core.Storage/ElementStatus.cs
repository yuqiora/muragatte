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

        public static readonly string Header = "Id PositionX PositionY Direction Speed IsEnabled Species Group";

        private int _iElementID;
        private Vector2 _position;
        private Vector2 _direction;
        private double _dSpeed;
        private bool _bEnabled;
        private string _sSpeciesName;
        private int _iGroupID;

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

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}", _iElementID, _position, _direction.Angle.Degrees, _dSpeed, _bEnabled, _sSpeciesName, _iGroupID);
        }

        public static ElementStatus FromString(string info)
        {
            string[] items = info.Split(' ');
            int id, group;
            double posX, posY, angle, speed;
            bool enabled;
            if (items.Length == 8 && int.TryParse(items[0], out id) &&
                double.TryParse(items[1], out posX) && double.TryParse(items[2], out posY) &&
                double.TryParse(items[3], out angle) && double.TryParse(items[4], out speed) &&
                bool.TryParse(items[5], out enabled) && int.TryParse(items[7], out group))
            {
                return new ElementStatus(id, new Vector2(posX, posY), new Vector2(new Angle(angle)), speed, enabled, items[6], group);
            }
            else return null;
        }

        #endregion
    }
}

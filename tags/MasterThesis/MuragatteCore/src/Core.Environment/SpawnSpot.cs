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
using System.Xml.Serialization;
using Muragatte.Common;
using Muragatte.Random;

namespace Muragatte.Core.Environment
{
    public abstract class SpawnSpot : INotifyPropertyChanged
    {
        #region Fields

        protected string _sName;
        protected Vector2 _position;
        protected double _dWidth;
        protected double _dHeight;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public SpawnSpot() : this("Spawn", Vector2.Zero, 1, 1) { }

        public SpawnSpot(string name, Vector2 position, double width, double height)
        {
            _sName = name;
            _position = position;
            _dWidth = width;
            _dHeight = height;
        }

        #endregion

        #region Properties

        [XmlAttribute]
        public string Name
        {
            get { return _sName; }
            set
            {
                _sName = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("Info");
            }
        }

        [XmlIgnore]
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyPropertyChanged("Position");
                NotifyPropertyChanged("PositionX");
                NotifyPropertyChanged("PositionY");
                NotifyPropertyChanged("Info");
            }
        }

        [XmlAttribute]
        public double PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                NotifyPropertyChanged("Position");
                NotifyPropertyChanged("PositionX");
                NotifyPropertyChanged("Info");
            }
        }

        [XmlAttribute]
        public double PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                NotifyPropertyChanged("Position");
                NotifyPropertyChanged("PositionY");
                NotifyPropertyChanged("Info");
            }
        }

        [XmlIgnore]
        public virtual double Width
        {
            get { return _dWidth; }
            set
            {
                _dWidth = value;
                NotifyPropertyChanged("Width");
                NotifyPropertyChanged("Info");
            }
        }

        [XmlIgnore]
        public virtual double Height
        {
            get { return _dHeight; }
            set
            {
                _dHeight = value;
                NotifyPropertyChanged("Height");
                NotifyPropertyChanged("Info");
            }
        }

        public string Info
        {
            get { return string.Format("{0} {1}", _sName, ToString()); }
        }

        #endregion

        #region Methods

        public Vector2 Respawn()
        {
            return _position;
        }

        public abstract Vector2 Respawn(RandomMT random);

        public override string ToString()
        {
            return string.Format("{0}x{1} @ <{2}>", _dWidth, _dHeight, _position);
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

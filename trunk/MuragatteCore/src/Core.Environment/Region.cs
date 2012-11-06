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

namespace Muragatte.Core.Environment
{
    public class Region : INotifyPropertyChanged
    {
        #region Fields

        private int _iWidth = 0;
        private int _iHeight = 0;
        private bool _bBorderedHorizontal = true;   // - (y)
        private bool _bBorderedVertical = true;     // | (x)

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Contructors

        public Region() : this(100, true) { }

        public Region(int width, int height, bool borderedHorizontal, bool borderedVertical)
        {
            _iWidth = width;
            _iHeight = height;
            _bBorderedHorizontal = borderedHorizontal;
            _bBorderedVertical = borderedVertical;
        }

        public Region(int width, int height, bool bordered)
            : this(width, height, bordered, bordered) { }

        public Region(int length, bool borderedHorizontal, bool borderedVertical) :
            this(length, length, borderedHorizontal, borderedVertical) { }

        public Region(int length, bool bordered)
            : this(length, length, bordered, bordered) { }
        
        #endregion

        #region Properties
        
        [XmlAttribute]
        public int Width
        {
            get { return _iWidth; }
            set
            {
                _iWidth = value;
                NotifyPropertyChanged("Width");
            }
        }

        [XmlAttribute]
        public int Height
        {
            get { return _iHeight; }
            set
            {
                _iHeight = value;
                NotifyPropertyChanged("Height");
            }
        }

        [XmlIgnore]
        public bool IsBordered
        {
            get { return _bBorderedHorizontal && _bBorderedVertical; }
            set
            {
                _bBorderedHorizontal = value;
                _bBorderedVertical = value;
                NotifyPropertyChanged("IsBorderedHorizontally");
                NotifyPropertyChanged("IsBorderedVertically");
                NotifyPropertyChanged("IsBordered");
            }
        }

        [XmlAttribute]
        public bool IsBorderedHorizontally
        {
            get { return _bBorderedHorizontal; }
            set
            {
                _bBorderedHorizontal = value;
                NotifyPropertyChanged("IsBorderedHorizontally");
                NotifyPropertyChanged("IsBordered");
            }
        }

        [XmlAttribute]
        public bool IsBorderedVertically
        {
            get { return _bBorderedVertical; }
            set
            {
                _bBorderedVertical= value;
                NotifyPropertyChanged("IsBorderedVertically");
                NotifyPropertyChanged("IsBordered");
            }
        }
        
        #endregion

        #region Methods

        public Vector2 Containment(Vector2 position, Vector2 direction, double range)
        {
            bool changed = false;
            Vector2 v = position + direction * range;
            Angle offset = new Angle(0);
            Angle b = new Angle(direction);
            if (_bBorderedVertical && (v.X < 0 || v.X >= _iWidth))
            {
                Angle a = new Angle(new Vector2(v.X, 0));
                offset = b - a;
                offset.Normalize();
                changed = true;
            }
            if (_bBorderedHorizontal && (v.Y < 0 || v.Y >= _iHeight))
            {
                Angle a = new Angle(new Vector2(0, v.Y));
                Angle c = b - a;
                c.Normalize();
                if (c > offset)
                {
                    offset = c;
                }
                changed = true;
            }
            return changed ? direction + offset.Sign() * Vector2.Perpendicular(v - position) : direction;
        }

        public Vector2 Outside(Vector2 position)
        {
            return new Vector2(Outside(position.X, _iWidth, _bBorderedVertical), Outside(position.Y, _iHeight, _bBorderedHorizontal));
        }

        private double Outside(double value, double size, bool borders) {
            double tmp = value - value % size;
            if (value < 0)
            {
                return borders ? tmp : value % size + size;
            }
            if (value >= size)
            {
                return borders ? tmp : value % size;
            }
            return value;
        }

        public void Load(Region other)
        {
            Width = other._iWidth;
            Height = other._iHeight;
            IsBorderedHorizontally = other._bBorderedHorizontal;
            IsBorderedVertically = other._bBorderedVertical;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

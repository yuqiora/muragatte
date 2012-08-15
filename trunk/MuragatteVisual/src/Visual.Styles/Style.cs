// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.Visual.Shapes;

namespace Muragatte.Visual.Styles
{
    public class Style : INotifyPropertyChanged
    {
        #region Fields

        private string _sName = null;
        private Shape _shape = null;
        private double _dUnitWidth = 1;
        private double _dUnitHeight = 1;
        private int _iWidth = 1;
        private int _iHeight = 1;
        private Color _primaryColor = Colors.Black;
        private Color _secondaryColor = Colors.Black;
        private NeighbourhoodStyle _neighbourhood = null;
        private TrackStyle _track = null;
        private TrailStyle _trail = null;

        #endregion

        #region Constructors

        private Style(Shape shape, Color primaryColor, Color secondaryColor,
            NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
        {
            _shape = shape;
            _primaryColor = primaryColor;
            _secondaryColor = secondaryColor;
            _neighbourhood = neighbourhood;
            _track = track;
            _trail = trail;
        }

        public Style(Shape shape, Element element, double scale, Color primaryColor, Color secondaryColor,
            NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
            : this(shape, primaryColor, secondaryColor, neighbourhood, track, trail)
        {
            _sName = element.Name;
            _dUnitWidth = element.Width;
            _dUnitHeight = element.Height;
            _iWidth = (int)(element.Width * scale);
            _iHeight = (int)(element.Height * scale);
        }

        public Style(Shape shape, string name, double width, double height, double scale, Color primaryColor, Color secondaryColor,
            NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
            : this(shape, primaryColor, secondaryColor, neighbourhood, track, trail)
        {
            _sName = name;
            _dUnitWidth = width;
            _dUnitHeight = height;
            _iWidth = (int)(width * scale);
            _iHeight = (int)(height * scale);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
        }

        public Shape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                NotifyPropertyChanged("Shape");
            }
        }

        public int Width
        {
            get { return _iWidth; }
        }

        public int Height
        {
            get { return _iHeight; }
        }

        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set
            {
                _primaryColor = value;
                NotifyPropertyChanged("PrimaryColor");
            }
        }

        public Color SecondaryColor
        {
            get { return _secondaryColor; }
            set
            {
                _secondaryColor = value;
                NotifyPropertyChanged("SecondaryColor");
            }
        }

        public NeighbourhoodStyle Neighbourhood
        {
            get { return _neighbourhood; }
            set { _neighbourhood = value; }
        }

        public TrackStyle Track
        {
            get { return _track; }
            set { _track = value; }
        }

        public TrailStyle Trail
        {
            get { return _trail; }
            set { _trail = value; }
        }

        #endregion

        #region Methods

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction)
        {
            _shape.Draw(target, position, new Angle(direction), _primaryColor, _secondaryColor, _iWidth, _iHeight);
        }

        public void Rescale(double value)
        {
            _iWidth = (int)(_dUnitWidth * value);
            _iHeight = (int)(_dUnitHeight * value);
            if (_neighbourhood != null)
            {
                _neighbourhood.Rescale(value);
            }
        }

        public override string ToString()
        {
            return _sName;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

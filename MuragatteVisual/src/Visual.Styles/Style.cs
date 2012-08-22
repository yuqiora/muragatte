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
        private Shape _shape = EllipseShape.Instance();
        private double _dUnitWidth = 1;
        private double _dUnitHeight = 1;
        private int _iWidth = 1;
        private int _iHeight = 1;
        private Color _primaryColor = DefaultValues.AGENT_COLOR;
        private Color _secondaryColor = DefaultValues.AGENT_COLOR;
        private NeighbourhoodStyle _neighbourhood = null;
        private TrackStyle _track = null;
        private TrailStyle _trail = null;
        private bool _bNeighbourhood = false;
        private bool _bTrack = false;
        private bool _bTrail = false;

        #endregion

        #region Constructors

        public Style()
        {
            _sName = "New Style";
            _neighbourhood = new NeighbourhoodStyle();
            _track = new TrackStyle();
            _trail = new TrailStyle();
        }

        //private Style(Shape shape, Color primaryColor, Color secondaryColor,
        //    NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
        //{
        //    _shape = shape;
        //    _primaryColor = primaryColor;
        //    _secondaryColor = secondaryColor;
        //    _neighbourhood = neighbourhood ?? new NeighbourhoodStyle();
        //    Color c = primaryColor.NotTransparent() ? primaryColor : secondaryColor;
        //    _track = track ?? new TrackStyle(c);
        //    _trail = trail ?? new TrailStyle(c, DefaultValues.TRAIL_LENGTH);
        //}

        public Style(Shape shape, Element element, Color primaryColor, Color secondaryColor, NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
            : this(shape, element.Name, element.Width, element.Height, primaryColor, secondaryColor, neighbourhood, track, trail)
        {
            //_sName = element.Name;
            //_dUnitWidth = element.Width;
            //_dUnitHeight = element.Height;
            //_iWidth = (int)(element.Width * scale);
            //_iHeight = (int)(element.Height * scale);
        }

        public Style(Shape shape, string name, double width, double height, Color primaryColor, Color secondaryColor,
            NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
            //: this(shape, primaryColor, secondaryColor, neighbourhood, track, trail)
        {
            _sName = name;
            _dUnitWidth = width;
            _dUnitHeight = height;
            _shape = shape;
            _primaryColor = primaryColor;
            _secondaryColor = secondaryColor;
            if (neighbourhood == null)
            {
                _neighbourhood = new NeighbourhoodStyle();
            }
            else
            {
                _neighbourhood = neighbourhood;
                _bNeighbourhood = true;
            }
            Color c = primaryColor.NotTransparent() ? primaryColor : secondaryColor;
            if (track == null)
            {
                _track = new TrackStyle(c);
            }
            else
            {
                _track = track;
                _bTrack = true;
            }
            if (trail == null)
            {
                _trail = new TrailStyle(c, DefaultValues.TRAIL_LENGTH);
            }
            else
            {
                _trail = trail;
                _bTrail = true;
            }
            //_iWidth = (int)(width * scale);
            //_iHeight = (int)(height * scale);
        }

        public Style(Style other)
            : this(other._shape, "Copy of " + other.Name, other.UnitWidth, other.UnitHeight, other._primaryColor, other._secondaryColor,
            new NeighbourhoodStyle(other._neighbourhood), new TrackStyle(other._track), new TrailStyle(other._trail))
        {
            _bNeighbourhood = other._bNeighbourhood;
            _bTrack = other._bTrack;
            _bTrail = other._bTrail;
            //_dUnitWidth = other._dUnitWidth;
            //_dUnitHeight = other._dUnitHeight;
            //_iWidth = other._iWidth;
            //_iHeight = other._iHeight;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
            set
            {
                _sName = value;
                NotifyPropertyChanged("Name");
            }
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

        public double UnitWidth
        {
            get { return _dUnitWidth; }
            set
            {
                _dUnitWidth = value;
                NotifyPropertyChanged("UnitWidth");
                _iWidth = (int)(_dUnitWidth * DefaultValues.Scale);
            }
        }

        public double UnitHeight
        {
            get { return _dUnitHeight; }
            set
            {
                _dUnitHeight = value;
                NotifyPropertyChanged("UnitHeight");
                _iHeight = (int)(_dUnitHeight * DefaultValues.Scale);
            }
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

        public bool HasNeighbourhood
        {
            get { return _bNeighbourhood; }
            set
            {
                _bNeighbourhood = value;
                NotifyPropertyChanged("HasNeighbourhood");
            }
        }

        public bool HasTrack
        {
            get { return _bTrack; }
            set
            {
                _bTrack = value;
                NotifyPropertyChanged("HasTrack");
            }
        }

        public bool HasTrail
        {
            get { return _bTrail; }
            set
            {
                _bTrail = value;
                NotifyPropertyChanged("HasTrail");
            }
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

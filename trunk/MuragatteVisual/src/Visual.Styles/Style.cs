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
using System.Xml.Serialization;
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.Visual.IO;
using Muragatte.Visual.Shapes;

namespace Muragatte.Visual.Styles
{
    public class Style : INotifyPropertyChanged
    {
        #region Fields

        private string _sName = null;
        private Shape _shape = EllipseShape.Instance;
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
        private List<Coordinates> _coordinates = null;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Style()
        {
            _sName = "New Style";
            _neighbourhood = new NeighbourhoodStyle();
            _track = new TrackStyle();
            _trail = new TrailStyle();
            Rescale(DefaultValues.Scale);
        }

        public Style(Shape shape, Element element, Color primaryColor, Color secondaryColor, NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
            : this(shape, element.Name, element.Width, element.Height, primaryColor, secondaryColor, neighbourhood, track, trail) { }

        public Style(Shape shape, string name, double width, double height, Color primaryColor, Color secondaryColor,
            NeighbourhoodStyle neighbourhood, TrackStyle track, TrailStyle trail)
        {
            _sName = name;
            _dUnitWidth = width;
            _dUnitHeight = height;
            _shape = shape;
            _primaryColor = primaryColor;
            _secondaryColor = secondaryColor;
            NullNeighbourhood = neighbourhood;
            NullTrack = track;
            NullTrail = trail;
            Rescale(DefaultValues.Scale);
        }

        public Style(Style other) : this(other, "Copy of " + other.Name) { }

        public Style(Style other, string name)
            : this(other._shape, name, other.UnitWidth, other.UnitHeight, other._primaryColor, other._secondaryColor,
            new NeighbourhoodStyle(other._neighbourhood), new TrackStyle(other._track), new TrailStyle(other._trail))
        {
            _bNeighbourhood = other._bNeighbourhood;
            _bTrack = other._bTrack;
            _bTrail = other._bTrail;
            _coordinates = new List<Coordinates>(other._coordinates);
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
            }
        }

        [XmlElement(Type = typeof(XmlShape))]
        public Shape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                RecreateCoordinates();
                NotifyPropertyChanged("Shape");
            }
        }

        [XmlIgnore]
        public int Width
        {
            get { return _iWidth; }
        }

        [XmlIgnore]
        public int Height
        {
            get { return _iHeight; }
        }

        [XmlElement("Width")]
        public double UnitWidth
        {
            get { return _dUnitWidth; }
            set
            {
                _dUnitWidth = value;
                _iWidth = (int)(_dUnitWidth * DefaultValues.Scale);
                RecreateCoordinates();
                NotifyPropertyChanged("UnitWidth");
            }
        }

        [XmlElement("Height")]
        public double UnitHeight
        {
            get { return _dUnitHeight; }
            set
            {
                _dUnitHeight = value;
                _iHeight = (int)(_dUnitHeight * DefaultValues.Scale);
                RecreateCoordinates();
                NotifyPropertyChanged("UnitHeight");
            }
        }

        [XmlElement(Type = typeof(XmlColor))]
        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set
            {
                _primaryColor = value;
                NotifyPropertyChanged("PrimaryColor");
            }
        }

        [XmlElement(Type = typeof(XmlColor))]
        public Color SecondaryColor
        {
            get { return _secondaryColor; }
            set
            {
                _secondaryColor = value;
                NotifyPropertyChanged("SecondaryColor");
            }
        }

        [XmlIgnore]
        public NeighbourhoodStyle Neighbourhood
        {
            get { return _neighbourhood; }
            set { _neighbourhood = value; }
        }

        [XmlIgnore]
        public TrackStyle Track
        {
            get { return _track; }
            set { _track = value; }
        }

        [XmlIgnore]
        public TrailStyle Trail
        {
            get { return _trail; }
            set { _trail = value; }
        }

        [XmlIgnore]
        public bool HasNeighbourhood
        {
            get { return _bNeighbourhood; }
            set
            {
                _bNeighbourhood = value;
                NotifyPropertyChanged("HasNeighbourhood");
            }
        }

        [XmlIgnore]
        public bool HasTrack
        {
            get { return _bTrack; }
            set
            {
                _bTrack = value;
                NotifyPropertyChanged("HasTrack");
            }
        }

        [XmlIgnore]
        public bool HasTrail
        {
            get { return _bTrail; }
            set
            {
                _bTrail = value;
                NotifyPropertyChanged("HasTrail");
            }
        }

        [XmlElement(ElementName = "Neighbourhood", IsNullable = false)]
        public NeighbourhoodStyle NullNeighbourhood
        {
            get { return _bNeighbourhood ? _neighbourhood : null; }
            set
            {
                if (value == null)
                {
                    _neighbourhood = new NeighbourhoodStyle();
                    HasNeighbourhood = false;
                }
                else
                {
                    _neighbourhood = value;
                    HasNeighbourhood = true;
                }
            }
        }

        [XmlElement(ElementName = "Track", IsNullable = false)]
        public TrackStyle NullTrack
        {
            get { return _bTrack ? _track : null; }
            set
            {
                if (value == null)
                {
                    _track = new TrackStyle(_primaryColor.NotTransparent() ? _primaryColor : _secondaryColor);
                    HasTrack = false;
                }
                else
                {
                    _track = value;
                    HasTrack = true;
                }
            }
        }

        [XmlElement(ElementName = "Trail", IsNullable = false)]
        public TrailStyle NullTrail
        {
            get { return _bTrail ? _trail : null; }
            set
            {
                if (value == null)
                {
                    _trail = new TrailStyle(_primaryColor.NotTransparent() ? _primaryColor : _secondaryColor, DefaultValues.TRAIL_LENGTH);
                    HasTrail = false;
                }
                else
                {
                    _trail = value;
                    HasTrail = true;
                }
            }
        }

        #endregion

        #region Methods

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction)
        {
            _shape.Draw(target, position, new Angle(direction), _primaryColor, _secondaryColor, _coordinates);
        }

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction, Color primaryColor, Color secondaryColor)
        {
            _shape.Draw(target, position, new Angle(direction), primaryColor, secondaryColor, _coordinates);
        }

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            _shape.Draw(target, position, new Angle(direction), primaryColor, secondaryColor,
                coordinates == null || coordinates.Count == 0 ? _coordinates : coordinates);
        }

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction, List<Coordinates> coordinates)
        {
            _shape.Draw(target, position, new Angle(direction), _primaryColor, _secondaryColor,
                coordinates == null || coordinates.Count == 0 ? _coordinates : coordinates);
        }

        public void Rescale(double value)
        {
            _iWidth = (int)(_dUnitWidth * value);
            _iHeight = (int)(_dUnitHeight * value);
            RecreateCoordinates();
            if (_neighbourhood != null)
            {
                _neighbourhood.Rescale(value);
            }
        }

        public void RecreateCoordinates()
        {
            _coordinates = _shape.CreateCoordinates(_iWidth, _iHeight);
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
    }
}

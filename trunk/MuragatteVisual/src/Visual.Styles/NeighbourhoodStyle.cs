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
using Muragatte.Visual.Shapes;

namespace Muragatte.Visual.Styles
{
    public class NeighbourhoodStyle : INotifyPropertyChanged
    {
        #region Fields

        private Shape _shape = ArcShape.Instance;
        private Color _primaryColor = Colors.Transparent;
        private Color _secondaryColor = DefaultValues.NEIGHBOURHOOD_COLOR;
        private double _dUnitRadius = 1;
        private int _iRadius = 1;
        private Angle _angle = new Angle(DefaultValues.NEIGHBOURHOOD_ANGLE_DEGREES);
        private List<Coordinates> _coordinates = null;

        #endregion

        #region Constructors

        public NeighbourhoodStyle()
        {
            _coordinates = _shape.CreateCoordinates(_iRadius, _iRadius, _angle);
        }

        public NeighbourhoodStyle(Shape shape, Color primaryColor, Color secondaryColor, double radius, Angle angle, double scale)
        {
            _shape = shape;
            _primaryColor = primaryColor;
            _secondaryColor = secondaryColor;
            _dUnitRadius = radius;
            //_iRadius = (int)(radius * scale);
            Rescale(DefaultValues.Scale);
            _angle = angle;
        }

        public NeighbourhoodStyle(NeighbourhoodStyle other)
            : this(other._shape, other._primaryColor, other._secondaryColor, other._dUnitRadius, other._angle, 1)
        {
            _iRadius = other._iRadius;
            _coordinates = new List<Coordinates>(other._coordinates);
        }

        #endregion

        #region Properties

        public Shape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                NotifyPropertyChanged("Shape");
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

        public double UnitRadius
        {
            get { return _dUnitRadius; }
            set
            {
                _dUnitRadius = value;
                NotifyPropertyChanged("UnitRadius");
                Rescale(DefaultValues.Scale);
            }
        }

        public int Radius
        {
            get { return _iRadius; }
        }

        public Angle Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                NotifyPropertyChanged("Angle");
            }
        }

        #endregion

        #region Methods

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction)
        {
            _shape.Draw(target, position, new Angle(direction), _primaryColor, _secondaryColor, _coordinates);
            //_shape.Draw(target, position, new Angle(direction), _primaryColor, _secondaryColor, _iRadius, _iRadius, _angle);
        }

        public void Draw(WriteableBitmap target, Vector2 position, Vector2 direction, List<Coordinates> coordinates)
        {
            _shape.Draw(target, position, new Angle(direction), _primaryColor, _secondaryColor,
                coordinates == null || coordinates.Count == 0 ? _coordinates : coordinates);
        }

        public void Rescale(double value)
        {
            _iRadius = (int)(_dUnitRadius * value);
            _coordinates = _shape.CreateCoordinates(_iRadius, _iRadius, _angle);

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

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

        private Shape _shape = null;
        private Color _primaryColor = Colors.LightGreen;
        private Color _secondaryColor = Colors.Transparent;
        private double _dUnitRadius = 1;
        private int _iRadius = 1;
        private Angle _angle = new Angle(135);

        #endregion

        #region Constructors

        public NeighbourhoodStyle(Shape shape, Color primaryColor, Color secondaryColor, double radius, Angle angle, double scale)
        {
            _shape = shape;
            _primaryColor = primaryColor;
            _secondaryColor = secondaryColor;
            _dUnitRadius = radius;
            _iRadius = (int)(radius * scale);
            _angle = angle;
        }

        public NeighbourhoodStyle(NeighbourhoodStyle other)
            : this(other._shape, other._primaryColor, other._secondaryColor, other._dUnitRadius, other._angle, 1)
        {
            _iRadius = other._iRadius;
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
            _shape.Draw(target, position, new Angle(direction), _primaryColor, _secondaryColor, _iRadius, _iRadius, _angle);
        }

        public void Rescale(double value)
        {
            _iRadius = (int)(_dUnitRadius * value);
        }

        public void Update(Shape shape, Color primaryColor, Color secondaryColor, double radius, double scale, Angle angle)
        {
            if (shape != _shape) Shape = shape;
            if (primaryColor != _primaryColor) PrimaryColor = primaryColor;
            if (secondaryColor != _secondaryColor) SecondaryColor = secondaryColor;
            if (radius != _dUnitRadius)
            {
                UnitRadius = radius;
                Rescale(scale);
            }
            if (angle != _angle) Angle = angle;
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

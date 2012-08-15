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
        private Angle _angle = new Angle(0);

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

        public int Radius
        {
            get { return _iRadius; }
        }

        public Angle Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        #endregion

        #region Methods

        public void Rescale(double value)
        {
            _iRadius = (int)(_dUnitRadius * value);
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

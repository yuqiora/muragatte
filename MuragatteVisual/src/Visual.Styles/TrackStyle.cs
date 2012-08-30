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

namespace Muragatte.Visual.Styles
{
    public class TrackStyle : INotifyPropertyChanged
    {
        #region Fields

        protected Color _color = DefaultValues.AGENT_COLOR;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public TrackStyle() { }

        public TrackStyle(Color color)
        {
            _color = color;
        }

        public TrackStyle(TrackStyle other) : this(other._color) { }

        #endregion

        #region Properties

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                NotifyPropertyChanged("Color");
            }
        }

        #endregion

        #region Methods

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

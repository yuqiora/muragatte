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
    public class TrailStyle : TrackStyle
    {
        #region Fields

        private int _iLength = DefaultValues.TRAIL_LENGTH;

        #endregion

        #region Constructors

        public TrailStyle() : base() { }

        public TrailStyle(Color color, int length)
            : base(color)
        {
            _iLength = length;
        }

        public TrailStyle(TrailStyle other) : this(other._color, other._iLength) { }

        #endregion

        #region Properties

        public int Length
        {
            get { return _iLength; }
            set
            {
                _iLength = value;
                NotifyPropertyChanged("Length");
            }
        }

        #endregion
    }
}

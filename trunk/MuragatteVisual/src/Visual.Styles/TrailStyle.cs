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
        #region Constants

        public const int DEFAULT_TRAIL_LENGTH = 10;

        #endregion

        #region Fields

        private int _iLength = 1;

        #endregion

        #region Constructors

        public TrailStyle(Color color, int length)
            : base(color)
        {
            _iLength = length;
        }

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

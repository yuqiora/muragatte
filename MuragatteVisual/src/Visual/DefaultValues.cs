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
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Muragatte.Visual
{
    public class DefaultValues
    {
        #region Constants

        public const int NEIGHBOURHOOD_ANGLE_DEGREES = 135;
        public const int TRAIL_LENGTH = 10;
        public const int MAXIMUM_TRAIL_LENGTH = 100;
        public const double MAXIMUM_UNIT_SIZE = 100;
        public const double MAXIMUM_SCALE = 50;

        #endregion

        #region Static Fields

        public static readonly Color BACKGROUND_COLOR = Colors.White;
        public static readonly Color AGENT_COLOR = Colors.Black;
        public static readonly Color OBSTACLE_COLOR = Colors.Gray;
        public static readonly Color GOAL_COLOR = Colors.Red;
        public static readonly Color NEIGHBOURHOOD_COLOR = Colors.LightGreen;
        public static readonly Color CENTROID_COLOR = Colors.Silver;
        public static readonly Color HIGHLIGHT_COLOR = Colors.LightYellow;

        #endregion

        #region Constructors

        private DefaultValues() { }

        #endregion
    }
}

﻿// ------------------------------------------------------------------------
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
using Muragatte.Visual.Shapes;
using Muragatte.Visual.Styles;

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
        public static readonly Color EXTRAS_COLOR = Colors.Green;
        public static readonly Color SPAWNSPOT_COLOR = Colors.Yellow.WithA(128);
        public static readonly Color HIGHLIGHT_COLOR = Colors.Purple.WithA(128);

        private static double _dScale = 1;

        public static readonly Style STYLE = new Style(EllipseShape.Instance, "Default", 1, 1, AGENT_COLOR, Colors.Transparent, null, null, null);
        public static readonly Style AGENT_STYLE = new Style(PointingCircleShape.Instance, "Agent", 1, 1, Colors.Transparent, AGENT_COLOR,
            new NeighbourhoodStyle(ArcShape.Instance, Colors.Transparent, NEIGHBOURHOOD_COLOR, 10, new Common.Angle(NEIGHBOURHOOD_ANGLE_DEGREES), _dScale),
            new TrackStyle(AGENT_COLOR), new TrailStyle(AGENT_COLOR, TRAIL_LENGTH));
        public static readonly Style CENTROID_STYLE = new Style(PointingCircleShape.Instance, "Centroid", 1, 1, CENTROID_COLOR, AGENT_COLOR, null,
            new TrackStyle(CENTROID_COLOR), new TrailStyle(CENTROID_COLOR, TRAIL_LENGTH));

        #endregion

        #region Constructors

        private DefaultValues() { }

        #endregion

        #region Properties

        public static double Scale
        {
            get { return _dScale; }
            set
            {
                _dScale = value;
                if (_dScale < 1) _dScale = 1;
                if (_dScale > MAXIMUM_SCALE) _dScale = MAXIMUM_SCALE;
            }
        }

        #endregion
    }
}

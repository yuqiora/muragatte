// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
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
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.Visual;
using Muragatte.Visual.Shapes;

namespace Muragatte.Research
{
    public class ScenePreview : Canvas
    {
        #region Fields

        private bool _bSpawnSpots = true;

        #endregion

        #region Constructors

        public ScenePreview(int width, int height)
            : base(width, height, null)
        {
            IsAgentsEnabled = false;
        }

        #endregion

        #region Properties

        public bool IsSpawnSpotsEnabled
        {
            get { return _bSpawnSpots; }
            set
            {
                _bSpawnSpots = value;
                NotifyPropertyChanged("IsSpawnSpotsEnabled");
            }
        }

        #endregion

        #region Methods

        public void DrawSpawnSpot(IEnumerable<SpawnSpot> items)
        {
            if (_bSpawnSpots)
            {
                foreach (SpawnSpot s in items)
                {
                    DrawSpawnSpot(s);
                }
            }
        }

        public void DrawSpawnSpot(SpawnSpot spawn)
        {
            Shape shape = null;
            if (spawn is PointSpawnSpot) shape = EllipseShape.Instance;
            else if (spawn is EllipseSpawnSpot) shape = EllipseShape.Instance;
            else if (spawn is RectangleSpawnSpot) shape = RectangleShape.Instance;
            if (shape != null)
            {
                shape.Draw(Image, spawn.Position * Scale, Angle.Zero, DefaultValues.SPAWNSPOT_COLOR,
                    DefaultValues.SPAWNSPOT_COLOR, Scaled(spawn.Width), Scaled(spawn.Height));
            }
        }

        public void DrawStationaryElement(IEnumerable<Element> items)
        {
            if (IsEnvironmentEnabled)
            {
                foreach (Element s in items)
                {
                    DrawStationaryElement(s);
                }
            }
        }

        public void DrawStationaryElement(Element element)
        {
            Shape shape = EllipseShape.Instance;
            Color color = Colors.Black;
            if (element is PositionGoal || element is AreaGoal) color = DefaultValues.GOAL_COLOR;
            else if (element is EllipseObstacle) color = DefaultValues.OBSTACLE_COLOR;
            else if (element is RectangleObstacle)
            {
                shape = RectangleShape.Instance;
                color = DefaultValues.OBSTACLE_COLOR;
            }
            else if (element is AttractSpot || element is RepelSpot) color = DefaultValues.EXTRAS_COLOR;
            else if (element is Guidepost)
            {
                shape = TriangleShape.Instance;
                color = DefaultValues.EXTRAS_COLOR;
            }
            shape.Draw(Image, element.Position * Scale, element.Direction.Angle,
                color, color, Scaled(element.Width), Scaled(element.Height));
        }

        #endregion
    }
}

// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Core Library
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
using System.Xml.Serialization;
using Muragatte.Core;
using Muragatte.Core.Environment;

namespace Muragatte.IO
{
    public class XmlScene
    {
        #region Fields

        public Region Region = null;

        private SpawnSpot[] _spawn = null;
        private Element[] _stationary = null;

        #endregion

        #region Constructors

        public XmlScene() { }

        public XmlScene(Scene scene)
        {
            Region = scene.Region;
            _spawn = scene.SpawnSpots.ToArray();
            _stationary = scene.StationaryElements.ToArray();
        }

        #endregion

        #region Properties

        [XmlArrayItem(ElementName = "Point", Type = typeof(PointSpawnSpot)),
        XmlArrayItem(ElementName = "Ellipse", Type = typeof(EllipseSpawnSpot)),
        XmlArrayItem(ElementName = "Rectangle", Type = typeof(RectangleSpawnSpot))]
        public SpawnSpot[] SpawnSpots
        {
            get { return _spawn; }
            set
            {
                _spawn = value;
                XmlSpawnSpotReference.KnownSpawnSpots = _spawn;
            }
        }

        [XmlArrayItem(ElementName = "AreaGoal", Type = typeof(AreaGoal)),
        XmlArrayItem(ElementName = "PositionGoal", Type = typeof(PositionGoal)),
        XmlArrayItem(ElementName = "EllipseObstacle", Type = typeof(EllipseObstacle)),
        XmlArrayItem(ElementName = "RectangleObstacle", Type = typeof(RectangleObstacle)),
        XmlArrayItem(ElementName = "AttractSpot", Type = typeof(AttractSpot)),
        XmlArrayItem(ElementName = "RepelSpot", Type = typeof(RepelSpot)),
        XmlArrayItem(ElementName = "Guidepost", Type = typeof(Guidepost))]
        public Element[] StationaryElements
        {
            get { return _stationary; }
            set
            {
                _stationary = value;
                XmlGoalReference.KnownGoals = _stationary.OfType<Goal>();
            }
        }

        #endregion

        #region Methods

        public Scene ToScene()
        {
            return new Scene(Region, _spawn, _stationary);
        }

        public void ApplyToScene(Scene scene)
        {
            scene.Load(Region, _spawn, _stationary);
        }

        #endregion

        #region Operators

        public static implicit operator Scene(XmlScene x)
        {
            return x.ToScene();
        }

        public static implicit operator XmlScene(Scene s)
        {
            return new XmlScene(s);
        }

        #endregion
    }
}

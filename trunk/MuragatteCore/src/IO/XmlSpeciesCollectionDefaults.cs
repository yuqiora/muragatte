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
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.IO
{
    public class XmlSpeciesCollectionDefaults
    {
        #region Fields

        private string _sAgent = null;
        private string _sGoal = null;
        private string _sObstacle = null;
        private string _sCentroid = null;
        private string _sExtra = null;

        #endregion

        #region Constructors

        public XmlSpeciesCollectionDefaults() { }

        public XmlSpeciesCollectionDefaults(SpeciesCollection collection) :
            this(collection.DefaultForAgents, collection.DefaultForGoals, collection.DefaultForObstacles, collection.DefaultForCentroids, collection.DefaultForExtras) { }

        public XmlSpeciesCollectionDefaults(Species agent, Species goal, Species obstacle, Species centroid, Species extra)
        {
            _sAgent = GetName(agent);
            _sGoal = GetName(goal);
            _sObstacle = GetName(obstacle);
            _sCentroid = GetName(centroid);
            _sExtra = GetName(extra);
        }

        #endregion

        #region Properties

        public bool AnySpecified
        {
            get { return _sAgent != null || _sGoal != null || _sObstacle != null || _sCentroid != null || _sExtra != null; }
        }

        [XmlElement(IsNullable = false)]
        public string Agents
        {
            get { return _sAgent; }
            set { _sAgent = value; }
        }

        [XmlElement(IsNullable = false)]
        public string Goals
        {
            get { return _sGoal; }
            set { _sGoal = value; }
        }

        [XmlElement(IsNullable = false)]
        public string Obstacles
        {
            get { return _sObstacle; }
            set { _sObstacle = value; }
        }

        [XmlElement(IsNullable = false)]
        public string Centroids
        {
            get { return _sCentroid; }
            set { _sCentroid = value; }
        }

        [XmlElement(IsNullable = false)]
        public string Extras
        {
            get { return _sExtra; }
            set { _sExtra = value; }
        }

        #endregion

        #region Methods

        private string GetName(Species s)
        {
            return s == null ? null : s.FullName;
        }

        public void SetDefaultsToCollection(SpeciesCollection collection)
        {
            if (_sAgent != null) collection.DefaultForAgents = collection[_sAgent];
            if (_sGoal != null) collection.DefaultForGoals = collection[_sGoal];
            if (_sObstacle != null) collection.DefaultForObstacles = collection[_sObstacle];
            if (_sCentroid != null) collection.DefaultForCentroids = collection[_sCentroid];
            if (_sExtra != null) collection.DefaultForExtras = collection[_sExtra];
        }

        #endregion
    }
}

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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.IO;

namespace Muragatte.Core
{
    public class Scene
    {
        #region Fields

        private Region _region;
        private ObservableCollection<SpawnSpot> _spawn;
        private ObservableCollection<Element> _stationary;

        #endregion

        #region Constructors

        public Scene() : this(new Region(), null, null) { }

        public Scene(Region region) : this(region, null, null) { }

        public Scene(Region region, IEnumerable<SpawnSpot> spawnSpots) : this(region, spawnSpots, null) { }

        public Scene(Region region, IEnumerable<Element> stationaryElements) : this(region, null, stationaryElements) { }

        public Scene(Region region, IEnumerable<SpawnSpot> spawnSpots, IEnumerable<Element> stationaryElements)
        {
            _region = region;
            _spawn = GetSpawnSpotsOrDefault(spawnSpots);
            _stationary = stationaryElements == null ? new ObservableCollection<Element>() : new ObservableCollection<Element>(stationaryElements);
        }

        #endregion

        #region Properties

        public Region Region
        {
            get { return _region; }
        }

        [XmlIgnore]
        public ObservableCollection<SpawnSpot> SpawnSpots
        {
            get { return _spawn; }
        }

        [XmlIgnore]
        public ObservableCollection<Element> StationaryElements
        {
            get { return _stationary; }
        }

        #endregion

        #region Methods

        private ObservableCollection<SpawnSpot> GetSpawnSpotsOrDefault(IEnumerable<SpawnSpot> spawnSpots)
        {
            if (spawnSpots == null || spawnSpots.Count() == 0)
            {
                ObservableCollection<SpawnSpot> result = new ObservableCollection<SpawnSpot>();
                result.Add(new RectangleSpawnSpot("Anywhere", new Vector2(_region.Width / 2.0, _region.Height / 2.0), _region.Width * 0.9, _region.Height * 0.9));
                return result;
            }
            else
            {
                return new ObservableCollection<SpawnSpot>(spawnSpots);
            }
        }

        public IEnumerable<Element> ApplyStationaryElements(MultiAgentSystem model)
        {
            List<Element> result = new List<Element>();
            foreach (Element e in _stationary)
            {
                result.Add(e.CloneTo(model));
            }
            return result;
        }

        public void Load(Scene other)
        {
            Load(other._region, other._spawn, other._stationary);
        }

        public void Load(Region region, IEnumerable<SpawnSpot> spawnSpots, IEnumerable<Element> stationaryElements)
        {
            _region.Load(region);
            ReloadCollection(_spawn, spawnSpots);
            ReloadCollection(_stationary, stationaryElements);
        }

        private void ReloadCollection<T>(ObservableCollection<T> collection, IEnumerable<T> newContent)
        {
            collection.Clear();
            foreach (T x in newContent)
            {
                collection.Add(x);
            }
        }

        #endregion
    }
}

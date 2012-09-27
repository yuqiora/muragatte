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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.Random;

namespace Muragatte.Core
{
    public class MultiAgentSystem : INotifyPropertyChanged
    {
        #region Fields

        private int _iSteps = 0;
        private double _dTimePerStep;
        private IStorage _storage;
        private Region _region;
        private SpeciesCollection _species;
        private History _history = new History();
        private List<Group> _groups = new List<Group>();
        private RandomMT _random;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public MultiAgentSystem(IStorage storage, Region region, RandomMT random, double timePerStep = 1)
            : this(storage, region, new SpeciesCollection(true), random, timePerStep) { }

        public MultiAgentSystem(IStorage storage, Region region, SpeciesCollection species, RandomMT random, double timePerStep = 1)
        {
            _storage = storage;
            _region = region;
            _species = species;
            _random = random;
            _dTimePerStep = timePerStep;
        }

        #endregion

        #region Properties
        
        public int StepCount
        {
            get { return _iSteps; }
            protected set
            {
                _iSteps = value;
                NotifyPropertyChanged("StepCount");
            }
        }

        public double TimePerStep
        {
            get { return _dTimePerStep; }
            set { _dTimePerStep = value; }
        }

        public int Substeps
        {
            get { return (int)Math.Ceiling(1 / _dTimePerStep); }
        }

        public IStorage Elements
        {
            get { return _storage; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public SpeciesCollection Species
        {
            get { return _species; }
            //set { _species = value; }
        }

        public History History
        {
            get { return _history; }
        }

        public List<Group> Groups
        {
            get { return _groups; }
        }

        public RandomMT Random
        {
            get { return _random; }
        }
        
        #endregion

        #region Methods
        
        public void Clear()
        {
            StepCount = 0;
            _species.Clear();
            Environment.Species.ResetIDCounter();
            _storage.Clear();
            Element.ResetIDCounter();
            _history.Clear();
            _groups.Clear();
        }

        public void Reset()
        {
            StepCount = 0;
            if (_history.Count > 0)
            {
                foreach (Element e in _storage)
                {
                    e.LoadStatus(_history[0][e.ID]);
                }
                _history.Clear();
            }
            UpdateGroupsAndCentroids();
        }

        public void Initialize()
        {
            _storage.Initialize();
            CreateCentroids();
            HistoryRecord record = new HistoryRecord();
            foreach (Element e in _storage)
            {
                record.Add(e.ReportStatus());
            }
            _history.Add(record);
            //_history.Archive(_species.Values);
        }

        //will be removed
        public void Scatter()
        {
            IEnumerable<Agent> agents = _storage.Agents;
            foreach (Agent a in agents)
            {
                a.SetMovementInfo(
                    _random.UniformVector(0, _region.Width, 0, Region.Height),
                    _random.NormalizedVector());
            }
        }

        //will be removed
        public void GroupStart(double size)
        {
            IEnumerable<Agent> agents = _storage.Agents;
            Vector2 direction = _random.NormalizedVector();
            foreach (Agent a in agents)
            {
                Vector2 position = _random.Disk(new Vector2(_region.Width / 2, Region.Height / 2), size, size);
                a.SetMovementInfo(position, direction + _random.GaussAngle(5));
            }
        }

        public void Update()
        {
            foreach (Element e in _storage)
            {
                e.Update();
            }
            HistoryRecord record = new HistoryRecord();
            foreach (Element e in _storage)
            {
                if (!(e is Centroid))
                {
                    e.ConfirmUpdate();
                }
            }
            _storage.Update();
            UpdateGroupsAndCentroids();
            foreach (Element e in _storage)
            {
                record.Add(e.ReportStatus());
            }
            _history.Add(record);
            StepCount++;
        }

        public void UpdateGroupsAndCentroids()
        {
            foreach (Group g in _groups)
            {
                g.Clear();
            }
            _groups.Clear();
            foreach (Agent a in _storage.Agents)
            {
                if (a.Group == null)
                {
                    IEnumerable<Agent> members = a.GroupSearch();
                    if (members.Count() > 0)
                    {
                        _groups.Add(new Group(a, members));
                    }
                }
            }
            foreach (Centroid c in _storage.Centroids)
            {
                c.ConfirmUpdate();
            }
        }

        private void CreateCentroids()
        {
            List<Centroid> centroids = new List<Centroid>();
            int id = _storage.Count;
            foreach (Agent a in _storage.Agents)
            {
                a.CreateRepresentative(id);
                centroids.Add(a.Representative);
                id++;
            }
            _storage.Add(centroids);
            UpdateGroupsAndCentroids();
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class Scene
    {
        #region Fields

        private Region _region;
        private List<SpawnSpot> _spawn;
        private List<Element> _stationary;

        #endregion

        #region Constructors

        public Scene(Region region) : this(region, null, null) { }

        public Scene(Region region, IEnumerable<SpawnSpot> spawnSpots) : this(region, spawnSpots, null) { }

        public Scene(Region region, IEnumerable<Element> stationaryElements) : this(region, null, stationaryElements) { }

        public Scene(Region region, IEnumerable<SpawnSpot> spawnSpots, IEnumerable<Element> stationaryElements)
        {
            _region = region;
            _spawn = GetSpawnSpotsOrDefault(spawnSpots);
            _stationary = stationaryElements == null ? new List<Element>() : new List<Element>(stationaryElements);
        }

        #endregion

        #region Properties

        public Region Region
        {
            get { return _region; }
        }

        public List<SpawnSpot> SpawnSpots
        {
            get { return _spawn; }
        }

        public List<Element> StationaryElements
        {
            get { return _stationary; }
        }

        #endregion

        #region Methods

        private List<SpawnSpot> GetSpawnSpotsOrDefault(IEnumerable<SpawnSpot> spawnSpots)
        {
            if (spawnSpots == null || spawnSpots.Count() == 0)
            {
                List<SpawnSpot> result = new List<SpawnSpot>();
                result.Add(new RectangleSpawnSpot(new Vector2(_region.Width / 2.0, _region.Height / 2.0), _region.Width * 0.9, _region.Height * 0.9));
                return result;
            }
            else
            {
                return new List<SpawnSpot>(spawnSpots);
            }
        }

        #endregion
    }
}

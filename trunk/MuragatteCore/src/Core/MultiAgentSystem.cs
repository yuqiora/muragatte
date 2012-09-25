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

namespace Muragatte.Core
{
    public class MultiAgentSystem : INotifyPropertyChanged
    {
        #region Fields

        private int _iSteps = 0;
        private double _dTimePerStep = 1;
        private IStorage _storage = null;
        private Region _region = null;
        private SpeciesCollection _species = new SpeciesCollection();
        private History _history = new History();
        private List<Group> _groups = new List<Group>();

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public MultiAgentSystem(IStorage storage, Region region, double timePerStep = 1)
        {
            _storage = storage;
            _region = region;
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
        }

        public void Initialize()
        {
            _storage.Initialize();
            List<Centroid> centroids = new List<Centroid>();
            foreach (Agent a in _storage.Agents)
            {
                a.CreateRepresentative();
                centroids.Add(a.Representative);
            }
            _storage.Add(centroids);
            UpdateGroupsAndCentroids();
            HistoryRecord record = new HistoryRecord();
            foreach (Element e in _storage)
            {
                record.Add(e.ReportStatus());
            }
            _history.Add(record);
            //_history.Archive(_species.Values);
        }

        public void Scatter()
        {
            IEnumerable<Agent> agents = _storage.Agents;
            foreach (Agent a in agents)
            {
                a.SetMovementInfo(
                    Vector2.RandomUniform(_region.Width, _region.Height),
                    Vector2.RandomNormalized());
            }
        }

        public void GroupStart(double size)
        {
            Vector2 centre = Vector2.RandomUniform(_region.Width, _region.Height);
            IEnumerable<Agent> agents = _storage.Agents;
            Vector2 direction = Vector2.RandomNormalized();
            foreach (Agent a in agents)
            {
                double x;
                double y;
                double ss;
                RNGs.Ran2.Disk(out x, out y, out ss);
                Vector2 pos = new Vector2(x, y);
                pos *= size;
                pos += new Vector2(_region.Width / 2, Region.Height / 2);
                a.SetMovementInfo(pos, direction + Angle.Random(5));
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
                result.Add(new SpawnRectangle(new Vector2(_region.Width / 2.0, _region.Height / 2.0), _region.Width * 0.9, _region.Height * 0.9));
                return result;
            }
            else
            {
                return new List<SpawnSpot>(spawnSpots);
            }
        }

        #endregion
    }

    public abstract class Archetype
    {
        protected int _iCount;
        protected Species _species;

        public int Count
        {
            get { return _iCount; }
            set { _iCount = value; }
        }

        public Species Species
        {
            get { return _species; }
            set { _species = value; }
        }

        public abstract IEnumerable<Element> CreateItems();
    }
}

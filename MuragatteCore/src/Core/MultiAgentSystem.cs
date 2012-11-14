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

        private int _iInstance = 0;
        private int _iSteps = 0;
        private double _dTimePerStep;
        private IStorage _storage;
        private Region _region;
        private SpeciesCollection _species;
        private History _history = new History();
        private List<Group> _groups = new List<Group>();
        private List<Agent> _strays = new List<Agent>();
        private RandomMT _random;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public MultiAgentSystem(int instance, HistoryMode mode, IStorage storage, Region region, RandomMT random, double timePerStep = 1)
            : this(instance, mode, storage, region, new SpeciesCollection(true), random, timePerStep) { }

        public MultiAgentSystem(int instance, HistoryMode mode, IStorage storage, Region region, SpeciesCollection species, RandomMT random, double timePerStep = 1)
        {
            _iInstance = instance;
            _storage = storage;
            _region = region;
            _species = species;
            _random = random;
            _dTimePerStep = timePerStep;
            _history = new History(mode, Substeps);
        }

        public MultiAgentSystem(int instance, HistoryMode mode, IStorage storage, Scene scene, RandomMT random, double timePerStep = 1)
            : this(instance, mode, storage, scene, new SpeciesCollection(true), random, timePerStep) { }

        public MultiAgentSystem(int instance, HistoryMode mode, IStorage storage, Scene scene, SpeciesCollection species, RandomMT random, double timePerStep = 1)
            : this(instance, mode, storage, scene.Region, species, random, timePerStep)
        {
            _storage.Add(scene.ApplyStationaryElements(this));
        }

        #endregion

        #region Properties

        public int Instance
        {
            get { return _iInstance; }
        }

        public int StepCount
        {
            get { return _iSteps; }
            private set
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
            _storage.Clear();
            _history.Clear();
            _groups.Clear();
            _strays.Clear();
        }

        public void Reset()
        {
            StepCount = 0;
            if (_history.Count > 0)
            {
                LoadInitialElementStatus();
                _history.Clear();
            }
            UpdateGroupsAndCentroids();
        }

        private void LoadInitialElementStatus()
        {
            LoadElementStatus(0);
        }

        public void LoadCurrentElementStatus()
        {
            LoadElementStatus(_history.Last().Step);
        }

        private void LoadElementStatus(int step)
        {
            LoadElementStatus(_storage, step);
            LoadElementStatus(_storage.Centroids, step);
        }

        private void LoadElementStatus(IEnumerable<Element> items, int step)
        {
            foreach (Element e in items)
            {
                e.LoadStatus(_history[step][e.ID]);
            }
        }

        public void Initialize()
        {
            _storage.Initialize();
            CreateCentroids();
            ExpandHistory(0);
        }

        public void LoadedTo(int stepCount)
        {
            StepCount = stepCount;
        }

        private void ExpandHistory(int step)
        {
            if (_history.Mode != HistoryMode.NoSubsteps || step % Substeps == 0)
            {
                HistoryRecord record = new HistoryRecord(step);
                ReportStatus(record);
                _history.Add(record);
            }
        }

        private void ReportStatus(HistoryRecord record)
        {
            ReportStatus(_storage, record);
            ReportStatus(_storage.Centroids, record);
            record.GroupsAndStrays(_groups, _strays);
        }

        private void ReportStatus(IEnumerable<Element> items, HistoryRecord record)
        {
            foreach (Element e in items)
            {
                record.Add(e.ReportStatus());
            }
        }

        ////will be removed
        //public void Scatter()
        //{
        //    IEnumerable<Agent> agents = _storage.Agents;
        //    foreach (Agent a in agents)
        //    {
        //        a.SetMovementInfo(
        //            _random.UniformVector(0, _region.Width, 0, Region.Height),
        //            _random.NormalizedVector());
        //    }
        //}

        ////will be removed
        //public void GroupStart(double size)
        //{
        //    IEnumerable<Agent> agents = _storage.Agents;
        //    Vector2 direction = _random.NormalizedVector();
        //    foreach (Agent a in agents)
        //    {
        //        Vector2 position = _random.Disk(new Vector2(_region.Width / 2, Region.Height / 2), size, size);
        //        a.SetMovementInfo(position, direction + _random.GaussAngle(5));
        //    }
        //}

        public void Update()
        {
            foreach (Element e in _storage.Items)
            {
                e.Update();
            }
            foreach (Element e in _storage.Items)
            {
                e.ConfirmUpdate();
            }
            _storage.Update();
            StepCount++;
            UpdateGroupsAndCentroids();
            ExpandHistory(_iSteps);
        }

        private void UpdateGroupsAndCentroids()
        {
            if (_history.Mode != HistoryMode.NoSubsteps || _iSteps % Substeps == 0)
            {
                foreach (Centroid c in _storage.Centroids)
                {
                    c.Update();
                }
                _groups.Clear();
                _strays.Clear();
                foreach (Agent a in _storage.Agents)
                {
                    if (a.Group == null)
                    {
                        IEnumerable<Agent> members = a.GroupSearch();
                        if (members.Count() > 0)
                        {
                            _groups.Add(new Group(a, members));
                        }
                        else
                        {
                            _strays.Add(a);
                        }
                    }
                }
                foreach (Centroid c in _storage.Centroids)
                {
                    c.ConfirmUpdate();
                }
            }
        }

        private void CreateCentroids()
        {
            foreach (Agent a in _storage.Agents)
            {
                a.CreateRepresentative();
                _storage.Add(a.Representative);
            }
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
}

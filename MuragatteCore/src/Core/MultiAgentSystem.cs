﻿// ------------------------------------------------------------------------
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Common;
using Muragatte.Core.Storage;
using Muragatte.Core.Environment;

namespace Muragatte.Core
{
    public class MultiAgentSystem : INotifyPropertyChanged
    {
        #region Fields

        protected int _iCurrentStep = 0;
        protected int _iSteps = 0;
        protected double _dTimePerStep = 1;
        protected IStorage _storage = null;
        protected Region _region = null;
        protected SpeciesCollection _species = null;
        protected History _history = new History();
        protected List<Group> _groups = new List<Group>();

        #endregion

        #region Constructors

        public MultiAgentSystem(IStorage storage, Region region, double timePerStep = 1)
        {
            _storage = storage;
            _region = region;
            _dTimePerStep = timePerStep;
            _species = new SpeciesCollection();
        }

        #endregion

        #region Properties
        
        public int CurrentStep
        {
            get { return _iCurrentStep; }
            set { _iCurrentStep = value >= _iSteps ? _iSteps - 1 : value; }
        }

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
        
        #endregion

        #region Virtual Methods
        
        public virtual void Clear()
        {
            _iCurrentStep = 0;
            StepCount = 0;
            _species.Clear();
            Environment.Species.ResetIDCounter();
            _storage.Clear();
            Element.ResetIDCounter();
            _history.Clear();
        }

        public virtual void NextStep() { }

        public virtual void GoToStep(int i) { }

        public virtual void Initialize()
        {
            foreach (Agent a in _storage.Agents)
            {
                a.CreateRepresentative();
                _storage.Add(a.Representative);
            }
            UpdateGroupsAndCentroids(null);
            HistoryRecord record = new HistoryRecord();
            foreach (Element e in _storage)
            {
                record.Add(e.ReportStatus());
            }
            _history.Add(record);
            //_history.Archive(_species.Values);
        }

        public virtual void Scatter()
        {
            IEnumerable<Agent> agents = _storage.Agents;
            foreach (Agent a in agents)
            {
                a.SetMovementInfo(
                    Vector2.RandomUniform(_region.Width, _region.Height),
                    Vector2.RandomNormalized());
            }
        }

        public virtual void GroupStart(double size)
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

        public virtual void Update()
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
                    record.Add(e.ReportStatus());
                }
            }
            UpdateGroupsAndCentroids(record);
            _history.Add(record);
            _iCurrentStep = _iSteps;
            StepCount++;
        }

        public virtual void UpdateGroupsAndCentroids(HistoryRecord record)
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
            if (record == null)
            {
                foreach (Centroid c in _storage.Centroids)
                {
                    c.ConfirmUpdate();
                }
            }
            else
            {
                foreach (Centroid c in _storage.Centroids)
                {
                    c.ConfirmUpdate();
                    record.Add(c.ReportStatus());
                }
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

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
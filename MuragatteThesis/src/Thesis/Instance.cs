// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
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
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.Random;
using Muragatte.Thesis.Results;

namespace Muragatte.Thesis
{
    public class Instance
    {
        #region Fields

        private int _iNumber;
        private int _iLength;
        private MultiAgentSystem _mas;
        private bool _bComplete = false;
        private InstanceResults _results = null;
        private uint _uiSeed;
        private RandomMT _random;
        private List<ArchetypeOverviewInfo> _observedInfos = new List<ArchetypeOverviewInfo>();

        #endregion

        #region Constructors

        public Instance(int number, int length, double timePerStep, IStorage storage, Scene scene, IEnumerable<ObservedArchetype> archetypes, SpeciesCollection species, uint seed)
        {
            _iNumber = number;
            _iLength = length;
            _uiSeed = seed;
            _random = new RandomMT(_uiSeed);
            _mas = new MultiAgentSystem(storage.NewInstance(), scene, species, _random, timePerStep);
            AgentsFromArchetypes(scene.StationaryElements.Count, archetypes);
            _mas.Initialize();
        }

        public Instance(int number, int length, InstanceDefinition definition, uint seed)
            : this(number, length, definition.TimePerStep, definition.Storage, definition.Scene, definition.Archetypes, definition.Species, seed) { }

        #endregion

        #region Properties

        public int Number
        {
            get { return _iNumber; }
        }

        public int Length
        {
            get { return _iLength; }
        }

        public MultiAgentSystem Model
        {
            get { return _mas; }
        }

        public bool IsComplete
        {
            get { return _bComplete; }
        }

        public InstanceResults Results
        {
            get { return _results; }
        }

        public uint Seed
        {
            get { return _uiSeed; }
        }

        #endregion

        #region Methods

        public void Run()
        {
            if (!_bComplete)
            {
                for (int i = 0; i < _iLength; i++)
                {
                    _mas.Update();
                }
                ProcessResults();
                _bComplete = true;
            }
        }

        public void Update()
        {
            if (!_bComplete)
            {
                _mas.Update();
                if (_mas.StepCount == _iLength)
                {
                    ProcessResults();
                    _bComplete = true;
                }
            }
        }

        public void Reset()
        {
            _mas.Clear();
            _results = null;
            _bComplete = false;
        }

        private void AgentsFromArchetypes(int startID, IEnumerable<ObservedArchetype> archetypes)
        {
            foreach (ObservedArchetype a in archetypes)
            {
                _mas.Elements.Add(a.CreateAgents(startID, _mas));
                if (a.IsObserved) _observedInfos.Add(a.OverviewInfo);
                startID += a.Archetype.Count;
            }
        }

        private void ProcessResults()
        {
            _results = new InstanceResults(_iNumber, _mas.History, _mas.Substeps, _observedInfos);
        }

        #endregion
    }
}

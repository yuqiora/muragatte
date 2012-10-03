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

        #endregion

        #region Constructors

        public Instance(int number, int length, double timePerStep, Scene scene, IEnumerable<AgentArchetype> archetypes, SpeciesCollection species, uint seed)
        {
            _iNumber = number;
            _iLength = length;
            _uiSeed = seed;
            _random = new RandomMT(_uiSeed);
            //storage type fixed for now, might be selectable in the future
            _mas = new MultiAgentSystem(new SimpleBruteForceStorage(), scene.Region, species, _random, timePerStep);
            _mas.Elements.Add(scene.ApplyStationaryElements(_mas));
            AgentsFromArchetypes(scene.StationaryElements.Count, archetypes);
            _mas.Initialize();
        }

        public Instance(int number, int length, InstanceDefinition definition, uint seed)
            : this(number, length, definition.TimePerStep, definition.Scene, definition.Archetypes, definition.Species, seed) { }

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
                for (int i = _mas.StepCount; i < _iLength; i++)
                {
                    _mas.Update();
                }
                //results post-processing
                ProcessResults();
                _bComplete = true;
            }
        }

        private void AgentsFromArchetypes(int startID, IEnumerable<AgentArchetype> archetypes)
        {
            foreach (AgentArchetype a in archetypes)
            {
                _mas.Elements.Add(a.CreateAgents(startID, _mas));
                startID += a.Count;
            }
        }

        private void ProcessResults() { }

        #endregion
    }

    public class InstanceResults { }
}

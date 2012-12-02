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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using Muragatte.IO;

namespace Muragatte.Research
{
    public class InstanceDefinition : INotifyPropertyChanged
    {
        #region Fields

        private double _dTimePerStep = 1;
        private int _iLength = 100;
        private bool _bKeepSubsteps = false;
        private Scene _scene = null;
        private SpeciesCollection _species = null;
        private IStorage _storage = null;
        private ObservableCollection<ObservedArchetype> _archetypes = new ObservableCollection<ObservedArchetype>();

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public InstanceDefinition() { }

        public InstanceDefinition(double timePerStep, int length, bool keepSubsteps, Scene scene, SpeciesCollection species, IStorage storage, IEnumerable<ObservedArchetype> archetypes)
        {
            _dTimePerStep = timePerStep;
            _iLength = length;
            _bKeepSubsteps = keepSubsteps;
            _scene = scene;
            _species = species;
            _storage = storage;
            _archetypes = archetypes == null ? new ObservableCollection<ObservedArchetype>() : new ObservableCollection<ObservedArchetype>(archetypes);
        }

        #endregion

        #region Properties

        public double TimePerStep
        {
            get { return _dTimePerStep; }
            set
            {
                _dTimePerStep = value;
                NotifyPropertyChanged("TimePerStep");
            }
        }

        public int Length
        {
            get { return _iLength; }
            set
            {
                _iLength = value;
                NotifyPropertyChanged("Length");
            }
        }

        public bool KeepSubsteps
        {
            get { return _bKeepSubsteps; }
            set
            {
                _bKeepSubsteps = value;
                NotifyPropertyChanged("KeepSubsteps");
            }
        }

        public Scene Scene
        {
            get { return _scene; }
        }

        public IStorage Storage
        {
            get { return _storage; }
            set
            {
                _storage = value;
                NotifyPropertyChanged("Storage");
            }
        }

        public SpeciesCollection Species
        {
            get { return _species; }
        }

        public ObservableCollection<ObservedArchetype> Archetypes
        {
            get { return _archetypes; }
        }

        public int AgentCount
        {
            get
            {
                int count = 0;
                foreach (ObservedArchetype oa in _archetypes)
                {
                    count += oa.Archetype.Count;
                }
                return count;
            }
        }

        #endregion

        #region Methods

        public Instance CreateInstance(int number, uint seed)
        {
            return new Instance(number, _iLength, _dTimePerStep, _bKeepSubsteps, _storage, _scene, _archetypes, _species, seed);
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

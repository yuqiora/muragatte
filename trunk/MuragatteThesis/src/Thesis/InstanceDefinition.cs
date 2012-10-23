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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.Thesis
{
    public class InstanceDefinition : INotifyPropertyChanged
    {
        #region Fields

        private double _dTimePerStep;
        private int _iLength;
        private Scene _scene = null;
        private SpeciesCollection _species;
        private ObservableCollection<ObservedArchetype> _archetypes = new ObservableCollection<ObservedArchetype>();

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public InstanceDefinition(double timePerStep, int length, Scene scene, SpeciesCollection species, IEnumerable<ObservedArchetype> archetypes)
        {
            _dTimePerStep = timePerStep;
            _iLength = length;
            _scene = scene;
            _species = species;
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

        public Scene Scene
        {
            get { return _scene; }
        }

        public SpeciesCollection Species
        {
            get { return _species; }
        }

        public ObservableCollection<ObservedArchetype> Archetypes
        {
            get { return _archetypes; }
        }

        #endregion

        #region Methods

        public Instance CreateInstance(int number, uint seed)
        {
            return new Instance(number, _iLength, _dTimePerStep, _scene, _archetypes, _species, seed);
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

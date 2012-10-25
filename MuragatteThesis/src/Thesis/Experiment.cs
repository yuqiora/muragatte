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
using Muragatte.Random;
using Muragatte.Thesis.Results;
using Muragatte.Visual.Styles;

namespace Muragatte.Thesis
{
    public class Experiment : INotifyPropertyChanged
    {
        #region Fields

        private string _sName;
        private string _sPath;
        private int _iRepeatCount;
        private InstanceDefinition _definition;
        private List<Instance> _instances = new List<Instance>();
        private ObservableCollection<Style> _styles;
        private bool _bComplete = false;
        private bool _bCanceled = false;
        private ExperimentResults _results = null;
        private uint _uiSeed;
        private RandomMT _random;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Experiment(string name, string path, int repeat, InstanceDefinition definition, IEnumerable<Style> styles, uint seed)
        {
            _sName = name;
            _sPath = path;
            _iRepeatCount = repeat;
            _definition = definition;
            _styles = styles == null ? new ObservableCollection<Style>() : new ObservableCollection<Style>(styles);
            _uiSeed = seed;
            _random = new RandomMT(_uiSeed);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
            set
            {
                _sName = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Path
        {
            get { return _sPath; }
            set
            {
                _sPath = value;
                NotifyPropertyChanged("Path");
            }
        }

        public int RepeatCount
        {
            get { return _iRepeatCount; }
            set
            {
                _iRepeatCount = value;
                NotifyPropertyChanged("RepeatCount");
            }
        }

        public List<Instance> Instances
        {
            get { return _instances; }
        }

        public ObservableCollection<Style> Styles
        {
            get { return _styles; }
        }

        public bool IsComplete
        {
            get { return _bComplete; }
            private set
            {
                _bComplete = value;
                NotifyPropertyChanged("IsComplete");
            }
        }

        public bool IsCanceled
        {
            get { return _bCanceled; }
            private set
            {
                _bCanceled= value;
                NotifyPropertyChanged("IsCanceled");
            }
        }

        public uint Seed
        {
            get { return _uiSeed; }
            set
            {
                _uiSeed=value;
                NotifyPropertyChanged("Seed");
            }
        }

        public ExperimentResults Results
        {
            get { return _results; }
        }

        public InstanceDefinition Definition
        {
            get { return _definition; }
        }

        #endregion

        #region Methods

        public void Cancel()
        {
            IsCanceled = true;
        }

        public void Reset()
        {
            foreach (Instance i in _instances)
            {
                i.Reset();
            }
            _instances.Clear();
            _results = null;
            IsComplete = false;
            IsCanceled = false;
        }

        public void Run()
        {
            if (!_bComplete)
            {
                PreProcessing();
                for (int i = 0; i < _iRepeatCount; i++)
                {
                    _instances[i].Run();
                }
                PostProcessing();
            }
        }

        public void PreProcessing()
        {
            if (!_bComplete)
            {
                for (int i = 0; i < _iRepeatCount; i++)
                {
                    CreateNewInstance(i);
                }
            }
        }

        public void PostProcessing()
        {
            if (!_bComplete && !_bCanceled)
            {
                _results = new ExperimentResults(/*_iRepeatCount, _definition.Length, _definition.AgentCount, */_instances);
                IsComplete = true;
            }
        }

        private void CreateNewInstance(int number)
        {
            _instances.Add(_definition.CreateInstance(number, _random.UInt()));
        }

        //private void ProcessResults() { }

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

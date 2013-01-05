// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
//
// Copyright (C) 2012-2013  Jiří Vejmola.
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
using Muragatte.Research.Results;
using Muragatte.Visual.Styles;

namespace Muragatte.Research
{
    public enum ExperimentStatus
    {
        Loading,
        Ready,
        InProgress,
        Completed,
        Canceled
    }

    public class Experiment : INotifyPropertyChanged
    {
        #region Fields

        private string _sName = string.Empty;
        private int _iRepeatCount = 1;
        private InstanceDefinition _definition = null;
        private List<Instance> _instances = new List<Instance>();
        private ObservableCollection<Style> _styles = null;
        private ExperimentStatus _status = ExperimentStatus.Ready;
        private ExperimentResults _results = null;
        private uint _uiSeed = 0;
        private RandomMT _random = null;
        private ExperimentExtraSetting _extraSetting = new ExperimentExtraSetting();
        private BackgroundWorker _worker = new BackgroundWorker();

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Experiment()
        {
            InitializeWorker();
        }

        public Experiment(string name, int repeat, InstanceDefinition definition, IEnumerable<Style> styles, uint seed)
        {
            _sName = name;
            _iRepeatCount = repeat;
            _definition = definition;
            _styles = styles == null ? new ObservableCollection<Style>() : new ObservableCollection<Style>(styles);
            _uiSeed = seed;
            InitializeWorker();
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

        public int RepeatCount
        {
            get { return _iRepeatCount; }
            set
            {
                _iRepeatCount = value;
                NotifyPropertyChanged("RepeatCount");
            }
        }

        public uint Seed
        {
            get { return _uiSeed; }
            set
            {
                _uiSeed = value;
                NotifyPropertyChanged("Seed");
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

        public ExperimentStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                NotifyPropertyChanged("Status");
                NotifyPropertyChanged("IsComplete");
                NotifyPropertyChanged("CanRun");
            }
        }

        public bool IsComplete
        {
            get { return _status == ExperimentStatus.Completed; }
        }

        public bool CanRun
        {
            get { return _status == ExperimentStatus.Ready || _status == ExperimentStatus.Canceled; }
        }

        public ExperimentResults Results
        {
            get { return _results; }
        }

        public InstanceDefinition Definition
        {
            get { return _definition; }
        }

        public ExperimentExtraSetting ExtraSetting
        {
            get { return _extraSetting; }
        }

        public BackgroundWorker Worker
        {
            get { return _worker; }
        }

        #endregion

        #region Methods

        public void Cancel()
        {
            Status = ExperimentStatus.Canceled;
        }

        public void CancelAsync()
        {
            Status = ExperimentStatus.Canceled;
            _worker.CancelAsync();
        }

        public void Reset()
        {
            foreach (Instance i in _instances)
            {
                i.Reset();
            }
            _instances.Clear();
            _results = null;
            Status = ExperimentStatus.Ready;
        }

        public void Run()
        {
            if (_status == ExperimentStatus.Canceled) Reset();
            if (_status == ExperimentStatus.Ready)
            {
                PreProcessing();
                for (int i = 0; i < _iRepeatCount; i++)
                {
                    _instances[i].Run();
                }
                PostProcessing();
            }
        }

        public void RunAsync()
        {
            if (CanRun && !_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
        }

        public void PreProcessing()
        {
            if (_status == ExperimentStatus.Ready)
            {
                Status = ExperimentStatus.InProgress;
                _random = new RandomMT(_uiSeed);
                for (int i = 0; i < _iRepeatCount; i++)
                {
                    CreateNewInstance(i);
                }
            }
        }

        public void PostProcessing()
        {
            if (_status == ExperimentStatus.InProgress || _status == ExperimentStatus.Loading)
            {
                _results = new ExperimentResults(_instances);
                Status = ExperimentStatus.Completed;
            }
        }

        private void CreateNewInstance(int number)
        {
            _instances.Add(_definition.CreateInstance(number, _random.UInt()));
        }

        public IEnumerable<History> GetHistories()
        {
            List<History> histories = new List<History>();
            foreach (Instance i in _instances)
            {
                histories.Add(i.Model.History);
            }
            return histories;
        }

        public void StartLoading()
        {
            PreProcessing();
            foreach (Instance i in _instances)
            {
                i.Model.History.Clear();
            }
            Status = ExperimentStatus.Loading;
        }

        public void FinishLoading()
        {
            foreach (Instance i in _instances)
            {
                i.FinishLoading();
            }
            PostProcessing();
        }

        private void InitializeWorker()
        {
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_status == ExperimentStatus.Canceled) Reset();
            if (_status == ExperimentStatus.Ready)
            {
                ExperimentProgress progress = new ExperimentProgress(_iRepeatCount, _definition.Length);
                _worker.ReportProgress(0, progress);
                PreProcessing();
                for (int i = 0; i < _iRepeatCount; i++)
                {
                    if (_worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    _instances[i].RunAsync(_worker, progress);
                }
                PostProcessing();
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

    public class ExperimentExtraSetting : INotifyPropertyChanged
    {
        #region Fields

        private string _sPath = null;
        private bool _bAutoSave = true;
        private Ionic.Zlib.CompressionLevel _compression = Ionic.Zlib.CompressionLevel.Default;
        private bool _bLayeredSnapshot = true;
        private byte _lsAlpha = 128;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ExperimentExtraSetting() { }

        #endregion

        #region Properties

        public string Path
        {
            get { return _sPath; }
            set
            {
                _sPath = value;
                NotifyPropertyChanged("Path");
            }
        }

        public bool IsAutoSaved
        {
            get { return _bAutoSave; }
            set
            {
                _bAutoSave = value;
                NotifyPropertyChanged("IsAutoSaved");
            }
        }

        public Ionic.Zlib.CompressionLevel Compression
        {
            get { return _compression; }
            set
            {
                _compression = value;
                NotifyPropertyChanged("Compression");
            }
        }

        public bool TakeLayeredSnapshot
        {
            get { return _bLayeredSnapshot; }
            set
            {
                _bLayeredSnapshot = value;
                NotifyPropertyChanged("TakeLayeredSnapshot");
            }
        }

        public byte Alpha
        {
            get { return _lsAlpha; }
            set
            {
                _lsAlpha = value;
                NotifyPropertyChanged("Alpha");
            }
        }

        #endregion

        #region Methods

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

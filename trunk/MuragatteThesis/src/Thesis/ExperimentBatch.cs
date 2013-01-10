// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
//
// Copyright (C) 2013  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Muragatte.Core;
using Muragatte.Core.Storage;
using Muragatte.Random;
using Muragatte.Research;
using Muragatte.Research.IO;
using Muragatte.Visual.IO;
using Muragatte.Visual.Styles;

namespace Muragatte.Thesis
{
    public abstract class ExperimentBatch
    {
        #region Fields

        protected string _sPath = null;
        protected StreamWriter _writer = null;
        protected int _iCount = 1;
        protected int _iRuns = 1;
        protected int _iLength = 1;
        protected double _dTimePerStep = 0.1;
        protected ObservableCollection<Style> _styles = null;
        protected SpeciesCollection _species = null;
        protected Scene _scene = null;

        protected RandomMT _random = new RandomMT();
        protected BackgroundWorker _worker = new BackgroundWorker();
        protected CompletedExperimentArchiver _archiver = new CompletedExperimentArchiver();
        protected Queue<Experiment> _toSave = new Queue<Experiment>();
        //protected string _sTempExpName = null;
        protected SnapshotSaver _snapshots = new SnapshotSaver();

        protected readonly string _sPathCompleted;
        protected readonly string _sPathSettings;
        protected readonly string _sPathExperiments;
        protected readonly string _sPathSnapshots;

        #endregion

        #region Constructors

        public ExperimentBatch(string path, int count, int runs, int length, double timePerStep,
            ObservableCollection<Style> styles, SpeciesCollection species, Scene scene)
        {
            _sPath = path;
            _iCount = count;
            _iRuns = runs;
            _iLength = length;
            _dTimePerStep = timePerStep;
            _styles = styles;
            _species = species;
            _scene = scene;
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _sPathCompleted = Path.Combine(Path.GetDirectoryName(_sPath), "Completed");
            _sPathSettings = Path.Combine(Path.GetDirectoryName(_sPath), "Settings");
            _sPathExperiments = Path.Combine(Path.GetDirectoryName(_sPath), "Experiments");
            _sPathSnapshots = Path.Combine(Path.GetDirectoryName(_sPath), "Snapshots");
        }

        #endregion

        #region Properties

        public BackgroundWorker Worker
        {
            get { return _worker; }
        }

        public CompletedExperimentArchiver Archiver
        {
            get { return _archiver; }
        }

        public bool IsAllSaved
        {
            get { return _toSave.Count == 0; }
        }

        public int InQueueToSave
        {
            get { return _toSave.Count; }
        }

        #endregion

        #region Methods

        protected abstract void _worker_DoWork(object sender, DoWorkEventArgs e);

        public void RunAsync()
        {
            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
        }

        public void CancelAsync()
        {
            _worker.CancelAsync();
            _toSave.Clear();
        }

        //run + cancel methods
        //able to continue from incomplete set based on last entry in file

        //create experiment?

        protected void Save(Experiment e)
        {
            if (_archiver.Worker.IsBusy)
                _toSave.Enqueue(e);
            else
                SaveNext(e);
        }

        public void SaveNext()
        {
            if (_toSave.Count > 0)
            {
                SaveNext(_toSave.Dequeue());
            }
        }

        protected virtual void SaveNext(Experiment e)
        {
            //_sTempExpName = e.Name;
            _archiver.Save(e, e.ExtraSetting.Compression);
            if (_archiver.Worker.IsBusy) _archiver.Worker.ReportProgress(0, e.Name);
            TakeSnapshot(e);
        }

        protected virtual void PrepareWriter(bool withHeader)
        {
            _writer = new StreamWriter(_sPath, true);
            if (withHeader) WriteBatchResultsHeader();
        }

        protected abstract void WriteBatchResultsHeader();

        protected abstract void TakeSnapshot(Experiment e);

        #endregion
    }
}

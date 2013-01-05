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
using System.Linq;
using System.Text;
using Muragatte.Research;

namespace Muragatte.Thesis
{
    public class ExperimentBatchProgress : ExperimentProgress
    {
        #region Fields

        private string _sName = null;
        private int _iBatchSize = 1;
        private int _iExperiment = 0;

        #endregion

        #region Constructors

        public ExperimentBatchProgress(int batchSize, int repeat, int length) : this(batchSize, repeat, length, 0, 0, 0) { }

        public ExperimentBatchProgress(int batchSize, int repeat, int length, int experiment, int instance, int step)
            : base(repeat, length, instance, step)
        {
            _iBatchSize = batchSize;
            _iExperiment = experiment;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
            set { _sName = value; }
        }

        public int BatchSize
        {
            get { return _iBatchSize; }
        }

        public int Experiment
        {
            get { return _iExperiment; }
        }

        public double BatchPercent
        {
            get { return 100d * _iExperiment / _iBatchSize; }
        }

        #endregion

        #region Methods

        public void ResetAll()
        {
            Reset();
            _iExperiment = 0;
        }

        public void UpdateExperiment(int value)
        {
            _iExperiment = value;
            UpdateInstance(0);
        }

        #endregion
    }
}

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

namespace Muragatte.Thesis
{
    public class ExperimentProgress
    {
        #region Fields

        private int _iRepeat = 1;
        private int _iLength = 1;
        private int _iInstance = 0;
        private int _iStep = 0;
        private int _iTotalLength = 1;

        #endregion

        #region Constructors

        public ExperimentProgress(int repeat, int length) : this(repeat, length, 0, 0) { }

        public ExperimentProgress(int repeat, int length, int instance, int step)
        {
            _iRepeat = repeat;
            _iLength = length;
            _iTotalLength = _iRepeat * _iLength;
            _iInstance = instance;
            _iStep = step;
        }

        #endregion

        #region Properties

        public int Repeat
        {
            get { return _iRepeat; }
        }

        public int Length
        {
            get { return _iLength; }
        }

        public int Instance
        {
            get { return _iInstance; }
        }

        public int Step
        {
            get { return _iStep; }
        }

        public double InstancePercent
        {
            get { return 100d * _iStep / _iLength; }
        }

        public double ExperimentPercent
        {
            get { return 100d * (_iInstance * _iLength + _iStep) / _iTotalLength; }
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _iInstance = 0;
            _iStep = 0;
        }

        public void UpdateInstance(int value)
        {
            _iInstance = value;
            _iStep = 0;
        }

        public ExperimentProgress UpdateStep(int value)
        {
            _iStep = value;
            return this;
        }

        public ExperimentProgress Next()
        {
            _iStep++;
            return this;
        }

        #endregion
    }
}

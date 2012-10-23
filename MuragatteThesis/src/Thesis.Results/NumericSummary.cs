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

namespace Muragatte.Thesis.Results
{
    public class NumericSummary
    {
        #region Fields

        private int _minimum = int.MaxValue;
        private int _maximum = int.MinValue;
        private double _average = 0;
        private int _sum = 0;

        #endregion

        #region Constructors

        public NumericSummary() { }

        public NumericSummary(int min, int max, double avg, int sum)
        {
            _minimum = min;
            _maximum = max;
            _average = avg;
            _sum = sum;
        }

        #endregion

        #region Properties

        public int Minimum
        {
            get { return _minimum; }
        }

        public int Maximum
        {
            get { return _maximum; }
        }

        public double Average
        {
            get { return _average; }
        }

        public int Sum
        {
            get { return _sum; }
        }

        #endregion

        #region Methods

        public void UpdateMinMaxSum(int value)
        {
            if (value < _minimum) _minimum = value;
            if (value > _maximum) _maximum = value;
            _sum += value;
        }

        public void UpdateAverage(double count)
        {
            if (count > 0)
            {
                _average = _sum / count;
            }
        }

        #endregion
    }
}

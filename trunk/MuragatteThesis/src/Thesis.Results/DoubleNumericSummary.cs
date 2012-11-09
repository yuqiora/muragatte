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
    public class DoubleNumericSummary
    {
        #region Fields

        private double? _minimum = null;
        private double? _maximum = null;
        private double? _average = null;
        private double? _sum = null;
        private int _iInvalidUpdates = 0;

        #endregion

        #region Constructors

        public DoubleNumericSummary() { }

        public DoubleNumericSummary(double min, double max, double avg, double sum)
        {
            _minimum = min;
            _maximum = max;
            _average = avg;
            _sum = sum;
        }

        #endregion

        #region Properties

        public double? Minimum
        {
            get { return _minimum; }
        }

        public double? Maximum
        {
            get { return _maximum; }
        }

        public double? Average
        {
            get { return _average; }
        }

        public double? Sum
        {
            get { return _sum; }
        }

        #endregion

        #region Methods

        public void UpdateMinMaxSum(double? value)
        {
            if (value.HasValue)
            {
                if (!_minimum.HasValue || value.Value < _minimum.Value) _minimum = value.Value;
                if (!_maximum.HasValue || value.Value > _maximum.Value) _maximum = value.Value;
                if (_sum.HasValue)
                {
                    _sum += value;
                }
                else
                {
                    _sum = value.Value;
                }
            }
            else
            {
                _iInvalidUpdates++;
            }
        }

        public void UpdateAverage(double count)
        {
            count -= _iInvalidUpdates;
            if (count > 0 && _sum.HasValue)
            {
                _average = _sum / count;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", _minimum, _average, _maximum);
        }

        #endregion
    }
}

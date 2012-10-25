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

    public class ExtendedNumericSummary
    {
        #region Fields

        private NumericSummary _start = new NumericSummary();
        private NumericSummary _overall = new NumericSummary();
        private NumericSummary _end = new NumericSummary();

        #endregion

        #region Constructors

        public ExtendedNumericSummary() { }

        #endregion

        #region Properties

        public NumericSummary Start
        {
            get { return _start; }
        }

        public NumericSummary Overall
        {
            get { return _overall; }
        }

        public NumericSummary End
        {
            get { return _end; }
        }

        #endregion

        #region Methods

        public void UpdateStartMinMaxSum(int value)
        {
            _start.UpdateMinMaxSum(value);
        }

        public void UpdateOverallMinMaxSum(int value)
        {
            _overall.UpdateMinMaxSum(value);
        }

        public void UpdateEndMinMaxSum(int value)
        {
            _end.UpdateMinMaxSum(value);
        }

        public void UpdateMinMaxSum(int value)
        {
            UpdateMinMaxSum(value, value, value);
        }

        public void UpdateMinMaxSum(int startValue, int overallValue, int endValue)
        {
            UpdateStartMinMaxSum(startValue);
            UpdateOverallMinMaxSum(overallValue);
            UpdateEndMinMaxSum(endValue);
        }

        public void UpdateStartAverage(double count)
        {
            _start.UpdateAverage(count);
        }

        public void UpdateOverallAverage(double count)
        {
            _overall.UpdateAverage(count);
        }

        public void UpdateEndAverage(double count)
        {
            _end.UpdateAverage(count);
        }

        public void UpdateAverage(double count)
        {
            UpdateAverage(count, count, count);
        }

        public void UpdateAverage(double startCount, double overallCount, double endCount)
        {
            UpdateStartAverage(startCount);
            UpdateStartAverage(overallCount);
            UpdateStartAverage(endCount);
        }

        #endregion
    }

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

        #endregion
    }
}

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

        public void UpdateMinMaxSum(int startValue, int endValue)
        {
            UpdateStartMinMaxSum(startValue);
            UpdateEndMinMaxSum(endValue);
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
            UpdateOverallAverage(overallCount);
            UpdateEndAverage(endCount);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", _start, _overall, _end);
        }

        #endregion
    }
}

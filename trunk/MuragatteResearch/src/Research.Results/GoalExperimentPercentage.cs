// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
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
using Muragatte.Core.Environment;

namespace Muragatte.Research.Results
{
    public class GoalExperimentPercentage : GoalInstancePercentage
    {
        #region Fields

        private double _dStart = 0;
        private double _dEnd = 0;
        private int _iStartCount = 0;
        private int _iEndCount = 0;
        private int _iOverallCount = 0;

        #endregion

        #region Constructors

        public GoalExperimentPercentage() : this(null, 0, 0, 0) { }

        public GoalExperimentPercentage(Goal goal, double start, double overall, double end)
            : base(goal, overall)
        {
            StartPercent = start;
            EndPercent = end;
        }

        #endregion

        #region Properties

        public double StartPercent
        {
            get { return _dStart; }
            set { _dStart = InRange(value); }
        }

        public double EndPercent
        {
            get { return _dEnd; }
            set { _dEnd = InRange(value); }
        }

        public string StartPercentString
        {
            get { return CreatePercentString(_dStart); }
        }

        public string EndPercentString
        {
            get { return CreatePercentString(_dEnd); }
        }

        public bool NotZero
        {
            get { return _dPercent + _dEnd + _dStart > 0; }
        }

        #endregion

        #region Methods

        public void IncreaseStart()
        {
            _iStartCount++;
        }

        public void IncreaseEnd()
        {
            _iEndCount++;
        }

        public void IncreaseOverall()
        {
            _iOverallCount++;
        }

        public void ConvertCountToPercent(double subtotal, double total)
        {
            StartPercent = CountToPercent(_iStartCount, subtotal);
            EndPercent = CountToPercent(_iEndCount, subtotal);
            Percent = CountToPercent(_iOverallCount, total);
        }

        private double CountToPercent(int value, double count)
        {
            return 100d * value / count;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", GoalName, _dStart, _dPercent, _dEnd);
        }

        #endregion
    }
}

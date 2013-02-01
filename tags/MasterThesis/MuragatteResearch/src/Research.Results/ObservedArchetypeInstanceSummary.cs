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
    public class ObservedArchetypeInstanceSummary
    {
        #region Fields

        private List<ObservedArchetypeOverview> _details;
        private NumericSummary _inMainGroup = new NumericSummary();
        private NumericSummary _groups = new NumericSummary();
        private NumericSummary _stray = new NumericSummary();
        private NumericSummary _sharedGroupGoal = new NumericSummary();
        private NumericSummary _majorityGroupSize = new NumericSummary();
        private double _dInOneGroup;

        #endregion

        #region Constructors

        public ObservedArchetypeInstanceSummary(List<ObservedArchetypeOverview> details)
        {
            _details = details;
            int oneGroup = 0;
            foreach (ObservedArchetypeOverview o in _details)
            {
                _inMainGroup.UpdateMinMaxSum(o.InMainGroupCount);
                _groups.UpdateMinMaxSum(o.GroupCount);
                _stray.UpdateMinMaxSum(o.StrayCount);
                _sharedGroupGoal.UpdateMinMaxSum(o.SharedGroupGoalCount);
                _majorityGroupSize.UpdateMinMaxSum(o.MajorityGroupSize);
                if (o.AllInOneGroup) oneGroup++;
            }
            _dInOneGroup = 100d * oneGroup / _details.Count;
            UpdateSummaryAverage(_details.Count);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _details[0].Name; }
        }

        public IEnumerable<ObservedArchetypeOverview> Details
        {
            get { return _details; }
        }

        public int Count
        {
            get { return _details[0].Count; }
        }

        public int InMainGroupCountStart
        {
            get { return _details[0].InMainGroupCount; }
        }

        public int InMainGroupCountEnd
        {
            get { return _details.Last().InMainGroupCount; }
        }

        public NumericSummary InMainGroupCount
        {
            get { return _inMainGroup; }
        }

        public int GroupCountStart
        {
            get { return _details[0].GroupCount; }
        }

        public int GroupCountEnd
        {
            get { return _details.Last().GroupCount; }
        }

        public NumericSummary GroupCount
        {
            get { return _groups; }
        }

        public int StrayCountStart
        {
            get { return _details[0].StrayCount; }
        }

        public int StrayCountEnd
        {
            get { return _details.Last().StrayCount; }
        }

        public NumericSummary StrayCount
        {
            get { return _stray; }
        }

        public int SharedGroupGoalCountStart
        {
            get { return _details[0].SharedGroupGoalCount; }
        }

        public int SharedGroupGoalCountEnd
        {
            get { return _details.Last().SharedGroupGoalCount; }
        }

        public NumericSummary SharedGroupGoalCount
        {
            get { return _sharedGroupGoal; }
        }

        public int MajorityGroupSizeStart
        {
            get { return _details[0].MajorityGroupSize; }
        }

        public int MajorityGroupSizeEnd
        {
            get { return _details.Last().MajorityGroupSize; }
        }

        public NumericSummary MajorityGroupSize
        {
            get { return _majorityGroupSize; }
        }

        public bool AllInOneGroupStart
        {
            get { return _details[0].AllInOneGroup; }
        }

        public bool AllInOneGroupEnd
        {
            get { return _details.Last().AllInOneGroup; }
        }

        public double AllInOneGroup
        {
            get { return _dInOneGroup; }
        }

        public Goal Goal
        {
            get { return _details[0].Goal; }
        }

        public bool HasGoal
        {
            get { return _details[0].HasGoal; }
        }

        public double? GoalEndDistanceMinimum
        {
            get { return _details.Last().GoalDistanceMinimum; }
        }

        public double? GoalEndDistanceMaximum
        {
            get { return _details.Last().GoalDistanceMaximum; }
        }

        public double? GoalEndDistanceAverage
        {
            get { return _details.Last().GoalDistanceAverage; }
        }

        public double? GoalEndDistanceSum
        {
            get { return _details.Last().GoalDistanceSum; }
        }

        #endregion

        #region Methods

        private void UpdateSummaryAverage(double count)
        {
            _inMainGroup.UpdateAverage(count);
            _groups.UpdateAverage(count);
            _stray.UpdateAverage(count);
            _sharedGroupGoal.UpdateAverage(count);
            _majorityGroupSize.UpdateAverage(count);
        }

        #endregion
    }
}

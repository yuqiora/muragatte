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
using Muragatte.Core.Environment;

namespace Muragatte.Thesis.Results
{
    public class ObservedArchetypeExperimentSummary
    {
        #region Fields

        private List<ObservedArchetypeInstanceSummary> _details;
        private ExtendedNumericSummary _inMainGroup = new ExtendedNumericSummary();
        private ExtendedNumericSummary _groups = new ExtendedNumericSummary();
        private ExtendedNumericSummary _stray = new ExtendedNumericSummary();
        private ExtendedNumericSummary _sharedGroupGoal = new ExtendedNumericSummary();
        private ExtendedNumericSummary _majorityGroupSize = new ExtendedNumericSummary();
        private GoalExperimentPercentage _allInOneGroup = new GoalExperimentPercentage();
        private DoubleNumericSummary _goalEndDistanceMinimum = new DoubleNumericSummary();
        private DoubleNumericSummary _goalEndDistanceMaximum = new DoubleNumericSummary();
        private DoubleNumericSummary _goalEndDistanceAverage = new DoubleNumericSummary();

        #endregion

        #region Constructors

        public ObservedArchetypeExperimentSummary(List<ObservedArchetypeInstanceSummary> details)
        {
            _details = details;
            foreach (ObservedArchetypeInstanceSummary o in _details)
            {
                _inMainGroup.UpdateMinMaxSum(o.InMainGroupCountStart, o.InMainGroupCountEnd);
                _groups.UpdateMinMaxSum(o.GroupCountStart, o.GroupCountEnd);
                _stray.UpdateMinMaxSum(o.StrayCountStart, o.StrayCountEnd);
                _sharedGroupGoal.UpdateMinMaxSum(o.SharedGroupGoalCountStart, o.SharedGroupGoalCountEnd);
                _majorityGroupSize.UpdateMinMaxSum(o.MajorityGroupSizeStart, o.MajorityGroupSizeEnd);
                _goalEndDistanceMinimum.UpdateMinMaxSum(o.GoalEndDistanceMinimum);
                _goalEndDistanceMaximum.UpdateMinMaxSum(o.GoalEndDistanceMaximum);
                _goalEndDistanceAverage.UpdateMinMaxSum(o.GoalEndDistanceAverage);
                if (o.AllInOneGroupStart) _allInOneGroup.IncreaseStart();
                if (o.AllInOneGroupEnd) _allInOneGroup.IncreaseEnd();
                foreach (ObservedArchetypeOverview a in o.Details)
                {
                    _inMainGroup.UpdateOverallMinMaxSum(a.InMainGroupCount);
                    _groups.UpdateOverallMinMaxSum(a.GroupCount);
                    _stray.UpdateOverallMinMaxSum(a.StrayCount);
                    _sharedGroupGoal.UpdateOverallMinMaxSum(a.SharedGroupGoalCount);
                    _majorityGroupSize.UpdateOverallMinMaxSum(a.MajorityGroupSize);
                    if (a.AllInOneGroup) _allInOneGroup.IncreaseOverall();
                }
            }
            double subtotal = _details.Count;
            double total = _details.Count * _details[0].Details.Count();
            _inMainGroup.UpdateAverage(subtotal, total, subtotal);
            _groups.UpdateAverage(subtotal, total, subtotal);
            _stray.UpdateAverage(subtotal, total, subtotal);
            _sharedGroupGoal.UpdateAverage(subtotal, total, subtotal);
            _majorityGroupSize.UpdateAverage(subtotal, total, subtotal);
            _allInOneGroup.ConvertCountToPercent(subtotal, total);
            _goalEndDistanceMinimum.UpdateAverage(subtotal);
            _goalEndDistanceMaximum.UpdateAverage(subtotal);
            _goalEndDistanceAverage.UpdateAverage(subtotal);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _details[0].Name; }
        }

        public int Count
        {
            get { return _details[0].Count; }
        }

        public IEnumerable<ObservedArchetypeInstanceSummary> Details
        {
            get { return _details; }
        }

        public ExtendedNumericSummary InMainGroupCount
        {
            get { return _inMainGroup; }
        }

        public ExtendedNumericSummary GroupCount
        {
            get { return _groups; }
        }

        public ExtendedNumericSummary StrayCount
        {
            get { return _stray; }
        }

        public ExtendedNumericSummary SharedGroupGoalCount
        {
            get { return _sharedGroupGoal; }
        }

        public ExtendedNumericSummary MajorityGroupSize
        {
            get { return _majorityGroupSize; }
        }

        public Goal Goal
        {
            get { return _details[0].Goal; }
        }

        public bool HasGoal
        {
            get { return _details[0].HasGoal; }
        }

        public double AllInOneGroupStart
        {
            get { return _allInOneGroup.StartPercent; }
        }

        public double AllInOneGroupEnd
        {
            get { return _allInOneGroup.EndPercent; }
        }

        public double AllInOneGroup
        {
            get { return _allInOneGroup.Percent; }
        }

        public DoubleNumericSummary GoalEndDistanceMinimum
        {
            get { return _goalEndDistanceMinimum; }
        }

        public DoubleNumericSummary GoalEndDistanceMaximum
        {
            get { return _goalEndDistanceMaximum; }
        }

        public DoubleNumericSummary GoalEndDistanceAverage
        {
            get { return _goalEndDistanceAverage; }
        }

        #endregion
    }
}

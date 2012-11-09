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
    public class ExperimentResults
    {
        #region Fields

        private List<InstanceResults> _instances = new List<InstanceResults>();
        private ExtendedNumericSummary _groupCount = new ExtendedNumericSummary();
        private ExtendedNumericSummary _strayCount = new ExtendedNumericSummary();
        private ExtendedNumericSummary _strayWanderCount = new ExtendedNumericSummary();
        private ExtendedNumericSummary _strayGoalCount = new ExtendedNumericSummary();
        private ExtendedNumericSummary _mainGroupSize = new ExtendedNumericSummary();
        private List<GoalExperimentPercentage> _mainGroupGoalPercentage = new List<GoalExperimentPercentage>();
        private double _dMainGroupNoEndGoalPercent = 0;
        private DoubleNumericSummary _mainGroupGoalEndDistanceMinimum = new DoubleNumericSummary();
        private DoubleNumericSummary _mainGroupGoalEndDistanceMaximum = new DoubleNumericSummary();
        private DoubleNumericSummary _mainGroupGoalEndDistanceAverage = new DoubleNumericSummary();
        private DoubleNumericSummary _mainGroupGoalEndDistanceCentroid = new DoubleNumericSummary();
        List<ObservedArchetypeExperimentSummary> _observed = new List<ObservedArchetypeExperimentSummary>();

        #endregion

        #region Constructors

        public ExperimentResults(IEnumerable<Instance> instances)
        {
            foreach (Instance i in instances)
            {
                _instances.Add(i.Results);
            }
            InstancesSummary();
            ObservedSummary(_instances[0].Observed.Count());
        }

        #endregion

        #region Properties

        public IEnumerable<InstanceResults> InstanceDetails
        {
            get { return _instances; }
        }

        public ExtendedNumericSummary GroupCount
        {
            get { return _groupCount; }
        }

        public ExtendedNumericSummary StrayCount
        {
            get { return _strayCount; }
        }

        public ExtendedNumericSummary StrayWanderCount
        {
            get { return _strayWanderCount; }
        }

        public ExtendedNumericSummary StrayGoalCount
        {
            get { return _strayGoalCount; }
        }

        public ExtendedNumericSummary MainGroupSize
        {
            get { return _mainGroupSize; }
        }

        public IEnumerable<GoalExperimentPercentage> MainGroupGoalPercentage
        {
            get { return _mainGroupGoalPercentage; }
        }

        public double MainGroupNoEndGoalPercent
        {
            get { return _dMainGroupNoEndGoalPercent; }
        }

        public DoubleNumericSummary MainGroupGoalEndDistanceMinimum
        {
            get { return _mainGroupGoalEndDistanceMinimum; }
        }

        public DoubleNumericSummary MainGroupGoalEndDistanceMaximum
        {
            get { return _mainGroupGoalEndDistanceMaximum; }
        }

        public DoubleNumericSummary MainGroupGoalEndDistanceAverage
        {
            get { return _mainGroupGoalEndDistanceAverage; }
        }

        public DoubleNumericSummary MainGroupGoalEndDistanceCentroid
        {
            get { return _mainGroupGoalEndDistanceCentroid; }
        }

        public IEnumerable<ObservedArchetypeExperimentSummary> Observed
        {
            get { return _observed; }
        }

        public bool IsEndGoalDefined
        {
            get { return _dMainGroupNoEndGoalPercent < 100; }
        }

        public bool HasAnyObserved
        {
            get { return _observed.Count > 0; }
        }

        #endregion

        #region Methods

        private void InstancesSummary()
        {
            GoalExperimentPercentage noGoal = new GoalExperimentPercentage();
            Dictionary<Goal, GoalExperimentPercentage> goals = new Dictionary<Goal, GoalExperimentPercentage>();
            int countNoEndGoal = 0;
            foreach (InstanceResults ir in _instances)
            {
                _groupCount.UpdateMinMaxSum(ir.GroupCountStart, ir.GroupCountEnd);
                _strayCount.UpdateMinMaxSum(ir.StrayCountStart, ir.StrayCountEnd);
                _strayWanderCount.UpdateMinMaxSum(ir.StrayWanderCountStart, ir.StrayWanderCountEnd);
                _strayGoalCount.UpdateMinMaxSum(ir.StrayGoalCountStart, ir.StrayGoalCountEnd);
                _mainGroupSize.UpdateMinMaxSum(ir.MainGroupSizeStart, ir.MainGroupSizeEnd);
                _mainGroupGoalEndDistanceMinimum.UpdateMinMaxSum(ir.MainGroupGoalEndDistanceMinimum);
                _mainGroupGoalEndDistanceMaximum.UpdateMinMaxSum(ir.MainGroupGoalEndDistanceMaximum);
                _mainGroupGoalEndDistanceAverage.UpdateMinMaxSum(ir.MainGroupGoalEndDistanceAverage);
                _mainGroupGoalEndDistanceCentroid.UpdateMinMaxSum(ir.MainGroupGoalEndDistanceCentroid);
                if (ir.MainGroupGoalEnd == null) countNoEndGoal++;
                foreach (StepOverview so in ir.StepDetails)
                {
                    _groupCount.UpdateOverallMinMaxSum(so.GroupCount);
                    _strayCount.UpdateOverallMinMaxSum(so.StrayCount);
                    _strayWanderCount.UpdateOverallMinMaxSum(so.StrayWanderCount);
                    _strayGoalCount.UpdateOverallMinMaxSum(so.StrayGoalCount);
                    _mainGroupSize.UpdateOverallMinMaxSum(so.MainGroup.Size);
                    if (so.MainGroup.HasGoal)
                    {
                        if (!goals.ContainsKey(so.MainGroup.MajorityGoal))
                        {
                            goals.Add(so.MainGroup.MajorityGoal, new GoalExperimentPercentage(so.MainGroup.MajorityGoal, 0, 0, 0));
                        }
                        goals[so.MainGroup.MajorityGoal].IncreaseOverall();
                    }
                    else
                    {
                        noGoal.IncreaseOverall();
                    }
                }
                if (ir.MainGroupGoalStart == null)
                    noGoal.IncreaseStart();
                else
                    goals[ir.MainGroupGoalStart].IncreaseStart();
                if (ir.MainGroupGoalEnd == null)
                    noGoal.IncreaseEnd();
                else
                    goals[ir.MainGroupGoalEnd].IncreaseEnd();
            }
            double countStartEnd = _instances.Count;
            double countOverall = _instances.Count * _instances[0].StepDetails.Count();
            _groupCount.UpdateAverage(countStartEnd, countOverall, countStartEnd);
            _strayCount.UpdateAverage(countStartEnd, countOverall, countStartEnd);
            _strayWanderCount.UpdateAverage(countStartEnd, countOverall, countStartEnd);
            _strayGoalCount.UpdateAverage(countStartEnd, countOverall, countStartEnd);
            _mainGroupSize.UpdateAverage(countStartEnd, countOverall, countStartEnd);
            _mainGroupGoalEndDistanceMinimum.UpdateAverage(countStartEnd);
            _mainGroupGoalEndDistanceMaximum.UpdateAverage(countStartEnd);
            _mainGroupGoalEndDistanceAverage.UpdateAverage(countStartEnd);
            _mainGroupGoalEndDistanceCentroid.UpdateAverage(countStartEnd);
            _dMainGroupNoEndGoalPercent = 100d * countNoEndGoal / countStartEnd;
            GoalPercentageConvertAndAdd(noGoal, countStartEnd, countOverall);
            foreach (GoalExperimentPercentage g in goals.Values)
            {
                GoalPercentageConvertAndAdd(g, countStartEnd, countOverall);
            }
        }

        private void GoalPercentageConvertAndAdd(GoalExperimentPercentage gep, double subtotal, double total)
        {
            gep.ConvertCountToPercent(subtotal, total);
            if (gep.NotZero) _mainGroupGoalPercentage.Add(gep);
        }

        private void ObservedSummary(int count)
        {
            for (int o = 0; o < count; o++)
            {
                List<ObservedArchetypeInstanceSummary> details = new List<ObservedArchetypeInstanceSummary>();
                for (int i = 0; i < _instances.Count; i++)
                {
                    details.Add(_instances[i].Observed[o]);
                }
                _observed.Add(new ObservedArchetypeExperimentSummary(details));
            }
        }

        #endregion
    }
}

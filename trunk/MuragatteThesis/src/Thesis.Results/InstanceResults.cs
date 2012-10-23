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
using Muragatte.Core.Storage;

namespace Muragatte.Thesis.Results
{
    public class InstanceResults
    {
        #region Fields

        private int _iInstance;
        private List<StepOverview> _stepDetails = new List<StepOverview>();
        private NumericSummary _groupCount = new NumericSummary();
        private NumericSummary _strayWanderCount = new NumericSummary();
        private NumericSummary _strayGoalCount = new NumericSummary();
        private NumericSummary _mainGroupSize = new NumericSummary();
        private List<Tuple<Goal, double>> _mainGroupGoalPercentage = new List<Tuple<Goal, double>>();
        private List<ObservedArchetypeInstanceSummary> _observed = new List<ObservedArchetypeInstanceSummary>();

        #endregion

        #region Constructors

        public InstanceResults(int number, History history, int substeps, List<ArchetypeOverviewInfo> observedInfo)
        {
            _iInstance = number;
            CreateStepDetails(history, substeps, observedInfo);
            StepsSummary();
            ObservedSummary(observedInfo.Count);
        }

        #endregion

        #region Properties

        public int InstanceNumber
        {
            get { return _iInstance; }
        }

        public int GroupCountStart
        {
            get { return _stepDetails[0].GroupCount; }
        }

        public int GroupCountEnd
        {
            get { return _stepDetails.Last().GroupCount; }
        }

        public NumericSummary GroupCount
        {
            get { return _groupCount; }
        }

        public int StrayWanderCountStart
        {
            get { return _stepDetails[0].StrayWanderCount; }
        }

        public int StrayWanderCountEnd
        {
            get { return _stepDetails.Last().StrayWanderCount; }
        }

        public NumericSummary StrayWanderCount
        {
            get { return _strayWanderCount; }
        }

        public int StrayGoalCountStart
        {
            get { return _stepDetails[0].StrayGoalCount; }
        }

        public int StrayGoalCountEnd
        {
            get { return _stepDetails.Last().StrayGoalCount; }
        }

        public NumericSummary StrayGoalCount
        {
            get { return _strayGoalCount; }
        }

        public IEnumerable<StepOverview> StepDetails
        {
            get { return _stepDetails; }
        }

        public int MainGroupSizeStart
        {
            get { return _stepDetails[0].MainGroup.Size; }
        }

        public int MainGroupSizeEnd
        {
            get { return _stepDetails.Last().MainGroup.Size; }
        }

        public NumericSummary MainGroupSize
        {
            get { return _mainGroupSize; }
        }

        public Goal MainGroupGoalStart
        {
            get { return _stepDetails[0].MainGroup.MajorityGoal; }
        }

        public Goal MainGroupGoalEnd
        {
            get { return _stepDetails.Last().MainGroup.MajorityGoal; }
        }

        public List<Tuple<Goal, double>> MainGroupGoalPercentage
        {
            get { return _mainGroupGoalPercentage; }
        }

        #endregion

        #region Methods

        private void CreateStepDetails(History history, int substeps, List<ArchetypeOverviewInfo> observedInfo)
        {
            for (int i = 0; i < history.Count; i += substeps)
            {
                AddStepOverview(i, history, observedInfo);
            }
            int last = history.Count - 1;
            if (_stepDetails.Last().Step != last) AddStepOverview(last, history, observedInfo);
        }

        private void AddStepOverview(int step, History history, List<ArchetypeOverviewInfo> observedInfo)
        {
            _stepDetails.Add(new StepOverview(step, history[step], observedInfo));
        }

        private void StepsSummary()
        {
            int noGoal = 0;
            Dictionary<Goal, int> goals = new Dictionary<Goal, int>();
            foreach (StepOverview so in _stepDetails)
            {
                _groupCount.UpdateMinMaxSum(so.GroupCount);
                _strayWanderCount.UpdateMinMaxSum(so.StrayWanderCount);
                _strayGoalCount.UpdateMinMaxSum(so.StrayGoalCount);
                _mainGroupSize.UpdateMinMaxSum(so.MainGroup.Size);
                if (!so.MainGroup.HasGoal)
                {
                    noGoal++;
                }
                else
                {
                    AnotherGoal(goals, so.MainGroup.MajorityGoal);
                }
            }
            GoalPercentage(noGoal, goals, _stepDetails.Count);
            UpdateSummaryAverage(_stepDetails.Count);
        }

        private void ObservedSummary(int count)
        {
            for (int o = 0; o < count; o++)
            {
                List<ObservedArchetypeOverview> details = new List<ObservedArchetypeOverview>();
                for (int s = 0; s < _stepDetails.Count; s++)
                {
                    details.Add(_stepDetails[s].Observed[o]);
                }
                _observed.Add(new ObservedArchetypeInstanceSummary(details));
            }
        }

        private void AnotherGoal(Dictionary<Goal, int> goals, Goal goal)
        {
            if (goals.ContainsKey(goal))
            {
                goals[goal]++;
            }
            else
            {
                goals.Add(goal, 1);
            }
        }

        private void GoalPercentage(int noGoal, Dictionary<Goal, int> goals, int count)
        {
            if (noGoal > 0) _mainGroupGoalPercentage.Add(new Tuple<Goal, double>(null, 100d * noGoal / count));
            foreach (KeyValuePair<Goal, int> g in goals)
            {
                _mainGroupGoalPercentage.Add(new Tuple<Goal, double>(g.Key, 100d * g.Value / count));
            }
        }

        private void UpdateSummaryAverage(double count)
        {
            _groupCount.UpdateAverage(count);
            _strayWanderCount.UpdateAverage(count);
            _strayGoalCount.UpdateAverage(count);
            _mainGroupSize.UpdateAverage(count);
        }

        #endregion
    }
}

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
        private NumericSummary _strayCount = new NumericSummary();
        private NumericSummary _strayWanderCount = new NumericSummary();
        private NumericSummary _strayGoalCount = new NumericSummary();
        private NumericSummary _mainGroupSize = new NumericSummary();
        private List<GoalInstancePercentage> _mainGroupGoalPercentage = new List<GoalInstancePercentage>();
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

        public int StrayCountStart
        {
            get { return _stepDetails[0].StrayCount; }
        }

        public int StrayCountEnd
        {
            get { return _stepDetails.Last().StrayCount; }
        }

        public NumericSummary StrayCount
        {
            get { return _strayCount; }
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

        public IEnumerable<GoalInstancePercentage> MainGroupGoalPercentage
        {
            get { return _mainGroupGoalPercentage; }
        }

        public List<ObservedArchetypeInstanceSummary> Observed
        {
            get { return _observed; }
        }

        public double? MainGroupGoalEndDistanceMinimum
        {
            get { return _stepDetails.Last().MainGroup.MinimumDistance; }
        }

        public double? MainGroupGoalEndDistanceMaximum
        {
            get { return _stepDetails.Last().MainGroup.MaximumDistance; }
        }

        public double? MainGroupGoalEndDistanceAverage
        {
            get { return _stepDetails.Last().MainGroup.AverageDistance; }
        }

        public double? MainGroupGoalEndDistanceCentroid
        {
            get { return _stepDetails.Last().MainGroup.CentroidDistance; }
        }

        public double? MainGroupGoalEndDistanceSum
        {
            get { return _stepDetails.Last().MainGroup.DistanceSum; }
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
                _strayCount.UpdateMinMaxSum(so.StrayCount);
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
            if (noGoal > 0) _mainGroupGoalPercentage.Add(new GoalInstancePercentage(null, 100d * noGoal / count));
            foreach (KeyValuePair<Goal, int> g in goals)
            {
                _mainGroupGoalPercentage.Add(new GoalInstancePercentage(g.Key, 100d * g.Value / count));
            }
        }

        private void UpdateSummaryAverage(double count)
        {
            _groupCount.UpdateAverage(count);
            _strayCount.UpdateAverage(count);
            _strayWanderCount.UpdateAverage(count);
            _strayGoalCount.UpdateAverage(count);
            _mainGroupSize.UpdateAverage(count);
        }

        #endregion
    }

    public class GoalInstancePercentage
    {
        #region Fields

        protected Goal _goal;
        protected double _dPercent;

        #endregion

        #region Constructors

        public GoalInstancePercentage(Goal goal, double percent)
        {
            _goal = goal;
            Percent = percent;
        }

        #endregion

        #region Properties

        public Goal Goal
        {
            get { return _goal; }
        }

        public double Percent
        {
            get { return _dPercent; }
            set { _dPercent = InRange(value); }
        }

        public string GoalName
        {
            get { return _goal == null ? "None" : _goal.Name; }
        }

        public string PercentString
        {
            get { return CreatePercentString(_dPercent); }
        }

        #endregion

        #region Methods

        protected string CreatePercentString(double value)
        {
            return string.Format("{0:F2}%", value);
        }

        protected double InRange(double value)
        {
            if (_dPercent < 0) return 0;
            if (_dPercent > 100) return 100;
            return value;
        }

        #endregion
    }

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
            set { _dStart= InRange(value); }
        }

        public double EndPercent
        {
            get { return _dEnd; }
            set { _dEnd= InRange(value); }
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

        #endregion
    }
}

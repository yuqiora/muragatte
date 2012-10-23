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
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;

namespace Muragatte.Thesis.Results
{
    public class ObservedArchetypeOverview
    {
        #region Fields

        private string _sName;
        private List<int> _memberIDs;
        private int _iInMainGroup = 0;
        private int _iStray = 0;
        private int _iSharedGroupGoal = 0;
        private int _iMajorityGroupSize = 0;
        private int _iGroupCount;
        private bool _bInOneGroup = true;
        private Goal _goal;
        private double? _dMinDistance = null;
        private double? _dMaxDistance = null;
        private double? _dAvgDistance = null;
        private double? _dSumDistance = null;

        #endregion

        #region Constructors

        public ObservedArchetypeOverview(HistoryRecord record, ArchetypeOverviewInfo overviewInfo, List<GroupOverview> groupDetails, int mainGroup)
        {
            _sName = overviewInfo.Name;
            _memberIDs = overviewInfo.Members;
            _goal = overviewInfo.Goal;
            if (_memberIDs.Count == 1)
            {
                OneMember(record[_memberIDs[0]], groupDetails, mainGroup);
            }
            else
            {
                ManyMembers(record, groupDetails, mainGroup);
            }
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
        }

        public int Count
        {
            get { return _memberIDs.Count; }
        }

        public int InMainGroupCount
        {
            get { return _iInMainGroup; }
        }

        public int SharedGroupGoalCount
        {
            get { return _iSharedGroupGoal; }
        }

        public int StrayCount
        {
            get { return _iStray; }
        }

        public int MajorityGroupSize
        {
            get { return _iMajorityGroupSize; }
        }

        public int GroupCount
        {
            get { return _iGroupCount; }
        }

        public bool AllInOneGroup
        {
            get { return _bInOneGroup; }
        }

        public bool IsGoalSpecified
        {
            get { return _goal != null; }
        }

        public Goal Goal
        {
            get { return _goal; }
        }

        public double? GoalDistanceMinimum
        {
            get { return _dMinDistance; }
        }

        public double? GoalDistanceMaximum
        {
            get { return _dMaxDistance; }
        }

        public double? GoalDistanceAverage
        {
            get { return _dAvgDistance; }
        }

        public double? GoalDistanceSum
        {
            get { return _dSumDistance; }
        }

        #endregion

        #region Methods

        private void OneMember(ElementStatus es, List<GroupOverview> groupDetails, int mainGroup)
        {
            if (es.GroupID < 0)
            {
                _iStray = 1;
                _iSharedGroupGoal = 1;
                _iMajorityGroupSize = 1;
                _iGroupCount = 0;
            }
            else
            {
                if (es.GroupID == mainGroup) _iInMainGroup = 1;
                GroupOverview go = groupDetails.Find(g => g.ID == es.GroupID);
                _iSharedGroupGoal = _goal == null || _goal == go.MajorityGoal ? 1 : 0;
                _iMajorityGroupSize = go.Size;
                _iGroupCount = 1;
            }
            if (_goal != null)
            {
                _dMinDistance = Vector2.Distance(es.Position, _goal.Position);
                _dMaxDistance = _dMinDistance;
                _dAvgDistance = _dMinDistance;
                _dSumDistance = _dMinDistance;
            }
        }

        private void ManyMembers(HistoryRecord record, List<GroupOverview> groupDetails, int mainGroup)
        {
            Dictionary<int, int> groups = new Dictionary<int, int>();
            foreach (int i in _memberIDs)
            {
                ElementStatus es = record[i];
                if (es.GroupID < 0)
                {
                    _iStray++;
                    _iSharedGroupGoal++;
                }
                else
                {
                    if (es.GroupID == mainGroup) _iInMainGroup++;
                    if (_goal == null || _goal == groupDetails.Find(g => g.ID == es.GroupID).MajorityGoal) _iSharedGroupGoal++;
                    if (groups.ContainsKey(es.GroupID))
                    {
                        groups[es.GroupID]++;
                    }
                    else
                    {
                        groups.Add(es.GroupID, 1);
                    }
                }
            }
            _iGroupCount = groups.Count;
            _bInOneGroup = groups.Count == 1;
            KeyValuePair<int, int> majority = new KeyValuePair<int, int>(-1, int.MinValue);
            foreach (KeyValuePair<int, int> g in groups)
            {
                if (g.Value > majority.Value) majority = g;
            }
            if (majority.Key >= 0) _iMajorityGroupSize = groupDetails.Find(go => go.ID == majority.Key).Size;
            Distances(record);
        }

        private void Distances(HistoryRecord record)
        {
            if (_goal != null)
            {
                _dMinDistance = double.MaxValue;
                _dMaxDistance = double.MinValue;
                _dAvgDistance = 0;
                _dSumDistance = 0;
                foreach (int i in _memberIDs)
                {
                    double dist = Vector2.Distance(record[i].Position, _goal.Position);
                    if (dist < _dMinDistance) _dMinDistance = dist;
                    if (dist > _dMaxDistance) _dMaxDistance = dist;
                    _dSumDistance += dist;
                }
                _dAvgDistance = _dSumDistance / _memberIDs.Count;
            }
        }

        #endregion
    }
}

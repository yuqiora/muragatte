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
using Muragatte.Core.Storage;

namespace Muragatte.Research.Results
{
    public class StepOverview
    {
        #region Fields

        private int _iStep;
        private int _iStrayWander;
        private int _iStrayGoal;
        private IEnumerable<Agent> _strays;
        private List<GroupOverview> _groupDetails = new List<GroupOverview>();
        private GroupOverview _mainGroup;
        private List<ObservedArchetypeOverview> _observed = new List<ObservedArchetypeOverview>();

        #endregion

        #region Constructors

        public StepOverview(HistoryRecord record, List<ArchetypeOverviewInfo> observedInfo)
        {
            _iStep = record.Step;
            _strays = record.StrayAgents;
            _iStrayWander = StraysWander.Count();
            _iStrayGoal = StraysGoal.Count();
            GatherGroupDetails(record.Groups, record);
            foreach (ArchetypeOverviewInfo info in observedInfo)
            {
                _observed.Add(new ObservedArchetypeOverview(record, info, _groupDetails, _mainGroup.ID));
            }
        }

        #endregion

        #region Properties

        public int Step
        {
            get { return _iStep; }
        }

        public int StrayCount
        {
            get { return _strays.Count(); }
        }

        public int StrayWanderCount
        {
            get { return _iStrayWander; }
        }

        public int StrayGoalCount
        {
            get { return _iStrayGoal; }
        }

        public IEnumerable<Agent> Strays
        {
            get { return _strays; }
        }

        public IEnumerable<Agent> StraysWander
        {
            get { return _strays.Where(a => a.Goal == null); }
        }

        public IEnumerable<Agent> StraysGoal
        {
            get { return _strays.Where(a => a.Goal != null); }
        }

        public int GroupCount
        {
            get { return _groupDetails.Count; }
        }

        public IEnumerable<GroupOverview> GroupDetails
        {
            get { return _groupDetails; }
        }

        public GroupOverview MainGroup
        {
            get { return _mainGroup; }
        }

        public List<ObservedArchetypeOverview> Observed
        {
            get { return _observed; }
        }

        public bool HasAnyObserved
        {
            get { return _observed.Count > 0; }
        }

        #endregion

        #region Methods

        private void GatherGroupDetails(IEnumerable<Group> groups, HistoryRecord record)
        {
            foreach (Group g in groups)
            {
                GroupOverview go = new GroupOverview(g, record);
                _groupDetails.Add(go);
                if (_mainGroup == null || go.Size > _mainGroup.Size) _mainGroup = go;
            }
            if (_mainGroup != null) _mainGroup.IsMain = true;
        }

        #endregion
    }
}

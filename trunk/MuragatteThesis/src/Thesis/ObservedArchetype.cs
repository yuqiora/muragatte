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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Core;
using Muragatte.Core.Environment;

namespace Muragatte.Thesis
{
    public class ObservedArchetype : INotifyPropertyChanged
    {
        #region Fields

        private bool _bObserved = false;
        private AgentArchetype _archetype;
        private ArchetypeOverviewInfo _overviewInfo = null;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ObservedArchetype(AgentArchetype archetype)
        {
            _archetype = archetype;
        }

        #endregion

        #region Properties

        public bool IsObserved
        {
            get { return _bObserved; }
            set
            {
                _bObserved = value;
                NotifyPropertyChanged("IsObserved");
            }
        }

        public AgentArchetype Archetype
        {
            get { return _archetype; }
        }

        public ArchetypeOverviewInfo OverviewInfo
        {
            get { return _overviewInfo; }
        }

        #endregion

        #region Methods

        public IEnumerable<Agent> CreateAgents(int startID, MultiAgentSystem model)
        {
            if (_bObserved && _overviewInfo == null)
            {
                List<int> ids = new List<int>();
                int endID = startID + _archetype.Count;
                for (int i = startID; i < endID; i++) ids.Add(i);
                _overviewInfo = new ArchetypeOverviewInfo(_archetype.Name, Archetype.Specifics.Goal, ids);
            }
            return _archetype.CreateAgents(startID, model);
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

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
using System.Xml.Serialization;
using Muragatte.Core;
using Muragatte.Core.Environment;
using Muragatte.Core.Environment.Agents;

namespace Muragatte.Thesis
{
    public class ObservedArchetype : INotifyPropertyChanged
    {
        #region Fields

        private bool _bObserved = false;
        private AgentArchetype _archetype = null;
        private ArchetypeOverviewInfo _overviewInfo = null;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ObservedArchetype() { }

        public ObservedArchetype(AgentArchetype archetype)
        {
            _archetype = archetype;
        }

        #endregion

        #region Properties

        [XmlAttribute]
        public bool IsObserved
        {
            get { return _bObserved; }
            set
            {
                _bObserved = value;
                NotifyPropertyChanged("IsObserved");
            }
        }

        [XmlElement(ElementName = "Boid", Type = typeof(BoidAgentArchetype)),
        XmlElement(ElementName = "AdvancedBoid", Type = typeof(AdvancedBoidAgentArchetype)),
        XmlElement(ElementName = "Versatile", Type = typeof(VersatileAgentArchetype)),
        XmlElement(ElementName = "LoneWanderer", Type = typeof(LoneWandererAgentArchetype))]
        public AgentArchetype Archetype
        {
            get { return _archetype; }
            set { _archetype = value; }
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

// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Core Library
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

namespace Muragatte.Core.Environment
{
    public class Group : IEnumerable<Agent>
    {
        #region Fields

        private int _iGroupID;
        private List<Agent> _members = new List<Agent>();

        #endregion

        #region Constructors

        public Group(Agent source, IEnumerable<Agent> agents)
        {
            _iGroupID = source.ID;
            Add(source);
            source.Representative.IsEnabled = true;
            if (agents != null)
            {
                foreach (Agent a in agents)
                {
                    Add(a);
                }
            }
        }

        public Group(int id, IEnumerable<Agent> agents)
        {
            _iGroupID = id;
            _members.AddRange(agents);
        }

        #endregion

        #region Properties

        public int ID
        {
            get { return _iGroupID; }
        }

        public int Count
        {
            get { return _members.Count; }
        }

        public Agent FirstMember
        {
            get { return _members.Count > 0 ? _members[0] : null; }
        }

        public Centroid Centroid
        {
            get { return _members.Count > 0 ? _members[0].Representative : null; }
        }

        #endregion

        #region Methods

        public void Add(Agent agent)
        {
            _members.Add(agent);
            agent.Representative.Group = this;
        }

        public void Clear()
        {
            foreach (Agent a in _members)
            {
                a.Representative.Group = null;
            }
            _members.Clear();
        }

        public Goal GetGoal()
        {
            return GetGoal(Centroid.Direction, Centroid.Position);
        }

        public Goal GetGoal(Vector2 centroidDir, Vector2 centroidPos)
        {
            Dictionary<Goal, int> goals = new Dictionary<Goal, int>();
            foreach (Agent a in _members)
            {
                if (a.Goal != null)
                {
                    if (goals.ContainsKey(a.Goal))
                    {
                        goals[a.Goal]++;
                    }
                    else
                    {
                        goals.Add(a.Goal, 1);
                    }
                }
            }
            int count = 0;
            Goal goal = null;
            foreach (KeyValuePair<Goal, int> gi in goals)
            {
                if (gi.Value > count || (gi.Value == count && AngleToGoal(centroidDir, centroidPos, gi.Key) < AngleToGoal(centroidDir, centroidPos, goal)))
                {
                    goal = gi.Key;
                    count = gi.Value;
                }
            }
            return goal;
        }

        private Angle AngleToGoal(Vector2 centroidDirection, Vector2 centroidPosition, Goal goal)
        {
            return Vector2.AngleBetween(centroidDirection, goal.Position - centroidPosition);
        }

        public IEnumerator<Agent> GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("Group {0} [{1}]", _iGroupID, _members.Count);
        }

        #endregion
    }
}

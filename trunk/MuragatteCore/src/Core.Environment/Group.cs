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
            return _iGroupID.ToString();
        }

        #endregion
    }
}

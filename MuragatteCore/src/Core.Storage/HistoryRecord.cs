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
using Muragatte.Core.Environment;

namespace Muragatte.Core.Storage
{
    public class HistoryRecord : IEnumerable<ElementStatus>
    {
        #region Fields

        private Dictionary<int, ElementStatus> _items = new Dictionary<int, ElementStatus>();
        private List<Group> _groups = null;
        private List<Agent> _strays = null;

        #endregion

        #region Constructors

        public HistoryRecord() { }

        #endregion

        #region Properties

        public ElementStatus this[int id]
        {
            get { return _items[id]; }
        }

        public IEnumerable<Group> Groups
        {
            get { return _groups; }
        }

        public IEnumerable<Agent> StrayAgents
        {
            get { return _strays; }
        }

        #endregion

        #region Methods

        public void Add(ElementStatus status)
        {
            if (status != null)
            {
                _items.Add(status.ElementID, status);
            }
        }

        public void GroupsAndStrays(IEnumerable<Group> groups, IEnumerable<Agent> strays)
        {
            _groups = groups == null ? new List<Group>() : new List<Group>(groups);
            _strays = strays == null ? new List<Agent>() : new List<Agent>(strays);
        }

        public void Clear()
        {
            _items.Clear();
            _groups = null;
        }

        public IEnumerator<ElementStatus> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        #endregion
    }
}

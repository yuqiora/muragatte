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

namespace Muragatte.Core.Storage
{
    public class HistoryRecord : IEnumerable<ElementStatus>
    {
        #region Fields

        private Dictionary<int, ElementStatus> _items = new Dictionary<int, ElementStatus>();

        #endregion

        #region Constructors

        public HistoryRecord() { }

        #endregion

        #region Properties

        public ElementStatus this[int id]
        {
            get { return _items[id]; }
        }

        #endregion

        #region Methods

        public void Add(ElementStatus status)
        {
            _items.Add(status.ElementID, status);
        }

        public void Clear()
        {
            _items.Clear();
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

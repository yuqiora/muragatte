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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Muragatte.Core.Environment;

namespace Muragatte.Core.Storage
{
    public class SpeciesCollection : Dictionary<int, Species>, INotifyCollectionChanged
    {
        #region Fields

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Constructors

        public SpeciesCollection() : base() { }

        public SpeciesCollection(IDictionary<int, Species> dictionary) : base(dictionary) { }

        public SpeciesCollection(IEnumerable<Species> items)
            : base()
        {
            foreach (Species s in items)
            {
                base.Add(s.ID, s);
            }
        }

        #endregion

        #region Properties

        public new Species this[int key]
        {
            get { return base[key]; }
            set { }
        }

        #endregion

        #region Methods

        public void Add(Species item)
        {
            Add(item.ID, item);
        }

        public new void Add(int key, Species value)
        {
            base.Add(key, value);
            NotifyCollectionChanged(NotifyCollectionChangedAction.Add, value);
        }

        public void Add(IEnumerable<Species> items)
        {
            foreach (Species s in items)
            {
                Add(s.ID, s);
            }
        }

        public new bool Remove(int key)
        {
            Species s;
            if (TryGetValue(key, out s))
            {
                base.Remove(key);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, s);
                return true;
            }
            else
            {
                return false;
            }
        }

        public new void Clear()
        {
            base.Clear();
            NotifyCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        protected void NotifyCollectionChanged(NotifyCollectionChangedAction action, Species changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }

        #endregion
    }
}

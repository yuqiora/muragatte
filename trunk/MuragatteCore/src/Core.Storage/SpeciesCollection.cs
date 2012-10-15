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
    public class SpeciesCollection : ICollection<Species>, INotifyCollectionChanged
    {
        #region Constants

        public const string DEFAULT_AGENTS_LABEL = "Agents";
        public const string DEFAULT_GOALS_LABEL = "Goals";
        public const string DEFAULT_OBSTACLES_LABEL = "Obstacles";
        public const string DEFAULT_CENTROIDS_LABEL = "Centroids";
        public const string DEFAULT_EXTRAS_LABEL = "Extras";

        #endregion

        #region Fields

        private static readonly List<string> _labels = new List<string>() { DEFAULT_AGENTS_LABEL, DEFAULT_GOALS_LABEL, DEFAULT_OBSTACLES_LABEL, DEFAULT_CENTROIDS_LABEL, DEFAULT_EXTRAS_LABEL };

        private List<Species> _items = new List<Species>();
        private Dictionary<string, Species> _defaults = new Dictionary<string, Species>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Constructors

        public SpeciesCollection(bool withDefaults = false)
        {
            InitializeDefaults();
            if (withDefaults) CreateDefaults();
        }

        public SpeciesCollection(IEnumerable<Species> items)
        {
            InitializeDefaults();
            Add(items);
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Species this[string name]
        {
            get { return _items.Find(s => s.FullName == name); }
        }

        public Species this[int index]
        {
            get { return index >= 0 && index < _items.Count ? _items[index] : null; }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            _items.Clear();
            InitializeDefaults();
            NotifyCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        public bool Contains(Species item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(Species[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public Species GetDefault(string label)
        {
            return _labels.Contains(label) ? _defaults[label] : null;
        }

        public void Add(Species item)
        {
            if (item.Ancestor == null || !_items.Contains(item.Ancestor))
            {
                _items.Add(item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
            else
            {
                int i = _items.FindLastIndex(s => s.IsDescendantOf(item.Ancestor));
                int index = (i < 0 ? _items.IndexOf(item.Ancestor) : i) + 1;
                if (index >= _items.Count)
                {
                    _items.Add(item);
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
                }
                else
                {
                    _items.Insert(index, item);
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
                }
            }
        }

        public void Add(Species item, string key)
        {
            Add(item);
            if (_labels.Contains(key)) _defaults[key] = item;
        }

        public void Add(IEnumerable<Species> items)
        {
            foreach (Species s in items)
            {
                _items.Add(s);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, s);
            }
        }

        public bool Remove(Species item)
        {
            if (_items.Contains(item))
            {
                RemoveAndNotify(item);
                List<Species> toRemove = new List<Species>();
                foreach (Species s in _items)
                {
                    if (s.IsDescendantOf(item)) toRemove.Add(s);
                }
                foreach (Species s in toRemove)
                {
                    RemoveAndNotify(s);
                }
                return true;
            }
            else return false;
        }

        private void RemoveAndNotify(Species item)
        {
            int index = _items.IndexOf(item);
            _items.RemoveAt(index);
            NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        private void InitializeDefaults()
        {
            _defaults.Clear();
            foreach (string s in _labels)
            {
                _defaults.Add(s, null);
            }
        }

        private void CreateDefaults()
        {
            foreach (string s in _labels)
            {
                Add(new Species(s), s);
            }
        }

        public void SetDefaultSpecies(Species species, string key)
        {
            if (_labels.Contains(key))
            {
                if (_items.Contains(species))
                {
                    _defaults[key] = species;
                }
                else
                {
                    Add(species, key);
                }
            }
        }

        public IEnumerator<Species> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, Species changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, Species changedItem, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem, index));
            }
        }

        #endregion
    }
}

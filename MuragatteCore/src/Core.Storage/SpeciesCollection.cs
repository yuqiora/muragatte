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
    //needs some reworking, current version not sufficent
    public class SpeciesCollection : Dictionary<int, Species>, INotifyCollectionChanged
    {
        #region Constants

        public const string DEFAULT_AGENTS_LABEL = "Agents";
        public const string DEFAULT_GOALS_LABEL = "Goals";
        public const string DEFAULT_OBSTACLES_LABEL = "Obstacles";
        public const string DEFAULT_CENTROIDS_LABEL = "Centroids";
        public const string DEFAULT_EXTRAS_LABEL = "Extras";

        #endregion

        #region Fields

        private Dictionary<string, Species> _defaults = new Dictionary<string, Species>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Constructors

        public SpeciesCollection(bool withDefaults = false) : base()
        {
            if (withDefaults) CreateDefaultSpecies();
        }

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

        public Species this[string key]
        {
            get
            {
                Species s;
                if (!_defaults.TryGetValue(key, out s))
                {
                    s = null;
                }
                return s;
            }
        }

        public bool HasDefaults
        {
            get { return _defaults.Count > 0; }
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
            NotifyCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<int, Species>(key, value));
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
                _defaults.Remove(s.Name);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<int, Species>(key, s));
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
            _defaults.Clear();
            NotifyCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        protected void NotifyCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<int, Species>? changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }

        public void CreateDefaultSpecies()
        {
            if (_defaults.Count == 0)
            {
                CreateDefaultSpecies(DEFAULT_AGENTS_LABEL);
                CreateDefaultSpecies(DEFAULT_GOALS_LABEL);
                CreateDefaultSpecies(DEFAULT_OBSTACLES_LABEL);
                CreateDefaultSpecies(DEFAULT_CENTROIDS_LABEL);
                CreateDefaultSpecies(DEFAULT_EXTRAS_LABEL);
            }
        }

        private void CreateDefaultSpecies(string name)
        {
            Species s = new Species(name);
            _defaults.Add(name, s);
            Add(s);
        }

        #endregion
    }
}

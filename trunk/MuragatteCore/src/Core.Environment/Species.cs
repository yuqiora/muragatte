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

namespace Muragatte.Core.Environment
{
    public class Species : Storage.ISpareItem
    {
        #region Statics

        private static Counter IdCounter = new Counter();

        public static void ResetIDCounter()
        {
            IdCounter.Reset();
        }

        #endregion

        #region Fields

        private int _iSpeciesID = -1;
        private string _sName = null;
        private string _sFullName = null;
        private Species _ancestor = null;
        private Dictionary<Species, ElementNature> _relationships = new Dictionary<Species, ElementNature>();
        protected object _item = null;

        #endregion

        #region Constructors

        public Species(string name)
        {
            _iSpeciesID = IdCounter.Next();
            _sName = name;
            _sFullName = name;
        }

        public Species(string name, Species ancestor)
        {
            _iSpeciesID = IdCounter.Next();
            _sName = name;
            _ancestor = ancestor;
            _sFullName = _ancestor == null ? name : _ancestor.FullName + "." + name;
        }

        #endregion

        #region Properties

        public int ID
        {
            get { return _iSpeciesID; }
        }

        public string Name
        {
            get { return _sName; }
        }

        public string FullName
        {
            get { return _sFullName; }
        }

        public Species Ancestor
        {
            get { return _ancestor; }
        }

        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        #endregion

        #region Methods

        public bool FormRelationship(Species species, ElementNature nature)
        {
            if (_relationships.ContainsKey(species))
            { return false; }
            else
            {
                _relationships.Add(species, nature);
                return true;
            }
        }

        public bool RevokeRelationship(Species species)
        {
            return _relationships.Remove(species);
        }

        public bool ChangeRelationship(Species species, ElementNature nature)
        {
            if (_relationships.ContainsKey(species))
            {
                _relationships[species] = nature;
                return true;
            }
            else
            { return false; }
        }

        public bool RelationshipWith(Species species, out ElementNature nature)
        {
            bool isSpecified = _relationships.TryGetValue(species, out nature);
            if (!isSpecified)
            {
                if (species._ancestor == null)
                {
                    nature = ElementNature.Unknown;
                }
                else
                {
                    return RelationshipWith(species._ancestor, out nature);
                }
            }
            return isSpecified;
        }

        public Species CreateSubSpecies(string name)
        {
            return new Species(name, this);
        }

        public T GetItemAs<T>() where T : class
        {
            if (_ancestor != null && _item == null)
            {
                return _ancestor.GetItemAs<T>();
            }
            else
            {
                return _item is T ? (T)_item : null;
            }
        }

        public bool Equals(Species s)
        {
            return _iSpeciesID == s._iSpeciesID && _sFullName == s._sFullName;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Species)
            {
                Species s = (Species)obj;
                return _iSpeciesID == s._iSpeciesID && _sFullName == s._sFullName;
            }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return _iSpeciesID;
        }

        public override string ToString()
        {
            return _sFullName;
        }

        #endregion
    }

    public class SpeciesCollection : Dictionary<int, Species>, INotifyCollectionChanged
    {
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

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}

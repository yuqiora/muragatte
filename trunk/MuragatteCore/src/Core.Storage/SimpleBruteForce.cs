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
using Muragatte.Common;
using Muragatte.Core.Environment;

namespace Muragatte.Core.Storage
{
    public class SimpleBruteForceStorage : IStorage
    {
        #region Fields

        private List<Element> _items = new List<Element>();
        private List<Centroid> _centroids = new List<Centroid>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Constructors

        public SimpleBruteForceStorage() { }

        public SimpleBruteForceStorage(IEnumerable<Element> items)
        {
            Add(items);
        }

        #endregion

        #region Properties
        
        public int Count
        {
            get { return _items.Count; }
        }

        public int CountAll
        {
            get { return _items.Count + _centroids.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Element this[int id]
        {
            get
            {
                int index = _items.FindIndex(e => e.ID == id);
                return index < 0 ? null : _items[index];
            }
        }

        public IEnumerable<Element> Items
        {
            get { return _items; }
        }

        public IEnumerable<Agent> Agents
        {
            get { return _items.OfType<Agent>(); }
        }

        public IEnumerable<Obstacle> Obstacles
        {
            get { return _items.OfType<Obstacle>(); }
        }

        public IEnumerable<Goal> Goals
        {
            get { return _items.OfType<Goal>(); }
        }

        public IEnumerable<Extras> Extras
        {
            get { return _items.OfType<Extras>(); }
        }

        public IEnumerable<Element> Stationary
        {
            get { return _items.Where(e => e.IsStationary); }
        }

        public IEnumerable<Centroid> Centroids
        {
            get { return _centroids; }
        }

        #endregion

        #region Methods
        
        public void Add(Element item)
        {
            if (item is Centroid)
            {
                _centroids.Add(item as Centroid);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
            else
            {
                _items.Add(item);
                if (_centroids.Count > 0)
                {
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, _items.Count - 1);
                }
                else
                {
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
                }
            }
        }

        public void Add(IEnumerable<Element> items)
        {
            foreach (Element e in items)
            {
                Add(e);
            }
        }

        public bool Remove(Element item)
        {
            return Remove(item.ID);
        }

        public void Remove(IEnumerable<Element> items)
        {
            foreach (Element e in items)
            {
                Remove(e.ID);
            }
        }

        public bool Remove(int id)
        {
            int index = _items.FindIndex(e => e.ID == id);
            if (index < 0)
            {
                return false;
            }
            else
            {
                Element e = _items[index];
                _items.RemoveAt(index);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, e, index);
                if (e is Agent) RemoveCentroid((e as Agent).Representative);
                return true;
            }
        }

        private void RemoveCentroid(Centroid item)
        {
            int index = _centroids.FindIndex(e => e.ID == item.ID);
            if (index >= 0)
            {
                Centroid c = _centroids[index];
                _centroids.RemoveAt(index);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, c, index + _items.Count);
            }
        }

        public void Clear()
        {
            _items.Clear();
            _centroids.Clear();
            NotifyCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        public bool Contains(Element item)
        {
            return item is Centroid ? _items.Contains(item) : _centroids.Contains(item as Centroid);
        }

        public void CopyTo(Element[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public Element Nearest(Element e)
        {
            return Nearest<Element>(e);
        }

        public T Nearest<T>(Element e) where T : Element
        {
            double dDistance = int.MaxValue;
            T nearest = null;
            foreach (Element n in _items)
            {
                if (e != n && n.IsEnabled && n is T)
                {
                    double dTempDist = Vector2.Distance(e.Position, n.Position);
                    if (dTempDist < dDistance)
                    {
                        nearest = (T)n;
                    }
                }
            }
            return nearest;
        }

        public IEnumerable<Element> RangeSearch(Element e, double range)
        {
            List<Element> result = new List<Element>();
            foreach (Element n in _items)
            {
                if (e != n && n.IsEnabled &&
                    Vector2.Distance(e.Position, n.Position) - n.Radius < range)
                {
                    result.Add(n);
                }
            }
            return result;
        }

        public IEnumerable<T> RangeSearch<T>(Element e, double range) where T : Element
        {
            return RangeSearch(e, range).OfType<T>();
        }

        public IEnumerable<Element> RangeSearch(Element e, double range, Func<Element, bool> match)
        {
            return RangeSearch(e, range).Where(match);
        }

        public IEnumerable<T> RangeSearch<T>(Element e, double range, Func<T, bool> match) where T : Element
        {
            return RangeSearch<T>(e, range).Where(match);
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        public void Initialize() { }

        public void Update() { }

        public void Rebuild() { }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, Element changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, Element changedItem, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem, index));
            }
        }

        #endregion
    }
}

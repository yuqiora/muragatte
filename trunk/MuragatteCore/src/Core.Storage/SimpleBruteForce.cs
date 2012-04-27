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
using Muragatte.Core.Environment;

namespace Muragatte.Core.Storage
{
    public class SimpleBruteForce : IStorage
    {
        #region Fields

        private List<Element> _items = null;

        #endregion

        #region Constructors

        public SimpleBruteForce()
        {
            _items = new List<Element>();
        }

        public SimpleBruteForce(IEnumerable<Element> items)
        {
            _items = new List<Element>(items);
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
            get { return ItemsOfType<Agent>(); }
        }

        public IEnumerable<Obstacle> Obstacles
        {
            get { return ItemsOfType<Obstacle>(); }
        }

        public IEnumerable<Goal> Goals
        {
            get { return ItemsOfType<Goal>(); }
        }

        public IEnumerable<Extras> Extras
        {
            get { return ItemsOfType<Extras>(); }
        }

        public IEnumerable<Element> Stationary
        {
            get { return _items.Where(e => e.IsStationary); }
        }

        public IEnumerable<Centroid> Centroids
        {
            get { return ItemsOfType<Centroid>(); }
        }

        #endregion

        #region Methods
        
        public void Add(Element item)
        {
            _items.Add(item);
        }

        public void Add(IEnumerable<Element> items)
        {
            _items.AddRange(items);
        }

        public bool Remove(Element item)
        {
            return _items.Remove(item);
        }

        public void Remove(IEnumerable<Element> items)
        {
            if (this == items)
            {
                Clear();
            }
            else
            {
                foreach (Element e in items)
                {
                    _items.Remove(e);
                }
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
                _items.RemoveAt(index);
                return true;
            }
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(Element item)
        {
            return _items.Contains(item);
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
            return RangeSearch<Element>(e, range);
        }

        public IEnumerable<T> RangeSearch<T>(Element e, double range) where T : Element
        {
            List<T> inRange = new List<T>();
            foreach (Element n in _items)
            {
                if (e != n && n.IsEnabled && n is T &&
                    Vector2.Distance(e.Position, n.Position) - n.Radius < range)
                {
                    inRange.Add((T)n);
                }
            }
            return inRange;
        }

        public IEnumerable<Element> RangeSearch(Element e, double range, Predicate<Element> match)
        {
            return RangeSearch<Element>(e, range, match);
        }

        public IEnumerable<T> RangeSearch<T>(Element e, double range, Predicate<Element> match) where T : Element
        {
            List<T> inRange = new List<T>();
            foreach (Element n in _items)
            {
                if (e != n && n.IsEnabled && n is T &&
                    Vector2.Distance(e.Position, n.Position) - n.Radius < range && match(n))
                {
                    inRange.Add((T)n);
                }
            }
            return inRange;
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        private IEnumerable<T> ItemsOfType<T>() where T : Element
        {
            List<T> items = new List<T>();
            foreach (Element e in _items)
            {
                if (e is T)
                {
                    items.Add((T)e);
                }
            }
            return items;
        }
        
        #endregion
    }
}

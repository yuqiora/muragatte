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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Muragatte.Common;
using Muragatte.Core.Environment;
using Muragatte.Core.Storage.ONG;

namespace Muragatte.Core.Storage
{
    public class OrthantNeighbourhoodGraphStorage : IStorage
    {
        #region Fields

        private Dictionary<int, Element> _items = new Dictionary<int, Element>();
        private OrthantNeighbourhoodGraph _ong = new OrthantNeighbourhoodGraph();

        private Dictionary<int, Agent> _agents = new Dictionary<int, Agent>();
        private Dictionary<int, Obstacle> _obstacles = new Dictionary<int, Obstacle>();
        private Dictionary<int, Goal> _goals = new Dictionary<int, Goal>();
        private Dictionary<int, Extras> _extras = new Dictionary<int, Extras>();
        private Dictionary<int, Element> _stationary = new Dictionary<int, Element>();
        private Dictionary<int, Centroid> _centroids = new Dictionary<int, Centroid>();

        private bool _bInitialized = false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Constructors

        public OrthantNeighbourhoodGraphStorage() { }

        //public OrthantNeighbourhoodGraphStorage(IEnumerable<Element> items)
        //{
        //    foreach (Element e in items)
        //    {

        //    }
        //}

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
                Element e;
                if (!_items.TryGetValue(id, out e))
                {
                    e = null;
                }
                return e;
            }
        }

        public IEnumerable<Element> Items
        {
            get { return _items.Values; }
        }

        public IEnumerable<Agent> Agents
        {
            get { return _agents.Values; }
        }

        public IEnumerable<Obstacle> Obstacles
        {
            get { return _obstacles.Values; }
        }

        public IEnumerable<Goal> Goals
        {
            get { return _goals.Values; }
        }

        public IEnumerable<Extras> Extras
        {
            get { return _extras.Values; }
        }

        public IEnumerable<Element> Stationary
        {
            get { return _stationary.Values; }
        }

        public IEnumerable<Centroid> Centroids
        {
            get { return _centroids.Values; }
        }

        public IEnumerable<Vertex> Vertices
        {
            get { return _ong.Vertices; }
        }

        #endregion

        #region Methods

        public void Add(Element item)
        {
            _items.Add(item.ID, item);
            if (item is Agent) _agents.Add(item.ID, (Agent)item);
            if (item is Obstacle) _obstacles.Add(item.ID, (Obstacle)item);
            if (item is Goal) _goals.Add(item.ID, (Goal)item);
            if (item is Extras) _extras.Add(item.ID, (Extras)item);
            if (item.IsStationary) _stationary.Add(item.ID, item);
            if (item is Centroid) _centroids.Add(item.ID, (Centroid)item);
            else
            {
                if (_bInitialized)
                {
                    _ong.Add(item);
                    //if (!item.IsStationary) item.PropertyChanged += ElementPositionChanged;
                }
            }
            NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
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
            Element e;
            if (!_items.TryGetValue(id, out e))
            {
                return false;
            }
            _items.Remove(id);
            RemoveFromCategories(id);
            //e.PropertyChanged -= ElementPositionChanged;
            _ong.Remove(id);
            NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, e);
            return true;
        }

        private void RemoveFromCategories(int id)
        {
            _agents.Remove(id);
            _obstacles.Remove(id);
            _goals.Remove(id);
            _extras.Remove(id);
            _stationary.Remove(id);
            _centroids.Remove(id);
        }

        public void Clear()
        {
            //foreach (Element e in _items.Values)
            //{
            //    e.PropertyChanged -= ElementPositionChanged;
            //}
            _ong.Clear();
            _items.Clear();
            _agents.Clear();
            _obstacles.Clear();
            _goals.Clear();
            _extras.Clear();
            _stationary.Clear();
            _centroids.Clear();
            NotifyCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        public bool Contains(Element item)
        {
            return _items.ContainsKey(item.ID);
        }

        public void CopyTo(Element[] array, int arrayIndex)
        {
            _items.Values.CopyTo(array, arrayIndex);
        }

        public Element Nearest(Element e)
        {
            return _ong[e.ID].NearestNeighbour(Metric.Euclidean).Value;
        }

        public T Nearest<T>(Element e) where T : Element
        {
            T nearest = null;
            double dist = double.PositiveInfinity;
            foreach (T item in _items.Values.OfType<T>())
            {
                double d = Vector2.Distance(e.Position, item.Position);
                if (d < dist)
                {
                    dist = d;
                    nearest = item;
                }
            }
            return nearest;
        }

        public IEnumerable<Element> RangeSearch(Element e, double range)
        {
            List<Vertex> vertices = _ong.RangeSearch(_ong[e.ID], range, false, true);
            List<Element> result = new List<Element>();
            foreach (Vertex v in vertices)
            {
                result.Add(v.Value);
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
            //List<Vertex> vertices = _ong.RangeSearch(_ong[e.ID], range, Metric.Euclidean, false);
            //List<Element> result = new List<Element>();
            //foreach (Vertex v in vertices)
            //{
            //    if (match(v.Value))
            //    {
            //        result.Add(v.Value);
            //    }
            //}
            //return result;
        }

        public IEnumerable<T> RangeSearch<T>(Element e, double range, Func<T, bool> match) where T : Element
        {
            return RangeSearch<T>(e, range).Where(match);
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, Element changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }

        private void ElementPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Position")
            {
                _ong.Move(((Element)sender).ID);
            }
        }

        public void Initialize()
        {
            _bInitialized = true;
            foreach (Element e in _items.Values)
            {
                if (!(e is Centroid))
                {
                    _ong.Add(e);
                    //if (!e.IsStationary) e.PropertyChanged += ElementPositionChanged;
                }

            }
        }

        public void Update()
        {
            _ong.Move();
        }

        public void Rebuild()
        {
            _ong.Rebuild();
        }

        #endregion
    }
}

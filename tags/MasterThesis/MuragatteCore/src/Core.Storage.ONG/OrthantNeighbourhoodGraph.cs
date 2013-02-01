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

namespace Muragatte.Core.Storage.ONG
{
    public class OrthantNeighbourhoodGraph : ICollection<Vertex>
    {
        #region Fields

        private static readonly List<Quadrant> _quadrants = new List<Quadrant>() { Quadrant.NorthEast, Quadrant.SouthEast, Quadrant.SouthWest, Quadrant.NorthWest };

        private Dictionary<int, Vertex> _items = new Dictionary<int, Vertex>();
        private List<int> _flagged = new List<int>();
        
        #endregion

        #region Constructors

        public OrthantNeighbourhoodGraph() { }

        public OrthantNeighbourhoodGraph(IEnumerable<Element> items)
        {
            foreach (Element e in items)
            {
                Add(e);
            }
        }

        #endregion

        #region Properties

        public IEnumerable<Vertex> Vertices
        {
            get { return _items.Values; }
        }

        public Vertex this[int id]
        {
            get
            {
                Vertex v;
                if (!_items.TryGetValue(id, out v))
                {
                    v = null;
                }
                return v;
            }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Methods

        public void Add(Vertex item)
        {
            //_items.Add(item.Value.ID, item);
            Insert(null, item);
        }

        public void Add(Element e)
        {
            Insert(null, new Vertex(e));
        }

        public bool Remove(Vertex item)
        {
            if (_items.Remove(item.Value.ID))
            {
                Delete(item);
                return true;
            }
            return false;
        }

        public bool Remove(Element e)
        {
            return Remove(e.ID);
        }

        public bool Remove(int id)
        {
            Vertex v;
            if (!_items.TryGetValue(id, out v))
            {
                return false;
            }
            Delete(v);
            return _items.Remove(id);
        }

        public void Clear()
        {
            DestroyStructure();
            _items.Clear();
        }

        public bool Contains(Vertex item)
        {
            return _items.ContainsKey(item.Value.ID);
        }

        public void CopyTo(Vertex[] array, int arrayIndex)
        {
            _items.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Vertex> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        private void Insert(Vertex s, Vertex p, bool newItem = true)
        {
            if (_items.Count == 0)
            {
                if (newItem) _items.Add(p.Value.ID, p);
                return;
            }
            if (newItem) _items.Add(p.Value.ID, p);
            if (s == null) s = _items.Values.First();
            foreach (Quadrant q in _quadrants)
            {
                Vertex sq = InsertInternal(s, p, q);
                p[q] = NearestNeighbour(p, sq);
                if (p[q] != null) s = p[q];
            }
            List<Vertex> r = ReverseNN(p, true);
            foreach (Vertex ri in r)
            {
                Quadrant q = ri.QuadrantOf(p);
                ri[q] = p;
            }
        }

        private Vertex InsertInternal(Vertex s, Vertex p, Quadrant q)
        {
            Vertex sq = null;
            double d = double.PositiveInfinity;// Metric.Manhattan.Distance(s.Position, p.Position);
            NNInternal(s, p, ref sq, q, ref d, SearchRegion.CoverQuadrant(p, q));
            ClearFlags();
            return sq;
        }
        
        private void Delete(Vertex p)
        {
            List<Vertex> r = ReverseNN(p);
            List<Quadrant> q = new List<Quadrant>();
            List<Vertex> s = new List<Vertex>();
            foreach (Vertex ri in r)
            {
                Quadrant qi = ri.QuadrantOf(p);
                q.Add(qi);
                Vertex si = NearestNeighbour(ri, p, false);
                s.Add(si);
            }
            for (int i = 0; i < r.Count; i++)
            {
                r[i][q[i]] = s[i];
            }
        }

        private void Move(Vertex p)
        {
            if (p.Moved)
            {
                Vertex s = p.NearestNeighbour(Metric.Manhattan);
                Delete(p);
                p.Move();
                Insert(s, p, false);
            }
        }

        public void Move(int id)
        {
            Vertex v;
            if (_items.TryGetValue(id, out v))
            {
                Move(v);
            }
        }

        public void Move()
        {
            foreach (Vertex v in _items.Values)
            {
                Move(v);
            }
        }

        private Vertex NearestNeighbour(Vertex p, Vertex s, bool notSecond = true)
        {
            if (s == null) return null;
            Quadrant q = p.QuadrantOf(s);
            double d = Metric.Manhattan.Distance(s.Position, p.Position);
            double nearest = d;
            do
            {
                nearest = d;
                SearchRegion r = new SearchRegion(p.Position, d, s.Position, q);
                SetFlag(s);
                foreach (Quadrant qi in q.Others())
                {
                    if (NNInternal(s[qi], p, ref s, q, ref d, r, notSecond)) break;
                }
                ClearFlags();
            }
            while (d < nearest);
            return s;
        }

        private bool NNInternal(Vertex n, Vertex p, ref Vertex s, Quadrant q, ref double d, SearchRegion r, bool notSecond = true)
        {
            if (n == null || n.IsFlagged) return false;
            Quadrant qn = p.QuadrantOf(n);
            if (qn == q && Metric.Manhattan.Distance(n.Position, p.Position) < d && (notSecond || (n != p && n != p[q])))
            {
                s = n;
                d = Metric.Manhattan.Distance(n.Position, p.Position);
                return true;
            }
            SetFlag(n);
            foreach (Quadrant qi in qn.Others())
            {
                if (r.Cuts(p, qi) && NNInternal(n[qi], p, ref s, q, ref d, r, notSecond))
                {
                    return true;
                }
            }
            return false;
        }

        private List<Vertex> ReverseNN(Vertex p, bool insert = false)
        {
            List<Vertex> result = new List<Vertex>();
            Queue<Vertex> Q = new Queue<Vertex>();
            Dictionary<Quadrant, SearchRegion> R = SearchRegion.CoverQuadrants(p);
            SetFlag(p);
            foreach (Vertex n in p)
            {
                if (n != null)
                {
                    SetFlag(n);
                    ClipSearchRegion(n, p, R);
                    Q.Enqueue(n);
                }
            }
            while (Q.Count > 0)
            {
                Vertex s = Q.Dequeue();
                Quadrant qs = p.QuadrantOf(s);
                Quadrant qso = qs.Opposite();
                if (insert)
                {
                    if (s[qso] == null || Metric.Manhattan.Distance(s.Position, p.Position) < Metric.Manhattan.Distance(s.Position, s[qso].Position))
                        result.Add(s);
                }
                else
                {
                    if (s[qso] == p) result.Add(s);
                }
                foreach (Quadrant qi in _quadrants)
                {
                    Vertex n = s[qi];
                    if (n != null && !n.IsFlagged && qi != qs && R.Values.Any(ri => ri.Cuts(s, qi)))
                    {
                        SetFlag(n);
                        ClipSearchRegion(n, p, R);
                        Q.Enqueue(n);
                    }
                }
            }
            ClearFlags();
            return result;
        }

        private void ClipSearchRegion(Vertex n, Vertex p, Dictionary<Quadrant, SearchRegion> r)
        {
            Quadrant qn = p.QuadrantOf(n);
            Vector2 v = n.Position - p.Position;
            if (Math.Abs(v.X) < Math.Abs(v.Y))
            {
                Quadrant q = qn.InvertX();
                r[q].ClipY(n.Position, q.SignY());
            }
            else
            {
                Quadrant q = qn.InvertY();
                r[q].ClipX(n.Position, q.SignX());
            }
        }

        public List<Vertex> RangeSearch(Vertex p, double r, bool allowPartial = false)
        {
            return RangeSearch(p, r, Metric.Manhattan, true, allowPartial);
        }

        public List<Vertex> RangeSearch(Vertex p, double r, Metric sndMetric, bool allowPartial = false)
        {
            return RangeSearch(p, r, sndMetric, true, allowPartial);
        }

        public List<Vertex> RangeSearch(Vertex p, double r, bool withP, bool allowPartial = false)
        {
            return RangeSearch(p, r, Metric.Manhattan, withP, allowPartial);
        }

        public List<Vertex> RangeSearch(Vertex p, double r, Metric sndMetric, bool withP, bool allowPartial = false)
        {
            List<Vertex> result = new List<Vertex>();
            Queue<Vertex> Q = new Queue<Vertex>();
            SearchRegion R = new SearchRegion(p.Position, r);
            Vertex origin = p;
            SetFlag(p);
            while (p != null)
            {
                if (R.Covers(p, allowPartial) && (withP || p != origin) &&
                    (sndMetric == Metric.Manhattan || sndMetric.Distance(origin.Position, p.Position, p.Value.Radius) <= r))
                {
                    result.Add(p);
                }
                foreach (Quadrant qi in _quadrants)
                {
                    Vertex n = p[qi];
                    if (n != null && !n.IsFlagged && R.Cuts(p, qi))
                    {
                        SetFlag(n);
                        Q.Enqueue(n);
                    }
                }
                p = Q.Count == 0 ? null : Q.Dequeue();
            }
            ClearFlags();
            return result;
        }

        public void Rebuild()
        {
            DestroyStructure();
            Vertex s = null;
            foreach (Vertex v in _items.Values)
            {
                Insert(s, v, false);
                s = v;
            }
        }

        private void DestroyStructure()
        {
            foreach (Vertex v in _items.Values)
            {
                v.Clear();
            }
        }

        private void SetFlag(Vertex v)
        {
            _flagged.Add(v.Value.ID);
            v.IsFlagged = true;
        }

        private void ClearFlags()
        {
            foreach (int i in _flagged)
            {
                _items[i].IsFlagged = false;
            }
            _flagged.Clear();
        }

        private void ClearAllFlags()
        {
            foreach (Vertex v in _items.Values)
            {
                v.IsFlagged = false;
            }
            _flagged.Clear();
        }

        #endregion
    }
}

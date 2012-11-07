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

namespace Muragatte.Core.Storage
{
    public class History : IEnumerable<HistoryRecord>
    {
        #region Fields

        private List<HistoryRecord> _records = new List<HistoryRecord>();

        #endregion

        #region Constructors

        public History() { }

        public History(IEnumerable<HistoryRecord> records)
        {
            _records.AddRange(records);
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _records.Count; }
        }

        public HistoryRecord this[int index]
        {
            get { return _records[index]; }
        }

        #endregion

        #region Methods

        public void Add(HistoryRecord record)
        {
            _records.Add(record);
        }

        public void Add(IEnumerable<HistoryRecord> records)
        {
            _records.AddRange(records);
        }

        public void Clear()
        {
            _records.Clear();
        }

        public IEnumerator<HistoryRecord> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        public IEnumerable<ElementStatus> GetElementInfo(int id)
        {
            return GetElementInfo(id, _records.Count);
        }

        public IEnumerable<ElementStatus> GetElementInfo(int id, int count)
        {
            List<ElementStatus> result = new List<ElementStatus>();
            for (int i = 0; i < count; i++)
            {
                result.Add(_records[i][id]);
            }
            return result;
        }

        public List<Vector2> GetElementPositions(int id)
        {
            return GetElementPositions(id, 0, _records.Count);
        }

        public List<Vector2> GetElementPositions(int id, int count)
        {
            return GetElementPositions(id, 0, count);
        }

        public List<Vector2> GetElementPositions(int id, int start, int count)
        {
            List<Vector2> positions = new List<Vector2>();
            for (int i = start; i < start + count; i++)
            {
                positions.Add(_records[i][id].Position);
            }
            return positions;
        }

        #endregion
    }
}

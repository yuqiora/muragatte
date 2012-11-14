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
    public enum HistoryMode
    {
        KeepAll,
        LastOnly,
        NoSubsteps
    }

    public class History : IEnumerable<HistoryRecord>
    {
        #region Fields

        private SortedDictionary<int, HistoryRecord> _records = new SortedDictionary<int, HistoryRecord>();
        private HistoryMode _mode = HistoryMode.KeepAll;
        private int _iSubsteps = 1;
        private int _iLastStep = -1;

        #endregion

        #region Constructors

        public History() { }

        public History(HistoryMode mode, int substeps)
        {
            _mode = mode;
            _iSubsteps = _mode == HistoryMode.NoSubsteps ? substeps : 1;
        }

        public History(IEnumerable<HistoryRecord> records)
        {
            Add(records);
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _records.Count; }
        }

        public HistoryMode Mode
        {
            get { return _mode; }
        }

        public int Length
        {
            get { return _iLastStep; }
        }

        public HistoryRecord this[int step]
        {
            get
            {
                if (_mode == HistoryMode.LastOnly) return _records.Values.FirstOrDefault();
                if (_mode == HistoryMode.NoSubsteps && step % _iSubsteps > 0) step = ((step / _iSubsteps) + 1) * _iSubsteps;
                if (step < 0) step = 0;
                if (step > _iLastStep) step = _iLastStep;
                return _records[step];
            }
        }

        #endregion

        #region Methods

        private void AddRecord(HistoryRecord record)
        {
            _records.Add(record.Step, record);
            if (record.Step > _iLastStep) _iLastStep = record.Step;
        }

        public void Add(HistoryRecord record)
        {
            if (_mode == HistoryMode.LastOnly) _records.Clear();
            if (_mode == HistoryMode.NoSubsteps && record.Step % _iSubsteps > 0) return;
            AddRecord(record);
        }

        public void Add(IEnumerable<HistoryRecord> records)
        {
            foreach (HistoryRecord r in records)
            {
                Add(r);
            }
        }

        public void Clear()
        {
            _records.Clear();
        }

        public IEnumerator<HistoryRecord> GetEnumerator()
        {
            return _records.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        public IEnumerable<ElementStatus> GetElementInfo(int id)
        {
            List<ElementStatus> result = new List<ElementStatus>();
            foreach (HistoryRecord r in _records.Values)
            {
                result.Add(r[id]);
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
            for (int i = start; i < start + count; i += _iSubsteps)
            {
                positions.Add(_records[i][id].Position);
            }
            return positions;
        }

        #endregion
    }
}

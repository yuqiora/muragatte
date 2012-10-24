// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
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
using Muragatte.Core.Storage;

namespace Muragatte.Thesis.Results
{
    public class GroupOverview
    {
        #region Fields

        private Group _source;
        private Goal _goal = null;
        private ElementStatus _centroid;
        private double? _dMinimumDistance = null;
        private double? _dMaximumDistance = null;
        private double? _dAverageDistance = null;
        private double? _dCentroidDistance = null;
        private double? _dDistanceSum = null;
        private bool _bMain = false;

        #endregion

        #region Constructors

        public GroupOverview(Group source, HistoryRecord record)
        {
            _source = source;
            _goal = _source.GetGoal();
            _centroid = record[-_source.ID];
            Distances();
        }

        #endregion

        #region Properties

        public int ID
        {
            get { return _source.ID; }
        }

        public int Size
        {
            get { return _source.Count; }
        }

        public IEnumerable<Agent> Members
        {
            get { return _source; }
        }

        public Vector2 Direction
        {
            get { return _centroid.Direction; }
        }

        public double Speed
        {
            get { return _centroid.Speed; }
        }

        public Goal MajorityGoal
        {
            get { return _goal; }
        }

        public bool HasGoal
        {
            get { return _goal != null; }
        }

        public double? MinimumDistance
        {
            get { return _dMinimumDistance; }
        }

        public double? MaximumDistance
        {
            get { return _dMaximumDistance; }
        }

        public double? AverageDistance
        {
            get { return _dAverageDistance; }
        }

        public double? CentroidDistance
        {
            get { return _dCentroidDistance; }
        }

        public double? DistanceSum
        {
            get { return _dDistanceSum; }
        }

        public bool IsMain
        {
            get { return _bMain; }
            set { _bMain = value; }
        }

        #endregion

        #region Methods

        private void Distances()
        {
            if (_goal != null)
            {
                _dCentroidDistance = Vector2.Distance(_centroid.Position, _goal.Position);
                _dMinimumDistance = double.MaxValue;
                _dMaximumDistance = 0;
                _dDistanceSum = 0;
                foreach (Agent a in _source)
                {
                    double distance = Vector2.Distance(a.Position, _goal.Position);
                    if (distance < _dMinimumDistance) _dMinimumDistance = distance;
                    if (distance > _dMaximumDistance) _dMaximumDistance = distance;
                    _dDistanceSum += distance;
                }
                _dAverageDistance = _dDistanceSum / _source.Count;
            }
        }

        public override string ToString()
        {
            return _source.ToString();
        }

        #endregion
    }
}

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
using Muragatte.Core.Environment;

namespace Muragatte.Thesis.Results
{
    public class GoalInstancePercentage
    {
        #region Fields

        protected Goal _goal;
        protected double _dPercent;

        #endregion

        #region Constructors

        public GoalInstancePercentage(Goal goal, double percent)
        {
            _goal = goal;
            Percent = percent;
        }

        #endregion

        #region Properties

        public Goal Goal
        {
            get { return _goal; }
        }

        public double Percent
        {
            get { return _dPercent; }
            set { _dPercent = InRange(value); }
        }

        public string GoalName
        {
            get { return _goal == null ? "None" : _goal.Name; }
        }

        public string PercentString
        {
            get { return CreatePercentString(_dPercent); }
        }

        #endregion

        #region Methods

        protected string CreatePercentString(double value)
        {
            return string.Format("{0:F2}%", value);
        }

        protected double InRange(double value)
        {
            if (_dPercent < 0) return 0;
            if (_dPercent > 100) return 100;
            return value;
        }

        #endregion
    }
}

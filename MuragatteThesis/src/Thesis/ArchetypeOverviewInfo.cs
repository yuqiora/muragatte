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

namespace Muragatte.Thesis
{
    public class ArchetypeOverviewInfo
    {
        #region Fields

        private string _sName;
        private Goal _goal;
        private List<int> _members;

        #endregion

        #region Constructors

        public ArchetypeOverviewInfo(string name, Goal goal, List<int> members)
        {
            _sName = name;
            _goal = goal;
            _members = members;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
        }

        public Goal Goal
        {
            get { return _goal; }
        }

        public List<int> Members
        {
            get { return _members; }
        }

        #endregion
    }
}

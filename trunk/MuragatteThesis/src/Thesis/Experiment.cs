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
using Muragatte.Core.Storage;
using Muragatte.Visual.Styles;

namespace Muragatte.Thesis
{
    public class Experiment
    {
        #region Fields

        private string _sName = null;
        private string _sPath = null;
        private int _iRepeatCount = 1;
        private int _iLength = 1;
        private List<Instance> _instances = new List<Instance>();
        private SpeciesCollection _species = new SpeciesCollection();
        private List<Style> _styles = new List<Style>();
        private bool _bComplete = false;
        private ExperimentResults _results = null;

        #endregion

        #region Constructors
        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
            set { _sName = value; }
        }

        public int RepeatCount
        {
            get { return _iRepeatCount; }
        }

        public int Length
        {
            get { return _iLength; }
        }

        public bool IsComplete
        {
            get { return _bComplete; }
        }

        #endregion

        #region Methods

        public void Run()
        {
            if (!_bComplete)
            {
                //run experiment
            }
        }

        //create new instance

        #endregion
    }

    public class ExperimentResults { }

    public class Instance { }

    public class InstanceResults { }
}

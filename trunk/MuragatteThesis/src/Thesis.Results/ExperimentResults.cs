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

namespace Muragatte.Thesis.Results
{
    public class ExperimentResults
    {
        #region Fields

        private List<InstanceResults> _instances = new List<InstanceResults>();

        #endregion

        #region Constructors

        public ExperimentResults(IEnumerable<Instance> instances)
        {
            foreach (Instance i in instances)
            {
                _instances.Add(i.Results);
            }
        }

        #endregion

        #region Properties

        public IEnumerable<InstanceResults> InstanceDetails
        {
            get { return _instances; }
        }

        #endregion

        #region Methods
        #endregion
    }
}

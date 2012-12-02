// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
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
using Muragatte.Research.GUI;
using Muragatte.Visual.IO;

namespace Muragatte.Research.IO
{
    public class XmlExperimentArchiver : XmlBaseArchiver<XmlExperimentRoot, ExperimentEditorWindow>
    {
        #region Constructors

        public XmlExperimentArchiver(ExperimentEditorWindow owner) : base("Experiment", owner) { }

        #endregion

        #region Methods

        protected override void LoadPostProcessing(XmlExperimentRoot item)
        {
            _owner.LoadExperiment(item.Experiment);
        }

        #endregion
    }
}

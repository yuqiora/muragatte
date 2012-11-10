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
using Muragatte.Thesis.GUI;
using Muragatte.Visual.IO;

namespace Muragatte.Thesis.IO
{
    public class XmlExperimentArchiver : XmlBaseArchiver<XmlExperimentRoot, ThesisExperimentEditorWindow>
    {
        #region Constructors

        public XmlExperimentArchiver(ThesisExperimentEditorWindow owner) : base("Experiment", owner) { }

        #endregion

        #region Methods

        protected override void LoadPostProcessing(XmlExperimentRoot item)
        {
            _owner.LoadExperiment(item.Experiment);
        }

        #endregion
    }
}

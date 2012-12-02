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
using System.Xml.Serialization;

namespace Muragatte.Research.IO
{
    [XmlRoot(ElementName = "Muragatte", Namespace = "Muragatte/Experiment")]
    public class XmlExperimentRoot
    {
        #region Fields

        public XmlExperiment Experiment = null;

        #endregion

        #region Constructors

        public XmlExperimentRoot() { }

        public XmlExperimentRoot(Experiment experiment)
        {
            Experiment = new XmlExperiment(experiment);
        }

        #endregion
    }
}

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
using System.Xml.Serialization;
using Muragatte.Core;

namespace Muragatte.IO
{
    [XmlRoot(ElementName = "Muragatte", Namespace = "Muragatte/Scene")]
    public class XmlSceneRoot
    {
        #region Fields

        public XmlScene Scene = null;

        #endregion

        #region Constructors

        public XmlSceneRoot() { }

        public XmlSceneRoot(Scene scene)
        {
            Scene = new XmlScene(scene);
        }

        #endregion
    }
}

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
using Muragatte.IO;
using Muragatte.Thesis.GUI;
using Muragatte.Visual.IO;

namespace Muragatte.Thesis.IO
{
    public class XmlSceneArchiver : XmlBaseArchiver<XmlSceneRoot, ThesisSceneEditorWindow>
    {
        #region Constructors

        public XmlSceneArchiver(ThesisSceneEditorWindow owner) : base("Scene", owner) { }

        #endregion

        #region Methods

        protected override void LoadPostProcessing(XmlSceneRoot item)
        {
            _owner.LoadScene(item.Scene);
        }

        #endregion
    }
}

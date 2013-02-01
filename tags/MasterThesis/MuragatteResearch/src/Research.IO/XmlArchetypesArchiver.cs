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
    public class XmlArchetypesArchiver : XmlBaseArchiver<XmlArchetypesRoot, ArchetypesEditorWindow>
    {
        #region Constructors

        public XmlArchetypesArchiver(ArchetypesEditorWindow owner) : base("Archetypes", owner) { }

        #endregion

        #region Methods

        protected override void LoadPostProcessing(XmlArchetypesRoot item)
        {
            item.AddToCollection(_owner.GetArchetypes);
        }

        #endregion
    }
}

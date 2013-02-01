// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
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
using Muragatte.Visual.GUI;

namespace Muragatte.Visual.IO
{
    public class XmlStylesArchiver : XmlBaseArchiver<XmlStylesRoot, OptionsWindow>
    {
        #region Constructors

        public XmlStylesArchiver(OptionsWindow owner) : base("Styles", owner) { }

        #endregion

        #region Methods

        protected override void LoadPostProcessing(XmlStylesRoot item)
        {
            item.AddToCollection(_owner.GetStyles);
        }

        #endregion
    }
}

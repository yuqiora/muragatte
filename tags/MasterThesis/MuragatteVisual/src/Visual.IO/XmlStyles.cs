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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Muragatte.Visual.Styles;

namespace Muragatte.Visual.IO
{
    [XmlRoot(ElementName = "Muragatte", Namespace = "Muragatte/Styles")]
    public class XmlStylesRoot
    {
        #region Fields

        [XmlArray("Styles")]
        [XmlArrayItem(ElementName = "Style")]
        public Style[] Items = null;

        #endregion

        #region Constructors

        public XmlStylesRoot() { }

        public XmlStylesRoot(ObservableCollection<Style> items)
        {
            Items = items.ToArray();
        }

        #endregion

        #region Methods

        public void AddToCollection(ObservableCollection<Style> collection)
        {
            foreach (Style s in Items)
            {
                collection.Add(s);
            }
        }

        #endregion
    }
}

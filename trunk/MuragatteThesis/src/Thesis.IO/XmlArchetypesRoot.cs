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
using System.Xml.Serialization;

namespace Muragatte.Thesis.IO
{
    [XmlRoot(ElementName = "Muragatte", Namespace = "Muragatte/Archetypes")]
    public class XmlArchetypesRoot
    {
        #region Fields

        [XmlArrayItem("Archetype")]
        public ObservedArchetype[] Archetypes = null;

        #endregion

        #region Constructors

        public XmlArchetypesRoot() { }

        public XmlArchetypesRoot(IEnumerable<ObservedArchetype> items)
        {
            Archetypes = items.ToArray();
        }

        #endregion

        #region Methods

        public void AddToCollection(ICollection<ObservedArchetype> collection)
        {
            foreach (ObservedArchetype oa in Archetypes)
            {
                collection.Add(oa);
            }
        }

        #endregion
    }
}

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
using Muragatte.Core.Environment;

namespace Muragatte.IO
{
    public class XmlAgentArgsNeighbourhood
    {
        #region Fields

        [XmlAttribute]
        public string Name = null;

        public Neighbourhood Field = null;

        #endregion

        #region Constructors

        public XmlAgentArgsNeighbourhood() { }

        public XmlAgentArgsNeighbourhood(KeyValuePair<string, Neighbourhood> item) : this(item.Key, item.Value) { }

        public XmlAgentArgsNeighbourhood(string name, Neighbourhood field)
        {
            Name = name;
            Field = field;
        }

        #endregion

        #region Operators

        public static implicit operator KeyValuePair<string, Neighbourhood>(XmlAgentArgsNeighbourhood x)
        {
            return new KeyValuePair<string, Neighbourhood>(x.Name, x.Field);
        }

        public static implicit operator XmlAgentArgsNeighbourhood(KeyValuePair<string, Neighbourhood> n)
        {
            return new XmlAgentArgsNeighbourhood(n);
        }

        #endregion
    }
}

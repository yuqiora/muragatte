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

namespace Muragatte.IO
{
    public class XmlAgentArgsModifier
    {
        #region Fields

        [XmlAttribute]
        public string Name = null;

        [XmlAttribute]
        public double Value = 0;

        #endregion

        #region Constructors

        public XmlAgentArgsModifier() { }

        public XmlAgentArgsModifier(KeyValuePair<string, double> item) : this(item.Key, item.Value) { }

        public XmlAgentArgsModifier(string name, double value)
        {
            Name = name;
            Value = value;
        }

        #endregion

        #region Operators

        public static implicit operator KeyValuePair<string, double>(XmlAgentArgsModifier x)
        {
            return new KeyValuePair<string, double>(x.Name, x.Value);
        }

        public static implicit operator XmlAgentArgsModifier(KeyValuePair<string, double> m)
        {
            return new XmlAgentArgsModifier(m);
        }

        #endregion
    }
}

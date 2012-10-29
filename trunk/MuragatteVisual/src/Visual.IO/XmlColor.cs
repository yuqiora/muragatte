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
using System.Windows.Media;
using System.Xml.Serialization;

namespace Muragatte.Visual.IO
{
    public class XmlColor
    {
        #region Fields

        private Color _color = Colors.Black;

        #endregion

        #region Constructors

        public XmlColor() { }

        public XmlColor(Color c)
        {
            _color = c;
        }

        #endregion

        #region Properties

        [XmlAttribute]
        public byte Alpha
        {
            get { return _color.A; }
            set { _color.A = value; }
        }

        [XmlAttribute]
        public byte Red
        {
            get { return _color.R; }
            set { _color.R = value; }
        }

        [XmlAttribute]
        public byte Green
        {
            get { return _color.G; }
            set { _color.G = value; }
        }

        [XmlAttribute]
        public byte Blue
        {
            get { return _color.B; }
            set { _color.B = value; }
        }

        #endregion

        #region Methods

        public Color ToColor()
        {
            return _color;
        }

        public void FromColor(Color c)
        {
            _color = c;
        }

        #endregion

        #region Operators

        public static implicit operator Color(XmlColor x)
        {
            return x.ToColor();
        }

        public static implicit operator XmlColor(Color c)
        {
            return new XmlColor(c);
        }

        #endregion
    }
}

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
using System.Xml.Serialization;
using Muragatte.Visual.Shapes;

namespace Muragatte.Visual.IO
{
    public class XmlShape
    {
        #region Fields

        [XmlText]
        public ShapeLabel Label = ShapeLabel.Ellipse;

        #endregion

        #region Constructors

        public XmlShape() { }

        public XmlShape(Shape shape) : this(shape.Label) { }

        public XmlShape(ShapeLabel label)
        {
            Label = label;
        }

        #endregion

        #region Methods

        public Shape ToShape()
        {
            return Label.ToShape();
        }

        #endregion

        #region Operators

        public static implicit operator Shape(XmlShape s)
        {
            return s.ToShape();
        }

        public static implicit operator XmlShape(Shape s)
        {
            return new XmlShape(s);
        }

        #endregion
    }
}

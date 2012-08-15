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
using Muragatte.Common;

namespace Muragatte.Core.Environment
{
    public abstract class Neighbourhood : Storage.ISpareItem
    {
        #region Fields

        protected Agent _source = null;
        protected double _dRange = 0;
        protected object _item = null;

        #endregion

        #region Constructors

        public Neighbourhood(double range)
        {
            _dRange = range;
        }

        public Neighbourhood(Agent source, double range)
        {
            _source = source;
            _dRange = range;
        }

        #endregion

        #region Properties
        
        public Agent Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        #endregion

        #region Virtual Properties

        public virtual double Range
        {
            get { return _dRange; }
        }

        #endregion

        #region Abstract Properties

        public abstract Angle Angle { get; set; }

        #endregion

        #region Methods

        public T GetItemAs<T>() where T : class
        {
            return _item is T ? (T)_item : null;
        }

        #endregion

        #region Abstract Methods

        public abstract IEnumerable<T> Within<T>(IEnumerable<T> elements) where T : Environment.Element;

        public abstract IEnumerable<T> Within<T>(IEnumerable<T> elements, Angle angle) where T : Environment.Element;

        public abstract bool Covers(Element e);

        public abstract bool Covers(Element e, Angle angle);

        #endregion

    }
}

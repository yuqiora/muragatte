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
    public abstract class Element : Storage.ISpareItem
    {
        #region Fields

        protected static Counter IdCounter = new Counter();

        protected int _iElementID = -1;
        protected MultiAgentSystem _model = null;
        //list containing history instead of one vector?
        protected Vector2 _position = new Vector2(0, 0);
        //direction and speed not needed for all elements
        protected Vector2 _direction = new Vector2(0, 0);
        protected double _dSpeed = 0;
        protected bool _bStationary = true;
        protected bool _bEnabled = true;
        protected Species _species = null;
        protected object _item = null;

        #endregion

        #region Constructors

        public Element(MultiAgentSystem model)
        {
            _iElementID = IdCounter.Next();
            _model = model;
        }

        public Element(int id, MultiAgentSystem model)
        {
            _iElementID = id;
            _model = model;
        }

        public Element(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, double speed)
        {
            _iElementID = id;
            _model = model;
            _position = position;
            _direction = direction;
            _dSpeed = speed;
        }

        #endregion

        #region Properties

        public int GetID
        {
            get { return _iElementID; }
        }

        public MultiAgentSystem Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public bool IsStationary
        {
            get { return _bStationary; }
            set { _bStationary = value; }
        }

        public bool IsEnabled
        {
            get { return _bEnabled; }
            set { _bEnabled = value; }
        }

        public Species Species
        {
            get { return _species; }
            set { _species = value; }
        }

        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        #endregion

        #region Virtual Properties

        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public virtual Vector2 Direction
        {
            get { return _direction; }
        }

        public virtual double Speed
        {
            get { return _dSpeed; }
        }

        #endregion

        #region Abstract Properties

        public abstract double Size { get; }

        public abstract ElementNature DefaultNature { get; }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Element)
            {
                Element e = (Element)obj;
                return _iElementID == e._iElementID;
            }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return _iElementID;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2} @({3})",
                _iElementID, _bStationary ? "s" : "", _bEnabled ? "" : "d", _position);
        }

        public T GetItemAs<T>() where T : class
        {
            if (_species != null && _item == null)
            {
                return _species.GetItemAs<T>();
            }
            else
            {
                if (_item is T)
                {
                    return (T)_item;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Virtual Methods

        public virtual bool Equals(Element e)
        {
            return _iElementID == e._iElementID;
        }

        #endregion

        #region Abstract Methods

        public abstract void Update();

        public abstract void AfterUpdate();

        public abstract ElementNature RelationshipWith(Element e);

        #endregion
    }
}

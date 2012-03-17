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

namespace Muragatte.Core.Environment
{
    public class Species : Storage.ISpareItem
    {
        #region Fields

        private static Counter IdCounter = new Counter();

        private int _iSpeciesID = -1;
        private string _sName = null;
        private string _sFullName = null;
        private Species _ancestor = null;
        private Dictionary<Species, ElementNature> _relationships = new Dictionary<Species, ElementNature>();
        protected object _item = null;

        #endregion

        #region Constructors

        public Species(string name, Species ancestor)
        {
            _iSpeciesID = IdCounter.Next();
            _sName = name;
            _ancestor = ancestor;
            _sFullName = _ancestor == null ? name : _ancestor.FullName + "." + name;
        }

        #endregion

        #region Properties

        public int GetID
        {
            get { return _iSpeciesID; }
        }

        public string Name
        {
            get { return _sName; }
        }

        public string FullName
        {
            get { return _sFullName; }
        }

        public Species Ancestor
        {
            get { return _ancestor; }
        }

        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        #endregion

        #region Methods

        public bool FormRelationship(Species species, ElementNature nature)
        {
            if (_relationships.ContainsKey(species))
            { return false; }
            else
            {
                _relationships.Add(species, nature);
                return true;
            }
        }

        public bool RevokeRelationship(Species species)
        {
            return _relationships.Remove(species);
        }

        public bool ChangeRelationship(Species species, ElementNature nature)
        {
            if (_relationships.ContainsKey(species))
            {
                _relationships[species] = nature;
                return true;
            }
            else
            { return false; }
        }

        public bool RelationshipWith(Species s, out ElementNature n)
        {
            bool isSpecified = _relationships.TryGetValue(s, out n);
            if (!isSpecified)
            {
                n = ElementNature.Unknown;
            }
            return isSpecified;
        }

        public Species CreateSubSpecies(string name)
        {
            return new Species(name, this);
        }

        public T GetItemAs<T>() where T : class
        {
            if (_ancestor != null && _item == null)
            {
                return _ancestor.GetItemAs<T>();
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

        public bool Equals(Species s)
        {
            return _sFullName == s._sFullName;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Species)
            {
                Species s = (Species)obj;
                return _sFullName == s._sFullName;
            }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return _iSpeciesID;
        }

        public override string ToString()
        {
            return _sFullName;
        }

        #endregion
    }
}

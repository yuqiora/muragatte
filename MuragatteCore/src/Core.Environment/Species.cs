﻿// ------------------------------------------------------------------------
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Muragatte.Core.Environment
{
    public class Species : INotifyPropertyChanged
    {
        #region Fields

        private string _sName = null;
        private Species _ancestor = null;
        private Dictionary<Species, ElementNature> _relationships = new Dictionary<Species, ElementNature>();

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Species(string name) : this(name, null) { }

        public Species(string name, Species ancestor)
        {
            _sName = name;
            _ancestor = ancestor;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _sName; }
            set
            {
                _sName = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("FullName");
            }
        }

        public string FullName
        {
            get { return _ancestor == null ? _sName : _ancestor.FullName + "." + _sName; }
        }

        public Species Ancestor
        {
            get { return _ancestor; }
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

        public bool RelationshipWith(Species species, out ElementNature nature)
        {
            bool isSpecified = _relationships.TryGetValue(species, out nature);
            if (!isSpecified)
            {
                if (species._ancestor == null)
                {
                    nature = ElementNature.Unknown;
                }
                else
                {
                    return RelationshipWith(species._ancestor, out nature);
                }
            }
            return isSpecified;
        }

        public Species CreateSubSpecies(string name)
        {
            return new Species(name, this);
        }

        public bool IsDescendantOf(Species species)
        {
            return _ancestor != null && (species == _ancestor || _ancestor.IsDescendantOf(species));
        }

        public bool Equals(Species s)
        {
            return FullName == s.FullName;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Species)
            {
                Species s = (Species)obj;
                return FullName == s.FullName;
            }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

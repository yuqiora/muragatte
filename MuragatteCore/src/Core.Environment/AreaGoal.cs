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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Muragatte.Common;

namespace Muragatte.Core.Environment
{
    public class AreaGoal : Goal
    {
        #region Fields

        private double _dWidth = 1;
        private double _dHeight = 1;

        #endregion

        #region Constructors

        public AreaGoal() : base() { }

        public AreaGoal(int id, MultiAgentSystem model, Species species, double width, double height)
            : base(id, model, species)
        {
            _dWidth = width;
            _dHeight = height;
        }

        public AreaGoal(int id, MultiAgentSystem model, Vector2 position, Species species, double width, double height)
            : base(id, model, position, species)
        {
            _dWidth = width;
            _dHeight = height;
        }

        protected AreaGoal(AreaGoal other, MultiAgentSystem model)
            : base(other, model)
        {
            _dWidth = other._dWidth;
            _dHeight = other._dHeight;
        }

        #endregion

        #region Properties

        [XmlElement]
        public override double Width
        {
            get { return _dWidth; }
            set
            {
                _dWidth = value;
                NotifyPropertyChanged("Width");
            }
        }

        [XmlElement]
        public override double Height
        {
            get { return _dHeight; }
            set
            {
                _dHeight = value;
                NotifyPropertyChanged("Height");
            }
        }

        public override double Radius
        {
            get { return Math.Max(_dWidth, _dHeight) / 2.0; }
        }

        public override string Name
        {
            get { return CreateName("Ga"); }
        }

        #endregion

        #region Methods

        public override Vector2 GetPosition()
        {
            return _model.Random.Disk(_position, _dWidth, _dHeight);
        }

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new AreaGoal(this, model);
        }

        #endregion
    }
}

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
    public class PositionGoal : Goal
    {
        #region Constructors

        public PositionGoal() : base() { }

        public PositionGoal(int id, MultiAgentSystem model, Species species)
            : base(id, model, species) { }

        public PositionGoal(int id, MultiAgentSystem model, Vector2 position, Species species)
            : base(id, model, position, species) { }

        protected PositionGoal(PositionGoal other, MultiAgentSystem model) : base(other, model) { }

        #endregion

        #region Properties

        [XmlIgnore]
        public override double Width
        {
            get { return 1; }
            set { }
        }

        [XmlIgnore]
        public override double Height
        {
            get { return 1; }
            set { }
        }

        public override bool IsResizeable
        {
            get { return false; }
        }

        public override double Radius
        {
            get { return DEFAULT_RADIUS; }
        }

        public override string Name
        {
            get { return CreateName("Gp"); }
        }

        #endregion

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new PositionGoal(this, model);
        }

        #endregion
    }
}

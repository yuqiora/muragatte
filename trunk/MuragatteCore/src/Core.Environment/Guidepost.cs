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
    public class Guidepost : Extras
    {
        #region Fields

        private Vector2 _direction;

        #endregion

        #region Constructors

        public Guidepost(int id, MultiAgentSystem model, Vector2 direction, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model, species, radius)
        {
            _direction = direction;
        }

        public Guidepost(int id, MultiAgentSystem model, Vector2 position, Vector2 direction, Species species, double radius = DEFAULT_RADIUS)
            : base(id, model, position, species, radius)
        {
            _direction = direction;
        }

        protected Guidepost(Guidepost other, MultiAgentSystem model)
            : base(other, model)
        {
            _direction = other._direction;
        }

        #endregion

        #region Properties

        public override Vector2 Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                NotifyPropertyChanged("Direction");
            }
        }

        public override bool IsDirectable
        {
            get { return true; }
        }

        public override ElementNature DefaultNature
        {
            get { return ElementNature.Companion; }
        }

        public override string Name
        {
            get { return CreateName("Eg"); }
        }

        #endregion

        #region Methods

        public override Element CloneTo(MultiAgentSystem model)
        {
            return new Guidepost(this, model);
        }

        #endregion
    }
}

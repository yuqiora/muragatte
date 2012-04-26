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
    public class Centroid : Extras
    {
        #region Fields

        private Vector2 _direction = new Vector2(0, 1);
        private double _dSpeed = 1;
        //group

        #endregion

        #region Constructors

        public Centroid(MultiAgentSystem model)
            : base(model, false) { }

        public Centroid(MultiAgentSystem model, Vector2 position, Vector2 direction, double speed)
            : base(model, position, false)
        {
            _direction = direction;
            _dSpeed = speed;
        }

        #endregion

        #region Properties

        public override Vector2 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public override double Speed
        {
            get { return _dSpeed; }
            set { _dSpeed = value; }
        }

        public override double Width
        {
            get { return 1; }
        }

        public override double Height
        {
            get { return 1; }
        }

        public override double Radius
        {
            get { return 0.5; }
        }

        #endregion

        #region Methods

        public override void Update() { }

        public override void ConfirmUpdate()
        {
            //update fields according to group members
        }

        public override string ToString()
        {
            return ToString("C");
        }

        #endregion
    }
}

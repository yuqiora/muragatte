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
    public class AreaGoal : Goal
    {
        #region Fields

        private double _dWidth = 0;
        private double _dHeight = 0;

        #endregion

        #region Constructors

        public AreaGoal(MultiAgentSystem model, double width, double height)
            : base(model)
        {
            _dWidth = width;
            _dHeight = height;
        }

        public AreaGoal(MultiAgentSystem model, Vector2 position, double width, double height)
            : base(model, position)
        {
            _dWidth = width;
            _dHeight = height;
        }

        #endregion

        #region Properties

        public override double Width
        {
            get { return _dWidth; }
        }

        public override double Height
        {
            get { return _dHeight; }
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
            double x;
            double y;
            double ss;
            //temporary
            //proper rng when Muragatte.Random done
            Muragatte.Core.Environment.RNGs.Ran2.Disk(out x, out y, out ss);
            x = _position.X + x * _dWidth - (_dWidth / 2);
            y = _position.Y + y * _dHeight - (_dHeight / 2);
            return new Vector2(x, y);
        }

        #endregion
    }
}

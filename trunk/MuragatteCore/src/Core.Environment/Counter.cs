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
    public class Counter
    {
        #region Fields

        private int _count = 0;

        #endregion

        #region Constructors

        public Counter() { }

        #endregion

        #region Properties

        public int Count
        {
            get { return _count; }
        }

        public int Last
        {
            get { return _count - 1; }
        }

        #endregion

        #region Methods

        public int Next()
        {
            //int id = _count;
            //_count++;
            //return id;
            return _count++;
        }

        public void Reset()
        {
            _count = 0;
        }

        #endregion
    }
}

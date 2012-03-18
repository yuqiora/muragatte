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
    public class Region
    {
        #region Fields

        private int _iWidth = 0;
        private int _iHeight = 0;
        private bool _bBorderedHorizontal = true;
        private bool _bBorderedVertical = true;

        #endregion

        #region Contructors

        public Region(int width, int height, bool borderedHorizontal, bool borderedVertical)
        {
            _iWidth = width;
            _iHeight = height;
            _bBorderedHorizontal = borderedHorizontal;
            _bBorderedVertical = borderedVertical;
        }

        public Region(int width, int height, bool bordered)
            : this(width, height, bordered, bordered) { }

        public Region(int length, bool borderedHorizontal, bool borderedVertical) :
            this(length, length, borderedHorizontal, borderedVertical) { }

        public Region(int length, bool bordered)
            : this(length, length, bordered, bordered) { }
        
        #endregion

        #region Properties
        
        public int Width
        {
            get { return _iWidth; }
        }

        public int Height
        {
            get { return _iHeight; }
        }

        public bool IsBordered
        {
            get { return _bBorderedHorizontal && _bBorderedVertical; }
            set
            {
                _bBorderedHorizontal = value;
                _bBorderedVertical = value;
            }
        }

        public bool IsBorderedHorizontally
        {
            get { return _bBorderedHorizontal; }
            set { _bBorderedHorizontal = value; }
        }

        public bool IsBorderedVertically
        {
            get { return _bBorderedVertical; }
            set { _bBorderedVertical= value; }
        }
        
        #endregion

        #region Methods

        public Vector2 Outside(Vector2 position)
        {
            return new Vector2(Outside(position.X, _iWidth), Outside(position.Y, _iHeight));
        }

        private double Outside(double value, double size) {
            if (value < 0)
            {
                return size + value;
            }
            if (value >= size)
            {
                return value - size;
            }
            return value;
        }

        #endregion

    }
}

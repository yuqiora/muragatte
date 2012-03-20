// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muragatte.Common;
using SysWin = System.Windows;

namespace Muragatte.Visual
{
    public class ComplexParticle : Particle
    {
        #region Fields

        private WriteableBitmap _wb = null;
        private SysWin.Rect _sourceRect;

        #endregion

        #region Constructors

        public ComplexParticle(WriteableBitmap wb, Color color)
            : base(color)
        {
            _wb = wb;
            _sourceRect = new SysWin.Rect(0, 0, _wb.PixelWidth, _wb.PixelHeight);
        }

        #endregion

        #region Properties

        public override int XC
        {
            get { return _wb.PixelWidth / 2; }
        }

        public override int YC
        {
            get { return _wb.PixelHeight / 2; }
        }
        
        #endregion

        #region Methods

        public override void DrawInto(WriteableBitmap wb, Vector2 position, Vector2 direction)
        {
            double angle = direction.DirectedAngle();
            SysWin.Point point = new SysWin.Point(position.X - XC, wb.PixelHeight - 1 - position.Y - YC);
            if (double.IsNaN(angle) || angle == 0)
            {
                wb.Blit(point, _wb, _sourceRect, _color, WriteableBitmapExtensions.BlendMode.Alpha);
            }
            else
            {
                wb.Blit(point, _wb.RotateFree((int)angle), _sourceRect, _color, WriteableBitmapExtensions.BlendMode.Alpha);
            }
        }
        
        #endregion
    }
}

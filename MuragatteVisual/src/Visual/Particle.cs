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
using Muragatte.Core.Environment;
using Muragatte.Core.Storage;
using SysWin = System.Windows;

namespace Muragatte.Visual
{
    public class Particle
    {
        #region Fields

        private WriteableBitmap _wb = null;
        private Color _color = Colors.Black;
        private SysWin.Rect _sourceRect;

        #endregion

        #region Constructors

        public Particle(WriteableBitmap wb, Color color)
        {
            _wb = wb;
            _color = color;
            _sourceRect = new SysWin.Rect(0, 0, _wb.PixelWidth, _wb.PixelHeight);
        }

        #endregion

        #region Properties

        public WriteableBitmap Image
        {
            get { return _wb; }
            set { _wb = value; }
        }

        public int XC
        {
            get { return _wb.PixelWidth / 2; }
        }

        public int YC
        {
            get { return _wb.PixelHeight / 2; }
        }

        public Color Color
        {
            get { return _color; }
        }

        public SysWin.Rect SourceRect
        {
            get { return _sourceRect; }
        }

        #endregion

        #region Methods

        public void DrawInto(WriteableBitmap wb, Vector2 position, Vector2 direction)
        {
            double angle = direction.DirectedAngle();
            SysWin.Point point = new SysWin.Point(position.X - XC, wb.PixelHeight - 1 - position.Y - YC);
            //SysWin.Point point = new SysWin.Point(position.X - XC, (position.Y - YC));
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

        #region Static Methods

        public static Particle Default(int size, Color color)
        {
            return Default(size, size, color);
        }

        public static Particle Default(int width, int height, Color color)
        {
            WriteableBitmap wb = BitmapFactory.New(width, height);
            wb.Clear();
            wb.FillEllipse(0, 0, width - 1, height - 1, Colors.White);
            return new Particle(wb, color);
        }

        public static Particle Agent(int size, Color color)
        {
            WriteableBitmap wb = BitmapFactory.New(size, size);
            wb.Clear();
            wb.FillEllipse(0, 0, size - 1, size - 1, Colors.White);
            int dirWidth = Math.Max(1, size / 5);
            wb.FillRectangle((size - dirWidth) / 2, 0, (size + dirWidth) / 2, size / 2, Colors.Transparent);
            return new Particle(wb, color);
        }

        public static Particle Ellipse(int size, Color color, bool filled)
        {
            return Ellipse(size, size, color, filled);
        }

        public static Particle Ellipse(int width, int height, Color color, bool filled)
        {
            WriteableBitmap wb = BitmapFactory.New(width, height);
            wb.Clear();
            if (filled)
            {
                wb.FillEllipse(0, 0, width - 1, height - 1, Colors.White);
            }
            else
            {
                wb.DrawEllipse(0, 0, width - 1, height - 1, Colors.White);
            }
            return new Particle(wb, color);
        }

        public static Particle Rectangle(int size, Color color, bool filled)
        {
            return Rectangle(size, size, color, filled);
        }

        public static Particle Rectangle(int width, int height, Color color, bool filled)
        {
            WriteableBitmap wb = BitmapFactory.New(width, height);
            if (filled)
            {
                wb.Clear(Colors.White);
            }
            else
            {
                wb.Clear();
                wb.DrawRectangle(0, 0, width - 1, height - 1, Colors.White);
            }
            return new Particle(wb, color);
        }

        //from image later (probably)

        #endregion
    }
}

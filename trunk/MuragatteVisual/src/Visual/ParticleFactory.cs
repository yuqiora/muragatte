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
//using Muragatte.Common;
//using SysWin = System.Windows;

namespace Muragatte.Visual
{
    public class ParticleFactory
    {
        #region Constructors

        private ParticleFactory() { }

        #endregion

        #region Static Methods

        public static Particle Elementary(Color color)
        {
            return new ElementaryParticle(color);
        }
        
        public static Particle Default(int size, Color color)
        {
            return Ellipse(size, size, color);
        }

        public static Particle AgentA(int size, Color color)
        {
            if (size == 1)
            {
                return new ElementaryParticle(color);
            }
            WriteableBitmap wb = BitmapFactory.New(size, size);
            if (size <= 2)
            {
                wb.Clear(color);
            }
            else
            {
                wb.Clear();
                wb.FillEllipse(0, 0, size - 1, size - 1, Colors.White);
                if (size >= 5)
                {
                    int dirWidth = size / 5;
                    wb.FillRectangle((size - dirWidth) / 2, 0, (size + dirWidth) / 2, size / 2, Colors.Transparent);
                }
            }
            return new ComplexParticle(wb, color);
        }

        public static Particle AgentB(int size, Color color)
        {
            if (size == 1)
            {
                return new ElementaryParticle(color);
            }
            WriteableBitmap wb = BitmapFactory.New(size, size);
            if (size <= 2)
            {
                wb.Clear(color);
            }
            else
            {
                wb.Clear();
                wb.FillTriangle(size / 2, 0, 1, size - 1, size - 2, size - 1, color);
            }
            return new ComplexParticle(wb, color);
        }

        public static Particle Ellipse(int size, Color color, bool filled = true)
        {
            return Ellipse(size, size, color, filled);
        }

        public static Particle Ellipse(int width, int height, Color color, bool filled = true)
        {
            if (width == 1 && height == 1)
            {
                return new ElementaryParticle(color);
            }
            WriteableBitmap wb = BitmapFactory.New(width, height);
            if (width <= 2 || height <= 2)
            {
                wb.Clear(color);
            }
            else
            {
                wb.Clear();
                if (filled)
                {
                    wb.FillEllipse(0, 0, width - 1, height - 1, Colors.White);
                }
                else
                {
                    wb.DrawEllipse(0, 0, width - 1, height - 1, Colors.White);
                }
            }
            return new ComplexParticle(wb, color);
        }

        public static Particle Rectangle(int size, Color color, bool filled = true)
        {
            return Rectangle(size, size, color, filled);
        }

        public static Particle Rectangle(int width, int height, Color color, bool filled = true)
        {
            if (width == 1 && height == 1)
            {
                return new ElementaryParticle(color);
            }
            WriteableBitmap wb = BitmapFactory.New(width, height);
            if (filled || width <= 2 || height <= 2)
            {
                wb.Clear(Colors.White);
            }
            else
            {
                wb.Clear();
                wb.DrawRectangle(0, 0, width - 1, height - 1, Colors.White);
            }
            return new ComplexParticle(wb, color);
        }
        
        #endregion
    }
}

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

namespace Muragatte.Visual
{
    public static class WBXExtensions
    {
        #region SetPixel

        public static void SetPixel(this WriteableBitmap wb, Vector2 position, Color color)
        {
            if (wb.Covers(position)) wb.SetPixel(position.Xi, position.Yi, color);
        }

        public static void SetPixel(this WriteableBitmap wb, Vector2 position, byte a, Color color)
        {
            if (wb.Covers(position)) wb.SetPixel(position.Xi, position.Yi, a, color);
        }

        #endregion

        #region Draw*

        public static void DrawEllipse(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Color color)
        {
            wb.DrawEllipse(p1.Xi, p1.Yi, p2.Xi, p2.Yi, color);
        }

        public static void DrawEllipseCentered(this WriteableBitmap wb, Vector2 center, int xr, int yr, Color color)
        {
            wb.DrawEllipseCentered(center.Xi, center.Yi, xr, yr, color);
        }

        public static void DrawRectangle(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Color color)
        {
            wb.DrawRectangle(p1.Xi, p1.Yi, p2.Xi, p2.Yi, color);
        }

        public static void DrawTriangle(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            wb.DrawTriangle(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, color);
        }

        public static void DrawQuad(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            wb.DrawQuad(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, p4.Xi, p4.Yi, color);
        }

        public static void DrawLine(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Color color)
        {
            wb.DrawLine(p1.Xi, p1.Yi, p2.Xi, p2.Yi, color);
        }

        public static void DrawBezier(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            wb.DrawBezier(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, p4.Xi, p4.Yi, color);
        }

        #endregion

        #region Fill*

        public static void FillEllipse(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Color color)
        {
            wb.FillEllipse(p1.Xi, p1.Yi, p2.Xi, p2.Yi, color);
        }

        public static void FillEllipseCentered(this WriteableBitmap wb, Vector2 center, int xr, int yr, Color color)
        {
            wb.FillEllipseCentered(center.Xi, center.Yi, xr, yr, color);
        }

        public static void FillRectangle(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Color color)
        {
            wb.FillRectangle(p1.Xi, p1.Yi, p2.Xi, p2.Yi, color);
        }

        public static void FillTriangle(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            wb.FillTriangle(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, color);
        }

        public static void FillQuad(this WriteableBitmap wb, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            wb.FillQuad(p1.Xi, p1.Yi, p2.Xi, p2.Yi, p3.Xi, p3.Yi, p4.Xi, p4.Yi, color);
        }

        #endregion

        #region Blit

        public static void Blit(this WriteableBitmap wb, Vector2 position, WriteableBitmap source)
        {
            wb.Blit(
                new System.Windows.Rect(
                    position.X - source.PixelWidth / 2,
                    position.Y - source.PixelHeight / 2,
                    source.PixelWidth, source.PixelHeight),
                source,
                new System.Windows.Rect(0, 0, source.PixelWidth, source.PixelHeight));
        }

        #endregion

        #region Color Related

        public static bool NotTransparent(this Color color)
        {
            return color.A > 0;
        }

        public static bool NotOpaque(this Color color)
        {
            return color.A < byte.MaxValue;
        }

        public static bool IsSemiTransparent(this Color color)
        {
            return NotTransparent(color) && NotOpaque(color);
        }

        public static Color WithA(this Color color, byte a)
        {
            Color c = color;
            c.A = a;
            return c;
        }

        #endregion

        #region Misc

        public static Vector2 Center(this WriteableBitmap wb)
        {
            return new Vector2(wb.PixelWidth / 2, wb.PixelHeight / 2);
        }

        public static bool Covers(this WriteableBitmap wb, Vector2 position)
        {
            return position.Xi >= 0 && position.Xi < wb.PixelWidth && position.Yi >= 0 && position.Yi < wb.PixelHeight;
        }

        #endregion
    }
}

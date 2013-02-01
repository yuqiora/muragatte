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
using System.Xml.Serialization;
using Muragatte.Common;

namespace Muragatte.Visual.Shapes
{
    public abstract class Shape
    {
        #region Constructors

        protected Shape() { }

        #endregion

        #region Properties

        public abstract string Symbol { get; }

        public abstract ShapeLabel Label { get; }

        #endregion

        #region Methods

        public virtual void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            if (width == 1 && height == 1)
            {
                PixelShape.Instance.Draw(target, position, angle, primaryColor, secondaryColor, width, height, other);
            }
            else
            {
                Draw(target, position, angle, primaryColor, secondaryColor, CreateCoordinates(width, height));
            }
        }

        public virtual void Draw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            if (primaryColor.IsSemiTransparent() || secondaryColor.IsSemiTransparent())
            {
                BlitDraw(target, position, angle, primaryColor, secondaryColor, coordinates);
            }
            else
            {
                FullDraw(target, position, angle, primaryColor, secondaryColor, coordinates);
            }
        }

        protected virtual void BlitDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            BlitDraw(target, position, angle, primaryColor, secondaryColor, CreateCoordinates(width, height));
        }

        protected virtual void BlitDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates)
        {
            coordinates[0].Bitmap.Clear();
            FullDraw(coordinates[0].Bitmap, coordinates[0].Bitmap.Center(), angle, primaryColor, secondaryColor, coordinates);
            target.Blit(position, coordinates[0].Bitmap);
        }

        protected virtual void FullDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, int width, int height, object other = null)
        {
            FullDraw(target, position, angle, primaryColor, secondaryColor, CreateCoordinates(width, height));
        }

        protected abstract void FullDraw(WriteableBitmap target, Vector2 position, Angle angle, Color primaryColor, Color secondaryColor, List<Coordinates> coordinates);

        public abstract List<Coordinates> CreateCoordinates(int width, int height, object other = null);

        protected List<Coordinates> ListOfOne(Coordinates item)
        {
            List<Coordinates> list = new List<Coordinates>();
            list.Add(item);
            return list;
        }

        #endregion
    }
}

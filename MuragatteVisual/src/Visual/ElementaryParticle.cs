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
    public class ElementaryParticle : Particle
    {
        #region Constructors

        public ElementaryParticle(Color color)
            : base(color) { }

        #endregion

        #region Properties

        public override int XC
        {
            get { return 0; }
        }

        public override int YC
        {
            get { return 0; }
        }

        #endregion

        #region Methods

        public override void DrawInto(WriteableBitmap wb, Vector2 position, Vector2 direction)
        {
            wb.SetPixel(position.Xi, wb.PixelHeight - 1 - position.Yi, _color);
        }

        #endregion
    }
}

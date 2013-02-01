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

namespace Muragatte.Core.Storage.ONG
{
    public enum Quadrant
    {
        NorthEast = 0,
        SouthEast = 1,
        SouthWest = 2,
        NorthWest = 3
    }

    public static class QuadrantExtensions
    {
        public static List<Quadrant> Others(this Quadrant q)
        {
            List<Quadrant> quadrants = new List<Quadrant>();
            quadrants.Add(Shift(q, 2));
            quadrants.Add(Shift(q, 1));
            quadrants.Add(Shift(q, 3));
            return quadrants;
        }

        public static Quadrant Opposite(this Quadrant q)
        {
            return Shift(q, 2);
        }

        private static Quadrant Shift(this Quadrant q, int n)
        {
            return (Quadrant)((int)(q + n) % 4);
        }

        public static Quadrant InvertX(this Quadrant q)
        {
            switch (q)
            {
                case Quadrant.NorthEast:
                    return Quadrant.NorthWest;
                case Quadrant.SouthEast:
                    return Quadrant.SouthWest;
                case Quadrant.SouthWest:
                    return Quadrant.SouthEast;
                case Quadrant.NorthWest:
                    return Quadrant.NorthEast;
                default:
                    return Quadrant.NorthEast;
            }
        }

        public static Quadrant InvertY(this Quadrant q)
        {
            switch (q)
            {
                case Quadrant.NorthEast:
                    return Quadrant.SouthEast;
                case Quadrant.SouthEast:
                    return Quadrant.NorthEast;
                case Quadrant.SouthWest:
                    return Quadrant.NorthWest;
                case Quadrant.NorthWest:
                    return Quadrant.SouthWest;
                default:
                    return Quadrant.NorthEast;
            }
        }

        public static int SignX(this Quadrant q)
        {
            return q == Quadrant.NorthEast || q == Quadrant.SouthEast ? +1 : -1;
        }

        public static int SignY(this Quadrant q)
        {
            return q == Quadrant.NorthEast || q == Quadrant.NorthWest ? +1 : -1;
        }
    }
}

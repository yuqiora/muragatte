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

namespace Muragatte.Common
{
    public enum Metric
    {
        Euclidean,
        Manhattan,
        Maximum
    }

    public static class MetricExtensions
    {
        public static double Distance(this Metric m, Vector2 a, Vector2 b)
        {
            return Distance(m, a, b, 0);
        }

        public static double Distance(this Metric m, Vector2 a, Vector2 b, double tolerance)
        {
            switch (m)
            {
                case Metric.Euclidean:
                    return Vector2.Distance(a, b) - tolerance;
                case Metric.Manhattan:
                    return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) - tolerance;
                case Metric.Maximum:
                    return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y)) - tolerance;
                default:
                    return double.NaN;
            }
        }

        public static double Distance(this Metric m, double x1, double y1, double x2, double y2)
        {
            return Distance(m, new Vector2(x1, y1), new Vector2(x2, y2), 0);
        }

        public static double Distance(this Metric m, double x1, double y1, double x2, double y2, double tolerance)
        {
            return Distance(m, new Vector2(x1, y1), new Vector2(x2, y2), tolerance);
        }
    }
}

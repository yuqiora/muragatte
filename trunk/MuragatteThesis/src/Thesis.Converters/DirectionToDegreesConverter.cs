// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Thesis Application
//
// Copyright (C) 2012  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Muragatte.Common;

namespace Muragatte.Thesis.Converters
{
    [ValueConversion(typeof(Vector2), typeof(double?))]
    public class DirectionToDegreesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Vector2 direction = (Vector2)value;
            return direction.IsZero ? (double?)null : direction.Angle.Degrees;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? degrees = (double?)value;
            return degrees.HasValue ? Vector2.X0Y1 + new Angle(degrees.Value) : Vector2.Zero;
        }
    }
}

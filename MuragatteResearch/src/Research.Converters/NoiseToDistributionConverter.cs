// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Research Application
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
using Muragatte.Random;

namespace Muragatte.Research.Converters
{
    [ValueConversion(typeof(Noise), typeof(Distribution))]
    public class NoiseToDistributionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Noise noise = value as Noise;
            return noise.Distribution;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Distribution distribution = (Distribution)value;
            return distribution.Noise(0, 0);
        }
    }
}

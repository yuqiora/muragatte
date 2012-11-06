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
using Muragatte.Core.Storage;


namespace Muragatte.Thesis.Converters
{
    [ValueConversion(typeof(IStorage), typeof(Type))]
    public class StorageToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IStorage storage = (IStorage)value;
            return storage.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = (Type)value;
            return type.IsSubclassOf(typeof(IStorage)) ? (IStorage)type.GetConstructor(Type.EmptyTypes).Invoke(null) : null;
        }
    }
}

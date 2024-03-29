﻿using System.Globalization;

namespace NovelEvolue.Tools
{
    public class RatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { // do not let the culture default to local to prevent variable outcome re decimal syntax
            double size = Math.Round(System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture), 0);
            return size.ToString("G0", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { // read only converter...
            throw new NotImplementedException();
        }

    }
}

// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data.Converters;

namespace TauStellwerk.Desktop.Util;

public class ByteSizeConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double size = System.Convert.ToDouble(value as long? ?? 0);
        if (size > 1024 * 1024 * 1024)
        {
            return $"{size / 1024 / 1024 / 1024:0.00} GiB";
        }
        if (size > 1024 * 1024)
        {
            return $"{size / 1024 / 1024:0.00} MiB";
        }
        if (size > 1024)
        {
            return $"{size / 1024:0.00} KiB";
        }
        return $"{size} B";
        
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

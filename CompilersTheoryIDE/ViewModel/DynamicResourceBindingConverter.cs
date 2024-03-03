using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CompilersTheoryIDE.ViewModel;

public class DynamicResourceBindingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string key && Application.Current.Resources.Contains(key))
        {
            return Application.Current.Resources[key];
        }

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using Avalonia.Data.Converters;
using Avalonia.Input;
using System;
using System.Globalization;

namespace CalculatorAvalonia.Converters
{
    public class DigitHotKeyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return KeyGesture.Parse("D" + value?.ToString());
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var notification = new Avalonia.Data.BindingNotification(value);
            notification.AddError(new NotImplementedException(), Avalonia.Data.BindingErrorType.Error);
            return notification.Error;
        }
    }
}

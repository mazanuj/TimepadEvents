using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TimepadEvents.Converters
{
	[ValueConversion(typeof (bool), typeof (Brushes))]
	internal class BoolToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value ? Brushes.Green : Brushes.Red;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Brushes.AliceBlue;
		}
	}
}
using Facebook_MKT.WPF.State.Navigators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Facebook_MKT.WPF.Controls
{
	public class ViewTypeToStyleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ViewType selectedViewType && parameter is ViewType buttonViewType)
			{
				if (selectedViewType == buttonViewType)
				{
					return Application.Current.Resources["SelectedButtonStyle"] as Style;
				}
				else
				{
					return Application.Current.Resources["DefaultButtonStyle"] as Style;
				}
			}
			return Application.Current.Resources["DefaultButtonStyle"] as Style;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

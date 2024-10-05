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
			// Kiểm tra nếu value là SelectedViewType và parameter là ViewType của nút
			if (value is ViewType selectedViewType && parameter is ViewType buttonViewType)
			{
				// So sánh ViewType của nút với SelectedViewType
				if (selectedViewType == buttonViewType)
				{
					// Nếu khớp thì trả về SelectedButtonStyle
					return Application.Current.Resources["SelectedButtonStyle"] as Style;
				}
				else
				{
					// Nếu không khớp thì trả về DefaultButtonStyle
					return Application.Current.Resources["DefaultButtonStyle"] as Style;
				}
			}

			// Trả về DefaultButtonStyle nếu không có giá trị hợp lệ
			return Application.Current.Resources["DefaultButtonStyle"] as Style;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


}

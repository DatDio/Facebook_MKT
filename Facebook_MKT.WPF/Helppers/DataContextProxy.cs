using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Facebook_MKT.WPF.Helppers
{
	public class DataContextProxy : Freezable
	{
		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.Register("DataContext", typeof(object), typeof(DataContextProxy), new PropertyMetadata(null));

		public object DataContext
		{
			get { return GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}

		protected override Freezable CreateInstanceCore()
		{
			return new DataContextProxy();
		}
	}
}

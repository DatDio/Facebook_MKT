using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Facebook_MKT.WPF.Helppers
{
	public class BrowserPositionHelper
	{
		public static int CurrentWidth = 0, CurrentHeight = 0;
		public static string GetNewPosition(int w, int h, double scale = 1)
		{
			var current_size = "";


			current_size = $"{CurrentWidth},{CurrentHeight}";

			CurrentWidth += w;

			if (CurrentWidth + w >= SystemParameters.PrimaryScreenWidth / scale)
			{
				CurrentWidth = 0;
				CurrentHeight += h;
			}

			if (CurrentHeight + h >= SystemParameters.PrimaryScreenHeight / scale)
			{
				CurrentWidth = 0;
				CurrentHeight = 0;
			}
			return current_size;
		}
	}
}

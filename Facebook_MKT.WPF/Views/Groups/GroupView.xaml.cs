﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Facebook_MKT.WPF.Views.Groups
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
    public partial class GroupView : UserControl
    {
        public GroupView()
        {
            InitializeComponent();
        }
		private void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			ExpanderContent.Visibility = Visibility.Visible;
		}

		private void Expander_Collapsed(object sender, RoutedEventArgs e)
		{
			ExpanderContent.Visibility = Visibility.Collapsed;
		}
	}
}

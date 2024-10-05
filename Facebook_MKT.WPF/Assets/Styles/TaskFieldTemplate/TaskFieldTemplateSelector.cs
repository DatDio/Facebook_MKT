using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Facebook_MKT.WPF.Models;
using Faceebook_MKT.Domain.Models;

namespace Facebook_MKT.WPF.Assets.Styles.TaskFieldTemplate
{
	public class TaskFieldTemplateSelector : DataTemplateSelector
	{
		public DataTemplate TextFieldTemplate { get; set; }
		public DataTemplate NumberFieldTemplate { get; set; }
		public DataTemplate FileFieldTemplate { get; set; }
		public DataTemplate LabelFieldTemplate { get; set; }
		public DataTemplate MultiTextFieldTemplate { get; set; }

		public DataTemplate MediaFieldTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var taskField = item as TaskField;
			return taskField?.FieldType switch
			{
				TaskFieldType.Text => TextFieldTemplate,
				TaskFieldType.MultiText => MultiTextFieldTemplate,
				TaskFieldType.Number => NumberFieldTemplate,
				TaskFieldType.File => FileFieldTemplate,
				TaskFieldType.Label=> LabelFieldTemplate,
				TaskFieldType.Media => MediaFieldTemplate,
				_ => base.SelectTemplate(item, container),
			};
		}
	}

}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Models
{
	public enum TaskFieldType
	{
		Text,
		MultiText,
		Number,
		File,
		Label,
		Media
	}

	public class TaskField : INotifyPropertyChanged
	{
		private string _label;
		private TaskFieldType _fieldType;
		private object _value;

		public string Label
		{
			get => _label;
			set
			{
				_label = value;
				OnPropertyChanged(nameof(Label));
			}
		}

		public TaskFieldType FieldType
		{
			get => _fieldType;
			set
			{
				_fieldType = value;
				OnPropertyChanged(nameof(FieldType));
			}
		}

		public object Value
		{
			get => _value;
			set
			{
				_value = value;
				OnPropertyChanged(nameof(Value));
			}
		}

		public TaskField(string label, TaskFieldType fieldType, object value = null)
		{
			Label = label;
			FieldType = fieldType;
			Value = value;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}



	public class TaskModel : INotifyPropertyChanged
	{
		private string _taskName;
		private List<TaskField> _fields;
		//private ObservableCollection<MediaFileModel> _mediaFile;
		private int _index;

		//public ObservableCollection<MediaFileModel> MediaFiles
		//{
		//	get { return _mediaFile; }
		//	set
		//	{
		//		_mediaFile = value;
		//		OnPropertyChanged(nameof(MediaFileModel));
		//	}
		//}
		public string TaskName
		{
			get => _taskName;
			set
			{
				_taskName = value;
				OnPropertyChanged(nameof(TaskName));
			}
		}

		public List<TaskField> Fields
		{
			get => _fields;
			set
			{
				_fields = value;
				OnPropertyChanged(nameof(Fields));
			}
		}

		public int Index
		{
			get => _index;
			set
			{
				_index = value;
				OnPropertyChanged(nameof(Index));
			}
		}

		public TaskModel()
		{
			Fields = new List<TaskField>();
			//MediaFiles = new ObservableCollection<MediaFileModel>();
		}

		public TaskModel(string taskName, List<TaskField> fields)
		{
			TaskName = taskName;
			Fields = fields;
			//MediaFiles = new ObservableCollection<MediaFileModel>();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}


}

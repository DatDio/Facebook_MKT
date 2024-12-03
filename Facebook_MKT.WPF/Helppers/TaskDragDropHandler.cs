using Facebook_MKT.WPF.Models;
using Faceebook_MKT.Domain.Models;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Facebook_MKT.WPF.Helppers
{
	public class TaskDragDropHandler : IDropTarget
	{
		private readonly ObservableCollection<TaskModel> _taskList;

		public TaskDragDropHandler(ObservableCollection<TaskModel> taskList)
		{
			_taskList = taskList;
		}

		public void DragOver(IDropInfo dropInfo)
		{
			dropInfo.Effects = DragDropEffects.Move;
		}

		public void Drop(IDropInfo dropInfo)
		{
			if (dropInfo.Data is not TaskModel sourceItem)
				return;

			try
			{
				int oldIndex = _taskList.IndexOf(sourceItem);
				int insertIndex = dropInfo.InsertIndex;

				if (insertIndex < 0)
				{
					insertIndex = 0;
				}
				else if (insertIndex > _taskList.Count)
				{
					insertIndex = _taskList.Count;
				}

				if (oldIndex == -1)
				{
					var newTaskItem = CloneTaskModel(sourceItem);
					_taskList.Insert(insertIndex, newTaskItem);
				}
				else
				{
					if (oldIndex != insertIndex)
					{
						_taskList.RemoveAt(oldIndex);
						_taskList.Insert(insertIndex, sourceItem);
					}
				}
			}
			catch
			{

			}

			// Cập nhật chỉ số cho tất cả các mục trong danh sách
			UpdateIndices();
		}


		private void UpdateIndices()
		{
			for (int i = 0; i < _taskList.Count; i++)
			{
				if (_taskList[i] is TaskModel taskModel)
				{
					taskModel.Index = i + 1; // Cập nhật chỉ số
				}
			}
		}
		// Hàm sao chép sâu TaskModel
		private TaskModel CloneTaskModel(TaskModel original)
		{
			return new TaskModel
			{
				Index = _taskList.Count + 1, // Gán chỉ số mới
				TaskName = original.TaskName,
				Fields = original.Fields.Select(field => CloneTaskField(field)).ToList() // Sao chép các trường
			};
		}

		// Hàm sao chép sâu từng TaskField và MediaFileModel nếu có
		private TaskField CloneTaskField(TaskField original)
		{
			var clonedField = new TaskField(original.Label, original.FieldType);

			// Nếu Value là ObservableCollection, tạo bản sao mới
			if (original.Value is ObservableCollection<MediaFileModel> mediaFiles)
			{
				clonedField.Value = new ObservableCollection<MediaFileModel>(mediaFiles.Select(m => new MediaFileModel
				{
					FilePath = m.FilePath,
					IsImage = m.IsImage,
					IsVideo = m.IsVideo,
					ThumbnailPath = m.ThumbnailPath
				}));
			}
			else
			{
				// Nếu Value không phải ObservableCollection, sao chép giá trị bình thường
				clonedField.Value = original.Value;
			}

			return clonedField;
		}
	}
}

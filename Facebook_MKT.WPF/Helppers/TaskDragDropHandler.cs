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
				// Nếu là một mục mới, bạn có thể tạo một bản sao của sourceItem
				var newTaskItem = new TaskModel
				{
					Index = insertIndex + 1,
					TaskName = sourceItem.TaskName,
					Fields = sourceItem.Fields.ToList() // Sao chép danh sách các trường
				};
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
	}
}

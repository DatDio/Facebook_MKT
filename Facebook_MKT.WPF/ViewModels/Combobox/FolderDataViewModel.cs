using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook_MKT.Data.Services;

namespace Facebook_MKT.WPF.ViewModels.Combobox
{
	public class FolderDataViewModel<T> : BaseViewModel where T : class
	{
		private readonly IDataService<T> _dataService;
		public ObservableCollection<T> Items { get; private set; } = new ObservableCollection<T>(); // Sử dụng ObservableCollection

		public T _selectedItem;
		public T SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
				OnSelectedItemChanged();
			}
		}

		

		public event Action<int> FolderChanged; // Sự kiện để thông báo thay đổi FolderIdKey

		public FolderDataViewModel(IDataService<T> dataService)
		{
			_dataService = dataService;
			LoadData().ConfigureAwait(false);
		}

		private async Task LoadData()
		{
			var result = await _dataService.GetAll();
			Items.Clear();

			foreach (var item in result)
			{
				Items.Add(item);
			}

			// Đặt mục mặc định nếu cần
			if (typeof(T) == typeof(Folder))
			{
				var folderItems = Items.Cast<Folder>().ToList();
				SelectedItem = folderItems.FirstOrDefault(f => f.FolderName == "All") as T;
			}
		}

		private void OnSelectedItemChanged()
		{
			if (SelectedItem is Folder folder)
			{
				FolderChanged?.Invoke(folder.FolderIdKey);
			}
		}
	}


}

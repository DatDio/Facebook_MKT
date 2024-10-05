using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Faceebook_MKT.Domain.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace Facebook_MKT.WPF.ViewModels.DataGrid
{
	public abstract class BaseSelectableViewModel<T> : BaseViewModel
	   where T : class, INotifyPropertyChanged, ISelectable
	{

		private bool _isAllItemsSelected = false;
		public bool IsAllItemsSelected
		{
			get { return _isAllItemsSelected; }
			set
			{
				_isAllItemsSelected = value;
				OnPropertyChanged(nameof(IsAllItemsSelected));

				// Cập nhật tất cả các AccountModel
				foreach (var account in Items)
				{
					account.IsSelected = _isAllItemsSelected;  // Cập nhật IsSelected cho mỗi mục
				}
			}
		}

		private int _totalRows;
		public int TotalRows
		{
			get
			{
				return _totalRows;
			}
			set
			{
				_totalRows = value;
				OnPropertyChanged(nameof(TotalRows));
			}
		}

		private ObservableCollection<T> _itemsSelected = new ObservableCollection<T>();
		public ObservableCollection<T> ItemsSelected
		{
			get => _itemsSelected;
			set
			{
				_itemsSelected = value;
				OnPropertyChanged(nameof(ItemsSelected));
				OnPropertyChanged(nameof(HasItemsSelected)); // Cập nhật khi ItemsSelected thay đổi
			}
		}

		public ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

		public bool HasItemsSelected => ItemsSelected != null && ItemsSelected.Count > 0;

		public BaseSelectableViewModel()
		{
			Items.CollectionChanged += Items_CollectionChanged;

			// Lắng nghe sự thay đổi của ItemsSelected
			ItemsSelected.CollectionChanged += (s, e) =>
			{
				OnPropertyChanged(nameof(HasItemsSelected));
			};
		}

		private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (T newItem in e.NewItems)
				{
					newItem.PropertyChanged += Item_PropertyChanged;
				}
			}

			if (e.OldItems != null)
			{
				foreach (T oldItem in e.OldItems)
				{
					oldItem.PropertyChanged -= Item_PropertyChanged;
				}
			}

			// Cập nhật khi Items thay đổi
			OnPropertyChanged(nameof(Items));
			TotalRows = Items?.Count ?? 0;
			OnPropertyChanged(nameof(TotalRows));
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ISelectable.IsSelected))
			{
				var item = sender as T;
				if (item != null)
				{
					if (item.IsSelected && !ItemsSelected.Contains(item))
					{
						ItemsSelected.Add(item);  // Thêm vào ItemsSelected khi IsSelected là true
					}
					else if (!item.IsSelected && ItemsSelected.Contains(item))
					{
						ItemsSelected.Remove(item);  // Xóa khỏi ItemsSelected khi IsSelected là false
					}

					// Cập nhật HasItemsSelected
					OnPropertyChanged(nameof(HasItemsSelected));
				}
			}
		}
	}
}

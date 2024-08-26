using Faceebook_MKT.Domain.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace Facebook_MKT.WPF.ViewModels.DataGrid
{
	public abstract class BaseSelectableViewModel<T> : BaseViewModel
		where T : class, INotifyPropertyChanged, ISelectable
	{
		
		public ObservableCollection<T> ItemsSelected { get; set; }
		public ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

		public BaseSelectableViewModel()
		{
			Items.CollectionChanged += Items_CollectionChanged;
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
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ISelectable.IsSelected))
			{
				var item = sender as T;
				if (item.IsSelected)
				{
					ItemsSelected.Add(item);
				}
				else
				{
					ItemsSelected.Remove(item);
				}
			}
		}
	}
}

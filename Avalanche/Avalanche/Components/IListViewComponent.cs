using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche.Components
{
    public interface IListViewComponent
    {
        bool IsRefreshing { get; set; }
        ObservableCollection<MobileListViewItem> ItemsSource { get; set; }
        object SelectedItem { get; set; }
        double FontSize { get; set; }
        double IconSize { get; set; }
        Color TextColor { get; set; }
        Color IconColor { get; set; }
        double Columns { get; set; }

        event EventHandler Refreshing;
        event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        event EventHandler<ItemVisibilityEventArgs> ItemAppearing;
    }
}

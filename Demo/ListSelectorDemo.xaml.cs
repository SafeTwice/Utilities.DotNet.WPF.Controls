using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities.DotNet.Collections.Observables;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class ListSelectorDemo : UserControl, INotifyPropertyChanged
    {
        public ObservableList<string> AvailableItems { get; } = new()
        {
            "Item 1",
            "Item 9",
            "Item 2",
            "Item 4",
            "Item 3",
            "Item 5",
            "Item 7",
            "Item 6",
            "Item 8",
            "Item 10",
        };

        public ObservableList<string> SelectedItems { get; } = new();

        public bool IsAvailableItemsOrderEnabled { get; set; } = false;

        public bool IsSelectedItemsOrderEnabled { get; set; } = false;

        public ListSelectorDemo()
        {
            InitializeComponent();
        }

        // Called by Fody/PropertyChanged
        private void OnIsAvailableItemsOrderEnabledChanged()
        {
            AvailableItemsView.SortDescriptions.Clear();
            if( IsAvailableItemsOrderEnabled )
            {
                AvailableItemsView.SortDescriptions.Add( new SortDescription( string.Empty, ListSortDirection.Ascending ) );
            }
        }

        // Called by Fody/PropertyChanged
        private void OnIsSelectedItemsOrderEnabledChanged()
        {
            SelectedItemsView.SortDescriptions.Clear();
            if( IsSelectedItemsOrderEnabled )
            {
                SelectedItemsView.SortDescriptions.Add( new SortDescription( string.Empty, ListSortDirection.Ascending ) );
            }
        }

        private ICollectionView AvailableItemsView => CollectionViewSource.GetDefaultView( AvailableItems );
        private ICollectionView SelectedItemsView => CollectionViewSource.GetDefaultView( SelectedItems );
    }
}

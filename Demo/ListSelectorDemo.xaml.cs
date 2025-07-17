using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities.DotNet.Collections.Observables;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public class DemoItem
    {
        public string Name => $"Item {Id}";

        public int Id { get; }

        public DemoItem( int id ) => Id = id;
    }

    public partial class ListSelectorDemo : UserControl, INotifyPropertyChanged
    {
        public ObservableList<DemoItem> AvailableItems { get; } = new()
        {
            new DemoItem( 10 ),
            new DemoItem( 1 ),
            new DemoItem( 9 ),
            new DemoItem( 2 ),
            new DemoItem( 4 ),
            new DemoItem( 3 ),
            new DemoItem( 5 ),
            new DemoItem( 7 ),
            new DemoItem( 6 ),
            new DemoItem( 8 ),
        };

        public ObservableList<DemoItem> SelectedItems { get; } = new();

        public bool IsAvailableItemsOrderEnabled { get; set; } = false;

        public bool IsSelectedItemsOrderEnabled { get; set; } = false;

        public bool IsAvailableItemsFilterEnabled { get; set; } = false;

        public string LogText { get; private set; } = string.Empty;

        public ListSelectorDemo()
        {
            InitializeComponent();

            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        private void SelectedItems_CollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
        {
            switch( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    foreach( var item in e.NewItems!.Cast<DemoItem>() )
                    {
                        LogText = $"Selected {item.Name}\n" + LogText;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach( var item in e.OldItems!.Cast<DemoItem>() )
                    {
                        LogText = $"Unselected {item.Name}\n" + LogText;
                    }
                    break;

                default:
                    LogText = $"Unsupported action {e.Action}\n" + LogText;
                    break;
            }
        }


#pragma warning disable S1144, IDE0051 // Called by Fody/PropertyChanged

        private void OnIsAvailableItemsOrderEnabledChanged()
        {
            AvailableItemsView.SortDescriptions.Clear();
            if( IsAvailableItemsOrderEnabled )
            {
                AvailableItemsView.SortDescriptions.Add( new SortDescription( nameof( DemoItem.Id ), ListSortDirection.Ascending ) );
            }
        }

        private void OnIsSelectedItemsOrderEnabledChanged()
        {
            SelectedItemsView.SortDescriptions.Clear();
            if( IsSelectedItemsOrderEnabled )
            {
                SelectedItemsView.SortDescriptions.Add( new SortDescription( nameof( DemoItem.Id ), ListSortDirection.Ascending ) );
            }
        }

        private void OnIsAvailableItemsFilterEnabledChanged()
        {
            AvailableItemsView.Filter = null;
            if( IsAvailableItemsFilterEnabled )
            {
                AvailableItemsView.Filter = item => ( ( ( (DemoItem) item ).Id % 2 ) == 1 );
            }
        }

#pragma warning restore S1144, IDE0051

        private ICollectionView AvailableItemsView => CollectionViewSource.GetDefaultView( AvailableItems );
        private ICollectionView SelectedItemsView => CollectionViewSource.GetDefaultView( SelectedItems );
    }
}

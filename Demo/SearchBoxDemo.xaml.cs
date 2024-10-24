using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class SearchBoxDemo : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<string> SearchHistory { get; } = new ObservableCollection<string>();

        public string SearchingText { get; set; } = "No search text yet...";

        public SearchBoxDemo()
        {
            InitializeComponent();

            SearchBox_ClearButtonPosition.ItemsSource = Enum.GetValues( typeof( SearchBox.EHorizontalPosition ) );
            SearchBox_ClearButtonPosition.SelectedIndex = 1;
        }

        private void SearchBox_Find( object sender, FindEventArgs e )
        {
            var searchedText = e.Text;

            SearchingText = $"[{( e.SearchBackwards ? "Backward" : "Forward" )}] {searchedText}";

            var searchHistoryIndex = SearchHistory.IndexOf( searchedText );

            if( searchHistoryIndex > 0 )
            {
                SearchHistory.Move( searchHistoryIndex, 0 );
            }

            if( searchHistoryIndex < 0 )
            {
                SearchHistory.Insert( 0, searchedText );
            }

            if( SearchHistory.Count > 10 )
            {
                SearchHistory.RemoveAt( SearchHistory.Count - 1 );
            }
        }
    }
}

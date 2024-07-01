using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Themes

        public string[] AvailableThemes => THEMES;

        public int SelectedThemeIndex { get; set; } = 0;

        #endregion

        public bool IsContentToolbarVisible { get; set; } = false;

        #region SearchBox

        public ObservableCollection<string> SearchHistory { get; } = new ObservableCollection<string>();

        public string SearchingText { get; set; } = "No search text yet...";

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            SearchBox_ClearButtonPosition.ItemsSource = Enum.GetValues( typeof( SearchBox.EHorizontalPosition ) );
            SearchBox_ClearButtonPosition.SelectedIndex = 1;

            FileSelectionBox_ButtonPosition.ItemsSource = Enum.GetValues( typeof( FileSelectionBox.EHorizontalPosition ) );
            FileSelectionBox_ButtonPosition.SelectedIndex = 1;

            DataContext = this;
        }

        private void Window_OnKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if( ( e.Key == System.Windows.Input.Key.F3 ) && ToolbarsTabItem.IsSelected )
            {
                IsContentToolbarVisible = true;
                e.Handled = true;
            }
        }

        private void SlidingToolbar_IsCloseButtonVisible_Changed( object sender, RoutedEventArgs e )
        {
            SlidingToolbar.Visibility = Visibility.Visible;
        }

        private void SearchBox_Find( object sender, FindEventArgs e )
        {
            var searchedText = e.Text;

            SearchingText = $"[{(e.SearchBackwards?"Backward":"Forward")}] {searchedText}";

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

        private void OnSelectedThemeIndexChanged()
        {
            var themeResources = Application.Current.Resources.MergedDictionaries[ THEME_RESOURCES_INDEX ];

            string? wpfThemeUri = null;
            //string? libThemeUri = "/Utilities.DotNet.WPF.Controls;component/Themes/Generic.xaml";

            switch( SelectedThemeIndex )
            {
                case 1:
                    wpfThemeUri = "/PresentationFramework.Classic;component/themes/Classic.xaml";
                    break;
                case 2:
                    wpfThemeUri = "/PresentationFramework.Royale;component/themes/Royale.NormalColor.xaml";
                    break;
                case 3:
                    wpfThemeUri = "/PresentationFramework.Luna;component/themes/Luna.NormalColor.xaml";
                    break;
                case 4:
                    wpfThemeUri = "/PresentationFramework.Luna;component/themes/Luna.Metallic.xaml";
                    break;
                case 5:
                    wpfThemeUri = "/PresentationFramework.Luna;component/themes/Luna.Homestead.xaml";
                    break;
                case 6:
                    wpfThemeUri = "/PresentationFramework.Aero;component/themes/Aero.NormalColor.xaml";
                    //libThemeUri = "/Utilities.DotNet.WPF.Controls;component/Themes/Aero.NormalColor.xaml";
                    break;
                case 7:
                    wpfThemeUri = "/PresentationFramework.AeroLite;component/themes/AeroLite.NormalColor.xaml";
                    //libThemeUri = "/Utilities.DotNet.WPF.Controls;component/Themes/AeroLite.NormalColor.xaml";
                    break;
                case 8:
                    wpfThemeUri = "/PresentationFramework.Aero2;component/themes/Aero2.NormalColor.xaml";
                    break;
                default:
                    break;
            };

            themeResources.Clear();

            if( wpfThemeUri != null )
            {
                themeResources.Source = new Uri( wpfThemeUri, UriKind.Relative );
            }
            else
            {
                themeResources.Clear();
            }

            ReloadDynamicResources();

            /*if( libThemeUri != null )
            {
                themesResource.MergedDictionaries.Add( new ResourceDictionary()
                {
                    Source = new Uri( libThemeUri, UriKind.Relative )
                } );
            }*/
        }

        private static void ReloadDynamicResources()
        {
            var dynamicResources = Application.Current.Resources.MergedDictionaries[ DYNAMIC_RESOURCES_INDEX ];
            var originalSource = dynamicResources.Source;
            dynamicResources.Clear();
            dynamicResources.Source = originalSource;
        }

        private const int THEME_RESOURCES_INDEX = 0;
        private const int DYNAMIC_RESOURCES_INDEX = 1;

        private static readonly string[] THEMES = new string[]
        {
            "Default",
            "Classic (Win95/98/2000)",
            "Royale (WinXP Media Center)",
            "Luna (WinXP)",
            "Luna Silver (WinXP)",
            "Luna Olive (WinXP)",
            "Aero (Win7)",
            "AeroLite (Win8/10)",
            "Aero2 (Win8/10)",
        };
    }
}
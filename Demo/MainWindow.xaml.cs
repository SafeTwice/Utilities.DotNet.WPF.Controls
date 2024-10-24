using System.ComponentModel;
using System.Windows;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Themes

        public string[] AvailableThemes => THEMES;

        public int SelectedThemeIndex { get; set; } = 0;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Window_OnKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if( ( e.Key == System.Windows.Input.Key.F3 ) && ToolbarsTabItem.IsSelected )
            {
                ToolbarsDemo.IsContentToolbarVisible = true;
                e.Handled = true;
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
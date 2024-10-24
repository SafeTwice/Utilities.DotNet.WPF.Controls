using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class ToolbarsDemo : UserControl, INotifyPropertyChanged
    {
        public bool IsContentToolbarVisible { get; set; } = false;

        public ToolbarsDemo()
        {
            InitializeComponent();
        }

        private void SlidingToolbar_IsCloseButtonVisible_Changed( object sender, RoutedEventArgs e )
        {
            SlidingToolbar.Visibility = Visibility.Visible;
        }
    }
}

using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class FileSelectionBoxDemo : UserControl
    {
        public FileSelectionBoxDemo()
        {
            InitializeComponent();

            FileSelectionBox_ButtonPosition.ItemsSource = Enum.GetValues( typeof( FileSelectionBox.EHorizontalPosition ) );
            FileSelectionBox_ButtonPosition.SelectedIndex = 1;
        }
    }
}

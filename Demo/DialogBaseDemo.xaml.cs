using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.DotNet.WPF.Commands;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class DialogBaseDemo : UserControl, INotifyPropertyChanged
    {
        public ICommand ShowDialogCommand { get; }

        public string Result { get; private set; } = "No result yet...";

        public bool IsOkEnabled { get; set; } = true;

        public DialogBaseDemo()
        {
            ShowDialogCommand = new DelegateCommand( ShowDialog );

            InitializeComponent();
        }

        private void ShowDialog()
        {
            var dialog = new SampleDialog( Window.GetWindow( this ) );
            dialog.IsOkEnabled = IsOkEnabled;

            var result = dialog.ShowDialog();

            Result = result.HasValue ? ( result.Value ? "OK" : "Cancel" ) : "No result";
        }
    }
}

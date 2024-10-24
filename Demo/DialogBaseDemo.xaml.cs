using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.DotNet.WPF.Commands;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class DialogBaseDemo : UserControl, INotifyPropertyChanged
    {
        public ICommand DialogBase_ShowDialogCommand { get; }

        public string DialogBase_Result { get; private set; } = "No result yet...";

        public bool DialogBase_IsOkEnabled { get; set; } = true;

        public DialogBaseDemo()
        {
            DialogBase_ShowDialogCommand = new DelegateCommand( DialogBase_ShowDialog );

            InitializeComponent();
        }

        private void DialogBase_ShowDialog()
        {
            var dialog = new SampleDialog( Window.GetWindow( this ) );
            dialog.IsOkEnabled = DialogBase_IsOkEnabled;

            var result = dialog.ShowDialog();

            DialogBase_Result = result.HasValue ? ( result.Value ? "OK" : "Cancel" ) : "No result";
        }
    }
}

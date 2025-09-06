using System.ComponentModel;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class DualIntegerUpDownDemo : UserControl, INotifyPropertyChanged
    {
        public int Value { get; set; } = ( 17 * 60 ) + 50;

        public int MinValue { get; set; } = 10;

        public int MaxValue { get; set; } = 99 * 60;

        public int Factor { get; set; } = 60;

        public string UpperLabel { get; set; } = "hr";

        public string LowerLabel { get; set; } = "min";

        public DualIntegerUpDownDemo()
        {
            InitializeComponent();
        }
    }
}

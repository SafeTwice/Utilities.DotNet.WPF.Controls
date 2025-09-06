using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Utilities.DotNet.WPF.Controls.Demo
{
    public partial class IntegerUpDownDemo : UserControl, INotifyPropertyChanged
    {
        public int Value { get; set; } = 50;

        public int MinValue { get; set; } = 0;

        public int MaxValue { get; set; } = 100;

        public bool CustomValueToText { get; set; } = false;

        [DependsOn( nameof( CustomValueToText ) )]
        public Func<IntegerUpDown, int, string> ValueToText => CustomValueToText ? InternalValueToText : IntegerUpDown.DefaultValueToText;

        public IntegerUpDownDemo()
        {
            InitializeComponent();
        }

        private static string InternalValueToText( IntegerUpDown integerUpDown, int value )
        {
            return $"{value} €";
        }
    }
}

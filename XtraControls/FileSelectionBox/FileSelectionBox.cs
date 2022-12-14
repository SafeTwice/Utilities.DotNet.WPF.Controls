using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace XtraControls
{
    public class FileSelectionBox : TextBox
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        public static readonly DependencyProperty ExtensionProperty =
            DependencyProperty.Register( nameof( Extension ), typeof( string ), typeof( FileSelectionBox ),
                new FrameworkPropertyMetadata( string.Empty ) );

        [Bindable( true )]
        [Browsable( true )]
        public string Extension
        {
            get => (string) GetValue( ExtensionProperty );
            set => SetValue( ExtensionProperty, value );
        }

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register( nameof( Filter ), typeof( string ), typeof( FileSelectionBox ),
                new FrameworkPropertyMetadata( string.Empty ) );

        [Bindable( true )]
        [Browsable( true )]
        public string Filter
        {
            get => (string) GetValue( FilterProperty );
            set => SetValue( FilterProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        static FileSelectionBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( FileSelectionBox ), new FrameworkPropertyMetadata( typeof( FileSelectionBox ) ) );
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var fileDialogButton = GetTemplateChild( "FileDialogButton" ) as Button;
            if( fileDialogButton != null )
            {
                fileDialogButton.Click += FileDialogButton_Click;
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void FileDialogButton_Click( object sender, RoutedEventArgs e )
        {
            var openFileDialog = new OpenFileDialog();

            if( Text.Length > 0 )
            {
                var directory = Path.GetDirectoryName( Text );
                if( directory != null )
                {
                    openFileDialog.InitialDirectory = directory;
                }

                if( File.Exists( Text ) )
                {
                    openFileDialog.FileName = Text;
                }
            }
            openFileDialog.DefaultExt = Extension;
            openFileDialog.Filter = Filter;

            var result = openFileDialog.ShowDialog();
            if( result == true )
            {
                var text = openFileDialog.FileName;
                Text = text;

                var filenameTextBox = GetTemplateChild( "FilenameTextBox" ) as TextBox;
                if( filenameTextBox != null )
                {
                    filenameTextBox.Focus();
                    filenameTextBox.Select( text.Length, 0 );
                }
            }
        }
    }
}

/// @file
/// @copyright  Copyright (c) 2022-2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Control that allows the user to select a file using a text box which can open a file dialog.
    /// </summary>
    [TemplatePart( Name = "PART_FileDialogButton", Type = typeof( Button ) )]
    [TemplatePart( Name = "PART_FilenameTextBox", Type = typeof( TextBox ) )]
    public class FileSelectionBox : TextBox
    {
        //===========================================================================
        //                          PUBLIC NESTED TYPES
        //===========================================================================

        /// <summary>
        /// Specifies an horizontal position.
        /// </summary>
        public enum EHorizontalPosition
        {
            Left,
            Right
        }

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

        public static readonly DependencyProperty ButtonPositionProperty =
            DependencyProperty.Register( nameof( ButtonPosition ), typeof( EHorizontalPosition ), typeof( FileSelectionBox ),
                new FrameworkPropertyMetadata( EHorizontalPosition.Right, FrameworkPropertyMetadataOptions.AffectsArrange ) );

        [Bindable( true )]
        [Browsable( true )]
        public EHorizontalPosition ButtonPosition
        {
            get => (EHorizontalPosition) GetValue( ButtonPositionProperty );
            set => SetValue( ButtonPositionProperty, value );
        }

        //===========================================================================
        //                          INTERNAL PROPERTIES
        //===========================================================================

        private static readonly DependencyPropertyKey ButtonMarginPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( ButtonMargin ), typeof( Thickness ), typeof( FileSelectionBox ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( false )]
        [Browsable( false )]
        internal Thickness ButtonMargin
        {
            get => (Thickness) GetValue( ButtonMarginPropertyKey.DependencyProperty );
            private set => SetValue( ButtonMarginPropertyKey, value );
        }

        private static readonly DependencyPropertyKey TextBoxPaddingPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( TextBoxPadding ), typeof( Thickness ), typeof( FileSelectionBox ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( false )]
        [Browsable( false )]
        internal Thickness TextBoxPadding
        {
            get => (Thickness) GetValue( TextBoxPaddingPropertyKey.DependencyProperty );
            private set => SetValue( TextBoxPaddingPropertyKey, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        static FileSelectionBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( FileSelectionBox ), new FrameworkPropertyMetadata( typeof( FileSelectionBox ) ) );

            PaddingProperty.OverrideMetadata( typeof( FileSelectionBox ),
                new FrameworkPropertyMetadata( new Thickness(), FrameworkPropertyMetadataOptions.AffectsArrange ) );

            // BorderThickness and BorderBrush are not bound to the ComboBox properties in order to use the current theme ComboBox defaults
            // if the value is not overridden by the user.

            BorderThicknessProperty.OverrideMetadata( typeof( FileSelectionBox ),
                new FrameworkPropertyMetadata( new Thickness(), FrameworkPropertyMetadataOptions.AffectsArrange, OnBorderThicknessPropertyChanged ) );

            BorderBrushProperty.OverrideMetadata( typeof( FileSelectionBox ), new FrameworkPropertyMetadata( OnBorderBrushPropertyChanged ) );
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if( m_fileDialogButton != null )
            {
                m_fileDialogButton.Click -= FileDialogButton_OnClick;
            }

            m_fileDialogButton = GetTemplateChild( "PART_FileDialogButton" ) as Button;

            if( m_fileDialogButton != null )
            {
                m_fileDialogButton.Click += FileDialogButton_OnClick;
            }

            m_filenameTextBox = GetTemplateChild( "PART_FilenameTextBox" ) as TextBox;

            TransferTextBoxProperties();
        }

        protected override Size ArrangeOverride( Size finalSize )
        {
            var result = base.ArrangeOverride( finalSize );

            Arrange();

            return result;
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        protected override void OnGotKeyboardFocus( KeyboardFocusChangedEventArgs e )
        {
            Debug.Print( $"FileSelectionBox.OnGotKeyboardFocus: {e.Handled} / {e.NewFocus} / {e.OldFocus}" );
            if( !e.Handled && ( m_filenameTextBox != null ) && ( e.NewFocus == this ) )
            {
                Keyboard.Focus( m_filenameTextBox );
                e.Handled = true;
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void Arrange()
        {
            ButtonMargin = new Thickness( BorderThickness.Left + 1, BorderThickness.Top + 1, BorderThickness.Right + 1,
                                          BorderThickness.Bottom + 1 );

            if( m_fileDialogButton != null )
            {
                var buttonWidth = m_fileDialogButton.ActualWidth;

                if( ButtonPosition == EHorizontalPosition.Right )
                {
                    TextBoxPadding = new Thickness( Padding.Left, Padding.Top, Padding.Right + buttonWidth + 1, Padding.Bottom );
                }
                else
                {
                    TextBoxPadding = new Thickness( Padding.Left + buttonWidth + 1, Padding.Top, Padding.Right, Padding.Bottom );
                }
            }
            else
            {
                TextBoxPadding = Padding;
            }
        }

        private static void OnBorderBrushPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is FileSelectionBox fileSelectionBox )
            {
                fileSelectionBox.OnBorderBrushChanged( (Brush) e.NewValue );
            }
        }

        private void OnBorderBrushChanged( Brush newValue )
        {
            if( m_filenameTextBox != null )
            {
                m_filenameTextBox.BorderBrush = newValue;
            }
        }

        private static void OnBorderThicknessPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is FileSelectionBox fileSelectionBox )
            {
                fileSelectionBox.OnBorderThicknessChanged( (Thickness) e.NewValue );
            }
        }

        private void OnBorderThicknessChanged( Thickness newValue )
        {
            if( m_filenameTextBox != null )
            {
                m_filenameTextBox.BorderThickness = newValue;
            }
        }

        private void TransferTextBoxProperties()
        {
            var borderThickness = ReadLocalValue( BorderThicknessProperty );
            if( borderThickness != DependencyProperty.UnsetValue )
            {
                OnBorderThicknessChanged( (Thickness) borderThickness );
            }

            var borderBrush = ReadLocalValue( BorderBrushProperty );
            if( borderBrush != DependencyProperty.UnsetValue )
            {
                OnBorderBrushChanged( (Brush) borderBrush );
            }
        }

        private void FileDialogButton_OnClick( object sender, RoutedEventArgs e )
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

                if( m_filenameTextBox != null )
                {
                    m_filenameTextBox.Focus();
                    m_filenameTextBox.Select( text.Length, 0 );
                }
            }
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private TextBox? m_filenameTextBox;
        private Button? m_fileDialogButton;
    }
}

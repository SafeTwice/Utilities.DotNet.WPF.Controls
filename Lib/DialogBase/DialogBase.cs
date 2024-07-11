/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Base class for dialog windows.
    /// </summary>
    [TemplatePart( Name = "PART_OkButton", Type = typeof( Button ) )]
    public class DialogBase : Window
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for <see cref="IsOkEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsOkEnabledProperty =
            DependencyProperty.Register( nameof( IsOkEnabled ), typeof( bool ), typeof( DialogBase ),
                new FrameworkPropertyMetadata( true ) );

        /// <summary>
        /// Indicating whether the OK button is enabled.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Indicates whether the OK button is enabled." )]
        public bool IsOkEnabled
        {
            get => (bool) GetValue( IsOkEnabledProperty );
            set => SetValue( IsOkEnabledProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        static DialogBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( DialogBase ), new FrameworkPropertyMetadata( typeof( DialogBase ) ) );
        }

        public DialogBase() : this( null )
        {
        }

        public DialogBase( Window? owner )
        {
            Owner = owner ?? Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if( m_okButton != null )
            {
                m_okButton.Click -= OkButton_Click;
            }

            m_okButton = GetTemplateChild( "PART_OkButton" ) as Button;

            if( m_okButton != null )
            {
                m_okButton.Click += OkButton_Click;
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OkButton_Click( object sender, RoutedEventArgs e )
        {
            DialogResult = true;
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private Button? m_okButton;
    }
}

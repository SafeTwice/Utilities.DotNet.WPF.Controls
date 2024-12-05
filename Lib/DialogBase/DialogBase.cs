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

        /// <summary>
        /// Constructor for dialogs not owned by a window.
        /// </summary>
        public DialogBase() : this( null )
        {
        }

        /// <summary>
        /// Constructor for dialogs owned by a window.
        /// </summary>
        /// <param name="owner">Window that owns the dialog.</param>
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
        //                            PROTECTED METHODS
        //===========================================================================

        /// <summary>
        /// Called when the OK button is clicked.
        /// </summary>
        /// <remarks>
        /// Override this method in derived classes to perform additional checks or processing before closing the dialog when
        /// the OK button is pressed.
        /// </remarks>
        /// <returns><c>true</c> if the dialog is allowed to be closed; <c>false</c> to keep the dialog open.</returns>
        protected virtual bool OnOkClick()
        {
            return true;
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OkButton_Click( object sender, RoutedEventArgs e )
        {
            if( OnOkClick() )
            {
                DialogResult = true;
            }
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private Button? m_okButton;
    }
}

/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Toolbar that can be hidden and resized horizontally.
    /// </summary>
    [TemplatePart( Name = "PART_CloseButton", Type = typeof( Button ) )]
    public class SlidingToolbar : ContentControl
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        public static readonly DependencyProperty IsCloseButtonVisibleProperty =
            DependencyProperty.Register( nameof( IsCloseButtonVisible ), typeof( bool ), typeof( SlidingToolbar ),
                new FrameworkPropertyMetadata( true, OnIsCloseButtonVisibleChanged ) );

        [Bindable( true )]
        [Browsable( true )]
        public bool IsCloseButtonVisible
        {
            get => (bool) GetValue( IsCloseButtonVisibleProperty );
            set => SetValue( IsCloseButtonVisibleProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        static SlidingToolbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( SlidingToolbar ), new FrameworkPropertyMetadata( typeof( SlidingToolbar ) ) );

            VisibilityProperty.OverrideMetadata( typeof( SlidingToolbar ),
                new FrameworkPropertyMetadata( Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ) );
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if( m_closeButton != null )
            {
                m_closeButton.Click -= CloseButton_OnClick;
            }

            m_closeButton = GetTemplateChild( "PART_CloseButton" ) as Button;

            if( m_closeButton != null )
            {
                m_closeButton.Click += CloseButton_OnClick;

                OnIsCloseButtonVisibleChanged( IsCloseButtonVisible );
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void CloseButton_OnClick( object sender, RoutedEventArgs e )
        {
            Visibility = Visibility.Hidden;
        }

        private static void OnIsCloseButtonVisibleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as SlidingToolbar )?.OnIsCloseButtonVisibleChanged( (bool) e.NewValue );
        }

        private void OnIsCloseButtonVisibleChanged( bool newValue )
        {
            if( m_closeButton != null )
            {
                m_closeButton.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private Button? m_closeButton;
    }
}

/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// <see cref="ContentControl"> with an overlaid <see cref="SlidingToolbar"/> than can be visible or hidden.
    /// </summary>
    [TemplatePart( Name = "PART_Toolbar", Type = typeof( SlidingToolbar ) )]
    public class ContentControlWithToolbar : HeaderedContentControl
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        public static readonly DependencyProperty IsHeaderVisibleProperty =
            DependencyProperty.Register( nameof( IsHeaderVisible ), typeof( bool ), typeof( ContentControlWithToolbar ),
                new FrameworkPropertyMetadata( false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsHeaderVisibleChanged ) );

        [Bindable( true )]
        [Browsable( true )]
        public bool IsHeaderVisible
        {
            get => (bool) GetValue( IsHeaderVisibleProperty );
            set => SetValue( IsHeaderVisibleProperty, value );
        }

        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.Register( nameof( HeaderMargin ), typeof( Thickness ), typeof( ContentControlWithToolbar ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( true )]
        [Browsable( true )]
        public Thickness HeaderMargin
        {
            get => (Thickness) GetValue( HeaderMarginProperty );
            set => SetValue( HeaderMarginProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        static ContentControlWithToolbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( ContentControlWithToolbar ),
                new FrameworkPropertyMetadata( typeof( ContentControlWithToolbar ) ) );
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if( m_toolbar != null )
            {
                m_toolbar.IsVisibleChanged -= SlidingToolbar_OnIsVisibleChanged;
            }

            m_toolbar = GetTemplateChild( "PART_Toolbar" ) as SlidingToolbar;
            
            if( m_toolbar != null )
            {
                OnIsHeaderVisibleChanged( IsHeaderVisible );
                m_toolbar.IsVisibleChanged += SlidingToolbar_OnIsVisibleChanged;
            }
        }

        private static void OnIsHeaderVisibleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is ContentControlWithToolbar control )
            {
                control.OnIsHeaderVisibleChanged( (bool) e.NewValue );
            }
        }

        private void OnIsHeaderVisibleChanged( bool newValue )
        {
            if( m_toolbar != null )
            {
                m_toolbar.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SlidingToolbar_OnIsVisibleChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            if( sender is SlidingToolbar slidingToolbar )
            {
                IsHeaderVisible = slidingToolbar.IsVisible;
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private SlidingToolbar? m_toolbar;
    }
}

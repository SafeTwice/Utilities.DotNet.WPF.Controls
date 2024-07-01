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

        /// <summary>
        /// Dependency property for <see cref="IsHeaderVisible"/>.
        /// </summary>
        public static readonly DependencyProperty IsHeaderVisibleProperty =
            DependencyProperty.Register( nameof( IsHeaderVisible ), typeof( bool ), typeof( ContentControlWithToolbar ),
                new FrameworkPropertyMetadata( false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsHeaderVisibleChanged ) );

        /// <summary>
        /// Indicates and sets if the toolbar is visible.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Indicates and sets if the toolbar is visible." )]
        public bool IsHeaderVisible
        {
            get => (bool) GetValue( IsHeaderVisibleProperty );
            set => SetValue( IsHeaderVisibleProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="HeaderMargin"/>.
        /// </summary>
        [TypeConverter( typeof( ThicknessConverter ) )]
        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.Register( nameof( HeaderMargin ), typeof( Thickness ), typeof( ContentControlWithToolbar ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        /// <summary>
        /// "Margin around the toolbar."
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [TypeConverter( typeof( ThicknessConverter ) )]
        [Description( "Margin around the toolbar." )]
        public Thickness HeaderMargin
        {
            get => (Thickness) GetValue( HeaderMarginProperty );
            set => SetValue( HeaderMarginProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="HeaderMinWidth"/>.
        /// </summary>
        [TypeConverter( typeof( LengthConverter ) )]
        public static readonly DependencyProperty HeaderMinWidthProperty =
            DependencyProperty.Register( nameof( HeaderMinWidth ), typeof( double ), typeof( ContentControlWithToolbar ),
                new FrameworkPropertyMetadata( 0.0 ) );

        /// <summary>
        /// Minimum width of the toolbar.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [TypeConverter( typeof( LengthConverter ) )]
        [Description( "Minimum width of the toolbar." )]
        public double HeaderMinWidth
        {
            get => (double) GetValue( HeaderMinWidthProperty );
            set => SetValue( HeaderMinWidthProperty, value );
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
                m_toolbar.Visibility = newValue ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void SlidingToolbar_OnIsVisibleChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            if( sender is SlidingToolbar slidingToolbar )
            {
                IsHeaderVisible = ( slidingToolbar.Visibility == Visibility.Visible );
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

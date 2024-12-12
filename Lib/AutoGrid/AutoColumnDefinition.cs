/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Defines automatic column properties for <see cref="AutoGrid"/>.
    /// </summary>
    public class AutoColumnDefinition : Freezable
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for <see cref="Width"/>.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register( nameof( Width ), typeof( GridLength ), typeof( AutoColumnDefinition ),
                                         new PropertyMetadata( GridLength.Auto ) );

        /// <summary>
        /// Width of the column.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Layout" )]
        [Description( "Width of the column." )]
        public GridLength Width
        {
            get => (GridLength) GetValue( WidthProperty );
            set => SetValue( WidthProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="MinWidth"/>.
        /// </summary>
        [TypeConverter( typeof( LengthConverter ) )]
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register( nameof( MinWidth ), typeof( double ), typeof( AutoColumnDefinition ),
                                         new PropertyMetadata( 0.0 ) );

        /// <summary>
        /// Minimum width of the column.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [TypeConverter( typeof( LengthConverter ) )]
        [Category( "Layout" )]
        [Description( "Minimum width of the column." )]
        public double MinWidth
        {
            get => (double) GetValue( MinWidthProperty );
            set => SetValue( MinWidthProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="MaxWidth"/>.
        /// </summary>
        [TypeConverter( typeof( LengthConverter ) )]
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register( nameof( MaxWidth ), typeof( double ), typeof( AutoColumnDefinition ),
                                         new PropertyMetadata( double.PositiveInfinity ) );

        /// <summary>
        /// Maximum width of the column.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [TypeConverter( typeof( LengthConverter ) )]
        [Category( "Layout" )]
        [Description( "Maximum width of the column." )]
        public double MaxWidth
        {
            get => (double) GetValue( MaxWidthProperty );
            set => SetValue( MaxWidthProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        public AutoColumnDefinition()
        {
        }

        //===========================================================================
        //                            INTERNAL METHODS
        //===========================================================================

        internal ColumnDefinition CreateColumnDefinition()
        {
            return new ColumnDefinition
            {
                Width = Width,
                MinWidth = MinWidth,
                MaxWidth = MaxWidth
            };
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        protected override Freezable CreateInstanceCore()
        {
            return new AutoColumnDefinition();
        }
    }
}

/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Defines automatic row properties for <see cref="AutoGrid"/>.
    /// </summary>
    public class AutoRowDefinition : Freezable
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for <see cref="Height"/>.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register( nameof( Height ), typeof( GridLength ), typeof( AutoRowDefinition ),
                                         new PropertyMetadata( GridLength.Auto ) );

        /// <summary>
        /// Height of the row.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Layout" )]
        [Description( "Height of the row." )]
        public GridLength Height
        {
            get => (GridLength) GetValue( HeightProperty );
            set => SetValue( HeightProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="MinHeight"/>.
        /// </summary>
        [TypeConverter( typeof( LengthConverter ) )]
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register( nameof( MinHeight ), typeof( double ), typeof( AutoRowDefinition ),
                                         new PropertyMetadata( 0.0 ) );

        /// <summary>
        /// Minimum height of the row.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [TypeConverter( typeof( LengthConverter ) )]
        [Category( "Layout" )]
        [Description( "Minimum height of the row." )]
        public double MinHeight
        {
            get => (double) GetValue( MinHeightProperty );
            set => SetValue( MinHeightProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="MaxHeight"/>.
        /// </summary>
        [TypeConverter( typeof( LengthConverter ) )]
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register( nameof( MaxHeight ), typeof( double ), typeof( AutoRowDefinition ),
                                         new PropertyMetadata( double.PositiveInfinity ) );

        /// <summary>
        /// Maximum height of the row.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [TypeConverter( typeof( LengthConverter ) )]
        [Category( "Layout" )]
        [Description( "Maximum height of the row." )]
        public double MaxHeight
        {
            get => (double) GetValue( MaxHeightProperty );
            set => SetValue( MaxHeightProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        public AutoRowDefinition()
        {
        }

        //===========================================================================
        //                            INTERNAL METHODS
        //===========================================================================

        internal RowDefinition CreateRowDefinition()
        {
            return new RowDefinition
            {
                Height = Height,
                MinHeight = MinHeight,
                MaxHeight = MaxHeight
            };
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new AutoRowDefinition();
        }
    }
}

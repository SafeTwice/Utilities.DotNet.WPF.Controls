/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Extension of the <see cref="GridView"/> class that adds new behaviors.
    /// </summary>
    public class GridViewEx : GridView
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for the <see cref="AutoAdjustColumns"/> property.
        /// </summary>
        public static readonly DependencyProperty AutoAdjustColumnsProperty =
            DependencyProperty.Register( nameof( AutoAdjustColumns ), typeof( bool ), typeof( GridViewEx ),
                new PropertyMetadata( true ) );

        /// <summary>
        /// Indicates if the auto-sized columns must be updated when an item is added.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public bool AutoAdjustColumns
        {
            get => (bool) GetValue( AutoAdjustColumnsProperty );
            set => SetValue( AutoAdjustColumnsProperty, value );
        }

        /// <summary>
        /// Dependency property for the <see cref="ScrollToAddedItems"/> property.
        /// </summary>
        public static readonly DependencyProperty ScrollToAddedItemsProperty =
            DependencyProperty.Register( nameof( ScrollToAddedItems ), typeof( bool ), typeof( GridViewEx ),
                new PropertyMetadata( false ) );

        /// <summary>
        /// Indicates if the view must scroll to the added items.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public bool ScrollToAddedItems
        {
            get => (bool) GetValue( ScrollToAddedItemsProperty );
            set => SetValue( ScrollToAddedItemsProperty, value );
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        /// <inheritdoc/>
        protected override void PrepareItem( ListViewItem item )
        {
            base.PrepareItem( item );

            if( AutoAdjustColumns )
            {
                // Adjustment of auto-sized columns must be delayed until the item is loaded, because
                // at this point bindings are not setup properly and the auto-sizing would occur with
                // this item controls set to default values.
                item.Loaded += OnItemLoaded;
            }

            if( ScrollToAddedItems )
            {
                item.BringIntoView();
            }
        }

        private void OnItemLoaded( object? sender, EventArgs e )
        {
            Debug.Assert( sender is ListViewItem );

            var item = (ListViewItem) sender!;

            item.Loaded -= OnItemLoaded;

            foreach( var column in Columns.Where( c => double.IsNaN( c.Width ) ) )
            {
                column.Width = column.ActualWidth;
                column.Width = double.NaN;
            }
        }
    }
}

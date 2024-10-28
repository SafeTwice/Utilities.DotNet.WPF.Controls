/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Extension of the <see cref="GridView"/> class that updates auto-sized columns when an item is added.
    /// </summary>
    public class GridViewEx : GridView
    {
        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        /// <inheritdoc/>
        protected override void PrepareItem( ListViewItem item )
        {
            foreach( var column in Columns )
            {
                if( double.IsNaN( column.Width ) )
                {
                    column.Width = column.ActualWidth;
                    column.Width = double.NaN;
                }

                base.PrepareItem( item );
            }
        }
    }
}

/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// <see cref="Grid"/> that automatically adds rows and columns to fill the grid.
    /// </summary>
    public class AutoGrid : Grid
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for <see cref="AutoColumns"/>.
        /// </summary>
        public static readonly DependencyProperty AutoColumnsProperty =
            DependencyProperty.Register( nameof( AutoColumns ), typeof( int ), typeof( AutoGrid ),
                                         new PropertyMetadata( 1, OnPropertyChangeThatInvalidatesMeasure ) );

        /// <summary>
        /// Number of columns to automatically add to the grid.
        /// </summary>
        /// <remarks>
        /// If <see cref="Grid.ColumnDefinitions">ColumnDefinitions</see> is not empty, then extra columns
        /// will be added automatically to complete the columns up to number of automatic columns.
        /// </remarks>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Number of columns to automatically add to the grid." )]
        public int AutoColumns
        {
            get => (int) GetValue( AutoColumnsProperty );
            set => SetValue( AutoColumnsProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="AutoColumnsDefinition"/>.
        /// </summary>
        public static readonly DependencyProperty AutoColumnsDefinitionProperty =
            DependencyProperty.Register( nameof( AutoColumnsDefinition ), typeof( AutoColumnDefinition ), typeof( AutoGrid ),
                                         new PropertyMetadata( new AutoColumnDefinition(), OnPropertyChangeThatInvalidatesMeasure ) );

        /// <summary>
        /// Column definition for automatically added columns.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Column definition for automatically added columns." )]
        public AutoColumnDefinition AutoColumnsDefinition
        {
            get => (AutoColumnDefinition) GetValue( AutoColumnsDefinitionProperty );
            set => SetValue( AutoColumnsDefinitionProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="AutoRowsDefinition"/>.
        /// </summary>
        public static readonly DependencyProperty AutoRowsDefinitionProperty =
            DependencyProperty.Register( nameof( AutoRowsDefinition ), typeof( AutoRowDefinition ), typeof( AutoGrid ),
                                         new PropertyMetadata( new AutoRowDefinition(), OnPropertyChangeThatInvalidatesMeasure ) );

        /// <summary>
        /// Row definition for automatically added rows.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Row definition for automatically added rows." )]
        public AutoRowDefinition AutoRowsDefinition
        {
            get => (AutoRowDefinition) GetValue( AutoRowsDefinitionProperty );
            set => SetValue( AutoRowsDefinitionProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        public AutoGrid()
        {
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        /// <inheritdoc />
        protected override Size MeasureOverride( Size constraint )
        {
            PruneAutoColumns();
            PruneAutoRows();

            int currentRow = 0;
            int currentColumn = 0;

            foreach( UIElement child in InternalChildren )
            {
                child.InvalidateProperty( RowProperty );
                child.InvalidateProperty( ColumnProperty );

                var childRow = child.ReadLocalValue( RowProperty );
                var childColumn = child.ReadLocalValue( ColumnProperty );

                if( childRow == DependencyProperty.UnsetValue )
                {
                    child.SetCurrentValue( RowProperty, currentRow );
                }
                else
                {
                    currentRow = (int) childRow;
                }

                if( childColumn == DependencyProperty.UnsetValue )
                {
                    if( childRow != DependencyProperty.UnsetValue )
                    {
                        // Explicit setting of the row but not of the column assumes that the column is the first one.
                        currentColumn = 0;
                    }

                    child.SetCurrentValue( ColumnProperty, currentColumn );
                }
                else
                {
                    currentColumn = (int) childColumn;
                }

                while( ColumnDefinitions.Count <= currentColumn )
                {
                    AddAutoColumn();
                }

                while( RowDefinitions.Count <= currentRow )
                {
                    AddAutoRow();
                }

                currentColumn++;
                if( currentColumn >= ColumnDefinitions.Count )
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }

            return base.MeasureOverride( constraint );
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void PruneAutoColumns()
        {
            foreach( var column in m_autoColumns )
            {
                ColumnDefinitions.Remove( column );
            }

            m_autoColumns.Clear();

            while( ColumnDefinitions.Count < AutoColumns )
            {
                AddAutoColumn();
            }
        }

        private void PruneAutoRows()
        {
            foreach( var row in m_autoRows )
            {
                RowDefinitions.Remove( row );
            }

            m_autoRows.Clear();
        }

        private void AddAutoColumn()
        {
            var autoColumn = AutoColumnsDefinition.CreateColumnDefinition();

            ColumnDefinitions.Add( autoColumn );
            m_autoColumns.Add( autoColumn );
        }

        private void AddAutoRow()
        {
            var autoRow = AutoRowsDefinition.CreateRowDefinition();

            RowDefinitions.Add( autoRow );
            m_autoRows.Add( autoRow );
        }

        private static void OnPropertyChangeThatInvalidatesMeasure( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is FrameworkElement fe )
            {
                fe.InvalidateMeasure();
            }
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly List<ColumnDefinition> m_autoColumns = new();
        private readonly List<RowDefinition> m_autoRows = new();
    }
}

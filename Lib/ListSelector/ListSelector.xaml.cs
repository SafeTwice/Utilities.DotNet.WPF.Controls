/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utilities.DotNet.Collections;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Control that allows to choose items from a list.
    /// </summary>
    public partial class ListSelector : UserControl
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for the <see cref="AvailableItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty AvailableItemsSourceProperty =
            DependencyProperty.Register( nameof( AvailableItemsSource ), typeof( IEnumerable ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( null, OnAvailableItemsSourcePropertyChangedEvent ) );

        /// <summary>
        /// Sequence of the items that can be selected.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Common" )]
        public IEnumerable? AvailableItemsSource
        {
            get => (IEnumerable?) GetValue( AvailableItemsSourceProperty );
            set => SetValue( AvailableItemsSourceProperty, value );
        }

        /// <summary>
        /// Dependency property for the <see cref="SelectedItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsSourceProperty =
            DependencyProperty.Register( nameof( SelectedItemsSource ), typeof( ICollectionEx ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( null, OnSelectedItemsSourcePropertyChangedEvent ) );

        /// <summary>
        /// Collection of the items that have been selected.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Common" )]
        public ICollectionEx? SelectedItemsSource
        {
            get => (ICollectionEx?) GetValue( SelectedItemsSourceProperty );
            set => SetValue( SelectedItemsSourceProperty, value );
        }

        /// <summary>
        /// Dependency property for the <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register( nameof( ItemTemplate ), typeof( DataTemplate ), typeof( ListSelector ) );

        /// <summary>
        /// Data template used to display the items.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue( ItemTemplateProperty );
            set => SetValue( ItemTemplateProperty, value );
        }

        /// <summary>
        /// Dependency property for the <see cref="ItemTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
                DependencyProperty.Register( nameof( ItemTemplateSelector ), typeof( DataTemplateSelector ), typeof( ListSelector ) );

        /// <summary>
        /// Selector for choosing the data template used to display the items.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector) GetValue( ItemTemplateSelectorProperty );
            set => SetValue( ItemTemplateSelectorProperty, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListSelector()
        {
            InitializeComponent();

            ( (FrameworkElement) Content ).DataContext = this;
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OnUnselectAll( object sender, RoutedEventArgs e )
        {
            SelectedItemsSource?.Clear();

            AvailableItemsSourceView?.Refresh();
        }

        private void OnSelectAll( object sender, RoutedEventArgs e )
        {
            var availableItemsSourceView = AvailableItemsSourceView;
            if( availableItemsSourceView == null )
            {
                return;
            }

            SelectedItemsSource?.AddRange( availableItemsSourceView );

            AvailableItemsSourceView?.Refresh();
        }

        private void OnSelect( object sender, RoutedEventArgs e )
        {
            var selectedElements = AvailableListBox.SelectedItems.Cast<object>().ToArray();

            SelectedItemsSource?.AddRange( selectedElements );

            AvailableItemsSourceView?.Refresh();
        }

        private void OnUnselect( object sender, RoutedEventArgs e )
        {
            var selectedElements = SelectedListBox.SelectedItems.Cast<object>().ToArray();

            SelectedItemsSource?.RemoveRange( selectedElements );

            AvailableItemsSourceView?.Refresh();
        }

        private void AvailableListBox_OnMouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            OnSelect( sender, e );
        }

        private void SelectedListBox_OnMouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            OnUnselect( sender, e );
        }

        private static void OnAvailableItemsSourcePropertyChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( (ListSelector) d ).OnAvailableItemsSourcePropertyChangedEvent();
        }

        private void OnAvailableItemsSourcePropertyChangedEvent()
        {
            var availableItemsSourceView = AvailableItemsSourceView;
            if( availableItemsSourceView == null )
            {
                return;
            }

            availableItemsSourceView.Filter = item => !SelectedItemsSource?.Contains( item ) ?? true;
        }

        private static void OnSelectedItemsSourcePropertyChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( (ListSelector) d ).OnSelectedItemsSourcePropertyChangedEvent();
        }

        private void OnSelectedItemsSourcePropertyChangedEvent()
        {
            AvailableItemsSourceView?.Refresh();
        }

        //===========================================================================
        //                           PRIVATE PROPERTIES
        //===========================================================================

        private ICollectionView? AvailableItemsSourceView => ( AvailableItemsSource == null ) ? null : CollectionViewSource.GetDefaultView( AvailableItemsSource );
    }
}

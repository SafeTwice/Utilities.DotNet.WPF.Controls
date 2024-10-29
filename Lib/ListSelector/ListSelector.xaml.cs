/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utilities.DotNet.Collections;
using Utilities.DotNet.Collections.Observables;

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
        /// Dependency property for the <see cref="AvailableHeaderText"/> property.
        /// </summary>
        public static readonly DependencyProperty AvailableHeaderTextProperty =
            DependencyProperty.Register( nameof( AvailableHeaderText ), typeof( string ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( "Available" ) );

        /// <summary>
        /// Header text for the list of available items.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Common" )]
        [DefaultValue( "Available" )]
        public string AvailableHeaderText
        {
            get => (string) GetValue( AvailableHeaderTextProperty );
            set => SetValue( AvailableHeaderTextProperty, value );
        }

        /// <summary>
        /// Dependency property for the <see cref="SelectedHeaderText"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedHeaderTextProperty =
            DependencyProperty.Register( nameof( SelectedHeaderText ), typeof( string ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( "Selected" ) );

        /// <summary>
        /// Header text for the list of selected items.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Common" )]
        [DefaultValue( "Selected" )]
        public string SelectedHeaderText
        {
            get => (string) GetValue( SelectedHeaderTextProperty );
            set => SetValue( SelectedHeaderTextProperty, value );
        }

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

            InternalAvailableItems = m_internalAvailableItems;

            InternalAvailableItemsView.Filter = item => !SelectedItemsSource?.Contains( item ) ?? true;
        }

        //===========================================================================
        //                          INTERNAL PROPERTIES
        //===========================================================================

        private static readonly DependencyPropertyKey InternalAvailableItemsPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( InternalAvailableItems ), typeof( IObservableReadOnlyList<object> ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( null ) );

        [Bindable( false )]
        [Browsable( false )]
        private IObservableReadOnlyList<object> InternalAvailableItems
        {
            get => (IObservableReadOnlyList<object>) GetValue( InternalAvailableItemsPropertyKey.DependencyProperty );
            set => SetValue( InternalAvailableItemsPropertyKey, value );
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OnUnselectAll( object sender, RoutedEventArgs e )
        {
            SelectedItemsSource?.Clear();

            InternalAvailableItemsView.Refresh();
        }

        private void OnSelectAll( object sender, RoutedEventArgs e )
        {
            SelectedItemsSource?.AddRange( InternalAvailableItemsView );

            InternalAvailableItemsView.Refresh();
        }

        private void OnSelect( object sender, RoutedEventArgs e )
        {
            var selectedElements = AvailableListBox.SelectedItems.Cast<object>().ToArray();

            SelectedItemsSource?.AddRange( selectedElements );

            InternalAvailableItemsView.Refresh();
        }

        private void OnUnselect( object sender, RoutedEventArgs e )
        {
            var selectedElements = SelectedListBox.SelectedItems.Cast<object>().ToArray();

            SelectedItemsSource?.RemoveRange( selectedElements );

            InternalAvailableItemsView.Refresh();
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
            ( (ListSelector) d ).OnAvailableItemsSourcePropertyChangedEvent( (IEnumerable) e.OldValue, (IEnumerable?) e.NewValue );
        }

        private void OnAvailableItemsSourcePropertyChangedEvent( IEnumerable oldValue, IEnumerable? newValue )
        {
            var oldAvailableItemsSourceView = CollectionViewSource.GetDefaultView( oldValue );
            if( oldAvailableItemsSourceView != null )
            {
                oldAvailableItemsSourceView.CollectionChanged -= OnAvailableItemsCollectionChangedEvent;
            }

            RegenerateInternalAvailableItems( newValue?.Cast<object?>() );

            var newAvailableItemsSourceView = CollectionViewSource.GetDefaultView( newValue );
            if( newAvailableItemsSourceView != null )
            {
                newAvailableItemsSourceView.CollectionChanged += OnAvailableItemsCollectionChangedEvent;
            }
        }

        private void OnAvailableItemsCollectionChangedEvent( object? sender, NotifyCollectionChangedEventArgs e )
        {
            switch( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    if( e.NewItems != null )
                    {
                        m_internalAvailableItems.AddRange( e.NewItems.Cast<object>() );
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if( e.OldItems != null )
                    {
                        m_internalAvailableItems.RemoveRange( e.OldItems.Cast<object>() );

                        PruneInvalidSelectedItems();
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if( e.OldItems != null )
                    {
                        m_internalAvailableItems.RemoveRange( e.OldItems.Cast<object>() );

                        PruneInvalidSelectedItems();
                    }
                    if( e.NewItems != null )
                    {
                        m_internalAvailableItems.AddRange( e.NewItems.Cast<object>() );
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if( e.NewItems != null )
                    {
                        if( e.NewItems.Count > 0 )
                        {
                            throw new System.NotSupportedException( "Moving multiple items is not supported." );
                        }

                        m_internalAvailableItems.Move( e.OldStartingIndex, e.NewStartingIndex );
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RegenerateInternalAvailableItems( ( (IEnumerable) sender! ).Cast<object>() );
                    break;
            }
        }

        private void RegenerateInternalAvailableItems( IEnumerable<object>? items )
        {
            m_internalAvailableItems.Clear();
            if( items != null )
            {
                m_internalAvailableItems.AddRange( items );

            }

            PruneInvalidSelectedItems();
        }

        private void PruneInvalidSelectedItems()
        {
            if( SelectedItemsSource != null )
            {
                var invalidSelectedItems = SelectedItemsSource.Cast<object>().Where( item => !m_internalAvailableItems.Contains( item ) ).ToList();

                SelectedItemsSource.RemoveRange( invalidSelectedItems );
            }
        }

        private static void OnSelectedItemsSourcePropertyChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( (ListSelector) d ).OnSelectedItemsSourcePropertyChangedEvent();
        }

        private void OnSelectedItemsSourcePropertyChangedEvent()
        {
            PruneInvalidSelectedItems();

            InternalAvailableItemsView.Refresh();
        }

        //===========================================================================
        //                           PRIVATE PROPERTIES
        //===========================================================================

        private ICollectionView InternalAvailableItemsView => CollectionViewSource.GetDefaultView( InternalAvailableItems );

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly ObservableList<object> m_internalAvailableItems = new();
    }
}

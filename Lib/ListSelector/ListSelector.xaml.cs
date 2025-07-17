/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
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
        /// Dependency property for the <see cref="UnselectedItemsHeaderText"/> property.
        /// </summary>
        public static readonly DependencyProperty UnselectedItemsHeaderTextProperty =
            DependencyProperty.Register( nameof( UnselectedItemsHeaderText ), typeof( string ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( "Unselected" ) );

        /// <summary>
        /// Header text for the list of unselected items.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Common" )]
        [DefaultValue( "Unselected" )]
        public string UnselectedItemsHeaderText
        {
            get => (string) GetValue( UnselectedItemsHeaderTextProperty );
            set => SetValue( UnselectedItemsHeaderTextProperty, value );
        }

        /// <summary>
        /// Dependency property for the <see cref="SelectedItemsHeaderText"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsHeaderTextProperty =
            DependencyProperty.Register( nameof( SelectedItemsHeaderText ), typeof( string ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( "Selected" ) );

        /// <summary>
        /// Header text for the list of selected items.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Category( "Common" )]
        [DefaultValue( "Selected" )]
        public string SelectedItemsHeaderText
        {
            get => (string) GetValue( SelectedItemsHeaderTextProperty );
            set => SetValue( SelectedItemsHeaderTextProperty, value );
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
        /// <remarks>
        /// The available items collection is not modified by the control. If the collection is observable, new items added
        /// will automatically appear in the unselected items list and items removed will be automatically removed from the 
        /// unselected or selected items list.
        /// </remarks>
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
        /// <remarks>
        /// <para>
        /// The selected items collection is modified by the control. Additionally, if the collection is observable, new items added
        /// will automatically appear in the selected items list (only if they also belong to <see cref="AvailableItemsSource"/>) 
        /// and items removed will be automatically removed from the selected items list.
        /// </para>
        /// <para>
        /// This collection will always be a subset of the <see cref="AvailableItemsSource"/> collection.
        /// </para>
        /// </remarks>
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

            UnselectedItemsSource = m_internalAvailableItems;

            UnselectedItemsView.Filter = ( item => !SelectedItemsSource?.Contains( item ) ?? true );
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <summary>
        /// Selects all available items, adding them to the selected items.
        /// </summary>
        public void SelectAll()
        {
            SelectedItemsSource?.AddRange( UnselectedItemsView );
        }

        /// <summary>
        /// Unselects all selected items, removing them from the selected items.
        /// </summary>
        public void UnselectAll()
        {
            SelectedItemsSource?.Clear();
        }

        //===========================================================================
        //                          INTERNAL PROPERTIES
        //===========================================================================

        internal static readonly DependencyPropertyKey UnselectedItemsSourcePropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( UnselectedItemsSource ), typeof( IObservableReadOnlyList<object> ), typeof( ListSelector ),
                new FrameworkPropertyMetadata( null ) );

        [Bindable( false )]
        [Browsable( false )]
        internal IObservableReadOnlyList<object?> UnselectedItemsSource
        {
            get => (IObservableReadOnlyList<object?>) GetValue( UnselectedItemsSourcePropertyKey.DependencyProperty );
            set => SetValue( UnselectedItemsSourcePropertyKey, value );
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OnUnselectAll( object sender, RoutedEventArgs e )
        {
            UnselectAll();
        }

        private void OnSelectAll( object sender, RoutedEventArgs e )
        {
            SelectAll();
        }

        private void OnSelect( object sender, RoutedEventArgs e )
        {
            var selectedElements = UnselectedListBox.SelectedItems.Cast<object?>().ToArray();

            SelectedItemsSource?.AddRange( selectedElements );
        }

        private void OnUnselect( object sender, RoutedEventArgs e )
        {
            var selectedElements = SelectedListBox.SelectedItems.Cast<object?>().ToArray();

            SelectedItemsSource?.RemoveRange( selectedElements );
        }

        private void UnselectedListBox_OnMouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            OnSelect( sender, e );
        }

        private void SelectedListBox_OnMouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            OnUnselect( sender, e );
        }

        private static void OnAvailableItemsSourcePropertyChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( (ListSelector) d ).OnAvailableItemsSourcePropertyChangedEvent( (IEnumerable?) e.OldValue, (IEnumerable?) e.NewValue );
        }

        private void OnAvailableItemsSourcePropertyChangedEvent( IEnumerable? oldValue, IEnumerable? newValue )
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
                        m_internalAvailableItems.AddRange( e.NewItems.Cast<object?>() );
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if( e.OldItems != null )
                    {
                        m_internalAvailableItems.RemoveRange( e.OldItems.Cast<object?>() );

                        PruneInvalidSelectedItems();
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if( ( e.OldItems != null ) && ( e.NewItems != null ) )
                    {
                        if( ( e.NewItems.Count != 1 ) || ( e.OldItems.Count != 1 ) )
                        {
                            throw new System.NotSupportedException( "Moving multiple items is not supported." );
                        }

                        m_internalAvailableItems.Replace( e.OldItems[ 0 ], e.NewItems[ 0 ] );
                        SelectedItemsSource?.Replace( e.OldItems[ 0 ], e.NewItems[ 0 ] );

                        PruneInvalidSelectedItems();
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if( e.NewItems != null )
                    {
                        if( e.NewItems.Count > 1 )
                        {
                            throw new System.NotSupportedException( "Moving multiple items is not supported." );
                        }

                        m_internalAvailableItems.Move( e.OldStartingIndex, e.NewStartingIndex );
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RegenerateInternalAvailableItems( ( (IEnumerable) sender! ).Cast<object?>() );
                    break;
            }
        }

        private void RegenerateInternalAvailableItems( IEnumerable<object?>? items )
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
                var invalidSelectedItems = SelectedItemsSource.Cast<object?>().Where( item => !m_internalAvailableItems.Contains( item ) ).ToList();

                SelectedItemsSource.RemoveRange( invalidSelectedItems );
            }
        }

        private static void OnSelectedItemsSourcePropertyChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( (ListSelector) d ).OnSelectedItemsSourcePropertyChangedEvent( (IEnumerable?) e.OldValue, (IEnumerable?) e.NewValue );
        }

        private void OnSelectedItemsSourcePropertyChangedEvent( IEnumerable? oldValue, IEnumerable? newValue )
        {
            var oldSelectedItemsSourceView = CollectionViewSource.GetDefaultView( oldValue );
            if( oldSelectedItemsSourceView != null )
            {
                oldSelectedItemsSourceView.CollectionChanged -= OnSelectedItemsCollectionChangedEvent;
            }

            PruneInvalidSelectedItems();

            UnselectedItemsView.Refresh();

            var newSelectedItemsSourceView = CollectionViewSource.GetDefaultView( newValue );
            if( newSelectedItemsSourceView != null )
            {
                newSelectedItemsSourceView.CollectionChanged += OnSelectedItemsCollectionChangedEvent;
            }
        }

        private void OnSelectedItemsCollectionChangedEvent( object? sender, NotifyCollectionChangedEventArgs e )
        {
            switch( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    PruneInvalidSelectedItems();
                    break;

                default:
                    break;
            }

            UnselectedItemsView.Refresh();
        }

        //===========================================================================
        //                           PRIVATE PROPERTIES
        //===========================================================================

        private ICollectionView UnselectedItemsView => CollectionViewSource.GetDefaultView( UnselectedItemsSource );

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly ObservableList<object?> m_internalAvailableItems = new();
    }
}

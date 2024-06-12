/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Extension of <see cref="ComboBox"/> to use for introducing search terms.
    /// </summary>
    [TemplatePart( Name = "PART_ComboBox", Type = typeof( ComboBox ) )]
    [TemplatePart( Name = "PART_ClearButton", Type = typeof( Button ) )]
    public class SearchBox : Selector
    {
        //===========================================================================
        //                          PUBLIC NESTED TYPES
        //===========================================================================

        /// <summary>
        /// Specifies an horizontal position.
        /// </summary>
        public enum EHorizontalPosition
        {
            Left,
            Right
        }

        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for the search text.
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register( nameof( SearchText ), typeof( string ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( string.Empty ) );

        /// <summary>
        /// Text to search.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Text to search." )]
        public string SearchText
        {
            get => (string) GetValue( SearchTextProperty );
            set => SetValue( SearchTextProperty, value );
        }

        /// <summary>
        /// Dependency property for the maximum height of the drop-down popup.
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register( nameof( MaxDropDownHeight ), typeof( double ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( SystemParameters.PrimaryScreenHeight / 3 ) );

        /// <summary>
        /// Maximum height of the drop-down popup.
        /// </summary>
        [Bindable( true )]
        [Category( "Layout" )]
        [Browsable( true )]
        [TypeConverter( typeof( LengthConverter ) )]
        [Description( "The maximum height of the drop-down popup." )]
        public double MaxDropDownHeight
        {
            get => (double) GetValue( MaxDropDownHeightProperty );
            set => SetValue( MaxDropDownHeightProperty, value );
        }

        /// <summary>
        /// Dependency property for the search button position.
        /// </summary>
        public static readonly DependencyProperty ClearButtonPositionProperty =
            DependencyProperty.Register( nameof( ClearButtonPosition ), typeof( EHorizontalPosition ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( EHorizontalPosition.Right, OnChangeThatNeedsClearButtonRearrangement ) );

        /// <summary>
        /// Position of the clear button.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Position of the clear button." )]
        public EHorizontalPosition ClearButtonPosition
        {
            get => (EHorizontalPosition) GetValue( ClearButtonPositionProperty );
            set => SetValue( ClearButtonPositionProperty, value );
        }

        /// <summary>
        /// Dependency property for the trim search text flag.
        /// </summary>
        public static readonly DependencyProperty TrimSearchTextProperty =
            DependencyProperty.Register( nameof( TrimSearchText ), typeof( bool ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( false ) );

        /// <summary>
        /// Flag to indicate if the search text should be trimmed.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Indicates if the search text should be trimmed." )]
        public bool TrimSearchText
        {
            get => (bool) GetValue( TrimSearchTextProperty );
            set => SetValue( TrimSearchTextProperty, value );
        }

        /// <summary>
        /// Dependency property for the auto history size.
        /// </summary>
        public static readonly DependencyProperty AutoHistorySizeProperty =
            DependencyProperty.Register( nameof( AutoHistorySize ), typeof( int ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( 0, OnAutoHistorySizeChanged ) );

        /// <summary>
        /// Number of search terms to automatically keep in the history.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this value is set to zero (or a negative number), then the automatic search history is disabled.
        /// In this case, the history of searches can be manually managed by setting the <see cref="ItemsSource"/>
        /// property to a collection of searched terms that the user must update manually.
        /// </para>
        /// <para>
        /// If this value is set to a positive number, the search history is automatically managed by the control.
        /// In this case, setting the <see cref="ItemsSource"/> property is ignored.
        /// </para>
        /// </remarks>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Number of search terms to automatically keep in the history." )]
        public int AutoHistorySize
        {
            get => (int) GetValue( AutoHistorySizeProperty );
            set => SetValue( AutoHistorySizeProperty, value );
        }

        /// <summary>
        /// Dependency property for the Find command.
        /// </summary>
        public static readonly DependencyProperty FindCommandProperty =
            DependencyProperty.Register( nameof( FindCommand ), typeof( ICommand ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( null ) );

        /// <summary>
        /// Command to execute when a search is requested.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Command to execute when a search is requested." )]
        public ICommand FindCommand
        {
            get => (ICommand) GetValue( FindCommandProperty );
            set => SetValue( FindCommandProperty, value );
        }

        //===========================================================================
        //                          INTERNAL PROPERTIES
        //===========================================================================

        private static readonly DependencyPropertyKey ComboBoxPaddingPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( ComboBoxPadding ), typeof( Thickness ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( true )]
        [Browsable( false )]
        internal Thickness ComboBoxPadding
        {
            get => (Thickness) GetValue( ComboBoxPaddingPropertyKey.DependencyProperty );
            private set => SetValue( ComboBoxPaddingPropertyKey, value );
        }

        private static readonly DependencyPropertyKey ClearButtonMarginPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( ClearButtonMargin ), typeof( Thickness ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( true )]
        [Browsable( false )]
        internal Thickness ClearButtonMargin
        {
            get => (Thickness) GetValue( ClearButtonMarginPropertyKey.DependencyProperty );
            private set => SetValue( ClearButtonMarginPropertyKey, value );
        }

        //===========================================================================
        //                             PUBLIC EVENTS
        //===========================================================================

        /// <summary>
        /// Routed event for the Find event.
        /// </summary>
        public static readonly RoutedEvent FindEvent =
            EventManager.RegisterRoutedEvent( nameof( Find ), RoutingStrategy.Bubble, typeof( FindEventHandler ), typeof( SearchBox ) );

        /// <summary>
        /// Occurs when a search is requested.
        /// </summary>
        [Description( "Occurs when a search is requested." )]
        public event FindEventHandler Find
        {
            add => AddHandler( FindEvent, value );
            remove => RemoveHandler( FindEvent, value );
        }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( SearchBox ), new FrameworkPropertyMetadata( typeof( SearchBox ) ) );

            ItemsSourceProperty.OverrideMetadata( typeof( SearchBox ), new FrameworkPropertyMetadata( null, null, CoerceItemsSource ) );

            BorderThicknessProperty.OverrideMetadata( typeof( SearchBox ), new FrameworkPropertyMetadata( OnChangeThatNeedsClearButtonRearrangement ) );

            PaddingProperty.OverrideMetadata( typeof( SearchBox ), new FrameworkPropertyMetadata( OnChangeThatNeedsClearButtonRearrangement ) );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchBox()
        {
            Loaded += OnLoaded;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if( m_clearButton != null )
            {
                m_clearButton.Click -= ClearButton_OnClick;
            }

            m_clearButton = GetTemplateChild( "PART_ClearButton" ) as Button;

            if( m_clearButton != null )
            {
                m_clearButton.Click += ClearButton_OnClick;
            }

            if( m_comboBox != null )
            {
                m_comboBox.KeyDown -= OnKeyDown;
            }

            m_comboBox = GetTemplateChild( "PART_ComboBox" ) as ComboBox;

            if( m_comboBox != null )
            {
                m_comboBox.KeyDown += OnKeyDown;
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OnLoaded( object sender, RoutedEventArgs e )
        {
            ArrangeClearButton();
        }

        private static void OnChangeThatNeedsClearButtonRearrangement( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is SearchBox searchBox )
            {
                searchBox.ArrangeClearButton();
            }
        }

        private void ArrangeClearButton()
        {
            if( m_clearButton != null )
            {
                double toggleButtonWidth;

                if( m_comboBox is SearchComboBox searchComboBox )
                {
                    toggleButtonWidth = searchComboBox.ToggleButtonWidth;
                }
                else
                {
                    toggleButtonWidth = SystemParameters.VerticalScrollBarWidth;
                }

                var clearButtonRightMargin = toggleButtonWidth + BorderThickness.Right;

                var comboBoxPaddingOffset = m_clearButton.ActualWidth;

                if( ClearButtonPosition == EHorizontalPosition.Right )
                {
                    ClearButtonMargin = new Thickness( 0, BorderThickness.Top, clearButtonRightMargin, BorderThickness.Bottom );
                    ComboBoxPadding = new Thickness( Padding.Left, Padding.Top, Padding.Right + comboBoxPaddingOffset, Padding.Bottom );
                }
                else
                {
                    ClearButtonMargin = new Thickness( BorderThickness.Left, BorderThickness.Top, 0, BorderThickness.Bottom );
                    ComboBoxPadding = new Thickness( Padding.Left + comboBoxPaddingOffset, Padding.Top, Padding.Right, Padding.Bottom );
                }
            }
            else
            {
                ComboBoxPadding = Padding;
            }
        }

        private static void OnAutoHistorySizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as SearchBox )?.OnAutoHistorySizeChanged( (int) e.NewValue );
        }

        private void OnAutoHistorySizeChanged( int newValue )
        {
            if( newValue > 0 )
            {
                m_searchHistory ??= new ObservableCollection<string>();

                ItemsSource = m_searchHistory;

                PruneSearchHistory();
            }
            else
            {
                if( ItemsSource == m_searchHistory )
                {
                    ItemsSource = null;
                }
                m_searchHistory = null;
            }
        }

        private static object CoerceItemsSource( DependencyObject d, object baseValue )
        {
            if ( d is SearchBox searchBox )
            {
                return searchBox.CoerceItemsSource( baseValue );
            }
            else
            {
                return baseValue;
            }
        }

        private object CoerceItemsSource( object baseValue )
        {
            return ( ( m_searchHistory == null ) || ( baseValue == m_searchHistory ) ) ? baseValue : DependencyProperty.UnsetValue;
        }

        private void ClearButton_OnClick( object sender, RoutedEventArgs e )
        {
            SearchText = string.Empty;
        }

        private void OnKeyDown( object sender, KeyEventArgs e )
        {
            if( e.Key == Key.Enter )
            {
                OnFind();
            }
        }

        private void OnFind()
        {
            var searchText = SearchText;
            if( TrimSearchText )
            {
                searchText = searchText.Trim();
            }
            if( SearchText != searchText )
            {
                SearchText = searchText;
            }

            if( searchText.Length == 0 )
            {
                return;
            }

            UpdateSearchHistory( searchText );

            var newEvent = new FindEventArgs( FindEvent, this, searchText );
            RaiseEvent( newEvent );

            if( FindCommand?.CanExecute( searchText ) ?? false )
            {
                FindCommand.Execute( searchText );
            }
        }

        private void UpdateSearchHistory( string searchText )
        {
            if( m_searchHistory == null )
            {
                return;
            }

            var searchHistoryIndex = m_searchHistory.IndexOf( searchText );

            if( searchHistoryIndex > 0 )
            {
                m_searchHistory.Move( searchHistoryIndex, 0 );
            }

            if( searchHistoryIndex < 0 )
            {
                m_searchHistory.Insert( 0, searchText );
            }

            PruneSearchHistory();
        }

        private void PruneSearchHistory()
        {
            if( m_searchHistory == null )
            {
                return;
            }

            while( m_searchHistory.Count > AutoHistorySize )
            {
                m_searchHistory.RemoveAt( m_searchHistory.Count - 1 );
            }
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private Button? m_clearButton;
        private ComboBox? m_comboBox;

        private ObservableCollection<string>? m_searchHistory;
    }
}

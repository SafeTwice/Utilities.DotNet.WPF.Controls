/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

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
    [TemplatePart( Name = "PART_FindNextButton", Type = typeof( Button ) )]
    [TemplatePart( Name = "PART_FindPreviousButton", Type = typeof( Button ) )]
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
        /// Dependency property for the search backwards flag.
        /// </summary>
        public static readonly DependencyProperty IsSearchBackwardsProperty =
            DependencyProperty.Register( nameof( IsSearchBackwards ), typeof( bool ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( false ) );

        /// <summary>
        /// Indicates if the search should be done backwards.
        /// </summary>
        [Bindable( true )]
        [Browsable( false )]
        [Description( "Indicates if the search should be done backwards." )]
        public bool IsSearchBackwards
        {
            get => (bool) GetValue( IsSearchBackwardsProperty );
            set => SetValue( IsSearchBackwardsProperty, value );
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
        /// Dependency property for the search button position.
        /// </summary>
        public static readonly DependencyProperty ClearButtonPositionProperty =
            DependencyProperty.Register( nameof( ClearButtonPosition ), typeof( EHorizontalPosition ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( EHorizontalPosition.Right, OnChangeThatNeedsRearrangement ) );

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
        /// Dependency property for the visibility of the find buttons.
        /// </summary>
        public static readonly DependencyProperty AreFindButtonsVisibleProperty =
            DependencyProperty.Register( nameof( AreFindButtonsVisible ), typeof( bool ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( true ) );

        /// <summary>
        /// Visibility of the find next buttons.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Visibility of the find next buttons." )]
        public bool AreFindButtonsVisible
        {
            get => (bool) GetValue( AreFindButtonsVisibleProperty );
            set => SetValue( AreFindButtonsVisibleProperty, value );
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
        /// Dependency property for the hint text.
        /// </summary>
        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register( nameof( HintText ), typeof( string ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( null ) );

        /// <summary>
        /// Hint text that is displayed when the search box is empty.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Text to display as a hint when the search box is empty." )]
        public string HintText
        {
            get => (string) GetValue( HintTextProperty );
            set => SetValue( HintTextProperty, value );
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

        /// <summary>
        /// Dependency property for the parameter to pass to the Find
        /// </summary>
        public static readonly DependencyProperty FindCommandParameterProperty =
            DependencyProperty.Register( nameof( FindCommandParameter ), typeof( object ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( null ) );

        /// <summary>
        /// Parameter to pass to the Find command.
        /// </summary>
        /// <remarks>
        /// If this property has a non-<c>null</c> value, then this property is passed as the parameter to the <see cref="FindCommand"/>;
        /// otherwise, a <see cref="FindCommandArgs"/> object is passed.
        /// </remarks>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Parameter to pass to the Find command." )]
        public object FindCommandParameter
        {
            get => GetValue( FindCommandParameterProperty );
            set => SetValue( FindCommandParameterProperty, value );
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

        private static readonly DependencyPropertyKey HintMarginPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( HintMargin ), typeof( Thickness ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( true )]
        [Browsable( false )]
        internal Thickness HintMargin
        {
            get => (Thickness) GetValue( HintMarginPropertyKey.DependencyProperty );
            private set => SetValue( HintMarginPropertyKey, value );
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

            BorderThicknessProperty.OverrideMetadata( typeof( SearchBox ), new FrameworkPropertyMetadata( OnChangeThatNeedsRearrangement ) );

            PaddingProperty.OverrideMetadata( typeof( SearchBox ), new FrameworkPropertyMetadata( OnChangeThatNeedsRearrangement ) );
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

            PrepareComboBox();

            PrepareClearButton();

            PrepareFindNextButton();

            PrepareFindPreviousButton();
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        protected override void OnGotKeyboardFocus( KeyboardFocusChangedEventArgs e )
        {
            if( !e.Handled && ( m_comboBox != null ) && ( e.NewFocus == this ) )
            {
                Keyboard.Focus( m_comboBox );
                e.Handled = true;
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OnLoaded( object sender, RoutedEventArgs e )
        {
            Arrange();
        }

        private static void OnChangeThatNeedsRearrangement( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is SearchBox searchBox )
            {
                searchBox.Arrange();
            }
        }

        private void Arrange()
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

            if( m_clearButton != null )
            {
                var clearButtonRightMargin = toggleButtonWidth + BorderThickness.Right;

                var clearButtonWidth = m_clearButton.ActualWidth;

                if( ClearButtonPosition == EHorizontalPosition.Right )
                {
                    ClearButtonMargin = new Thickness( BorderThickness.Left, BorderThickness.Top, clearButtonRightMargin, BorderThickness.Bottom );
                    ComboBoxPadding = new Thickness( Padding.Left, Padding.Top, Padding.Right + clearButtonWidth, Padding.Bottom );
                    HintMargin = new Thickness( Padding.Left + 1, Padding.Top, Padding.Right + clearButtonWidth + toggleButtonWidth, Padding.Bottom );
                }
                else
                {
                    ClearButtonMargin = new Thickness( BorderThickness.Left, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom );
                    ComboBoxPadding = new Thickness( Padding.Left + clearButtonWidth, Padding.Top, Padding.Right, Padding.Bottom );
                    HintMargin = new Thickness( Padding.Left + clearButtonWidth + 1, Padding.Top, Padding.Right + toggleButtonWidth, Padding.Bottom );
                }
            }
            else
            {
                ComboBoxPadding = Padding;
                HintMargin = new Thickness( Padding.Left, Padding.Top, Padding.Right + toggleButtonWidth, Padding.Bottom );
            }
        }

        private void PrepareComboBox()
        {
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

        private void PrepareClearButton()
        {
            if( m_clearButton != null )
            {
                m_clearButton.Click -= ClearButton_OnClick;
            }

            m_clearButton = GetTemplateChild( "PART_ClearButton" ) as Button;

            if( m_clearButton != null )
            {
                m_clearButton.Click += ClearButton_OnClick;
            }
        }

        private void PrepareFindNextButton()
        {
            if( m_findNextButton != null )
            {
                m_findNextButton.Click -= FindNextButton_OnClick;
            }

            m_findNextButton = GetTemplateChild( "PART_FindNextButton" ) as Button;

            if( m_findNextButton != null )
            {
                m_findNextButton.Click += FindNextButton_OnClick;
            }
        }

        private void PrepareFindPreviousButton()
        {
            if( m_findPreviousButton != null )
            {
                m_findPreviousButton.Click -= FindPreviousButton_OnClick;
            }

            m_findPreviousButton = GetTemplateChild( "PART_FindPreviousButton" ) as Button;

            if( m_findPreviousButton != null )
            {
                m_findPreviousButton.Click += FindPreviousButton_OnClick;
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

        private void FindNextButton_OnClick( object sender, RoutedEventArgs e )
        {
            IsSearchBackwards = false;

            OnFind();
        }

        private void FindPreviousButton_OnClick( object sender, RoutedEventArgs e )
        {
            IsSearchBackwards = true;

            OnFind();
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

            if( !AreFindButtonsVisible )
            {
                IsSearchBackwards = false;
            }

            var newEvent = new FindEventArgs( FindEvent, this, searchText, IsSearchBackwards );
            RaiseEvent( newEvent );

            if( FindCommand != null )
            {
                var parameter = FindCommandParameter ?? new FindCommandArgs( searchText, IsSearchBackwards );

                if( FindCommand.CanExecute( parameter ) )
                {
                    FindCommand.Execute( parameter );
                }
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

        private ComboBox? m_comboBox;
        private Button? m_clearButton;
        private Button? m_findNextButton;
        private Button? m_findPreviousButton;

        private ObservableCollection<string>? m_searchHistory;
    }
}

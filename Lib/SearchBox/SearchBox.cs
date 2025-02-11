/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Utilities.DotNet.WPF.Common;

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
            /// <summary>Left position.</summary>
            Left,
            /// <summary>Right position.</summary>
            Right
        }

        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Dependency property for <see cref="SearchText"/>.
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
        /// Dependency property for <see cref="IsSearchBackwards"/>.
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
        /// Dependency property for <see cref="TrimSearchText"/>.
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
        /// Dependency property for <see cref="ClearButtonPosition"/>.
        /// </summary>
        public static readonly DependencyProperty ClearButtonPositionProperty =
            DependencyProperty.Register( nameof( ClearButtonPosition ), typeof( EHorizontalPosition ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( EHorizontalPosition.Right, FrameworkPropertyMetadataOptions.AffectsArrange ) );

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
        /// Dependency property for <see cref="AreFindButtonsVisible"/>.
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
        /// Dependency property for <see cref="AutoHistorySize"/>.
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
        /// In this case, the history of searches can be manually managed by setting the <see cref="ItemsControl.ItemsSource"/>
        /// property to a collection of searched terms that the user must update manually.
        /// </para>
        /// <para>
        /// If this value is set to a positive number, the search history is automatically managed by the control.
        /// In this case, setting the <see cref="ItemsControl.ItemsSource"/> property is ignored.
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
        /// Dependency property for <see cref="HintText"/>.
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
        /// Dependency property for <see cref="MaxDropDownHeight"/>.
        /// </summary>
        [TypeConverter( typeof( LengthConverter ) )]
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register( nameof( MaxDropDownHeight ), typeof( double ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( SystemParameters.PrimaryScreenHeight / 3 ) );

        /// <summary>
        /// Maximum height of the drop-down popup.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [TypeConverter( typeof( LengthConverter ) )]
        [Category( "Layout" )]
        [Description( "The maximum height of the drop-down popup." )]
        public double MaxDropDownHeight
        {
            get => (double) GetValue( MaxDropDownHeightProperty );
            set => SetValue( MaxDropDownHeightProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="FindCommand"/>.
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
        /// Dependency property for <see cref="FindCommandParameter"/>.
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

        /// <summary>
        /// Dependency property for <see cref="FindNext"/>.
        /// </summary>
        public static readonly DependencyProperty FindNextProperty =
            DependencyProperty.Register( nameof( FindNext ), typeof( DelegateTrigger ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( null, OnFindNextPropertyChanged ) );

        /// <summary>
        /// Trigger to find the next occurrence.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Trigger to find the next occurrence." )]
        public DelegateTrigger FindNext
        {
            get => (DelegateTrigger) GetValue( FindNextProperty );
            set => SetValue( FindNextProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="FindPrevious"/>.
        /// </summary>
        public static readonly DependencyProperty FindPreviousProperty =
            DependencyProperty.Register( nameof( FindPrevious ), typeof( DelegateTrigger ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( null, OnFindPreviousPropertyChanged ) );

        /// <summary>
        /// Trigger to find the previous occurrence.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        [Description( "Trigger to find the previous occurrence." )]
        public DelegateTrigger FindPrevious
        {
            get => (DelegateTrigger) GetValue( FindPreviousProperty );
            set => SetValue( FindPreviousProperty, value );
        }

        //===========================================================================
        //                          INTERNAL PROPERTIES
        //===========================================================================

        private static readonly DependencyPropertyKey ActiveZoneMarginPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( ActiveZoneMargin ), typeof( Thickness ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( false )]
        [Browsable( false )]
        internal Thickness ActiveZoneMargin
        {
            get => (Thickness) GetValue( ActiveZoneMarginPropertyKey.DependencyProperty );
            private set => SetValue( ActiveZoneMarginPropertyKey, value );
        }

        private static readonly DependencyPropertyKey ComboBoxPaddingPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( ComboBoxPadding ), typeof( Thickness ), typeof( SearchBox ),
                new FrameworkPropertyMetadata( new Thickness() ) );

        [Bindable( false )]
        [Browsable( false )]
        internal Thickness ComboBoxPadding
        {
            get => (Thickness) GetValue( ComboBoxPaddingPropertyKey.DependencyProperty );
            private set => SetValue( ComboBoxPaddingPropertyKey, value );
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

            PaddingProperty.OverrideMetadata( typeof( SearchBox ),
                new FrameworkPropertyMetadata( new Thickness(), FrameworkPropertyMetadataOptions.AffectsArrange ) );

            // BorderThickness and BorderBrush are not bound to the ComboBox properties in order to use the current theme ComboBox defaults
            // if the value is not overridden by the user.

            BorderThicknessProperty.OverrideMetadata( typeof( SearchBox ),
                new FrameworkPropertyMetadata( new Thickness(), FrameworkPropertyMetadataOptions.AffectsArrange, OnBorderThicknessPropertyChanged ) );

            BorderBrushProperty.OverrideMetadata( typeof( SearchBox ), new FrameworkPropertyMetadata( OnBorderBrushPropertyChanged ) );
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

        /// <inheritdoc/>
        protected override void OnGotKeyboardFocus( KeyboardFocusChangedEventArgs e )
        {
            if( !e.Handled && ( m_comboBox != null ) && ( e.NewFocus == this ) )
            {
                Keyboard.Focus( m_comboBox );
                e.Handled = true;
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride( Size finalSize )
        {
            var result = base.ArrangeOverride( finalSize );

            Arrange();

            return result;
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void Arrange()
        {
            if( m_comboBox is SearchComboBox searchComboBox )
            {
                ActiveZoneMargin = searchComboBox.ActiveZoneMargin;
            }
            else
            {
                ActiveZoneMargin = new Thickness( BorderThickness.Left, BorderThickness.Top, BorderThickness.Right + SystemParameters.VerticalScrollBarWidth,
                                                  BorderThickness.Bottom );
            }

            if( m_clearButton != null )
            {
                var clearButtonWidth = m_clearButton.ActualWidth;

                if( ClearButtonPosition == EHorizontalPosition.Right )
                {
                    ComboBoxPadding = new Thickness( Padding.Left, Padding.Top, Padding.Right + clearButtonWidth, Padding.Bottom );
                }
                else
                {
                    ComboBoxPadding = new Thickness( Padding.Left + clearButtonWidth, Padding.Top, Padding.Right, Padding.Bottom );
                }
            }
            else
            {
                ComboBoxPadding = Padding;
            }
        }

        private static void OnBorderBrushPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is SearchBox searchBox )
            {
                searchBox.OnBorderBrushChanged( (Brush) e.NewValue );
            }
        }

        private void OnBorderBrushChanged( Brush newValue )
        {
            if( m_comboBox != null )
            {
                m_comboBox.BorderBrush = newValue;
            }
        }

        private static void OnBorderThicknessPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is SearchBox searchBox )
            {
                searchBox.OnBorderThicknessChanged( (Thickness) e.NewValue );
            }
        }

        private void OnBorderThicknessChanged( Thickness newValue )
        {
            if( m_comboBox != null )
            {
                m_comboBox.BorderThickness = newValue;
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

                // Transfer non-bound properties to the ComboBox when it is available.
                TransferComboBoxProperties();
            }
        }

        private void TransferComboBoxProperties()
        {
            var borderThickness = ReadLocalValue( BorderThicknessProperty );
            if( borderThickness != DependencyProperty.UnsetValue )
            {
                OnBorderThicknessChanged( (Thickness) borderThickness );
            }

            var borderBrush = ReadLocalValue( BorderBrushProperty );
            if( borderBrush != DependencyProperty.UnsetValue )
            {
                OnBorderBrushChanged( (Brush) borderBrush );
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
            if( d is SearchBox searchBox )
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
            OnFindNext();
        }

        private void FindPreviousButton_OnClick( object sender, RoutedEventArgs e )
        {
            OnFindPrevious();
        }

        private void OnKeyDown( object sender, KeyEventArgs e )
        {
            if( e.Key == Key.Enter )
            {
                OnFind();
            }
        }

        private void OnFindNext()
        {
            IsSearchBackwards = false;

            OnFind();
        }

        private void OnFindPrevious()
        {
            IsSearchBackwards = true;

            OnFind();
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

        private static void OnFindNextPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is SearchBox searchBox )
            {
                if( e.OldValue is DelegateTrigger oldTrigger )
                {
                    oldTrigger.Activated -= searchBox.OnFindNext;
                }

                if( e.NewValue is DelegateTrigger newTrigger )
                {
                    newTrigger.Activated += searchBox.OnFindNext;
                }
            }
        }

        private static void OnFindPreviousPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is SearchBox searchBox )
            {
                if( e.OldValue is DelegateTrigger oldTrigger )
                {
                    oldTrigger.Activated -= searchBox.OnFindPrevious;
                }

                if( e.NewValue is DelegateTrigger newTrigger )
                {
                    newTrigger.Activated += searchBox.OnFindPrevious;
                }
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

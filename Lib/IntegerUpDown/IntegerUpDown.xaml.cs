using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace Utilities.DotNet.WPF.Controls
{
    using ValueToTextFunc = Func<IntegerUpDown, int, string>;

    /// <summary>
    /// Control for selecting an integer value.
    /// </summary>
    public partial class IntegerUpDown : UserControl, INotifyPropertyChanged
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        #region Bindable Properties

        /// <summary>
        /// Dependency property for <see cref="Value"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register( nameof( Value ), typeof( int ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( 0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChangedEvent ) );

        /// <summary>
        /// Value of the control.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public int Value
        {
            get => (int) GetValue( ValueProperty );
            set => SetValue( ValueProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="Minimum"/>.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register( nameof( Minimum ), typeof( int ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( 0, FrameworkPropertyMetadataOptions.AffectsRender, OnMinimumChangedEvent ) );

        /// <summary>
        /// Minimum value of the control.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public int Minimum
        {
            get => (int) GetValue( MinimumProperty );
            set => SetValue( MinimumProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="Maximum"/>.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register( nameof( Maximum ), typeof( int ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( 1000, FrameworkPropertyMetadataOptions.AffectsRender, OnMaximumChangedEvent ) );

        /// <summary>
        /// Maximum value of the control.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public int Maximum
        {
            get => (int) GetValue( MaximumProperty );
            set => SetValue( MaximumProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="ValueToText"/>.
        /// </summary>
        public static readonly DependencyProperty ValueToTextProperty =
            DependencyProperty.Register( nameof( ValueToText ), typeof( ValueToTextFunc ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( new ValueToTextFunc( ( _, v ) => DefaultValueToText( v ) ),
                    FrameworkPropertyMetadataOptions.AffectsRender, OnValueChangedEvent ) );

        /// <summary>
        /// Function to convert the value to a text representation.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public ValueToTextFunc ValueToText
        {
            get => (ValueToTextFunc) GetValue( ValueToTextProperty );
            set => SetValue( ValueToTextProperty, value );
        }

        #endregion

        #region Internal-use non-bindable properties

        /// <summary>
        /// Text representation of the value.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public string ValueText
        {
            get => m_valueText;
            set
            {
                m_valueText = value;
                OnValueTextChanged();
            }
        }

        /// <summary>
        /// Decorated text representation of the value.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public string DecoratedValueText { get; private set; } = string.Empty;

        /// <summary>
        /// Valid spin directions for the control.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public ValidSpinDirections ValidSpinDirection
        {
            get
            {
                if( Minimum >= Maximum )
                {
                    return ValidSpinDirections.None;
                }
                else if( Value >= Maximum )
                {
                    return ValidSpinDirections.Decrease;
                }
                else if( Value <= Minimum )
                {
                    return ValidSpinDirections.Increase;
                }
                else
                {
                    return ValidSpinDirections.Increase | ValidSpinDirections.Decrease;
                }
            }
        }

        /// <summary>
        /// Maximum length of the text representation of the value.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public int ValueMaxLength => CalculateMaxLength();

        #endregion

        //===========================================================================
        //                             PUBLIC EVENTS
        //===========================================================================

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        public IntegerUpDown()
        {
            InitializeComponent();

            UpdateViewFromValue();
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <summary>
        /// Converts the given value to a text representation.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Text representation of the value.</returns>
        public static string DefaultValueToText( int value )
        {
            return value.ToString();
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private static void OnValueChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as IntegerUpDown )?.OnValueChangedEvent();
        }

        private void OnValueChangedEvent()
        {
            UpdateViewFromValue();
        }

        private static void OnMinimumChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as IntegerUpDown )?.OnMinimumChangedEvent();
        }

        private void OnMinimumChangedEvent()
        {
            InvokePropertyChanged( nameof( ValidSpinDirection ) );
        }

        private static void OnMaximumChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as IntegerUpDown )?.OnMaximumChangedEvent();
        }

        private void OnMaximumChangedEvent()
        {
            InvokePropertyChanged( nameof( ValidSpinDirection ) );
            InvokePropertyChanged( nameof( ValueMaxLength ) );
        }

        private void OnSpin( object sender, SpinEventArgs e )
        {
            if( e.Direction == SpinDirection.Increase )
            {
                IncrementValue( 1 );
            }
            else
            {
                DecrementValue( 1 );
            }
        }

        private void OnValuePreviewTextInput( object sender, TextCompositionEventArgs e )
        {
            if( !CheckIsValidText( e.Text ) )
            {
                e.Handled = true;
            }
        }

        private void OnValueTextChanged()
        {
            try
            {
                int value = TextToValue( ValueText );

                Value = CoerceValue( value );
            }
            catch( Exception )
            {
                // Value update ignored
            }

            UpdateViewFromValue();
        }

        private void UpdateViewFromValue()
        {
            m_valueText = DefaultValueToText( Value );
            DecoratedValueText = ValueToText( this, Value );

            InvokePropertyChanged( nameof( ValueText ) );
            InvokePropertyChanged( nameof( DecoratedValueText ) );
            InvokePropertyChanged( nameof( ValidSpinDirection ) );
        }

        private void IncrementValue( int increment )
        {
            try
            {
                if( checked(Value + increment) <= Maximum )
                {
                    Value += increment;
                }
                else
                {
                    Value = Maximum;
                }
            }
            catch( OverflowException )
            {
                Value = Maximum;
            }
        }

        private void DecrementValue( int decrement )
        {
            try
            {
                if( checked(Value - decrement) >= Minimum )
                {
                    Value -= decrement;
                }
                else
                {
                    Value = Minimum;
                }
            }
            catch( OverflowException )
            {
                Value = Minimum;
            }
        }

        private int CoerceValue( int value )
        {
            if( value < Minimum )
            {
                return Minimum;
            }
            else if( value > Maximum )
            {
                return Maximum;
            }
            else
            {
                return value;
            }
        }

        private void InvokePropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        private int TextToValue( string text )
        {
            return int.Parse( text );
        }

        private bool CheckIsValidText( string text )
        {
            if( Minimum < 0 )
            {
                return SINT_INPUT_REGEX.IsMatch( text );
            }
            else
            {
                return UINT_INPUT_REGEX.IsMatch( text );
            }
        }

        private int CalculateMaxLength()
        {
            return Math.Max( (int) Math.Ceiling( Math.Log10( Maximum ) ),
                             ( Minimum < 0 ) ? 1 + (int) Math.Ceiling( Math.Log10( -Minimum ) ) : 0 );
        }

        //===========================================================================
        //                           PRIVATE CONSTANTS
        //===========================================================================

        private static readonly Regex UINT_INPUT_REGEX = new( @"^\d*$" );
        private static readonly Regex SINT_INPUT_REGEX = new( @"^[-\d]*$" );

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private string m_valueText = string.Empty;
    }
}

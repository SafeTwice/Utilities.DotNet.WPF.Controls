using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.DotNet.WPF.Controls.Helpers;
using Xceed.Wpf.Toolkit;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Control for selecting an integer value displayed to the user as two values, one representing the value divided
    /// by a factor and the other representing the remainder.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The value of the control corresponds to the following formula: Value = (UpperValue * Factor) + LowerValue
    /// </para>
    /// <para>
    /// Correspondingly, UpperValue = Value / Factor and LowerValue = Value % Factor.
    /// </para>
    /// </remarks>
    public partial class DualIntegerUpDown : UserControl, INotifyPropertyChanged
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        #region Bindable Properties

        /// <summary>
        /// Dependency property for <see cref="Value"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register( nameof( Value ), typeof( int ), typeof( DualIntegerUpDown ),
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
            DependencyProperty.Register( nameof( Minimum ), typeof( int ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( int.MinValue, FrameworkPropertyMetadataOptions.AffectsRender, OnBoundsChangedEvent ) );

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
            DependencyProperty.Register( nameof( Maximum ), typeof( int ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( int.MaxValue, FrameworkPropertyMetadataOptions.AffectsRender, OnBoundsChangedEvent ) );

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
        /// Dependency property for <see cref="Factor"/>.
        /// </summary>
        public static readonly DependencyProperty FactorProperty =
            DependencyProperty.Register( nameof( Factor ), typeof( int ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( 10, FrameworkPropertyMetadataOptions.AffectsRender, OnFactorChangedEvent, CoerceFactor ) );

        /// <summary>
        /// Factor used to calculate <see cref="Value"/> from the two displayed values.
        /// </summary>
        /// <remarks>
        /// <para>Value = (UpperValue * Factor) + LowerValue</para>
        /// </remarks>
        [Bindable( true )]
        [Browsable( true )]
        public int Factor
        {
            get => (int) GetValue( FactorProperty );
            set => SetValue( FactorProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="UpperLabel"/>.
        /// </summary>
        public static readonly DependencyProperty UpperLabelProperty =
            DependencyProperty.Register( nameof( UpperLabel ), typeof( string ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( string.Empty, FrameworkPropertyMetadataOptions.AffectsRender ) );

        /// <summary>
        /// Label for the upper value.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public string UpperLabel
        {
            get => (string) GetValue( UpperLabelProperty );
            set => SetValue( UpperLabelProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="LowerLabel"/>.
        /// </summary>
        public static readonly DependencyProperty LowerLabelProperty =
            DependencyProperty.Register( nameof( LowerLabel ), typeof( string ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( string.Empty, FrameworkPropertyMetadataOptions.AffectsRender ) );

        /// <summary>
        /// Label for the lower value.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public string LowerLabel
        {
            get => (string) GetValue( LowerLabelProperty );
            set => SetValue( LowerLabelProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="UpperValueWidth"/>.
        /// </summary>
        public static readonly DependencyProperty UpperValueWidthProperty =
            DependencyProperty.Register( nameof( UpperValueWidth ), typeof( GridLength ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( new GridLength( 1.0, GridUnitType.Star ),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure ) );

        /// <summary>
        /// Width of the upper value text box.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public GridLength UpperValueWidth
        {
            get => (GridLength) GetValue( UpperValueWidthProperty );
            set => SetValue( UpperValueWidthProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="LowerValueWidth"/>.
        /// </summary>
        public static readonly DependencyProperty LowerValueWidthProperty =
            DependencyProperty.Register( nameof( LowerValueWidth ), typeof( GridLength ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( new GridLength( 1.0, GridUnitType.Star ),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure ) );

        /// <summary>
        /// Width of the lower value text box.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public GridLength LowerValueWidth
        {
            get => (GridLength) GetValue( LowerValueWidthProperty );
            set => SetValue( LowerValueWidthProperty, value );
        }

        /// <summary>
        /// Dependency property for <see cref="ValueFontSize"/>.
        /// </summary>
        public static readonly DependencyProperty ValueFontSizeProperty =
            DependencyProperty.Register( nameof( ValueFontSize ), typeof( double ), typeof( DualIntegerUpDown ),
                new FrameworkPropertyMetadata( FontSizeProperty.DefaultMetadata.DefaultValue,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure ) );

        /// <summary>
        /// Font size of the upper and lower value text boxes.
        /// </summary>
        [Bindable( true )]
        [Browsable( true )]
        public double ValueFontSize
        {
            get => (double) GetValue( ValueFontSizeProperty );
            set => SetValue( ValueFontSizeProperty, value );
        }

        #endregion

        #region Internal-use non-bindable properties

        /// <summary>
        /// Text representation of the upper value.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public string UpperValueText
        {
            get => m_upperValueText;
            set
            {
                m_upperValueText = value;
                OnUpperValueTextChanged();
            }
        }

        /// <summary>
        /// Text representation of the lower value.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public string LowerValueText
        {
            get => m_lowerValueText;
            set
            {
                m_lowerValueText = value;
                OnLowerValueTextChanged();
            }
        }

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
        /// Maximum length of the upper value text box.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public int UpperValueMaxLength => Math.Max( CalculateMaxLength( Maximum / Factor ),
                                                    CalculateMaxLength( Minimum / Factor ) );

        /// <summary>
        /// Maximum length of the lower value text box.
        /// </summary>
        [Bindable( false )]
        [Browsable( false )]
        public int LowerValueMaxLength => CalculateMaxLength( Factor - 1 );

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
        public DualIntegerUpDown()
        {
            InitializeComponent();

            UpdateViewFromValue();
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private static void OnValueChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as DualIntegerUpDown )?.OnValueChangedEvent();
        }

        private void OnValueChangedEvent()
        {
            UpdateViewFromValue();
        }

        private static void OnBoundsChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as DualIntegerUpDown )?.OnBoundsChangedEvent();
        }

        private void OnBoundsChangedEvent()
        {
            Value = CoerceValue( Value );

            InvokePropertyChanged( nameof( Value ) );
            InvokePropertyChanged( nameof( ValidSpinDirection ) );
            InvokePropertyChanged( nameof( UpperValueMaxLength ) );
        }

        private static void OnFactorChangedEvent( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as DualIntegerUpDown )?.OnFactorChangedEvent();
        }

        private void OnFactorChangedEvent()
        {
            InvokePropertyChanged( nameof( UpperValueMaxLength ) );
            InvokePropertyChanged( nameof( LowerValueMaxLength ) );

            UpdateViewFromValue();
        }

        private void OnLowerSpin( object sender, SpinEventArgs e )
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

        private void OnUpperSpin( object sender, SpinEventArgs e )
        {
            if( e.Direction == SpinDirection.Increase )
            {
                IncrementValue( Factor );
            }
            else
            {
                DecrementValue( Factor );
            }
        }

        private void OnUpperValuePreviewTextInput( object sender, TextCompositionEventArgs e )
        {
            if( !e.IsValidInteger( Minimum < 0 ) )
            {
                e.Handled = true;
            }
        }

        private void OnLowerValuePreviewTextInput( object sender, TextCompositionEventArgs e )
        {
            if( !e.Text.All( char.IsDigit ) )
            {
                e.Handled = true;
            }
        }

        private void OnUpperValueTextChanged()
        {
            try
            {
                checked
                {
                    int value = int.Parse( UpperValueText );
                    value *= Factor;

                    // Negativeness must be checked at string level to detect properly the "-0" case.
                    var isNegative = UpperValueText.StartsWith( "-" );

                    int lowerValue;
                    if( Value > 0 )
                    {
                        lowerValue = Value % Factor;
                    }
                    else
                    {
                        lowerValue = (int) ( ( -( (long) Value ) ) % Factor );
                    }

                    value += isNegative ? -lowerValue : lowerValue;

                    Value = CoerceValue( value );
                }
            }
            catch( OverflowException )
            {
                if( UpperValueText.StartsWith( "-" ) )
                {
                    Value = Minimum;
                }
                else
                {
                    Value = Maximum;
                }
            }
            catch( Exception )
            {
                // Value update ignored
            }

            UpdateViewFromValue();
        }

        private void OnLowerValueTextChanged()
        {
            try
            {
                checked
                {
                    int lowerValue = int.Parse( LowerValueText );

                    if( lowerValue < 0 )
                    {
                        lowerValue = 0;
                    }
                    else if( lowerValue >= Factor )
                    {
                        lowerValue = ( Factor - 1 );
                    }

                    int value = Value / Factor;
                    value *= Factor;

                    value += ( Value < 0 ) ? -lowerValue : lowerValue;

                    Value = CoerceValue( value );
                }
            }
            catch( Exception )
            {
                // Value update ignored
            }

            UpdateViewFromValue();
        }

        private void UpdateViewFromValue()
        {
            if( Value >= 0 )
            {
                var upperValue = Value / Factor;
                var lowerValue = Value % Factor;

                m_upperValueText = upperValue.ToString();
                m_lowerValueText = lowerValue.ToString();
            }
            else
            {
                var absValue = -( (long) Value );
                var upperValue = absValue / Factor;
                var lowerValue = absValue % Factor;

                m_upperValueText = "-" + upperValue.ToString();
                m_lowerValueText = lowerValue.ToString();
            }

            InvokePropertyChanged( nameof( UpperValueText ) );
            InvokePropertyChanged( nameof( LowerValueText ) );
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

        private static object CoerceFactor( DependencyObject d, object baseValue )
        {
            int factor = (int) baseValue;
            if( factor < 2 )
            {
                return 2;
            }
            else
            {
                return factor;
            }
        }

        private static int CalculateMaxLength( int value )
        {
            if( value == int.MaxValue )
            {
                return 11;
            }

            int magnitude;
            if( value == 0 )
            {
                magnitude = 0;
            }
            else if( value > 0 )
            {
                magnitude = (int) Math.Floor( Math.Log10( value ) );
            }
            else
            {
                magnitude = ( 1 + (int) Math.Floor( Math.Log10( -value ) ) );
            }

            return ( magnitude + 1 );
        }


        private void InvokePropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private string m_upperValueText = string.Empty;
        private string m_lowerValueText = string.Empty;
    }
}

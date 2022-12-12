﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using XtraControls.Utilities;

namespace XtraControls
{
    using ValueToTextFunc = Func<IntegerUpDown, int, string>;
    using TextToValueFunc = Func<IntegerUpDown, string, int>;
    using CheckIsValidTextFunc = Func<IntegerUpDown, string, bool>;
    using CalculateMaxLengthFunc = Func<IntegerUpDown, int>;

    /// <summary>
    /// Interaction logic for IntegerUpDown.xaml
    /// </summary>
    public partial class IntegerUpDown : UserControl, INotifyPropertyChanged
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        #region Bindable Properties

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register( nameof( Value ), typeof( int ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( 0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChangedEvent ) );

        [Bindable( true )]
        [Browsable( true )]
        public int Value
        {
            get => (int) GetValue( ValueProperty );
            set => SetValue( ValueProperty, value );
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register( nameof( Minimum ), typeof( int ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( 0, FrameworkPropertyMetadataOptions.AffectsRender, OnMinimumChangedEvent ) );

        [Bindable( true )]
        [Browsable( true )]
        public int Minimum
        {
            get => (int) GetValue( MinimumProperty );
            set => SetValue( MinimumProperty, value );
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register( nameof( Maximum ), typeof( int ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( 1000, FrameworkPropertyMetadataOptions.AffectsRender, OnMaximumChangedEvent ) );

        [Bindable( true )]
        [Browsable( true )]
        public int Maximum
        {
            get => (int) GetValue( MaximumProperty );
            set => SetValue( MaximumProperty, value );
        }

        public static readonly DependencyProperty ValueToTextProperty =
            DependencyProperty.Register( nameof( ValueToText ), typeof( ValueToTextFunc ), typeof( IntegerUpDown ),
                new FrameworkPropertyMetadata( new ValueToTextFunc( (o, v) => o.DefaultValueToText( v ) ),
                    FrameworkPropertyMetadataOptions.AffectsRender, OnValueChangedEvent ) );

        [Bindable( true )]
        [Browsable( true )]
        public ValueToTextFunc ValueToText
        {
            get => (ValueToTextFunc) GetValue( ValueToTextProperty );
            set => SetValue( ValueToTextProperty, value );
        }

        #endregion

        #region Internal-use non-bindable properties

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

        [Bindable( false )]
        [Browsable( false )]
        public string DecoratedValueText { get; private set; } = string.Empty;

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

        [Bindable( false )]
        [Browsable( false )]
        public int ValueMaxLength => CalculateMaxLength();

        #endregion

        //===========================================================================
        //                             PUBLIC EVENTS
        //===========================================================================

        public event PropertyChangedEventHandler? PropertyChanged;

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        public IntegerUpDown()
        {
            InitializeComponent();

            UpdateViewFromValue();
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public string DefaultValueToText( int value )
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

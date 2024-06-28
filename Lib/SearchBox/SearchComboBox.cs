/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Windows;
using System.Windows.Controls;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Specialized <see cref="ComboBox"/> that provides access to the toggle button.
    /// </summary>
    internal class SearchComboBox : ComboBox
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        public Thickness ActiveZoneMargin
        {
            get
            {
                double leftMargin = 0.0;
                double rightMargin = 0.0;
                double topMargin = 0.0;
                double bottomMargin = 0.0;

                if( m_textBox != null )
                {
                    // Force layout update
                    UpdateLayout();

                    var relativeLocation = m_textBox.TranslatePoint( new( 0, 0 ), this );

                    leftMargin = relativeLocation.X - m_textBox.Margin.Left;

                    var rightLocation = relativeLocation.X + m_textBox.ActualWidth + m_textBox.Margin.Right;
                    rightMargin = ActualWidth - rightLocation;

                    topMargin = relativeLocation.Y - m_textBox.Margin.Top;
                    
                    var bottomLocation = relativeLocation.Y + m_textBox.ActualHeight + m_textBox.Margin.Bottom;
                    bottomMargin = ActualHeight - bottomLocation;

                    if( ( ( topMargin - BorderThickness.Top ) < 2.0 ) &&
                        ( ( bottomMargin - BorderThickness.Bottom ) < 2.0 ) )
                    {
                        rightMargin -= 1.0; // Correction except with the Classic theme
                    }
                }

                if( leftMargin <= 0 )
                {
                    leftMargin = BorderThickness.Left;
                }
                if( rightMargin <= 0 )
                {
                    rightMargin = SystemParameters.VerticalScrollBarWidth + BorderThickness.Right;
                }
                if( topMargin <= 0 )
                {
                    topMargin = BorderThickness.Top;
                }
                if( bottomMargin <= 0 )
                {
                    bottomMargin = BorderThickness.Bottom;
                }

                return new Thickness( leftMargin, topMargin, rightMargin, bottomMargin );
            }
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_textBox = GetTemplateChild( "PART_EditableTextBox" ) as Control;
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private Control? m_textBox;
    }
}

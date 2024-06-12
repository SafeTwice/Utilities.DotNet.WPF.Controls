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

        public double ToggleButtonWidth
        {
            get
            {
                double width = 0.0;

                if( m_toggleButton != null )
                {
                    width = m_toggleButton.ActualWidth - ( m_toggleButton.BorderThickness.Left + m_toggleButton.BorderThickness.Right );
                }

                if( width <= 0 )
                {
                    width = SystemParameters.VerticalScrollBarWidth;
                }

                return width;
            }
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_toggleButton = GetTemplateChild( "toggleButton" ) as Button;
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private Button? m_toggleButton;
    }
}

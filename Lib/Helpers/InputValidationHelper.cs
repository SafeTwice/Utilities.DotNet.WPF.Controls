using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utilities.DotNet.WPF.Controls.Helpers
{
    internal static class InputValidationHelper
    {
        /// <summary>
        /// Computes the resulting text of a <see cref="TextBox"/> after applying the current text composition.
        /// </summary>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> the text input to compute.</param>
        /// <returns>The text of the <see cref="TextBox"/> as it would appear after the text composition is applied.</returns>
        public static string GetFinalText( this TextCompositionEventArgs e )
        {
            Debug.Assert( e.Source is TextBox );

            var textBox = (TextBox) e.Source;

            var mergedText = textBox.Text;
            if( textBox.SelectionLength > 0 )
            {
                mergedText = mergedText.Substring( 0, textBox.SelectionStart ) +
                             e.Text +
                             mergedText.Substring( textBox.SelectionStart + textBox.SelectionLength );
            }
            else
            {
                mergedText = mergedText.Substring( 0, textBox.CaretIndex ) +
                             e.Text +
                             mergedText.Substring( textBox.CaretIndex );
            }

            return mergedText;
        }

        /// <summary>
        /// Determines whether the text input from a <see cref="TextCompositionEventArgs"/> obtains a valid integer.
        /// </summary>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> containing the text input to validate.</param>
        /// <param name="canBeNegative">Indicates if the integer can be negative.</param>
        /// <returns><see langword="true"/> if the resulting string after the text input is a valid signed integer; 
        ///          <see langword="false"/> otherwise.</returns>
        public static bool IsValidInteger( this TextCompositionEventArgs e, bool canBeNegative )
        {
            var finalText = e.GetFinalText();

            if( canBeNegative )
            {
                return SINT_INPUT_REGEX.IsMatch( finalText );
            }
            else
            {
                return UINT_INPUT_REGEX.IsMatch( finalText );
            }
        }

        //===========================================================================
        //                           PRIVATE CONSTANTS
        //===========================================================================

        private static readonly Regex UINT_INPUT_REGEX = new( @"^\d*$" );
        private static readonly Regex SINT_INPUT_REGEX = new( @"^-?\d*$" );

    }
}

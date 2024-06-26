/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Windows;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Contains event data for the Find command.
    /// </summary>
    public class FindCommandArgs
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Text to find.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Indicates if the search should be done backwards.
        /// </summary>
        public bool SearchBackwards { get; }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text to find.</param>
        /// <param name="searchBackwards">Indicates if the search should be done backwards.</param>
        public FindCommandArgs( string text, bool searchBackwards )
        {
            Text = text;
            SearchBackwards = searchBackwards;
        }
    }
}

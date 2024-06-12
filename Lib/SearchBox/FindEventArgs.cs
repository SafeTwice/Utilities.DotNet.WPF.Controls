/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Windows;

namespace Utilities.DotNet.WPF.Controls
{
    /// <summary>
    /// Delegate for the Find event.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void FindEventHandler( object sender, FindEventArgs e );

    /// <summary>
    /// Contains event data for the Find event.
    /// </summary>
    public class FindEventArgs : RoutedEventArgs
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <summary>
        /// Text to find.
        /// </summary>
        public string Text { get; }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled.
        ///                      This pre-populates the <see cref="RoutedEventArgs.Source">Source</see> property.</param>
        /// <param name="text">Text to find.</param>
        public FindEventArgs( RoutedEvent routedEvent, object source, string text ) : base( routedEvent, source )
        {
            Text = text;
        }
    }
}

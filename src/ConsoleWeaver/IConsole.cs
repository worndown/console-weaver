namespace ConsoleWeaver
{
    using System;

    /// <summary>
    /// Abstraction for console I/O operations.
    /// Enables testing and alternative output targets.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Writes a character to the output stream.
        /// </summary>
        /// <param name="value">The character to write.</param>
        void Write(char value);

        /// <summary>
        /// Writes a string to the output stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        void Write(string? value);

        /// <summary>
        /// Writes a line terminator to the output stream.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a string followed by a line terminator to the output stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        void WriteLine(string? value);

        /// <summary>
        /// Reads a line of characters from the input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        string? ReadLine();

        /// <summary>
        /// Reads the next key pressed by the user.
        /// </summary>
        /// <param name="intercept">Whether to prevent the pressed key from being displayed in the console window.</param>
        /// <returns>Information about the key that was pressed.</returns>
        ConsoleKeyInfo ReadKey(bool intercept);

        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background color of the console.
        /// </summary>
        ConsoleColor BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cursor is visible.
        /// </summary>
        bool CursorVisible { get; set; }

        /// <summary>
        /// Gets a value indicating whether output has been redirected from the standard output stream.
        /// </summary>
        bool IsOutputRedirected { get; }
    }
}

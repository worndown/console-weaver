namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Interface for converting string values to target types.
    /// </summary>
    internal interface IValueConverter
    {
        /// <summary>
        /// Determines whether this converter can convert to the specified type.
        /// </summary>
        /// <param name="targetType">The target type to convert to.</param>
        /// <returns>true if this converter can handle the conversion; otherwise, false.</returns>
        bool CanConvert(Type targetType);

        /// <summary>
        /// Converts a string value to the specified target type.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentException">Thrown when conversion fails.</exception>
        object Convert(string value, Type targetType);
    }
}

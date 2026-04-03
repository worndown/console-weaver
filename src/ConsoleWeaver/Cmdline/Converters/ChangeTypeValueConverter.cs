namespace ConsoleWeaver.Cmdline.Converters
{
    using System;

    /// <summary>
    /// Converts string values using Convert.ChangeType.
    /// This is a fallback converter for primitive types and types implementing IConvertible.
    /// </summary>
    internal sealed class ChangeTypeValueConverter : IValueConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(Type targetType)
        {
            // ChangeType can handle any type that implements IConvertible
            // We return true as this is the fallback converter
            return true;
        }

        /// <inheritdoc/>
        public object Convert(string value, Type targetType)
        {
            return System.Convert.ChangeType(value, targetType);
        }
    }
}

namespace ConsoleWeaver.Cmdline.Converters
{
    using System;

    /// <summary>
    /// Converts string values to enum types.
    /// </summary>
    internal sealed class EnumValueConverter : IValueConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(Type targetType)
        {
            return targetType.IsEnum;
        }

        /// <inheritdoc/>
        public object Convert(string value, Type targetType)
        {
            return Enum.Parse(targetType, value, ignoreCase: true);
        }
    }
}

namespace ConsoleWeaver.Cmdline.Converters
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Converts string values using a constructor that accepts a single string parameter.
    /// Only applies to class types.
    /// </summary>
    internal sealed class ConstructorValueConverter : IValueConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(Type targetType)
        {
            return targetType.IsClass && GetStringConstructor(targetType) is not null;
        }

        /// <inheritdoc/>
        public object Convert(string value, Type targetType)
        {
            ConstructorInfo constructor = GetStringConstructor(targetType)
                ?? throw new ArgumentException($"Type {targetType.Name} does not have a constructor that accepts a string.");

            object? result = Activator.CreateInstance(targetType, value);
            return result ?? throw new ArgumentException($"Failed to create instance of {targetType.Name} from value '{value}'.");
        }

        private static ConstructorInfo? GetStringConstructor(Type type)
        {
            return type.GetConstructor([typeof(string)]);
        }
    }
}

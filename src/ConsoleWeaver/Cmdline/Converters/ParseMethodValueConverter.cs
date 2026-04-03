namespace ConsoleWeaver.Cmdline.Converters
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Converts string values using a static Parse(string) method on the target type.
    /// </summary>
    internal sealed class ParseMethodValueConverter : IValueConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(Type targetType)
        {
            return GetParseMethod(targetType) is not null;
        }

        /// <inheritdoc/>
        public object Convert(string value, Type targetType)
        {
            MethodInfo parseMethod = GetParseMethod(targetType)
                ?? throw new ArgumentException($"Type {targetType.Name} does not have a Parse(string) method.");

            object? result = parseMethod.Invoke(null, new object[] { value });
            return result ?? throw new ArgumentException($"{targetType.Name}.Parse returned null for value '{value}'.");
        }

        private static MethodInfo? GetParseMethod(Type type)
        {
            return type.GetMethod(
                "Parse",
                BindingFlags.Public | BindingFlags.ExactBinding | BindingFlags.Static,
                null,
                [typeof(string)],
                null);
        }
    }
}

namespace ConsoleWeaver.Cmdline
{
    using System;
    using ConsoleWeaver.Cmdline.Converters;

    /// <summary>
    /// A composite converter that tries each converter in order until one succeeds.
    /// </summary>
    internal sealed class ValueConverterChain : IValueConverter
    {
        /// <summary>
        /// Default converter chain with standard converters in priority order:
        /// 1. Enum conversion
        /// 2. Parse method conversion
        /// 3. String constructor conversion
        /// 4. ChangeType fallback
        /// </summary>
        public static readonly ValueConverterChain Default = new ValueConverterChain(
            new EnumValueConverter(),
            new ParseMethodValueConverter(),
            new ConstructorValueConverter(),
            new ChangeTypeValueConverter());

        private readonly IValueConverter[] converters;

        /// <summary>
        /// Initializes a new instance of the ValueConverterChain class.
        /// </summary>
        /// <param name="converters">The converters to use, in priority order.</param>
        public ValueConverterChain(params IValueConverter[] converters)
        {
            this.converters = Precondition.CheckNotNull(converters, nameof(converters));
        }

        /// <inheritdoc/>
        public bool CanConvert(Type targetType)
        {
            foreach (IValueConverter converter in this.converters)
            {
                if (converter.CanConvert(targetType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public object Convert(string value, Type targetType)
        {
            foreach (IValueConverter converter in this.converters)
            {
                if (converter.CanConvert(targetType))
                {
                    return converter.Convert(value, targetType);
                }
            }

            throw new ArgumentException($"No converter found for type {targetType.Name}.");
        }
    }
}

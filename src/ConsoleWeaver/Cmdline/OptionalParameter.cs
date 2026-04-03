namespace ConsoleWeaver.Cmdline
{
    /// <summary>
    /// Optional parameter
    /// </summary>
    internal sealed class OptionalParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the OptionalParameter class.
        /// </summary>
        /// <param name="shortName">Short parameter name</param>
        /// <param name="longName">Long parameter name</param>
        /// <param name="metaValue">Parameter meta-value</param>
        /// <param name="desc">Parameter description</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="isMultivalued">Whether parameter is multivalued</param>
        public OptionalParameter(string? shortName, string? longName, string metaValue, string desc, string? defaultValue = null, bool isMultivalued = false)
            : base(shortName, longName, metaValue, desc, false, isMultivalued, false, defaultValue, -1)
        {
        }
    }
}

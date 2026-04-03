namespace ConsoleWeaver.Cmdline
{
    /// <summary>
    /// Mandatory parameter
    /// </summary>
    internal sealed class MandatoryParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the MandatoryParameter class
        /// </summary>
        /// <param name="shortName">Short parameter name</param>
        /// <param name="longName">Long parameter name</param>
        /// <param name="metaValue">Parameter meta-value</param>
        /// <param name="desc">Parameter description</param>
        /// <param name="isMultivalued">Whether parameter is multi-values</param>
        /// <param name="position">Parameter position</param>
        public MandatoryParameter(string? shortName, string? longName, string metaValue, string desc, bool isMultivalued, int position)
            : base(shortName, longName, metaValue, desc, true, isMultivalued, false, null, position)
        {
        }
    }
}

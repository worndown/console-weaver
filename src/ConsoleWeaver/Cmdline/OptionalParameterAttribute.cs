namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Optional parameter attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionalParameterAttribute : Attribute
    {
        /// <summary>
        /// Short parameter name
        /// </summary>
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// Long parameter name
        /// </summary>
        public string LongName { get; set; } = string.Empty;

        /// <summary>
        /// Parameter meta-value
        /// </summary>
        public string MetaValue { get; set; } = string.Empty;

        /// <summary>
        /// Parameter description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Optional default value
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Creates parameter definition
        /// </summary>
        /// <param name="multiValued">Whether parameter is multivalued.</param>
        /// <returns>Parameter definition</returns>
        internal Parameter CreateParameterDefinition(bool multiValued)
        {
            return new OptionalParameter(this.ShortName, this.LongName, this.MetaValue, this.Description, this.DefaultValue, multiValued);
        }
    }
}

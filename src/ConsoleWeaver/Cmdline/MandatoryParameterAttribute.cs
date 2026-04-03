namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Mandatory parameter attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MandatoryParameterAttribute : Attribute
    {
        /// <summary>
        /// Initialized new instance of the MandatoryParameterAttribute class
        /// </summary>
        public MandatoryParameterAttribute()
        {
            this.Position = -1;
        }

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
        /// Optional parameter position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Creates parameter definition
        /// </summary>
        /// <param name="multiValued">Whether parameter is multivalued</param>
        /// <returns>Parameter definition</returns>
        internal Parameter CreateParameterDefinition(bool multiValued)
        {
            return new MandatoryParameter(this.ShortName, this.LongName, this.MetaValue, this.Description, multiValued, this.Position);
        }
    }
}

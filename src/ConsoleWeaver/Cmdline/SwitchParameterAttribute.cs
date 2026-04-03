namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Switch parameter attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SwitchParameterAttribute : Attribute
    {
        /// <summary>
        /// Short switch parameter name
        /// </summary>
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// Long switch parameter name
        /// </summary>
        public string LongName { get; set; } = string.Empty;

        /// <summary>
        /// Switch parameter description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Create parameter definition
        /// </summary>
        /// <returns>Switch parameter definition</returns>
        internal SwitchParameter CreateParameterDefinition()
        {
            return new SwitchParameter(this.ShortName, this.LongName, this.Description);
        }
    }
}

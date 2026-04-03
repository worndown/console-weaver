namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Parameter set attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ParameterSetAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ParameterSetAttribute class.
        /// </summary>
        /// <param name="parameterSetName">Parameter set name.</param>
        public ParameterSetAttribute(string parameterSetName)
        {
            this.Name = Precondition.CheckNotNull(parameterSetName, nameof(parameterSetName));
        }

        /// <summary>
        /// Parameter set name.
        /// </summary>
        public string Name { get; }
    }
}

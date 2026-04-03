namespace ConsoleWeaver.Cmdline
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This exception is raised when parameter was not defined correctly.
    /// </summary>
    [Serializable]
    public class ParameterDefinitionException : ParameterException
    {
        /// <summary>
        /// Initializes a new instance of the ParameterDefinitionException class
        /// </summary>
        /// <param name="message">Error message</param>
        public ParameterDefinitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParameterDefinitionException class
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="parameter">Parameter name</param>
        public ParameterDefinitionException(string parameter, string message)
            : base(parameter, message)
        {
        }
    }
}

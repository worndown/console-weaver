namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Parameter definition or parsing exception.
    /// </summary>
    [Serializable]
    public class ParameterException : PermanentException
    {
        /// <summary>
        /// Initializes a new instance of the ParameterException class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public ParameterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParameterException class.
        /// </summary>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="message">Error message.</param>
        public ParameterException(string? parameter, string message)
            : base(message)
        {
            this.ParameterName = parameter;
        }

        /// <summary>
        /// Initializes a new instance of the ParameterException class.
        /// </summary>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="format">Message format.</param>
        /// <param name="args">Format arguments.</param>
        public ParameterException(string? parameter, string format, params object[] args)
            : base(string.Format(format, args))
        {
            this.ParameterName = parameter;
        }

        /// <summary>
        /// Parameter name
        /// </summary>
        public string? ParameterName { get; private set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public override string Message =>
            this.ParameterName is not null
                ? $"{base.Message} Parameter: {this.ParameterName}."
                : base.Message;
    }
}

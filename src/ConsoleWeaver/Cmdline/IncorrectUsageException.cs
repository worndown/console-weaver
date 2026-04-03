namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Raised when incorrect cmdline options were specified.
    /// Exceptions message contains correct usage syntax.
    /// </summary>
    [Serializable]
    public class IncorrectUsageException : ParameterException
    {
        /// <summary>
        /// Initializes a new instance of the IncorrectUsageException class
        /// </summary>
        /// <param name="message">String containing correct usage</param>
        public IncorrectUsageException(string message)
            : base(message)
        {
        }
    }
}

namespace ConsoleWeaver
{
    using System;

    /// <summary>
    /// Exception representing a transient error.
    /// </summary>
    [Serializable]
    public abstract class TransientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TransientException class.
        /// </summary>
        protected TransientException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TransientException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected TransientException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TransientException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected TransientException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}

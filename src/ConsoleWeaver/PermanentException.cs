namespace ConsoleWeaver
{
    using System;

    /// <summary>
    /// Exception representing a permanent error.
    /// </summary>
    [Serializable]
    public abstract class PermanentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the PermanentException class.
        /// </summary>
        protected PermanentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PermanentException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected PermanentException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PermanentException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected PermanentException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
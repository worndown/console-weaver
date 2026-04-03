namespace ConsoleWeaver
{
    /// <summary>
    /// An assert exception.
    /// </summary>
    [Serializable]
    public sealed class AssertException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the AssertException class.
        /// </summary>
        public AssertException()
            : this("An assertion failed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssertException class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public AssertException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssertException class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public AssertException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}

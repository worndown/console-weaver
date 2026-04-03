namespace ConsoleWeaver.Cmdline
{
    using System;

    /// <summary>
    /// Thrown when parameter set cannot be resolved.
    /// </summary>
    [Serializable]
    public class ParameterSetResolutionException : ParameterException
    {
        /// <summary>
        /// Initializes a new instance of the ParameterSetResolutionException class.
        /// </summary>
        public ParameterSetResolutionException()
            : base("Parameter set cannot be resolved using specified parameters.")
        {
        }
    }
}

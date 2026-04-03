namespace ConsoleWeaver.Cmdline
{
    /// <summary>
    /// Switch parameter
    /// </summary>
    internal sealed class SwitchParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the SwitchParameter
        /// </summary>
        /// <param name="shortName">Short switch name</param>
        /// <param name="longName">Long switch name</param>
        /// <param name="desc">Switch parameter description</param>
        public SwitchParameter(string? shortName, string? longName, string desc)
            : base(shortName, longName, null, desc, false, false, true, null, -1)
        {
        }
    }
}

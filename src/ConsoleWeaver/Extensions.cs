namespace ConsoleWeaver
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Truncates string to the specified length.
        /// </summary>
        /// <param name="instance">String to truncate.</param>
        /// <param name="length">Length to truncate string to.</param>
        /// <param name="ellipsis">Use ellipsis at the end.</param>
        /// <returns>Truncated string value of the requested length.</returns>
        public static string Truncate(this string instance, int length, bool ellipsis = true)
        {
            Precondition.CheckArgument(length > (ellipsis ? 3 : 0), "Invalid length.", nameof(length));

            if (instance.Length <= length)
            {
                return instance;
            }

            return ellipsis ? $"{instance.Substring(0, length - 3)}..." : instance.Substring(0, length);
        }
    }
}

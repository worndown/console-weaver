namespace ConsoleWeaver
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Helper methods for checking various preconditions.
    /// </summary>
    internal static class Precondition
    {
        /// <summary>
        /// Asserts that the condition is true. Debug only.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new AssertException(errorMessage);
            }
        }

        /// <summary>
        /// Asserts that the condition is true.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void RetailAssert(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new AssertException(errorMessage);
            }
        }

        /// <summary>
        /// Throws ArgumentException if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void CheckArgument(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new ArgumentException(errorMessage);
            }
        }

        /// <summary>
        /// Throws ArgumentException with the specified message if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="paramName">The parameter name.</param>
        public static void CheckArgument(bool condition, string errorMessage, string paramName)
        {
            if (!condition)
            {
                throw new ArgumentException(errorMessage, paramName);
            }
        }

        /// <summary>
        /// Throws ArgumentNullException if reference is null.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="reference">The reference.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The reference that passed the check.</returns>
        public static T CheckNotNull<T>(T? reference, string paramName) where T : class
        {
            if (reference == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return reference;
        }

        /// <summary>
        /// Throws ArgumentException if string is null or empty.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The string that passed the check.</returns>
        public static string CheckNotNullOrEmpty(string? str, string paramName)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("String cannot be null or empty.", paramName);
            }

            return str;
        }

        /// <summary>
        /// Throws ArgumentException if string is null or empty.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The string that passed the check.</returns>
        public static string CheckNotNullOrEmpty(string? str, string errorMessage, string paramName)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(errorMessage, paramName);
            }

            return str;
        }

        /// <summary>
        /// Throws ArgumentException if string is null, empty or white space.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The string that passed the check.</returns>
        public static string CheckNotNullOrWhiteSpace(string? str, string paramName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("String cannot be null, empty or white space.", paramName);
            }

            return str;
        }

        /// <summary>
        /// Throws ArgumentException if string is null, empty or white space.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The string that passed the check.</returns>
        public static string CheckNotNullOrWhiteSpace(string? str, string errorMessage, string paramName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException(errorMessage, paramName);
            }

            return str;
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException if integer value is outside specified range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">Min inclusive value.</param>
        /// <param name="max">Max inclusive value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static int CheckInRange(int value, int min, int max, string paramName)
        {
            Assert(max > min, "Invalid range definition.");

            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    $"Argument is out of range. Valid range: [{min}-{max}].");
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException if double value is outside specified range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">Min inclusive value.</param>
        /// <param name="max">Max inclusive value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static double CheckInRange(double value, double min, double max, string paramName)
        {
            Assert(max > min, "Invalid range definition.");
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    $"Argument is out of range. Valid range: [{min}-{max}].");
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if short value is negative.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static short CheckNotNegative(short value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentException("The value cannot be negative.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if integer value is negative.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static int CheckNotNegative(int value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentException("The value cannot be negative.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if long value is negative.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static long CheckNotNegative(long value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentException("The value cannot be negative.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if float value is negative.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static float CheckNotNegative(float value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentException("The value cannot be negative.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if double value is negative.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static double CheckNotNegative(double value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentException("The value cannot be negative.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if short value is less than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static short CheckGreaterThanZero(short value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentException("The value must be greater than 0.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if integer value is less than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static int CheckGreaterThanZero(int value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentException("The value must be greater than 0.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if long value is less than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static long CheckGreaterThanZero(long value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentException("The value must be greater than 0.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if float value is less than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static float CheckGreaterThanZero(float value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentException("The value must be greater than 0.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws ArgumentException if double value is less than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <returns>The value that passed the check.</returns>
        public static double CheckGreaterThanZero(double value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentException("The value must be greater than 0.", paramName);
            }

            return value;
        }

        /// <summary>
        /// Throws InvalidOperationException if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public static void CheckState(bool condition)
        {
            if (!condition)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Throws InvalidOperationException with the specified message if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void CheckState(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Throws ObjectDisposedException if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="objectName">The object name.</param>
        public static void CheckNotDisposed(bool condition, string objectName)
        {
            if (!condition)
            {
                throw new ObjectDisposedException(objectName);
            }
        }
    }
}

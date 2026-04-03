namespace ConsoleWeaver.Utils
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Helper for parsing and formatting CSV strings.
    /// </summary>
    public static class CommaSeparatedValues
    {
        private const char QuoteChar = '"';
        private const char CommaChar = ',';

        /// <summary>
        /// This is special case when string contains single comma character.
        /// </summary>
        private static readonly string[] TwoEmptyColumnArray = [string.Empty, string.Empty];

        /// <summary>
        /// To minimize number of allocations parsed individual values are stored here.
        /// </summary>
        [ThreadStatic]
        private static List<string>? result;

        /// <summary>
        /// Parses comma separated values in the provided string.
        /// </summary>
        /// <param name="input">String with comma separated values.</param>
        /// <returns>Array of values.</returns>
        public static unsafe string[] Parse(string input)
        {
            Precondition.CheckNotNull(input, nameof(input));

            int length = input.Length;
            if (length < 2)
            {
                // Handle corner cases without doing more exhaustive parsing.
                if (length == 0)
                {
                    return Array.Empty<string>();
                }

                if (input[0] == CommaChar)
                {
                    return TwoEmptyColumnArray;
                }

                if (input[0] == QuoteChar)
                {
                    throw new FormatException("Double quotes are not allowed inside unquoted fields.");
                }

                return new string[] { input };
            }

            // Reset state
            if (result == null)
            {
                result = new List<string>(16);
            }
            else
            {
                result.Clear();
            }

            char[] output = ArrayPool<char>.Shared.Rent(input.Length);

            try
            {
                // Parse separated values
                fixed (char* pInputStart = input)
                fixed (char* pOutputStart = output)
                {
                    char* pInput = pInputStart;
                    char* pOutput = pOutputStart;
                    char* pLast = pInput + length - 1;

                    int consecutiveQuoteCount = 0;
                    bool insideQuotedValue = false;

                    while (pInput <= pLast)
                    {
                        if (*pInput == QuoteChar)
                        {
                            if (insideQuotedValue)
                            {
                                if ((pInput == pLast || *(pInput + 1) == CommaChar) && consecutiveQuoteCount == 0)
                                {
                                    result.Add(new string(pOutputStart, 0, (int)(pOutput - pOutputStart)));
                                    pOutput = pOutputStart;
                                    insideQuotedValue = false;
                                    ++pInput;
                                    continue;
                                }

                                if (++consecutiveQuoteCount == 1)
                                {
                                    ++pInput;
                                    continue;
                                }

                                if (*(pInput - 1) == QuoteChar)
                                {
                                    if (pInput < pLast)
                                    {
                                        *pOutput = *pInput;
                                        ++pOutput;
                                        ++pInput;
                                        consecutiveQuoteCount = 0;
                                        continue;
                                    }

                                    throw new FormatException($"Missing closing double quote at position {pInput - pInputStart}.");
                                }
                            }
                            else
                            {
                                if (pInput == pInputStart || *(pInput - 1) == CommaChar)
                                {
                                    // Beginning of a quoted value
                                    insideQuotedValue = true;
                                    ++pInput;
                                }
                                else
                                {
                                    throw new FormatException($"Double quotes are not allowed inside unquoted fields. Position {pInput - pInputStart}.");
                                }
                            }

                            continue;
                        }

                        if (consecutiveQuoteCount != 0)
                        {
                            throw new FormatException($"Unescaped double quote at position {pInput - pInputStart}.");
                        }

                        if (*pInput == CommaChar)
                        {
                            if (insideQuotedValue)
                            {
                                *pOutput++ = *pInput;
                            }
                            else
                            {
                                if (pOutput - pOutputStart > 0 || pInput == pInputStart)
                                {
                                    result.Add(new string(pOutputStart, 0, (int)(pOutput - pOutputStart)));
                                    pOutput = pOutputStart;
                                }
                                else if (*(pInput - 1) == CommaChar)
                                {
                                    result.Add(string.Empty);
                                }

                                if (pInput == pLast)
                                {
                                    // Last column is empty
                                    result.Add(string.Empty);
                                }
                            }

                            ++pInput;
                            continue;
                        }

                        *pOutput = *pInput;
                        ++pOutput;
                        ++pInput;
                    }

                    if (pOutput - pOutputStart > 0)
                    {
                        result.Add(new string(pOutputStart, 0, (int)(pOutput - pOutputStart)));
                    }

                    if (insideQuotedValue)
                    {
                        throw new FormatException("Missing closing double quote.");
                    }
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(output);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Formats string with comma separated values.
        /// </summary>
        /// <param name="values">List of values to format.</param>
        /// <returns>String with comma separated values.</returns>
        public static string Format(IEnumerable<object> values)
        {
            return Format(values.ToArray());
        }

        /// <summary>
        /// Formats string with comma separated values.
        /// </summary>
        /// <param name="values">List of values to format.</param>
        /// <returns>String with comma separated values.</returns>
        public static string Format(params object[] values)
        {
            StringBuilder sb = new StringBuilder(256);
            using (TextWriter writer = new StringWriter(sb))
            {
                Format(writer, values);
                writer.Flush();
                return sb.ToString();
            }
        }

        /// <summary>
        /// Formats string with comma separated values.
        /// </summary>
        /// <param name="output">Text writer to write output to.</param>
        /// <param name="values">List of values to format.</param>
        public static void Format(TextWriter output, IEnumerable<object?> values)
        {
            Format(output, Precondition.CheckNotNull(values, nameof(values)).ToArray());
        }

        /// <summary>
        /// Formats string with comma separated values.
        /// </summary>
        /// <param name="output">Text writer to write output to.</param>
        /// <param name="values">List of values to format.</param>
        public static void Format(TextWriter output, params object?[] values)
        {
            Precondition.CheckNotNull(output, nameof(output));
            Precondition.CheckNotNull(values, nameof(values));

            for (int i = 0; i < values.Length; ++i)
            {
                FormatValue(output, values[i]);
                if (values.Length - i > 1)
                {
                    output.Write(CommaChar);
                }
            }
        }

        /// <summary>
        /// Formats single value enclosing it in quotes and escaping special characters, if necessary.
        /// </summary>
        /// <param name="output">Text writer to write output to.</param>
        /// <param name="value">Value to format.</param>
        private static void FormatValue(TextWriter output, object? value)
        {
            if (value == null)
            {
                return;
            }

            Type type = value.GetType();
            string stringValue = type == typeof(string) ? (string)value : value.ToString()!;

            ValueFormatType formatType = GetValueFormatType(stringValue);
            if (formatType == ValueFormatType.Default)
            {
                // Write value as is
                output.Write(stringValue);
            }
            else if (formatType == ValueFormatType.Quoted)
            {
                // String contains comma and/or new line characters
                // and must be enclosed in quotation marks.
                output.Write(QuoteChar);
                output.Write(stringValue);
                output.Write(QuoteChar);
            }
            else
            {
                // String contains quotation marks that should be escaped.
                // Enclosing quotation marks are required as well.
                output.Write(QuoteChar);
                foreach (char ch in stringValue)
                {
                    output.Write(ch);
                    if (ch == QuoteChar)
                    {
                        output.Write(ch);
                    }
                }

                output.Write(QuoteChar);
            }
        }

        /// <summary>
        /// Scans string value to determine how it should be formatted.
        /// </summary>
        /// <param name="str">String to analyze.</param>
        /// <returns>Required value format.</returns>
        private static unsafe ValueFormatType GetValueFormatType(string str)
        {
            ValueFormatType flags = ValueFormatType.Default;

            fixed (char* pStr = str)
            {
                char* pChar = pStr;
                char* pEnd = pStr + str.Length;
                while (pChar < pEnd)
                {
                    if (*pChar == CommaChar || *pChar == '\n')
                    {
                        flags = ValueFormatType.Quoted;
                    }
                    else if (*pChar == QuoteChar)
                    {
                        return ValueFormatType.Escaped;
                    }

                    ++pChar;
                }
            }

            return flags;
        }

        private enum ValueFormatType
        {
            Default = 0,
            Quoted = 1,
            Escaped = 2
        }
    }
}

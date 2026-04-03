namespace ConsoleWeaver
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Text format helper.
    /// </summary>
    internal static class TextFormat
    {
        /// <summary>
        /// Pads and aligns string.
        /// </summary>
        /// <param name="text">String value to pad and align.</param>
        /// <param name="alignment">Text alignment.</param>
        /// <param name="width">Total width to align to.</param>
        /// <returns>Aligned text.</returns>
        public static string AlignText(string text, Alignment alignment, int width)
        {
            if (text.Length == width)
            {
                return text;
            }

            if (alignment == Alignment.Left)
            {
                return text.PadRight(width);
            }

            if (alignment == Alignment.Right)
            {
                return text.PadLeft(width);
            }

            text = text.PadLeft(text.Length + (width - text.Length) / 2);
            return text.PadRight(width);
        }

        /// <summary>
        /// Writes provided text using specified width and alignment to string builder.
        /// </summary>
        /// <param name="builder">String builder to write output to.</param>
        /// <param name="text">String to align and write to output.</param>
        /// <param name="alignment">Text alignment.</param>
        /// <param name="width">Total text width (padding is added if necessary).</param>
        /// <param name="padding">>Spaces between content and border.</param>
        public static void WriteAlignedText(StringBuilder builder, string text, Alignment alignment, int width, int padding = 0)
        {
            builder.AppendLine(AlignText(PadText(text, padding), alignment, width));
        }

        /// <summary>
        /// Writes provided text using specified width and alignment to string builder.
        /// </summary>
        /// <param name="builder">String builder to write output to.</param>
        /// <param name="text">One or more lines of text.</param>
        /// <param name="alignment">Text alignment.</param>
        /// <param name="width">Total text width (padding is added if necessary).</param>
        /// <param name="padding">>Spaces between content and border.</param>
        public static void WriteAlignedText(StringBuilder builder, IEnumerable<string> text, Alignment alignment, int width, int padding = 0)
        {
            foreach (string line in text)
            {
                builder.AppendLine(AlignText(PadText(line, padding), alignment, width));
            }
        }

        /// <summary>
        /// Writes provided text as a text box to console.
        /// </summary>
        /// <param name="builder">String builder to write output to.</param>
        /// <param name="text">One or more lines of text.</param>
        /// <param name="alignment">Text alignment within box.</param>
        /// <param name="width">Text box width.</param>
        /// <param name="padding">Spaces between content and border.</param>
        public static void WriteTextBox(StringBuilder builder, IEnumerable<string> text, Alignment alignment, int width, int padding = 0)
        {
            builder.Append('+');
            builder.Append("-".PadRight(width - 2, '-'));
            builder.Append('+');
            builder.AppendLine();

            foreach (string line in text)
            {
                builder.Append('|');
                builder.Append(AlignText(PadText(line, padding), alignment, width - 2));
                builder.Append('|');
                builder.AppendLine();
            }

            builder.Append('+');
            builder.Append("-".PadRight(width - 2, '-'));
            builder.Append('+');
        }

        /// <summary>
        /// Adds whitespaces at the beginning and the end of the line.
        /// </summary>
        /// <param name="text">Text to pad.</param>
        /// <param name="padding">Number of whitespaces to add.</param>
        /// <returns>Padded text.</returns>
        private static string PadText(string text, int padding)
        {
            if (padding < 1)
            {
                return text;
            }

            return " ".PadRight(padding) + text + " ".PadRight(padding);
        }
    }
}

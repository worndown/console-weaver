namespace ConsoleWeaver.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Outputs data to console as formatted ASCII table.
    /// </summary>
    public sealed class Table
    {
        /// <summary>
        /// Column definitions.
        /// </summary>
        private readonly List<TableColumnDefinition> columns;

        /// <summary>
        /// Data rows.
        /// </summary>
        private readonly List<object[]?> rows = new List<object[]?>(64);

        /// <summary>
        /// Padding width.
        /// </summary>
        private int padding = 1;

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        public Table()
            : this(new List<TableColumnDefinition>(8))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        /// <param name="title">Table title.</param>
        public Table(string title)
            : this(title, new List<TableColumnDefinition>(8))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        /// <param name="columns">Column definitions.</param>
        public Table(IEnumerable<TableColumnDefinition> columns)
            : this(null, columns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        /// <param name="title">Table title.</param>
        /// <param name="columns">Column definitions.</param>
        public Table(string? title, IEnumerable<TableColumnDefinition> columns)
            : this(title, columns, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        /// <param name="title">Table title.</param>
        /// <param name="columns">Column definitions.</param>
        /// <param name="foreground">Console foreground color.</param>
        /// <param name="background">Console background color.</param>
        public Table(
            string? title,
            IEnumerable<TableColumnDefinition> columns,
            ConsoleColor? foreground = null,
            ConsoleColor? background = null)
        {
            this.Title = title;
            this.columns = Precondition.CheckNotNull(columns, nameof(columns)).ToList();
            this.Foreground = foreground;
            this.Background = background;
            this.TitleAlignment = Alignment.Center;
            this.NumericUseGroupSeparator = false;
            this.FloatingNumberPrecision = 2;
        }

        /// <summary>
        /// Gets table's title.
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// Gets or sets title alignment.
        /// </summary>
        public Alignment TitleAlignment { get; set; }

        /// <summary>
        /// Gets list of footnotes to be added at the bottom of the table.
        /// </summary>
        public List<string> Footnotes { get; } = new List<string>(4);

        /// <summary>
        /// Gets or sets value indicating whether to use group separator in numeric values.
        /// Column specific format, if specified, takes precedence over this setting.
        /// </summary>
        public bool NumericUseGroupSeparator { get; set; }

        /// <summary>
        /// Gets or sets value indicating precision to use in numeric values.
        /// Column specific format, if specified, takes precedence over this setting.
        /// </summary>
        public int FloatingNumberPrecision { get; set; }

        /// <summary>
        /// Gets or sets value indicating value padding.
        /// </summary>
        public int PaddingWidth
        {
            get => this.padding;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Invalid padding value.");
                }

                this.padding = value;
            }
        }

        /// <summary>
        /// Gets or sets console foreground color.
        /// </summary>
        public ConsoleColor? Foreground { get; set; }

        /// <summary>
        /// Gets or sets console background color.
        /// </summary>
        public ConsoleColor? Background { get; set; }

        /// <summary>
        /// Adds new column definition to the table.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <returns>Index of newly added column.</returns>
        public int AddColumnDefinition(string name)
        {
            return this.AddColumnDefinition(new TableColumnDefinition(name));
        }

        /// <summary>
        /// Adds new column definition to the table.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="maxWidth">Max column width. Longer values will be truncated.</param>
        /// <returns>Index of newly added column.</returns>
        public int AddColumnDefinition(string name, int maxWidth)
        {
            return this.AddColumnDefinition(new TableColumnDefinition(name, maxWidth));
        }

        /// <summary>
        /// Adds new column definition to the table.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="format">Column format or null.</param>
        /// <returns>Index of newly added column.</returns>
        public int AddColumnDefinition(string name, string format)
        {
            return this.AddColumnDefinition(new TableColumnDefinition(name, format));
        }

        /// <summary>
        /// Adds new column definition to the table.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="format">Column format or null.</param>
        /// <param name="maxWidth">Max column width. Longer values will be truncated.</param>
        /// <returns>Index of newly added column.</returns>
        public int AddColumnDefinition(string name, string format, int maxWidth)
        {
            return this.AddColumnDefinition(new TableColumnDefinition(name, format, maxWidth));
        }

        /// <summary>
        /// Adds new column definition to the table.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="alignment">Column alignment.</param>
        /// <returns>Index of newly added column.</returns>
        public int AddColumnDefinition(string name, Alignment alignment)
        {
            return this.AddColumnDefinition(new TableColumnDefinition(name, alignment));
        }

        /// <summary>
        /// Adds new column definition to the table.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="format">Column format or null.</param>
        /// <param name="alignment">Column alignment.</param>
        /// <returns>Index of newly added column.</returns>
        public int AddColumnDefinition(string name, string format, Alignment alignment)
        {
            return this.AddColumnDefinition(new TableColumnDefinition(name, format, alignment));
        }

        /// <summary>
        /// Adds new column definition to the table.
        /// </summary>
        /// <param name="columnDefinition">Column definition.</param>
        /// <returns>Index of newly added column.</returns>
        public int AddColumnDefinition(TableColumnDefinition columnDefinition)
        {
            int index = this.columns.Count;
            this.columns.Add(Precondition.CheckNotNull(columnDefinition, nameof(columnDefinition)));
            return index;
        }

        /// <summary>
        /// Adds rows to the table.
        /// </summary>
        /// <param name="rowsToAdd">Rows to add.</param>
        public void AddRows(IEnumerable<IEnumerable<object>> rowsToAdd)
        {
            foreach (IEnumerable<object> row in Precondition.CheckNotNull(rowsToAdd, nameof(rowsToAdd)))
            {
                this.AddRow(row);
            }
        }

        /// <summary>
        /// Adds row to the table.
        /// </summary>
        /// <param name="row">Table row.</param>
        public void AddRow(IEnumerable<object> row)
        {
            this.AddRow(Precondition.CheckNotNull(row, nameof(row)).ToArray());
        }

        /// <summary>
        /// Adds row to the table.
        /// </summary>
        /// <param name="row">Table row.</param>
        public void AddRow(object[] row)
        {
            Precondition.CheckNotNull(row, nameof(row));
            if (this.columns.Count != row.Length)
            {
                throw new ArgumentException($"Row contains invalid number of columns ({row.Length}). Table has {columns.Count} columns.");
            }

            this.UpdateMaxColumnValueLength(row);
            this.rows.Add(row);
        }

        /// <summary>
        /// Adds row separator.
        /// </summary>
        public void AddRowSeparator()
        {
            // When printing table we treat null as horizontal line
            this.rows.Add(null);
        }

        /// <summary>
        /// Prints content of the table to console.
        /// </summary>
        public void ConsoleWrite()
        {
            StringBuilder sb = this.FormatTable();
            Shell.Write(sb.ToString(), this.Foreground, this.Background);
        }

        /// <summary>
        /// Exports content of the table as CSV into specified file.
        /// </summary>
        /// <param name="filename">Output file name.</param>
        /// <param name="append">Append, if file already exists.</param>
        public void ExportCsv(string filename, bool append)
        {
            if (File.Exists(filename) && append)
            {
                using (FileStream stream = File.OpenWrite(filename))
                {
                    stream.Seek(0, SeekOrigin.End);
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        this.ExportCsv(writer, false);
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(File.Create(filename)))
                {
                    this.ExportCsv(writer, true);
                }
            }
        }

        /// <summary>
        /// Exports content of the table as CSV into a stream.
        /// </summary>
        /// <param name="writer">Text writer to write CSV to.</param>
        /// <param name="writeHeaders">Whether to write headers.</param>
        private void ExportCsv(TextWriter writer, bool writeHeaders)
        {
            if (writeHeaders)
            {
                string headers = CommaSeparatedValues.Format(this.columns.Select(x => x.Name));
                writer.Write(headers);
                writer.WriteLine();
            }


            foreach (object[]? row in rows)
            {
                // Skip separator rows
                if (row is null)
                {
                    continue;
                }

                string csv = CommaSeparatedValues.Format(row);
                writer.Write(csv);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Formats ASCII table.
        /// </summary>
        /// <returns>String builder with formatter table.</returns>
        internal StringBuilder FormatTable()
        {
            StringBuilder builder = new StringBuilder(64 * this.rows.Count);

            int tableWidth = this.columns.Sum(x => Math.Min(x.MaxValueLength, x.MaxWidth) + this.PaddingWidth * 2) + this.columns.Count + 1;

            this.WriteHeader(builder, tableWidth);
            foreach (object[]? row in this.rows)
            {
                this.WriteRow(builder, row);
            }

            this.WriteSeparatorRow(builder);

            if (this.Footnotes.Count > 0)
            {
                foreach (string footnote in this.Footnotes)
                {
                    if (!string.IsNullOrEmpty(footnote))
                    {
                        builder.AppendLine(footnote.PadRight(tableWidth));
                    }
                }
            }

            return builder;
        }

        /// <summary>
        /// Writes table header.
        /// </summary>
        /// <param name="sb">Output string builder.</param>
        /// <param name="tableWidth">Table width.</param>
        private void WriteHeader(StringBuilder sb, int tableWidth)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                // Top line
                sb.Append('+');
                sb.Append("-".PadRight(tableWidth - 2, '-'));
                sb.Append('+');
                sb.AppendLine();

                // Title line
                sb.Append('|');
                WriteAligned(sb, Title, TitleAlignment, tableWidth - 2 - PaddingWidth * 2);
                sb.Append('|');
                sb.AppendLine();
            }

            WriteSeparatorRow(sb);

            // Header names
            sb.Append('|');
            for (int i = 0; i < columns.Count; ++i)
            {
                WriteAligned(
                    sb,
                    columns[i].Name,
                    columns[i].Alignment,
                    Math.Min(columns[i].MaxValueLength, columns[i].MaxWidth));

                sb.Append('|');
            }

            sb.AppendLine();
            WriteSeparatorRow(sb);
        }

        /// <summary>
        /// Writes single row.
        /// </summary>
        /// <param name="sb">Output string builder.</param>
        /// <param name="row">Row to write.</param>
        private void WriteRow(StringBuilder sb, object[]? row)
        {
            if (row is null)
            {
                this.WriteSeparatorRow(sb);
                return;
            }

            sb.Append('|');
            for (int i = 0; i < row.Length; ++i)
            {
                this.WriteColumnValue(sb, columns[i], row[i]);
                sb.Append('|');
            }

            sb.AppendLine();
        }

        /// <summary>
        /// Writes separator row.
        /// </summary>
        /// <param name="sb">Output string builder.</param>
        private void WriteSeparatorRow(StringBuilder sb)
        {
            sb.Append('+');
            for (int i = 0; i < columns.Count; ++i)
            {
                sb.Append("-".PadRight(Math.Min(columns[i].MaxValueLength, columns[i].MaxWidth) + PaddingWidth * 2, '-'));
                sb.Append('+');
            }

            sb.AppendLine();
        }

        /// <summary>
        /// Writes aligned text to output string builder.
        /// </summary>
        /// <param name="sb">Output string builder.</param>
        /// <param name="text">Text to align and write.</param>
        /// <param name="alignment">Text alignment.</param>
        /// <param name="width">Total width to align to.</param>
        private void WriteAligned(StringBuilder sb, string text, Alignment alignment, int width)
        {
            if (PaddingWidth > 0)
            {
                sb.Append(" ".PadRight(PaddingWidth));
            }

            sb.Append(TextFormat.AlignText(text, alignment, width));

            if (PaddingWidth > 0)
            {
                sb.Append(" ".PadRight(PaddingWidth));
            }
        }

        /// <summary>
        /// Pre-processes row to update max column value length for each column.
        /// </summary>
        /// <param name="row">Row to process.</param>
        private void UpdateMaxColumnValueLength(object[] row)
        {
            for (int i = 0; i < row.Length; ++i)
            {
                string value = this.FormatColumnValue(columns[i], row[i]);
                columns[i].UpdateMaxValueLength(value.Length);
            }
        }

        /// <summary>
        /// Writes column value.
        /// </summary>
        /// <param name="sb">Output string builder.</param>
        /// <param name="definition">Column definition.</param>
        /// <param name="value">Column value.</param>
        private void WriteColumnValue(StringBuilder sb, TableColumnDefinition definition, object value)
        {
            string formatted = this.FormatColumnValue(definition, value);
            WriteAligned(sb, formatted, definition.Alignment, definition.MaxValueLength);
        }

        /// <summary>
        /// Formats column value.
        /// </summary>
        /// <param name="definition">Column definition.</param>
        /// <param name="value">Column value.</param>
        /// <returns>Formatted value.</returns>
        private string FormatColumnValue(TableColumnDefinition definition, object? value)
        {
            if (value is null)
            {
                return definition.MaxWidth >= 6 ? "<null>" : "-";
            }

            string result;

            if (value is string str)
            {
                result = str;
            }
            else if (definition.Format is not null)
            {
                result = string.Format(CultureInfo.InvariantCulture, $"{{0:{definition.Format}}}", value);
            }
            else
            {
                if ((value is int || value is short || value is long) && NumericUseGroupSeparator)
                {
                    result = $"{value:N0}";
                }
                else if (value is float || value is double || value is decimal)
                {
                    string format = NumericUseGroupSeparator
                        ? $"{{0:N{FloatingNumberPrecision}}}"
                        : $"{{0:F{FloatingNumberPrecision}}}";

                    result = string.Format(format, value);
                }
                else
                {
                    result = value.ToString() ?? string.Empty;
                }
            }

            return result.Truncate(definition.MaxWidth);
        }
    }
}

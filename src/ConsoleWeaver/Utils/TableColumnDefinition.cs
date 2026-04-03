namespace ConsoleWeaver.Utils
{
    using System;

    /// <summary>
    /// Table column definition.
    /// </summary>
    public sealed class TableColumnDefinition
    {
        /// <summary>
        /// Default column alignment.
        /// </summary>
        internal const Alignment DefaultAlignment = Alignment.Left;

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        public TableColumnDefinition(string name)
            : this(name, null, DefaultAlignment, int.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="format">Column format.</param>
        public TableColumnDefinition(string name, string format)
            : this(name, format, DefaultAlignment, int.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="maxWidth">Max column width.</param>
        public TableColumnDefinition(string name, int maxWidth)
            : this(name, null, DefaultAlignment, maxWidth)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="alignment">Column alignment.</param>
        public TableColumnDefinition(string name, Alignment alignment)
            : this(name, null, alignment, int.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="format">Column format.</param>
        /// <param name="maxWidth">Max column width.</param>
        public TableColumnDefinition(string name, string format, int maxWidth)
            : this(name, format, DefaultAlignment, maxWidth)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="format">Column format.</param>
        /// <param name="alignment">Column alignment.</param>
        public TableColumnDefinition(string name, string format, Alignment alignment)
            : this(name, format, alignment, int.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="alignment">Column alignment.</param>
        /// <param name="maxWidth">Max column width. Values longer than this will be truncated.</param>
        public TableColumnDefinition(string name, Alignment alignment, int maxWidth)
            : this(name, null, alignment, maxWidth)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableColumnDefinition class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="format">Format string or null.</param>
        /// <param name="alignment">Column alignment.</param>
        /// <param name="maxWidth">Max column width. Values longer than this will be truncated.</param>
        public TableColumnDefinition(string name, string? format, Alignment alignment, int maxWidth)
        {
            Name = Precondition.CheckNotNullOrEmpty(name, nameof(name));
            Precondition.CheckArgument(maxWidth >= name.Length, "Max column width cannot be smaller than column name length.", nameof(maxWidth));
            Format = format;
            Alignment = alignment;
            MaxWidth = maxWidth;
            MaxValueLength = Name.Length;
        }

        /// <summary>
        /// Gets column name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets column format string.
        /// </summary>
        public string? Format { get; }

        /// <summary>
        /// Gets column alignment.
        /// </summary>
        public Alignment Alignment { get; }

        /// <summary>
        /// Gets max column width. If set, values are truncated.
        /// </summary>
        public int MaxWidth { get; }

        /// <summary>
        /// Gets maximum value length encountered in the data set.
        /// </summary>
        internal int MaxValueLength { get; private set; }

        /// <summary>
        /// Updates max value length.
        /// </summary>
        /// <param name="length">Value length.</param>
        internal void UpdateMaxValueLength(int length)
        {
            MaxValueLength = Math.Max(MaxValueLength, length);
        }
    }
}

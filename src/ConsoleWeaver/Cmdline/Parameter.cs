namespace ConsoleWeaver.Cmdline
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Base parameter class
    /// </summary>
    internal abstract class Parameter : IComparable<Parameter>, IEquatable<Parameter>
    {
        /// <summary>
        /// Default parameter set name.
        /// </summary>
        public const string DefaultParameterSetName = "_DefaultParameterSet";

        /// <summary>
        /// Whitespace character.
        /// </summary>
        private const char Whitespace = ' ';

        /// <summary>
        /// Parameter sets this parameter is part of.
        /// </summary>
        private List<string> parameterSets = [DefaultParameterSetName];

        /// <summary>
        /// Initializes a new instance of the Parameter class
        /// </summary>
        /// <param name="shortName">Short parameter name</param>
        /// <param name="longName">Long parameter name</param>
        /// <param name="metaValue">Parameter meta-value</param>
        /// <param name="desc">Parameter description</param>
        /// <param name="isMandatory">Whether parameter is mandatory</param>
        /// <param name="isMultivalued">Whether parameter is multivalued</param>
        /// <param name="isSwitch">Whether it is a switch parameter</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="position">Parameter position</param>
        protected Parameter(
            string? shortName,
            string? longName,
            string? metaValue,
            string desc,
            bool isMandatory,
            bool isMultivalued,
            bool isSwitch,
            string? defaultValue,
            int position)
        {
            if (string.IsNullOrEmpty(shortName) && string.IsNullOrEmpty(longName))
            {
                throw new ParameterDefinitionException("Short or long parameter name is required.");
            }

            if ((!string.IsNullOrEmpty(shortName) && shortName.Contains(Whitespace, StringComparison.Ordinal)) ||
                (!string.IsNullOrEmpty(longName) && longName.Contains(Whitespace, StringComparison.Ordinal)))
            {
                throw new ParameterDefinitionException(GetName(longName, shortName), "Parameter name cannot contain spaces.");
            }

            if (!string.IsNullOrEmpty(shortName) &&
                !string.IsNullOrEmpty(longName) &&
                string.Compare(shortName, longName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                throw new ParameterDefinitionException(longName, "Short and long parameter names cannot be same.");
            }

            if (string.IsNullOrEmpty(desc))
            {
                throw new ParameterDefinitionException(GetName(longName, shortName), "Parameter description is required.");
            }

            if (!isSwitch && string.IsNullOrEmpty(metaValue))
            {
                throw new ParameterDefinitionException(GetName(longName, shortName), "Parameter meta-value is required.");
            }

            if (isMandatory && position > -1 && string.IsNullOrEmpty(longName))
            {
                throw new ParameterDefinitionException("Long name is required for positional parameters.");
            }

            this.ShortName = shortName;
            this.LongName = longName;
            this.Description = desc;
            this.MetaValue = metaValue;
            this.IsMandatory = isMandatory;
            this.IsMultivalued = isMultivalued;
            this.IsSwitch = isSwitch;
            this.DefaultValue = defaultValue;
            this.Position = position;
            return;

            static string GetName(string? longName, string? shortName)
            {
                return !string.IsNullOrEmpty(longName) ? longName : shortName!;
            }
        }

        /// <summary>
        /// Short parameter name
        /// </summary>
        public string? ShortName { get; }

        /// <summary>
        /// Long parameter name
        /// </summary>
        public string? LongName { get; }

        /// <summary>
        /// Long or, if not present, short argument name
        /// </summary>
        public string Name => !string.IsNullOrEmpty(this.LongName) ? this.LongName : this.ShortName!;

        /// <summary>
        /// Meta-value for help generation
        /// </summary>
        public string? MetaValue { get; }

        /// <summary>
        /// Parameter description for help generation
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Raw parameter value as it was specified on the command line
        /// </summary>
        public string? RawValue { get; private set; }

        /// <summary>
        /// Default value, if any
        /// </summary>
        public string? DefaultValue { get; }

        /// <summary>
        /// Whether parameter is mandatory
        /// </summary>
        public bool IsMandatory { get; private set; }

        /// <summary>
        /// Whether it is a switch parameter
        /// </summary>
        public bool IsSwitch { get; private set; }

        /// <summary>
        /// Whether parameter is multivalued
        /// </summary>
        public bool IsMultivalued { get; private set; }

        /// <summary>
        /// Parameter position
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Whether parameter is positional
        /// </summary>
        public bool IsPositional => this.Position > -1;

        /// <summary>
        /// Whether short name is set
        /// </summary>
        public bool HasShortName => !string.IsNullOrEmpty(this.ShortName);

        /// <summary>
        /// Whether long name is set
        /// </summary>
        public bool HasLongName => !string.IsNullOrEmpty(this.LongName);

        /// <summary>
        /// Whether metavalue is set
        /// </summary>
        public bool HasMetaValue => !string.IsNullOrEmpty(this.MetaValue);

        /// <summary>
        /// Indicates whether parameter has value
        /// </summary>
        public bool HasValue => this.RawValue is not null;

        /// <summary>
        /// Whether parameter has default value
        /// </summary>
        public bool HasDefaultValue => this.DefaultValue is not null;

        /// <summary>
        /// Parameter sets this parameter belongs to.
        /// </summary>
        public IReadOnlyList<string> ParameterSets => this.parameterSets.AsReadOnly();

        /// <summary>
        /// Determines if this parameter is part of the specified parameter set.
        /// </summary>
        /// <param name="parameterSetName">Parameter set name.</param>
        /// <returns>true if this parameter belongs to the specified set; false otherwise.</returns>
        public bool IsPartOfParameterSet(string parameterSetName)
        {
            if (this.parameterSets.Count == 1 && this.parameterSets[0] == DefaultParameterSetName)
            {
                return true;
            }

            return this.parameterSets.Contains(parameterSetName);
        }

        /// <summary>
        /// Compares this parameter to the other parameter.
        /// Parameters considered equal if both contain same name (short or long).
        /// </summary>
        /// <param name="other">The other parameter.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(Parameter? other)
        {
            if (other is null)
            {
                return 1;
            }

            int longNameComparisonResult = int.MaxValue;
            int shortNameComparisonResult = int.MaxValue;
            int shortToLongComparisonResult = int.MaxValue;
            int longToShortComparisonResult = int.MaxValue;

            if (this.HasLongName && other.HasLongName)
            {
                longNameComparisonResult = string.Compare(this.LongName, other.LongName, StringComparison.OrdinalIgnoreCase);
                if (longNameComparisonResult == 0)
                {
                    return longNameComparisonResult;
                }
            }

            if (this.HasShortName && other.HasShortName)
            {
                shortNameComparisonResult = string.Compare(this.ShortName, other.ShortName, StringComparison.OrdinalIgnoreCase);
                if (shortNameComparisonResult == 0)
                {
                    return shortNameComparisonResult;
                }
            }

            // Compare short to long names
            if (this.HasShortName && other.HasLongName)
            {
                shortToLongComparisonResult = string.Compare(this.ShortName, other.LongName, StringComparison.OrdinalIgnoreCase);
                if (shortToLongComparisonResult == 0)
                {
                    return 0;
                }
            }

            if (this.HasLongName && other.HasShortName)
            {
                longToShortComparisonResult = string.Compare(this.LongName, other.ShortName, StringComparison.OrdinalIgnoreCase);
                if (longToShortComparisonResult == 0)
                {
                    return 0;
                }
            }

            if (longNameComparisonResult != int.MaxValue)
            {
                return longNameComparisonResult;
            }

            if (shortNameComparisonResult != int.MaxValue)
            {
                return shortNameComparisonResult;
            }

            if (shortToLongComparisonResult != int.MaxValue)
            {
                return shortToLongComparisonResult;
            }

            if (longToShortComparisonResult != int.MaxValue)
            {
                return longToShortComparisonResult;
            }

            return 1;
        }

        /// <summary>
        /// Determines if this parameter is equal to other
        /// </summary>
        /// <param name="other">Other parameter</param>
        /// <returns>true if parameters are equal; false otherwise</returns>
        public bool Equals(Parameter? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.CompareTo(other) == 0;
        }

        /// <summary>
        /// Determines if this object equals to other object
        /// </summary>
        /// <param name="obj">Object to compare to this object</param>
        /// <returns>true if this object equals to other object; false otherwise</returns>
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as Parameter);
        }

        /// <summary>
        /// Returns hash code
        /// </summary>
        /// <returns>Hash code value</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns string representing this object
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Sets parameter sets this parameter belongs to.
        /// </summary>
        /// <param name="sets">Parameter set names.</param>
        internal void SetParameterSets(List<string> sets)
        {
            this.parameterSets = Precondition.CheckNotNull(sets, nameof(sets));
        }

        /// <summary>
        /// Sets parameter value
        /// </summary>
        /// <param name="value">Value to set</param>
        internal void SetValue(string value)
        {
            if (this.HasValue)
            {
                throw new ParameterException(this.Name, "Parameter already has assigned value.");
            }

            this.RawValue = string.IsNullOrEmpty(value) ? value : value.Trim(['"']);
        }

        /// <summary>
        /// Gets parameter value as specified type
        /// </summary>
        /// <param name="type">Parameter type</param>
        /// <returns>Parameter value</returns>
        internal object? GetValue(Type type)
        {
            try
            {
                return type.IsArray
                    ? this.ConvertMultiValue(this.RawValue ?? this.DefaultValue, type)
                    : this.ConvertSingleValue(this.RawValue ?? this.DefaultValue, type);
            }
            catch (ArgumentException ex)
            {
                string message = ex.Message;

                if (type.IsArray)
                {
                    type = type.GetElementType() ?? throw new ParameterException(this.Name, "Array element type is not set.");
                }

                if (type.IsEnum)
                {
                    // Include list of valid values for this enum
                    Array array = Enum.GetValues(type);
                    StringBuilder values = new StringBuilder("Invalid parameter value. Valid values are: ");
                    for (int i = 0; i < array.Length; ++i)
                    {
                        values.Append(array.GetValue(i));
                        if (array.Length - i > 1)
                        {
                            values.Append(", ");
                        }
                    }

                    values.Append('.');
                    message = values.ToString();
                }

                throw new ParameterException(this.Name, message);
            }
            catch (Exception ex)
            {
                throw new ParameterException(this.Name, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        /// <summary>
        /// Converts string value into specified type
        /// </summary>
        /// <param name="value">String containing parameter value</param>
        /// <param name="type">Target type</param>
        /// <returns>Converted parameter value</returns>
        private object? ConvertSingleValue(string? value, Type type)
        {
            if (value is null)
            {
                throw new InvalidOperationException("Value is not set.");
            }

            return this.ConvertValue(value, type);
        }

        /// <summary>
        /// Converts string into multivalued array of specified type
        /// </summary>
        /// <param name="value">Value string</param>
        /// <param name="type">Array type</param>
        /// <returns>Array containing parameter values</returns>
        private object? ConvertMultiValue(string? value, Type type)
        {
            if (value is null)
            {
                throw new InvalidOperationException("Multi-value is not set.");
            }

            Type elementType = type.GetElementType() ?? throw new ParameterException(this.Name, "Array element type is not set.");

            string[] values = value.Split([','], StringSplitOptions.RemoveEmptyEntries);

            // Specifying array length invokes right constructor
            object? result = Activator.CreateInstance(type, new object[] { values.Length });
            if (result is null)
            {
                throw new ParameterException(this.Name, $"Couldn't create instance of {type.Name}.");
            }

            for (int i = 0; i < values.Length; ++i)
            {
                ((Array)result).SetValue(this.ConvertValue(values[i].Trim(), elementType), i);
            }

            return result;
        }

        /// <summary>
        /// Converts string into specified type
        /// </summary>
        /// <param name="value">String containing parameter value</param>
        /// <param name="type">Target type</param>
        /// <returns>Converted parameter value</returns>
        private object ConvertValue(string value, Type type)
        {
            return ValueConverterChain.Default.Convert(value, type);
        }
    }
}

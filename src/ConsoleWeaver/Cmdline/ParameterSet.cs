namespace ConsoleWeaver.Cmdline
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Parameter set
    /// </summary>
    internal sealed class ParameterSet : IList<Parameter>
    {
        /// <summary>
        /// List of all parameters.
        /// </summary>
        private readonly List<Parameter> parameters = new List<Parameter>();

        /// <summary>
        /// List of positional parameters.
        /// </summary>
        private readonly List<Parameter> positional = new List<Parameter>();

        /// <summary>
        /// Initializes a new instance of the ParameterSet class.
        /// </summary>
        /// <param name="name">Parameter set name.</param>
        public ParameterSet(string name)
        {
            this.Name = name;
            this.IsActive = true;
        }

        /// <summary>
        /// Parameter set name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether parameter set is active.
        /// </summary>
        public bool IsActive { get; internal set; }

        /// <summary>
        /// Number of mandatory parameters
        /// </summary>
        public int PositionalParameterCount => this.positional.Count;

        /// <summary>
        /// Number of switch parameters
        /// </summary>
        public int SwitchParameterCount => this.parameters.Count(p => p.IsSwitch);

        /// <summary>
        /// Whether parameter set has next unclaimed positional parameter
        /// </summary>
        public bool HasNextPositionalParameter => this.positional.Any(p => !p.HasValue);

        /// <summary>
        /// Performs parameter definition validation before values are bound.
        /// </summary>
        public void PreBindValidate()
        {
            // Check positional parameter indexes
            this.positional.Sort((a, b) => a.Position.CompareTo(b.Position));
            for (int i = 0; i < this.positional.Count; ++i)
            {
                if (this.positional[i].Position != i)
                {
                    throw new ParameterException(
                        this.positional[i].Name,
                        "Invalid parameter position {0} (should be {1}).",
                        this.positional[i].Position,
                        i);
                }
            }
        }

        /// <summary>
        /// Performs validation after parameter bindings.
        /// </summary>
        public void PostBindValidate()
        {
            // Check missing mandatory parameters
            string[] missing = this.parameters.Where(p => p.IsMandatory && !p.HasValue).Select(p => p.Name).ToArray();
            if (missing.Length > 0)
            {
                throw new ParameterException(
                    null,
                    "Missing mandatory parameter{0}: {1}.",
                    missing.Length > 1 ? "s" : string.Empty,
                    string.Join(", ", missing));
            }
        }

        /// <summary>
        /// Return next available positional parameter definition
        /// </summary>
        /// <returns>Parameter definition or null if not available</returns>
        public Parameter GetNextPositionalParameter()
        {
            return this.positional.Find(p => !p.HasValue)!;
        }

        /// <summary>
        /// Returns usage string for this parameter set.
        /// </summary>
        /// <param name="options">Program options.</param>
        /// <returns>Usage string.</returns>
        public string GetSyntax(IOptions options)
        {
            StringBuilder sb = new StringBuilder(512);

            Parameter[] mandatoryParameters = this.parameters.Where(p => p.IsMandatory && !p.IsPositional).ToArray();
            Parameter[] optionalParameters = this.parameters.Where(p => !p.IsMandatory && !p.IsSwitch).ToArray();
            Parameter[] switchParameters = this.parameters.Where(p => p.IsSwitch).ToArray();

            foreach (Parameter parameter in this.positional)
            {
                sb.Append(CultureInfo.InvariantCulture, $" <{parameter.Name}>");
            }

            foreach (Parameter parameter in mandatoryParameters)
            {
                if ((options.ParameterSyntax == ParameterSyntax.Mixed || options.ParameterSyntax == ParameterSyntax.LongOnly) && parameter.HasLongName)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" --{parameter.LongName}={parameter.MetaValue}");
                }
                else if ((options.ParameterSyntax == ParameterSyntax.Mixed || options.ParameterSyntax == ParameterSyntax.ShortOnly) && parameter.HasShortName)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" -{parameter.ShortName} {parameter.MetaValue}");
                }
                else
                {
                    throw new ParameterException($"Parameter '{parameter.Name}' missing name compatible with '{options.ParameterSyntax}' syntax.");
                }
            }

            foreach (Parameter parameter in optionalParameters)
            {
                if ((options.ParameterSyntax == ParameterSyntax.Mixed || options.ParameterSyntax == ParameterSyntax.LongOnly) && parameter.HasLongName)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" [--{parameter.LongName}={parameter.MetaValue}]");
                }
                else if ((options.ParameterSyntax == ParameterSyntax.Mixed || options.ParameterSyntax == ParameterSyntax.ShortOnly) && parameter.HasShortName)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" [-{parameter.ShortName} {parameter.MetaValue}]");
                }
                else
                {
                    throw new ParameterException($"Parameter '{parameter.Name}' missing name compatible with '{options.ParameterSyntax}' syntax.");
                }
            }

            foreach (Parameter parameter in switchParameters)
            {
                if ((options.ParameterSyntax == ParameterSyntax.Mixed || options.ParameterSyntax == ParameterSyntax.LongOnly) && parameter.HasLongName)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" [--{parameter.LongName}]");
                }
                else if ((options.ParameterSyntax == ParameterSyntax.Mixed || options.ParameterSyntax == ParameterSyntax.ShortOnly) && parameter.HasShortName)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" [-{parameter.ShortName}]");
                }
                else
                {
                    throw new ParameterException($"Parameter '{parameter.Name}' missing name compatible with '{options.ParameterSyntax}' syntax.");
                }
            }

            return sb.ToString();
        }

        #region IList<Parameter> implementation

        public Parameter this[int index]
        {
            get => this.parameters[index];
            set => this.parameters[index] = value;
        }
        
        public int Count => this.parameters.Count;

        public bool IsReadOnly => false;

        public void Add(Parameter parameter)
        {
            this.parameters.Add(Precondition.CheckNotNull(parameter, nameof(parameter)));
            if (parameter.IsPositional)
            {
                if (this.positional.Exists(p => p.Position == parameter.Position))
                {
                    throw new ParameterException(
                        parameter.Name,
                        "Parameter set '{0}' already has positional parameter with index {1}.",
                        this.Name,
                        parameter.Position);
                }

                this.positional.Add(parameter);
            }
        }

        public void Clear()
        {
            this.parameters.Clear();
        }

        public bool Contains(Parameter parameter)
        {
            return this.parameters.Contains(parameter);
        }

        public void CopyTo(Parameter[] array, int arrayIndex)
        {
            this.parameters.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        public int IndexOf(Parameter parameter)
        {
            return this.parameters.IndexOf(parameter);
        }

        public void Insert(int index, Parameter parameter)
        {
            this.parameters.Insert(index, parameter);
        }

        public bool Remove(Parameter parameter)
        {
            return this.parameters.Remove(parameter);
        }

        public void RemoveAt(int index)
        {
            this.parameters.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        #endregion
    }
}

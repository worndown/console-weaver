namespace ConsoleWeaver.Cmdline
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Command line argument parser
    /// </summary>
    internal sealed class CmdlineParser : IEnumerable<Parameter>
    {
        /// <summary>
        /// Matches parameter alike strings starting with '-' or '/'.
        /// </summary>
        private static readonly Regex ParameterLikeRegex = new Regex(@"^(-|/).*", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Matches long parameters in "--param=value" or "--param:value" formats.
        /// </summary>
        private static readonly Regex LongParameterRegex = new Regex(@"^--(?<parameter_name>\w+)((:|=)(?<parameter_value>.*))?$", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches short parameters in "-p" or "/p" formats.
        /// </summary>
        private static readonly Regex ShortParameterRegex = new Regex(@"^(-|/)(?<parameter_name>\w+)$", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// List of all parameters.
        /// </summary>
        private readonly List<Parameter> parameters = new List<Parameter>();

        /// <summary>
        /// Parameter sets.
        /// </summary>
        private readonly Dictionary<string, ParameterSet> sets = new Dictionary<string, ParameterSet>();

        /// <summary>
        /// Current parameter set.
        /// </summary>
        private string? currentParameterSet;

        /// <summary>
        /// Indicates whether parameter set was selected and cannot ge changed anymore.
        /// </summary>
        private bool parameterSetSelected;

        /// <summary>
        /// Max short parameter name length.
        /// </summary>
        private int maxShortNameLength;

        /// <summary>
        /// Max long parameter name length.
        /// </summary>
        private int maxLongNameLength;

        /// <summary>
        /// Initializes a new instance of the CmdlineArgumentParser class.
        /// </summary>
        /// <param name="parameters">List of argument definitions.</param>
        public CmdlineParser(IReadOnlyList<Parameter> parameters)
        {
            this.CreateParameterSets(parameters);

            foreach (Parameter parameter in parameters)
            {
                this.AddDefinition(parameter);
                this.UpdateParameterSets(parameter);
            }

            this.PreBindValidate();
        }

        /// <summary>
        /// Name of parameter set we ended up with.
        /// </summary>
        public string ParameterSetName => this.currentParameterSet ?? throw new ParameterSetResolutionException();

        /// <summary>
        /// Number of mandatory and optional parameters.
        /// </summary>
        public int ParameterCount => this.parameters.Count(p => !p.IsSwitch);
        
        /// <summary>
        /// Number of mandatory parameters.
        /// </summary>
        public int MandatoryParameterCount => this.parameters.Count(p => p.IsMandatory);

        /// <summary>
        /// Number of switch parameters.
        /// </summary>
        public int SwitchParameterCount => this.parameters.Count(p => p.IsSwitch);

        /// <summary>
        /// Indicates whether we have parameters with short names.
        /// </summary>
        public bool HasParametersWithShortName => this.maxShortNameLength > 0;

        /// <summary>
        /// Indicates whether we have parameters with long names.
        /// </summary>
        public bool HasParametersWithLongName => this.maxLongNameLength > 0;

        /// <summary>
        /// Subscript operator.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <returns>Parameter with the specified name.</returns>
        public Parameter this[string parameterName] => this.GetParameterByName(parameterName);

        /// <summary>
        /// Subscript operator.
        /// </summary>
        /// <param name="parameter">Parameter definition.</param>
        /// <returns>Parameter with the specified name.</returns>
        public Parameter this[Parameter parameter] => this.GetParameterByName(parameter.LongName ?? parameter.ShortName!);

        /// <summary>
        /// Adds cmdline argument definition.
        /// </summary>
        /// <param name="parameter">Argument definition.</param>
        public void AddDefinition(Parameter parameter)
        {
            foreach (Parameter existing in this.parameters)
            {
                if (existing.Equals(parameter))
                {
                    throw new ParameterDefinitionException(
                        GetIdenticalParameterName(existing, parameter),
                        "Duplicate parameter name.");
                }
            }
                        
            this.parameters.Add(parameter);

            if (parameter.HasShortName)
            {
                this.maxShortNameLength = Math.Max(
                    this.maxShortNameLength,
                    parameter.ShortName!.Length + 3 /* dash, comma and space */);
            }

            if (parameter.HasLongName)
            {
                this.maxLongNameLength = Math.Max(
                    this.maxLongNameLength,
                    parameter.LongName!.Length + 4 + (parameter.HasMetaValue ? parameter.MetaValue!.Length + 1 : 0));
            }
        }

        /// <summary>
        /// Detects incorrect usage or explicit request for help.
        /// </summary>
        /// <param name="cmdlineArguments">Cmdline arguments.</param>
        /// <returns>true if usage should be shown to user; false otherwise.</returns>
        public bool ShowUsage(string[] cmdlineArguments)
        {
            Precondition.CheckNotNull(cmdlineArguments, nameof(cmdlineArguments));

            if (cmdlineArguments.Length == 0 && this.MandatoryParameterCount > 0)
            {
                return true;
            }

            return cmdlineArguments.Length == 1 &&
                   Regex.IsMatch(
                       cmdlineArguments[0],
                       @"^((-{1,2})|(/))?(\?|h|help)$",
                       RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// Parses command line arguments and populates parameters with values.
        /// </summary>
        /// <param name="cmdlineArguments">Command line arguments.</param>
        public void Parse(string[] cmdlineArguments)
        {
            Precondition.CheckNotNull(cmdlineArguments, nameof(cmdlineArguments));
            Parameter? pendingParameter = null;

            foreach (string argument in cmdlineArguments)
            {
                if (pendingParameter != null)
                {
                    this.BindParameter(pendingParameter, argument);
                    pendingParameter = null;
                    continue;
                }

                if (this.TryBindLongParameter(argument))
                {
                    continue;
                }

                pendingParameter = this.TryBindShortParameter(argument);
                if (pendingParameter != null || ShortParameterRegex.IsMatch(argument))
                {
                    continue;
                }

                if (this.TryBindPositionalParameter(argument))
                {
                    continue;
                }

                throw new ParameterException(null, "Unknown command line argument: {0}.", argument);
            }

            this.PostBindValidate();
        }

        /// <summary>
        /// Binds a value to a parameter and updates parameter set selection.
        /// </summary>
        /// <param name="parameter">The parameter to bind.</param>
        /// <param name="value">The value to bind.</param>
        private void BindParameter(Parameter parameter, string value)
        {
            parameter.SetValue(value);
            this.SelectParameterSet(parameter);
        }

        /// <summary>
        /// Tries to parse and bind a long parameter (--param=value or --param:value format).
        /// </summary>
        /// <param name="argument">The command line argument.</param>
        /// <returns>true if the argument was a long parameter and was bound; false otherwise.</returns>
        private bool TryBindLongParameter(string argument)
        {
            Match match = LongParameterRegex.Match(argument);
            if (!match.Success)
            {
                return false;
            }

            string paramName = match.Groups["parameter_name"].Value;
            string paramValue = match.Groups["parameter_value"].Value;

            Parameter parameter = this.GetParameterByLongName(paramName);
            if (parameter.IsSwitch && !string.IsNullOrEmpty(paramValue))
            {
                throw new ParameterException(paramName, "Cannot assign value to switch parameter.");
            }

            this.BindParameter(parameter, paramValue);
            return true;
        }

        /// <summary>
        /// Tries to parse and bind a short parameter (-p or /p format).
        /// </summary>
        /// <param name="argument">The command line argument.</param>
        /// <returns>The parameter if it requires a value in the next argument; null if bound or not matched.</returns>
        private Parameter? TryBindShortParameter(string argument)
        {
            Match match = ShortParameterRegex.Match(argument);
            if (!match.Success)
            {
                return null;
            }

            string paramName = match.Groups["parameter_name"].Value;
            Parameter parameter = this.GetParameterByShortName(paramName);

            if (parameter.IsSwitch)
            {
                this.BindParameter(parameter, string.Empty);
                return null;
            }

            // Non-switch parameter needs value from next argument
            return parameter;
        }

        /// <summary>
        /// Tries to bind a positional parameter.
        /// </summary>
        /// <param name="argument">The command line argument.</param>
        /// <returns>true if the argument was bound to a positional parameter; false otherwise.</returns>
        private bool TryBindPositionalParameter(string argument)
        {
            // Skip if argument looks like a parameter (starts with - or /)
            if (ParameterLikeRegex.IsMatch(argument))
            {
                return false;
            }

            Parameter? positionalParameter = this.GetNextPositionalParameter();
            if (positionalParameter == null)
            {
                return false;
            }

            this.BindParameter(positionalParameter, argument);
            return true;
        }

        /// <summary>
        /// Gets the next available positional parameter from the current or candidate parameter set.
        /// </summary>
        /// <returns>The next positional parameter, or null if none available.</returns>
        private Parameter? GetNextPositionalParameter()
        {
            if (this.currentParameterSet is null)
            {
                // No parameter set selected yet - pick first set with positional parameters
                List<ParameterSet> candidates = this.GetParameterSetsWithPositionalParameters();
                return candidates.Count > 0 ? candidates[0].GetNextPositionalParameter() : null;
            }

            if (this.sets[this.currentParameterSet].HasNextPositionalParameter)
            {
                return this.sets[this.currentParameterSet].GetNextPositionalParameter();
            }

            // Try to switch to another parameter set that can accommodate more positional parameters
            ParameterSet? alternativeSet = this.TryFindCompatibleParameterSet();
            if (alternativeSet != null)
            {
                this.sets[this.currentParameterSet].IsActive = false;
                this.currentParameterSet = alternativeSet.Name;
                return alternativeSet.GetNextPositionalParameter();
            }

            return null;
        }

        /// <summary>
        /// Attempts to find an alternative parameter set that is compatible with currently bound parameters
        /// and has available positional parameters.
        /// </summary>
        /// <returns>A compatible parameter set with available positional parameters, or null if none found.</returns>
        private ParameterSet? TryFindCompatibleParameterSet()
        {
            // Cannot switch if parameter set selection is locked
            if (this.parameterSetSelected)
            {
                return null;
            }

            foreach (ParameterSet candidateSet in this.sets.Values)
            {
                // Skip current set, inactive sets, and sets without available positional parameters
                if (candidateSet.Name == this.currentParameterSet ||
                    !candidateSet.IsActive ||
                    !candidateSet.HasNextPositionalParameter)
                {
                    continue;
                }

                // Check if all currently bound parameters are compatible with this set
                if (this.CanSwitchToParameterSet(candidateSet))
                {
                    return candidateSet;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the parser can switch to the specified parameter set.
        /// This is possible when all parameters that have been assigned values are part of the target set.
        /// </summary>
        /// <param name="targetSet">The target parameter set to check compatibility with.</param>
        /// <returns>true if switching to the target set is possible; false otherwise.</returns>
        internal bool CanSwitchToParameterSet(ParameterSet targetSet)
        {
            Precondition.CheckNotNull(targetSet, nameof(targetSet));

            foreach (Parameter parameter in this.parameters)
            {
                // Only check parameters that have been assigned values
                if (parameter.HasValue && !parameter.IsPartOfParameterSet(targetSet.Name))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if parameter has a value assigned from command line.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <returns>true if parameter has a value; false otherwise.</returns>
        public bool HasValue(string parameterName)
        {
            return this[parameterName].HasValue;
        }

        /// <summary>
        /// Returns parameter enumerator
        /// </summary>
        /// <returns>Parameter enumerator</returns>
        public IEnumerator<Parameter> GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        /// <summary>
        /// Returns parameter enumerator.
        /// </summary>
        /// <returns>Parameter enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Generates usage and parameter help.
        /// </summary>
        /// <param name="output">Output to append usage text to.</param>
        /// <param name="options">Program options.</param>
        public void AppendUsageText(StringBuilder output, IOptions options)
        {
            Parameter[] mandatoryParameters = this.parameters.Where(p => p.IsMandatory).ToArray();
            Parameter[] optionalParameters = this.parameters.Where(p => !p.IsMandatory && !p.IsSwitch).ToArray();
            Parameter[] switchParameters = this.parameters.Where(p => p.IsSwitch).ToArray();

            output.AppendLine("Syntax:");
            foreach (ParameterSet set in this.sets.Values)
            {
                if (set.Name != Parameter.DefaultParameterSetName || this.sets.Count == 1)
                {
                    // During parameter creation we enforce either short or long name to be defined.
                    // GetSyntax() call below will also ensure that:
                    // a) Parameter has long name if LongOnly syntax is used.
                    // -or-
                    // b) Parameter has short name of ShortOnly syntax is used.
                    //
                    // It we don't throw here we don't need to do any further validation.
                    output.AppendLine("  " + options.ProgramName + set.GetSyntax(options));
                }
            }

            if (mandatoryParameters.Length > 0)
            {
                output.AppendLine();
                output.AppendLine("Mandatory Parameters:");
                foreach (Parameter parameter in mandatoryParameters)
                {
                    output.AppendLine(this.GetParameterUsageText(parameter, options));
                }
            }

            if (optionalParameters.Length > 0)
            {
                output.AppendLine();
                output.AppendLine("Optional Parameters:");
                foreach (Parameter parameter in optionalParameters)
                {
                    output.AppendLine(this.GetParameterUsageText(parameter, options));
                }
            }

            if (switchParameters.Length > 0)
            {
                output.AppendLine();
                output.AppendLine("Switch Parameters:");
                foreach (Parameter parameter in switchParameters)
                {
                    output.AppendLine(this.GetParameterUsageText(parameter, options));
                }
            }

            if (options.Examples.Count > 0)
            {
                output.AppendLine();
                output.AppendLine("Examples:");
                foreach (string example in options.Examples)
                {
                    output.AppendLine("  " + example);
                }
            }

            if (options.Remarks.Count > 0)
            {
                output.AppendLine();
                output.AppendLine("Remarks:");
                foreach (string remark in options.Remarks)
                {
                    output.AppendLine("  " + remark);
                }
            }
        }

        /// <summary>
        /// Attempts to find identical names (short or long) in two parameters.
        /// </summary>
        /// <param name="param1">First parameter</param>
        /// <param name="param2">Second parameter</param>
        /// <returns>Identical parameter name or empty string if not matched</returns>
        internal static string GetIdenticalParameterName(Parameter param1, Parameter param2)
        {
            if (string.Compare(param1.LongName, param2.LongName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return param1.LongName!;
            }

            if (string.Compare(param1.ShortName, param2.ShortName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return param1.ShortName!;
            }

            if (string.Compare(param1.LongName, param2.ShortName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return param1.LongName!;
            }

            if (string.Compare(param1.ShortName, param2.LongName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return param1.ShortName!;
            }

            return string.Empty;
        }

        /// <summary>
        /// Generates parameter usage text block.
        /// </summary>
        /// <param name="parameter">Parameter definition</param>
        /// <param name="options">Program options.</param>
        /// <returns>String containing parameter usage text</returns>
        private string GetParameterUsageText(Parameter parameter, IOptions options)
        {
            StringBuilder sb = new StringBuilder("  ");

            if (options.ParameterSyntax == ParameterSyntax.Mixed)
            {
                if (this.HasParametersWithShortName)
                {
                    if (parameter.HasShortName)
                    {
                        sb.Append(parameter.HasLongName
                            ? $"-{parameter.ShortName},".PadRight(this.maxShortNameLength)
                            : $"-{parameter.ShortName}".PadRight(this.maxShortNameLength));
                    }
                    else
                    {
                        sb.Append(" ".PadRight(this.maxShortNameLength));
                    }
                }

                if (this.HasParametersWithLongName)
                {
                    if (parameter.HasLongName)
                    {
                        if (parameter.HasLongName && parameter.IsSwitch)
                        {
                            sb.Append($"--{parameter.LongName}".PadRight(this.maxLongNameLength));
                        }
                        else if (parameter.HasLongName)
                        {
                            sb.Append($"--{parameter.LongName}={parameter.MetaValue}".PadRight(this.maxLongNameLength));
                        }
                    }
                    else
                    {
                        sb.Append(" ".PadRight(this.maxLongNameLength));
                    }
                }
            }
            else if (options.ParameterSyntax == ParameterSyntax.ShortOnly)
            {
                // We already checked that all parameters have short name
                sb.Append($"-{parameter.ShortName}".PadRight(this.maxShortNameLength));
            }
            else
            {
                // We already checked that all parameters have long name
                if (parameter.IsSwitch)
                {
                    sb.Append($"--{parameter.LongName}".PadRight(this.maxLongNameLength));
                }
                else
                {
                    sb.Append($"--{parameter.LongName}={parameter.MetaValue}".PadRight(this.maxLongNameLength));
                }
            }

            sb.Append(!parameter.HasDefaultValue
                ? parameter.Description
                : $"{parameter.Description} (default: {parameter.DefaultValue})");

            return sb.ToString();
        }

        /// <summary>
        /// Pre-parameter binding validation.
        /// </summary>
        private void PreBindValidate()
        {
            foreach (ParameterSet parameterSet in this.sets.Values.Where(p => p.IsActive))
            {
                parameterSet.PreBindValidate();
            }
        }

        /// <summary>
        /// Post-parameter binding validation.
        /// </summary>
        private void PostBindValidate()
        {
            // We may have more than one active parameter set at the end.
            // However, we only need to validate one we selected.
            this.sets[this.currentParameterSet!].PostBindValidate();
        }

        /// <summary>
        /// Finds parameter by short or long name.
        /// </summary>
        /// <param name="parameterName">Short or long parameter name</param>
        /// <returns>Parameter object</returns>
        private Parameter GetParameterByName(string parameterName)
        {
            foreach (Parameter parameter in this.parameters)
            {
                if (parameter.HasShortName &&
                    string.Compare(parameter.ShortName, parameterName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return parameter;
                }

                if (parameter.HasLongName &&
                    string.Compare(parameter.LongName, parameterName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return parameter;
                }
            }

            throw new ParameterException("Unknown parameter name: " + parameterName);
        }

        /// <summary>
        /// Finds parameter using short name.
        /// </summary>
        /// <param name="parameterName">Short parameter name</param>
        /// <returns>Parameter object</returns>
        private Parameter GetParameterByShortName(string parameterName)
        {
            foreach (Parameter parameter in this.parameters)
            {
                if (parameter.HasShortName &&
                    string.Compare(parameter.ShortName, parameterName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return parameter;
                }
            }

            throw new ParameterException(parameterName, "Unknown short parameter name.");
        }

        /// <summary>
        /// Finds parameter using long name.
        /// </summary>
        /// <param name="parameterName">Long parameter name</param>
        /// <returns>Parameter object</returns>
        private Parameter GetParameterByLongName(string parameterName)
        {
            foreach (Parameter parameter in this.parameters)
            {
                if (parameter.HasLongName &&
                    string.Compare(parameter.LongName, parameterName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return parameter;
                }
            }

            throw new ParameterException(parameterName, "Unknown long parameter name.");
        }

        /// <summary>
        /// Adds parameter definition to all parameter sets it belongs.
        /// </summary>
        /// <param name="parameter">Parameter definition</param>
        private void UpdateParameterSets(Parameter parameter)
        {
            // Create all distinct parameter sets
            foreach (string parameterSetName in parameter.ParameterSets)
            {
                if (this.sets.ContainsKey(parameterSetName) && !this.sets[parameterSetName].Contains(parameter))
                {
                    this.sets[parameterSetName].Add(parameter);
                }
            }

            // Make sure all parameters without ParameterSetAttribute are added to all parameter sets
            foreach (ParameterSet parameterSet in this.sets.Values)
            {
                if (parameter.IsPartOfParameterSet(parameterSet.Name) && !parameterSet.Contains(parameter))
                {
                    parameterSet.Add(parameter);
                }
            }
        }

        /// <summary>
        /// Creates parameter set instance for each defined parameter set.
        /// </summary>
        /// <param name="parameterDefinitions">All parameter definitions.</param>
        private void CreateParameterSets(IEnumerable<Parameter> parameterDefinitions)
        {
            foreach (Parameter parameter in parameterDefinitions)
            {
                foreach (string parameterSetName in parameter.ParameterSets)
                {
                    if (!this.sets.ContainsKey(parameterSetName))
                    {
                        this.sets.Add(parameterSetName, new ParameterSet(parameterSetName));
                    }
                }
            }
        }

        /// <summary>
        /// Selects parameter set
        /// </summary>
        /// <param name="parameter">Parameter definition</param>
        private void SelectParameterSet(Parameter parameter)
        {
            if (this.currentParameterSet is null || this.currentParameterSet == Parameter.DefaultParameterSetName)
            {
                this.currentParameterSet = parameter.ParameterSets[0];
                if (parameter.ParameterSets.Count == 1 && parameter.ParameterSets[0] != Parameter.DefaultParameterSetName)
                {
                    this.parameterSetSelected = true;
                }

                return;
            }

            if (parameter.IsPartOfParameterSet(this.currentParameterSet))
            {
                // Mark parameter sets this parameter does not belong to as inactive
                foreach (string parameterSetName in this.sets.Keys)
                {
                    if (!parameter.IsPartOfParameterSet(parameterSetName))
                    {
                        this.sets[parameterSetName].IsActive = false;
                    }
                }
            }
            else
            {
                if (this.parameterSetSelected)
                {
                    throw new ParameterSetResolutionException();
                }

                foreach (string parameterSetName in this.sets.Keys)
                {
                    if (this.sets[parameterSetName].IsActive && parameter.IsPartOfParameterSet(parameterSetName))
                    {
                        this.sets[this.currentParameterSet].IsActive = false;
                        this.currentParameterSet = parameterSetName;
                    }
                }
            }
        }

        /// <summary>
        /// Returns list of all parameter sets containing positional parameters.
        /// </summary>
        /// <returns>List of parameter sets containing positional parameters.</returns>
        private List<ParameterSet> GetParameterSetsWithPositionalParameters()
        {
            List<ParameterSet> result = new List<ParameterSet>();
            foreach (ParameterSet set in this.sets.Values)
            {
                if (set.PositionalParameterCount > 0)
                {
                    result.Add(set);
                }
            }

            return result;
        }
    }
}

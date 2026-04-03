namespace ConsoleWeaver.Cmdline
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Reads, parses and bind command line arguments using user defined parameters.
    /// </summary>
    public static class ProgramOptions
    {
        /// <summary>
        /// Parses and binds command line arguments.
        /// </summary>
        /// <typeparam name="TOptions">Type containing user defined options.</typeparam>
        /// <param name="cmdlineArguments">Command line arguments.</param>
        /// <returns>Instance with populated parameter values.</returns>
        public static TOptions Parse<TOptions>(string[] cmdlineArguments)
            where TOptions : IOptions, new()
        {
            Dictionary<Parameter, PropertyInfo> paramDefinitionMap = ReadParameterDefinitions(typeof(TOptions));
            TOptions options = SetParameterValues<TOptions>(paramDefinitionMap, cmdlineArguments);
            options.Validate();
            return options;
        }

        /// <summary>
        /// Reads parameter definition and maps them to corresponding properties.
        /// </summary>
        /// <param name="definingType">Type containing parameter definitions.</param>
        /// <returns>Parameter to property definition map.</returns>
        private static Dictionary<Parameter, PropertyInfo> ReadParameterDefinitions(Type definingType)
        {
            Dictionary<Parameter, PropertyInfo> definitionToPropertyMap = new Dictionary<Parameter, PropertyInfo>();

            MemberInfo[] memberInfos = definingType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                PropertyInfo? propertyInfo = memberInfo as PropertyInfo;
                if (propertyInfo == null)
                {
                    continue;
                }

                Parameter? parameter = null;
                List<string> parameterSets = new List<string>();
                Attribute[] attributes = Attribute.GetCustomAttributes(propertyInfo);
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is MandatoryParameterAttribute mandatoryAttribute)
                    {
                        parameter = mandatoryAttribute.CreateParameterDefinition(propertyInfo.PropertyType.IsArray);
                    }
                    else if (attribute is OptionalParameterAttribute optionalAttribute)
                    {
                        parameter = optionalAttribute.CreateParameterDefinition(propertyInfo.PropertyType.IsArray);
                    }
                    else if (attribute is SwitchParameterAttribute switchParameter)
                    {
                        parameter = switchParameter.CreateParameterDefinition();
                    }
                    else if (attribute is ParameterSetAttribute parameterSet)
                    {
                        string parameterSetName = parameterSet.Name;
                        if (!parameterSets.Contains(parameterSetName))
                        {
                            parameterSets.Add(parameterSetName);
                        }
                    }
                }

                if (parameter is not null)
                {
                    if (!propertyInfo.CanWrite)
                    {
                        throw new ParameterException(parameter.Name, "Supported property must be writable.");
                    }

                    if ((parameter is MandatoryParameter || parameter is OptionalParameter) && propertyInfo.PropertyType.IsGenericType)
                    {
                        throw new ParameterDefinitionException(
                            parameter.Name,
                            "Properties with generic types are not supported.");
                    }
                    
                    if (parameter is SwitchParameter && propertyInfo.PropertyType != typeof(bool))
                    {
                        throw new ParameterDefinitionException(
                            parameter.Name,
                            "Switch attribute can only be applied to boolean properties.");
                    }

                    // Cannot use Dictionary.ContainsKey() because Parameter.GetHashCode() returns different codes for what we consider equal parameters.
                    foreach (Parameter existing in definitionToPropertyMap.Keys)
                    {
                        if (existing.Equals(parameter))
                        {
                            throw new ParameterDefinitionException(
                                CmdlineParser.GetIdenticalParameterName(existing, parameter),
                                "Duplicate parameter name.");
                        }
                    }

                    if (parameterSets.Count > 0)
                    {
                        parameter.SetParameterSets(parameterSets);
                    }
                                  
                    definitionToPropertyMap.Add(parameter, propertyInfo);
                }
            }
            
            return definitionToPropertyMap;
        }

        /// <summary>
        /// Parses command line arguments
        /// </summary>
        /// <typeparam name="TOptions">Type containing parameter definitions</typeparam>
        /// <param name="parameterMap">Parameter definition to property map</param>
        /// <param name="args">Command line arguments</param>
        /// <returns>Options object with properties populated from command line</returns>
        private static TOptions SetParameterValues<TOptions>(Dictionary<Parameter, PropertyInfo> parameterMap, string[] args)
            where TOptions : IOptions, new()
        {
            TOptions options = new TOptions();
            CmdlineParser cmdlineParser = new CmdlineParser(parameterMap.Keys.ToList());

            // Check if we have to show help/usage
            if (cmdlineParser.ShowUsage(args))
            {
                StringBuilder usage = new StringBuilder(2048);
                AppendHeaderText(usage, options);
                cmdlineParser.AppendUsageText(usage, options);
                throw new IncorrectUsageException(usage.ToString());
            }

            // Parse command line. This validates presence of mandatory parameters.
            cmdlineParser.Parse(args);

            // Convert string values and populate properties
            foreach (Parameter parameter in parameterMap.Keys)
            {
                PropertyInfo propertyInfo = parameterMap[parameter];

                if (parameter.IsSwitch)
                {
                    propertyInfo.SetValue(options, parameter.HasValue);
                }
                else if (parameter.HasValue || parameter.HasDefaultValue)
                {
                    object? value = parameter.GetValue(propertyInfo.PropertyType);
                    propertyInfo.SetValue(options, value);
                }
            }

            options.ParameterSetName = cmdlineParser.ParameterSetName;
            return options;
        }

        /// <summary>
        /// Appends header text to the usage output.
        /// </summary>
        /// <param name="output">Output to append header to.</param>
        /// <param name="options">Program options.</param>
        private static void AppendHeaderText(StringBuilder output, IOptions options)
        {
            if (options.ProgramDescription.Count > 0)
            {
                // Add version string to description so it is also enclosed in the box
                options.ProgramDescription.Add("Version: " + options.ProgramVersion);

                if (options.HeaderBox)
                {
                    TextFormat.WriteTextBox(output, options.ProgramDescription, options.HeaderAlignment, options.HeaderWidth, 1);
                }
                else
                {
                    TextFormat.WriteAlignedText(output, options.ProgramDescription, options.HeaderAlignment, options.HeaderWidth, 1);
                }

                output.AppendLine();
            }
        }
    }
}

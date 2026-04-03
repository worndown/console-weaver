namespace ConsoleWeaver.Cmdline
{
    using ConsoleWeaver;
    using System.Collections.Generic;

    /// <summary>
    /// Basic information about program used to generate help.
    /// Program parameters should be defined in the type implementing this interface.
    /// </summary>
    public interface IOptions
    {
        /// <summary>
        /// Gets short program/executable name.
        /// If null, executing assembly name is used.
        /// </summary>
        string ProgramName { get; }

        /// <summary>
        /// Gets program description to be included in the help header.
        /// </summary>
        IList<string> ProgramDescription { get; }

        /// <summary>
        /// Gets program display version.
        /// </summary>
        string ProgramVersion { get; }

        /// <summary>
        /// Whether header text should be surrounded by a box.
        /// </summary>
        bool HeaderBox { get; }

        /// <summary>
        /// Gets header width in characters.
        /// </summary>
        int HeaderWidth { get; }

        /// <summary>
        /// Gets header text alignment.
        /// </summary>
        Alignment HeaderAlignment { get; }

        /// <summary>
        /// Gets parameter syntax that should be used in the help.
        /// </summary>
        ParameterSyntax ParameterSyntax { get; }

        /// <summary>
        /// Gets parameter set name being used.
        /// </summary>
        string ParameterSetName { get; set; }

        /// <summary>
        /// Gets optional list of remarks to be included in the help.
        /// </summary>
        IList<string> Remarks { get; }

        /// <summary>
        /// Gets optional list of usage examples to be included in the help.
        /// </summary>
        IList<string> Examples { get; }

        /// <summary>
        /// Performs validation after parameter binding.
        /// Use for validating parameters interdependency, value range etc.
        /// </summary>
        void Validate();
    }

    /// <summary>
    /// Parameter syntax used in the help/usage message.
    /// </summary>
    public enum ParameterSyntax
    {
        /// <summary>
        /// Both short and long options are allowed.
        /// </summary>
        Mixed,

        /// <summary>
        /// Only short names will be used.
        /// </summary>
        ShortOnly,

        /// <summary>
        /// Only long names will be used.
        /// </summary>
        LongOnly
    }
}

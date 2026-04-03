namespace ConsoleWeaver.Cmdline
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Base class for defining parameters.
    /// Populates program name and version.
    /// </summary>
    public abstract class OptionsBase : IOptions
    {
        /// <summary>
        /// Protected constructor
        /// </summary>
        protected OptionsBase()
        {
            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            this.ProgramName = assembly.GetName().Name ?? string.Empty;
            this.ProgramVersion = assembly.GetName().Version?.ToString() ?? string.Empty;
            this.ParameterSyntax = ParameterSyntax.Mixed;
        }

        /// <summary>
        /// Program name for usage help.
        /// </summary>
        public string ProgramName { get; protected set; }

        /// <summary>
        /// Program description to include in the help header.
        /// </summary>
        public IList<string> ProgramDescription { get; } = new List<string>();

        /// <summary>
        /// Program version to use in help
        /// </summary>
        public string ProgramVersion { get; protected set; }

        /// <summary>
        /// Whether header text should be surrounded by a box.
        /// </summary>
        public bool HeaderBox { get; protected set; } = true;

        /// <summary>
        /// Gets header width in characters.
        /// </summary>
        public int HeaderWidth { get; protected set; } = 60;

        /// <summary>
        /// Gets header text alignment.
        /// </summary>
        public Alignment HeaderAlignment { get; protected set; } = Alignment.Left;

        /// <summary>
        /// Which parameter syntax should be used.
        /// </summary>
        public ParameterSyntax ParameterSyntax { get; protected set; }

        /// <summary>
        /// Current parameter set name.
        /// </summary>
        public string ParameterSetName { get; set; } = string.Empty;

        /// <summary>
        /// Gets optional list of remarks to be included in the help.
        /// </summary>
        public IList<string> Remarks { get; } = new List<string>();

        /// <summary>
        /// Gets optional list of usage examples to be included in the help.
        /// </summary>
        public IList<string> Examples { get; } = new List<string>();

        /// <summary>
        /// Performs validation after parameter binding
        /// </summary>
        public void Validate()
        {
            this.InternalValidate();
        }

        /// <summary>
        /// Actual validation should be performed in derived class
        /// </summary>
        protected abstract void InternalValidate();
    }
}

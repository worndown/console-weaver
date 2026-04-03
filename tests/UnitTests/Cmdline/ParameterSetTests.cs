namespace UnitTests.Cmdline
{
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver;
    using ConsoleWeaver.Cmdline;

    internal class MyOptions : OptionsBase
    {
        public MyOptions()
        {
            this.ProgramName = "UnitTest";
            this.ProgramVersion = "1.0";
            this.ProgramDescription.Add("Mixed parameter syntax");
            this.Examples.Add("UnitTest notepad.exe --username=phillip --token=foobar --weight=124");
            this.Remarks.Add("When using process name make sure to include .exe extension.");
        }

        [ParameterSet("Name")]
        [MandatoryParameter(ShortName = "p", LongName = "process", MetaValue = "NAME", Description = "Process name", Position = 0)]
        public string ProcessName { get; set; }

        [ParameterSet("Pid")]
        [MandatoryParameter(ShortName = "i", LongName = "pid", MetaValue = "VAL", Description = "Process ID", Position = 0)]
        public int ProcessId { get; set; }

        [ParameterSet("Token")]
        [MandatoryParameter(ShortName = "t", LongName = "token", MetaValue = "VAL", Description = "Token value", Position = 0)]
        public string Token { get; set; }

        [ParameterSet("Token")]
        [MandatoryParameter(ShortName = "w", LongName = "weight", MetaValue = "VAL", Description = "Token weight", Position = 1)]
        public float TokenWeight { get; set; }

        [ParameterSet("Name")]
        [ParameterSet("Pid")]
        [ParameterSet("Token")]
        [OptionalParameter(ShortName = "u", LongName = "username", MetaValue = "VAL", Description = "User to impersonate")]
        public string Username { get; set; }

        [ParameterSet("Token")]
        [SwitchParameter(ShortName = "x", LongName = "extended", Description = "Use extended token format")]
        public bool Extended { get; set; }

        [OptionalParameter(ShortName = "d", LongName = "delay", MetaValue = "VAL", Description = "Delay in seconds")]
        public int Delay { get; set; }

        [SwitchParameter(ShortName = "f", LongName = "force", Description = "Overrides potential warnings")]
        public bool Force { get; set; }

        protected override void InternalValidate()
        {
        }
    }

    internal class MyOptionsLongSyntax : OptionsBase
    {
        public MyOptionsLongSyntax()
        {
            this.ProgramName = "UnitTest";
            this.ProgramVersion = "1.0";
            this.ProgramDescription.Add("Long parameter syntax only.");
            this.ParameterSyntax = ParameterSyntax.LongOnly;
            this.HeaderAlignment = Alignment.Center;
            this.HeaderWidth = 80;
        }

        [ParameterSet("Name")]
        [MandatoryParameter(ShortName = "p", LongName = "process", MetaValue = "NAME", Description = "Process name", Position = 0)]
        public string ProcessName { get; set; }

        [ParameterSet("Pid")]
        [MandatoryParameter(ShortName = "i", LongName = "pid", MetaValue = "VAL", Description = "Process ID", Position = 0)]
        public int ProcessId { get; set; }

        [ParameterSet("Token")]
        [MandatoryParameter(ShortName = "t", LongName = "token", MetaValue = "VAL", Description = "Token value", Position = 0)]
        public string Token { get; set; }

        [ParameterSet("Token")]
        [MandatoryParameter(ShortName = "w", LongName = "weight", MetaValue = "VAL", Description = "Token weight", Position = 1)]
        public float TokenWeight { get; set; }

        [ParameterSet("Name")]
        [ParameterSet("Pid")]
        [ParameterSet("Token")]
        [OptionalParameter(ShortName = "u", LongName = "username", MetaValue = "VAL", Description = "User to impersonate")]
        public string Username { get; set; }

        [ParameterSet("Token")]
        [SwitchParameter(ShortName = "x", LongName = "extended", Description = "Use extended token format")]
        public bool Extended { get; set; }

        [OptionalParameter(ShortName = "d", LongName = "delay", MetaValue = "VAL", Description = "Delay in seconds")]
        public int Delay { get; set; }

        [SwitchParameter(ShortName = "f", LongName = "force", Description = "Overrides potential warnings")]
        public bool Force { get; set; }

        protected override void InternalValidate()
        {
        }
    }

    internal class MyOptionsShortSyntax : OptionsBase
    {
        public MyOptionsShortSyntax()
        {
            this.ProgramName = "UnitTest";
            this.ProgramVersion = "1.0";
            this.ProgramDescription.Add("Short parameter syntax only");
            this.ParameterSyntax = ParameterSyntax.ShortOnly;
            this.HeaderAlignment = Alignment.Right;
            this.HeaderWidth = 76;
        }

        [ParameterSet("Name")]
        [MandatoryParameter(ShortName = "p", LongName = "process", MetaValue = "NAME", Description = "Process name", Position = 0)]
        public string ProcessName { get; set; }

        [ParameterSet("Pid")]
        [MandatoryParameter(ShortName = "i", LongName = "pid", MetaValue = "VAL", Description = "Process ID", Position = 0)]
        public int ProcessId { get; set; }

        [ParameterSet("Token")]
        [MandatoryParameter(ShortName = "t", LongName = "token", MetaValue = "VAL", Description = "Token value", Position = 0)]
        public string Token { get; set; }

        [ParameterSet("Token")]
        [MandatoryParameter(ShortName = "w", LongName = "weight", MetaValue = "VAL", Description = "Token weight", Position = 1)]
        public float TokenWeight { get; set; }

        [ParameterSet("Name")]
        [ParameterSet("Pid")]
        [ParameterSet("Token")]
        [OptionalParameter(ShortName = "u", MetaValue = "VAL", Description = "User to impersonate")]
        public string Username { get; set; }

        [ParameterSet("Token")]
        [SwitchParameter(ShortName = "x", Description = "Use extended token format")]
        public bool Extended { get; set; }

        [OptionalParameter(ShortName = "d", MetaValue = "VAL", Description = "Delay in seconds")]
        public int Delay { get; set; }

        [SwitchParameter(ShortName = "f", Description = "Overrides potential warnings")]
        public bool Force { get; set; }

        protected override void InternalValidate()
        {
        }
    }

    [TestClass]
    public class ParameterSetTests
    {
        [TestMethod]
        public void Parse_Conflicting_Parameter_Sets_Throws_ParameterSetResolutionException()
        {
            string[] args = ["-p", "notepad", "-i", "1336"];
            Assert.Throws<ParameterSetResolutionException>(() => ProgramOptions.Parse<MyOptions>(args));
        }

        [TestMethod]
        public void Parse_Missing_Mandatory_In_Selected_Set_Throws_ParameterException()
        {
            string[] args = ["--extended", "--delay=55", "--username=vadim", "--token=abc"];
            ParameterException ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<MyOptions>(args));
            Assert.AreEqual("Missing mandatory parameter: weight.", ex.Message);
        }

        [TestMethod]
        [DataRow("-p", "notepad", "--username=vadim")]
        [DataRow("--username=vadim", "--force", "-p", "notepad")]
        [DataRow("--force", "--delay=55", "--username=vadim", "-p", "notepad")]
        public void Parse_Process_Name_Selects_Name_Parameter_Set(params string[] args)
        {
            MyOptions options = ProgramOptions.Parse<MyOptions>(args);
            Assert.AreEqual("Name", options.ParameterSetName);
        }

        [TestMethod]
        public void Parse_Process_Id_Selects_Pid_Parameter_Set()
        {
            string[] args = ["--force", "--delay=55", "--username=vadim", "--pid=666"];
            MyOptions options = ProgramOptions.Parse<MyOptions>(args);
            Assert.AreEqual("Pid", options.ParameterSetName);
        }

        [TestMethod]
        public void GetUsageText_All_Syntax_Options_Generate_Help()
        {
            string? help = GetUsageText<MyOptions>();
            Assert.IsNotNull(help);
            Debug.Print(help);

            help = GetUsageText<MyOptionsLongSyntax>();
            Assert.IsNotNull(help);
            Debug.Print(help);

            help = GetUsageText<MyOptionsShortSyntax>();
            Assert.IsNotNull(help);
            Debug.Print(help);
        }

        private static string? GetUsageText<T>() where T : IOptions, new()
        {
            try
            {
                string[] args = ["--help"];
                T _ = ProgramOptions.Parse<T>(args);
            }
            catch (IncorrectUsageException ex)
            {
                return ex.Message;
            }

            return null;
        }
    }
}

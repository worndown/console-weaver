namespace UnitTests.Cmdline
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver;
    using ConsoleWeaver.Cmdline;

    internal enum LoggingLevel
    {
        Info,
        Debug,
        Warn,
        Error
    }

    internal enum ServerRole
    {
        Leader,
        Broker,
        Slave,
        Master
    }

    [SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "Unnecessary")]
    internal class Location : IEquatable<Location>
    {
        public Location(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException("Location cannot be empty");
            }

            string[] vals = location.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length != 2)
            {
                throw new FormatException("Location must be in \"X,Y\" format.");
            }

            this.X = int.Parse(vals[0]);
            this.Y = int.Parse(vals[1]);
        }

        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Equals(Location? other)
        {
            return other != null && (other.X == this.X && other.Y == this.Y);
        }
    }

    internal class TestOptions : OptionsBase
    {
        public TestOptions()
        {
            this.ProgramName = "UnitTest";
            this.ProgramVersion = "1.0";
            this.ProgramDescription.Add("UnitTest for testing all parameter types.");
            this.ProgramDescription.Add("Second line of description.");
            this.HeaderAlignment = Alignment.Center;
        }

        [MandatoryParameter(ShortName = "s", LongName = "server", MetaValue = "FQDN", Description = "Server fully qualified name", Position = 0)]
        public string Server { get; set; }

        [MandatoryParameter(ShortName = "p", LongName = "port", MetaValue = "VAL", Description = "Port number", Position = 1)]
        public int Port { get; set; }

        [OptionalParameter(ShortName = "u", LongName = "username", MetaValue = "VAL", Description = "User to impersonate")]
        public string Username { get; set; }

        [OptionalParameter(ShortName = "l", LongName = "log", MetaValue = "LEVEL", Description = "Logging level")]
        public LoggingLevel LogLevel { get; set; }

        [OptionalParameter(ShortName = "t", LongName = "timeout", MetaValue = "VAL", Description = "Operation timeout in seconds")]
        public int Timeout { get; set; }

        [OptionalParameter(ShortName = "r", LongName = "retries", MetaValue = "VAL", Description = "Number of retries", DefaultValue = "5")]
        public int Retries { get; set; }

        [OptionalParameter(ShortName = "m", LongName = "magic", MetaValue = "VAL[]", Description = "One or more magic numbers")]
        public int[]? MagicNumbers { get; set; }

        [OptionalParameter(ShortName = "c", LongName = "country", MetaValue = "VAL[]", Description = "One or more country codes")]
        public string[]? Countries { get; set; }

        [OptionalParameter(ShortName = "e", LongName = "role", MetaValue = "ROLE[]", Description = "One or more server roles")]
        public ServerRole[]? Roles { get; set; }

        [OptionalParameter(ShortName = "h", LongName = "home", MetaValue = "LOCATION", Description = "Home coordinates")]
        public Location HomeLocation { get; set; }

        [OptionalParameter(ShortName = "i", LongName = "interval", MetaValue = "TIMESPAN", Description = "Interval period")]
        public TimeSpan Interval { get; set; }

        [SwitchParameter(ShortName = "o", LongName = "overload", Description = "Overload server with requests")]
        public bool Overload { get; set; }

        [SwitchParameter(ShortName = "x", LongName = "extreme", Description = "Run in extreme mode")]
        public bool Extreme { get; set; }

        protected override void InternalValidate()
        {
            if (this.Port != 389)
            {
                throw new ParameterException("Invalid port number");
            }
        }
    }

    /// <summary>
    /// Parameters only have short names
    /// </summary>
    internal class TestOptionsShortNameOnly : OptionsBase
    {
        [MandatoryParameter(ShortName = "s", MetaValue = "FQDN", Description = "Server name")]
        public string Server { get; internal set; }

        [MandatoryParameter(ShortName = "p", MetaValue = "VAL", Description = "Port number")]
        public int Port { get; internal set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Parameters only have long names
    /// </summary>
    internal class TestOptionsLongNameOnly : OptionsBase
    {
        [MandatoryParameter(LongName = "server", MetaValue = "FQDN", Description = "Server name")]
        public string Server { get; internal set; }

        [MandatoryParameter(LongName = "port", MetaValue = "VAL", Description = "Port number")]
        public int Port { get; internal set; }

        protected override void InternalValidate()
        {
        }
    }

    /// <summary>
    /// Some parameter have only short name while others have only long name
    /// </summary>
    internal class TestOptionsMixedNames : OptionsBase
    {
        [MandatoryParameter(ShortName = "s", MetaValue = "FQDN", Description = "Server name")]
        public string Server { get; internal set; }

        [MandatoryParameter(LongName = "port", MetaValue = "VAL", Description = "Port number")]
        public int Port { get; internal set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented option with read-only port.
    /// </summary>
    internal class TestOptionsReadOnly : OptionsBase
    {
        [MandatoryParameter(ShortName = "p", LongName = "port", MetaValue = "VAL", Description = "Port number", Position = 0)]
        public int Port { get; } = -1;

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Generic option types are not supported.
    /// </summary>
    internal class TestOptionsGenericProperty : OptionsBase
    {
        [MandatoryParameter(ShortName = "n", LongName = "numbers", MetaValue = "VAL", Description = "Number values")]
        public List<int> Numbers { get; internal set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Switch parameters must be boolean.
    /// </summary>
    internal class TestOptionsBadSwitchParameter : OptionsBase
    {
        [SwitchParameter(ShortName = "e", LongName = "enabled", Description = "Enabled value")]
        public int Enabled { get; internal set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Parameter missing both short and long names.
    /// </summary>
    internal class TestOptionsMissingName : OptionsBase
    {
        [MandatoryParameter(MetaValue = "VAL", Description = "Number values", Position = 0)]
        public List<int> Numbers { get; internal set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Short parameter name contains whitespace.
    /// </summary>
    internal class TestOptionsWhiteSpaceShortName : OptionsBase
    {
        [MandatoryParameter(ShortName = "s v", LongName = "server", MetaValue = "FQDN", Description = "Server fully qualified name", Position = 0)]
        public string Server { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Long parameter name contains whitespace.
    /// </summary>
    internal class TestOptionsWhiteSpaceLongName : OptionsBase
    {
        [MandatoryParameter(ShortName = "s", LongName = "ser ver", MetaValue = "FQDN", Description = "Server fully qualified name", Position = 0)]
        public string Server { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Long and short parameters have identical name.
    /// </summary>
    internal class TestOptionsIdenticalNames : OptionsBase
    {
        [MandatoryParameter(ShortName = "srv", LongName = "Srv", MetaValue = "FQDN", Description = "Server fully qualified name", Position = 0)]
        public string Server { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Missing parameter description.
    /// </summary>
    internal class TestOptionsMissingDescription : OptionsBase
    {
        [MandatoryParameter(ShortName = "s", LongName = "server", MetaValue = "FQDN", Description = "", Position = 0)]
        public string Server { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Switch parameter attribute is set on multivalued property
    /// </summary>
    internal class TestOptionsMultivaluedSwitch : OptionsBase
    {
        [SwitchParameter(ShortName = "e", LongName = "enable", Description = "Enables a feature")]
        public bool[] Enable { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Missing meta-value.
    /// </summary>
    internal class TestOptionsMissingMetaValue : OptionsBase
    {
        [MandatoryParameter(ShortName = "s", LongName = "server", Description = "Server fully qualified name", Position = 0)]
        public string Server { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Positional parameter without required long name.
    /// </summary>
    internal class TestOptionsPositionalWithoutLongName : OptionsBase
    {
        [MandatoryParameter(ShortName = "s", MetaValue = "FQDN", Description = "Server fully qualified name", Position = 0)]
        public string Server { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Duplicate short parameter name.
    /// </summary>
    internal class TestOptionsDuplicateShortParameterName : OptionsBase
    {
        [MandatoryParameter(ShortName = "t", LongName = "title", MetaValue = "VAL", Description = "Document title", Position = 0)]
        public string Title { get; set; }

        [MandatoryParameter(ShortName = "t", LongName = "text", MetaValue = "VAL", Description = "Document body", Position = 1)]
        public string Text { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Incorrectly implemented options. Duplicate long parameter name.
    /// </summary>
    internal class TestOptionsDuplicateLongParameterName : OptionsBase
    {
        [MandatoryParameter(ShortName = "t", LongName = "text", MetaValue = "VAL", Description = "Document title", Position = 0)]
        public string Title { get; set; }

        [MandatoryParameter(ShortName = "b", LongName = "text", MetaValue = "VAL", Description = "Document body", Position = 1)]
        public string Text { get; set; }

        protected override void InternalValidate() { }
    }

    /// <summary>
    /// Test suite for testing ProgramOptions
    /// </summary>
    [TestClass]
    public class ProgramOptionsTests
    {
        /// <summary>
        /// Successfully parsing command line using short parameter names
        /// </summary>
        [TestMethod]
        public void Parse_Short_Names_Returns_Correct_Values()
        {
            string[][] cmdlines = new string[][]
            {
                // All short parameter names
                ["-s", "dc.foo.com", "-p", "389", "-u", "joe", "-l", "debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "-e", "broker,master", "-h", "15,69", "-o", "-x", "-i", "05:15:55.555"],
                // Short parameter names with spaces between multi-values
                ["-s", "dc.foo.com", "-p", "389", "-u", "joe", "-l", "debug", "-t", "300", "-r", "10", "-m", "1, 2, 3", "-c", "us, ru, il, az", "-e", "broker , master", "-h", "15 , 69", "-o", "-x", "-i", "05:15:55.555"],
                // Short parameter names in different order
                ["-o", "-s", "dc.foo.com", "-x", "-p", "389", "-u", "joe", "-h", "15,69", "-l", "debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-i", "05:15:55.555", "-c", "us,ru,il,az", "-e", "broker,master"],
                // Short parameter names with upper case
                ["-i", "05:15:55.555", "-S", "dc.foo.com", "-p", "389", "-u", "joe", "-l", "DEBUG", "-T", "300", "-r", "10", "-M", "1,2,3", "-c", "us,ru,il,az", "-e", "broker,master", "-h", "15,69", "-o", "-X"],
            };

            TestOptions expected = GetExpectedTestOptions();

            foreach (string[] cmdline in cmdlines)
            {
                TestOptions actual = ProgramOptions.Parse<TestOptions>(cmdline);
                AssertParametersAreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Successfully parsing command line using long parameter names with various formats
        /// </summary>
        [TestMethod]
        public void Parse_Long_Names_With_Different_Separators_Returns_Correct_Values()
        {
            string[][] cmdlines = new string[][]
            {
                // Long parameter names with equal sign
                ["--server=dc.foo.com", "--port=389", "--username=joe", "--log=debug", "--timeout=300", "--retries=10", "--magic=1,2,3", "--country=us,ru,il,az", "--role=broker,master", "--home=15,69", "--overload", "--extreme", "--interval=05:15:55.555"],
                // Long parameter names with equal sign in different order
                ["--server=dc.foo.com", "--overload", "--extreme", "--port=389", "--username=joe", "--magic=1,2,3", "--log=debug", "--timeout=300", "--retries=10", "--country=us,ru,il,az", "--role=broker,master", "--interval=05:15:55.555", "--home=15,69"],
                // Long parameter names with colon
                ["--server:dc.foo.com", "--port:389", "--username:joe", "--log:debug", "--timeout:300", "--retries:10", "--magic:1,2,3", "--country:us,ru,il,az", "--role:broker,master", "--home:15,69", "--overload", "--extreme", "--interval:05:15:55.555"],
            };

            TestOptions expected = GetExpectedTestOptions();

            foreach (string[] cmdline in cmdlines)
            {
                TestOptions actual = ProgramOptions.Parse<TestOptions>(cmdline);
                AssertParametersAreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Successfully parsing command line with quoted parameter values
        /// </summary>
        [TestMethod]
        public void Parse_Quoted_Values_Returns_Correct_Values()
        {
            string[][] cmdlines = new string[][]
            {
                // Long parameter names with equal sign and quoted values
                ["--server=\"dc.foo.com\"", "--port=\"389\"", "--username=\"joe\"", "--log=\"debug\"", "--timeout=\"300\"", "--retries=\"10\"", "--magic=\"1,2,3\"", "--country=\"us,ru,il,az\"", "--role=\"broker,master\"", "--home=\"15,69\"", "--overload", "--extreme", "--interval=\"05:15:55.555\""],
                // Long parameter names with colon and quoted values
                ["--server:\"dc.foo.com\"", "--port:\"389\"", "--username:\"joe\"", "--log:\"debug\"", "--timeout:\"300\"", "--retries:\"10\"", "--magic:\"1,2,3\"", "--country:\"us,ru,il,az\"", "--role:\"broker,master\"", "--home:\"15,69\"", "--overload", "--extreme", "--interval:\"05:15:55.555\""],
                // Long parameter names with quoted values and spaces in multi-values
                ["--server=\"dc.foo.com\"", "--port=\"389\"", "--username=\"joe\"", "--log=\"debug\"", "--timeout=\"300\"", "--retries=\"10\"", "--magic=\"1 , 2,3\"", "--country=\"us, ru, il, az\"", "--role=\"broker ,master\"", "--home=\"15, 69\"", "--overload", "--extreme", "--interval=\"05:15:55.555\""],
            };

            TestOptions expected = GetExpectedTestOptions();

            foreach (string[] cmdline in cmdlines)
            {
                TestOptions actual = ProgramOptions.Parse<TestOptions>(cmdline);
                AssertParametersAreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Successfully parsing command line with mixed short and long parameter names
        /// </summary>
        [TestMethod]
        public void Parse_Mixed_Short_And_Long_Names_Returns_Correct_Values()
        {
            string[][] cmdlines = new string[][]
            {
                // Mixing short and long parameter names
                ["-s", "dc.foo.com", "--port=389", "-u", "joe", "-l", "debug", "-t", "300", "-r", "10", "--magic=1,2,3", "-c", "us,ru,il,az", "-e", "broker,master", "--home=15,69", "-o", "--interval=05:15:55.555", "--extreme"],
                // Mixing long, short, equal signs, colons, spaces and quotes
                ["-s", "dc.foo.com", "--port=389", "--username:\"joe\"", "--log:debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "--role=\"broker, master\"", "-h", "15,69", "--overload", "-x", "--interval:05:15:55.555"],
                // Mixing long, short, equal signs, colons, spaces and quotes in different order
                ["--overload", "-x", "--timeout=300", "-s", "dc.foo.com", "--port=389", "--username:\"joe\"", "-i", "\"05:15:55.555\"",  "--log:debug", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "--role=\"broker, master\"", "-h", "15, 69"],
                // Mixing with different casing
                ["-C", "us,ru,il,az", "-s", "dc.foo.com", "--Port=389", "--USERNAME:\"joe\"", "--log:debug", "-t", "300", "-R", "10", "-m", "1,2,3", "--role=\"Broker, MASTER\"", "-h", "15,69", "--overload", "-X", "--Interval=05:15:55.555"],
            };

            TestOptions expected = GetExpectedTestOptions();

            foreach (string[] cmdline in cmdlines)
            {
                TestOptions actual = ProgramOptions.Parse<TestOptions>(cmdline);
                AssertParametersAreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Successfully parsing command line with positional parameters in various locations
        /// </summary>
        [TestMethod]
        public void Parse_Positional_Parameters_Returns_Correct_Values()
        {
            string[][] cmdlines = new string[][]
            {
                // First positional parameter at the beginning
                ["dc.foo.com", "-p", "389", "-u", "joe", "-l", "debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "-e", "broker,master", "-h", "15,69", "-o", "-x", "-i", "05:15:55.555"],
                // First positional parameter in the middle
                ["-p", "389", "-u", "joe", "-l", "debug", "-t", "300", "-r", "10", "dc.foo.com",  "-m", "1,2,3", "-c", "us,ru,il,az", "-e", "broker,master", "-h", "15,69", "-o", "-x", "-i", "05:15:55.555"],
                // First positional parameter at the end
                ["-p", "389", "-u", "joe", "-l", "debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "-e", "broker,master", "-h", "15,69", "-o", "-x", "-i", "05:15:55.555", "dc.foo.com"],
                // Both positional parameters at the beginning
                ["dc.foo.com", "389", "--username:\"joe\"", "--log:debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "--role=\"broker, master\"", "-h", "15, 69", "--overload", "-x", "--interval:05:15:55.555"],
                // Both positional parameters spread out
                ["dc.foo.com", "--username:\"joe\"", "--log:debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "--role=\"broker, master\"", "-h", "15,69", "--overload", "389", "-x", "--interval:05:15:55.555"],
                // Both positional parameters at different positions
                ["--username:\"joe\"", "dc.foo.com", "--log:debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "--role=\"broker, master\"", "-h", "15,69", "--overload", "-x", "--interval:05:15:55.555", "389"],
            };

            TestOptions expected = GetExpectedTestOptions();

            foreach (string[] cmdline in cmdlines)
            {
                TestOptions actual = ProgramOptions.Parse<TestOptions>(cmdline);
                AssertParametersAreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Successfully parsing command line with slash prefix for short parameters
        /// </summary>
        [TestMethod]
        public void Parse_Slash_Prefix_Returns_Correct_Values()
        {
            string[] cmdline = ["/u", "joe", "dc.foo.com", "--log:debug", "/t", "300", "/r", "10", "/m", "1,2,3", "/c", "us,ru,il,az", "--role=\"broker, master\"", "/h", "15,69", "--overload", "/x", "--interval:05:15:55.555", "389"];

            TestOptions expected = GetExpectedTestOptions();
            TestOptions actual = ProgramOptions.Parse<TestOptions>(cmdline);
            AssertParametersAreEqual(expected, actual);
        }

        private static TestOptions GetExpectedTestOptions()
        {
            return new TestOptions
            {
                Server = "dc.foo.com",
                Port = 389,
                Username = "joe",
                LogLevel = LoggingLevel.Debug,
                Timeout = 300,
                Retries = 10,
                MagicNumbers = [1, 2, 3],
                Countries = ["us", "ru", "il", "az"],
                Roles = [ServerRole.Broker, ServerRole.Master],
                HomeLocation = new Location(15, 69),
                Interval = TimeSpan.Parse("05:15:55.555"),
                Overload = true
            };
        }

        /// <summary>
        /// Custom validation routine rejects invalid argument value
        /// </summary>
        [TestMethod]
        public void Parse_Invalid_Parameter_Value_Throws_ParameterException()
        {
            string[] cmdline = ["-s", "dc.foo.com", "-p", "555", "-u", "joe", "-l", "debug", "-t", "300", "-r", "10", "-m", "1,2,3", "-c", "us,ru,il,az", "-e", "broker,master", "-h", "15,69", "-o", "-x", "-i", "05:15:55.555"];
            Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptions>(cmdline));
        }

        /// <summary>
        /// Passing null to cmdline parser causes exception
        /// </summary>
        [TestMethod]
        public void Parse_Null_Arguments_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ProgramOptions.Parse<TestOptions>(null));
        }

        /// <summary>
        /// Incorrectly implemented program options with read-only property
        /// </summary>
        [TestMethod]
        public void Parse_ReadOnly_Property_Throws_ParameterException()
        {
            string[] cmdline = ["--port=21"];
            Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptionsReadOnly>(cmdline));
        }

        /// <summary>
        /// Mapping cmdline parameters to generic types is not supported
        /// </summary>
        [TestMethod]
        public void Parse_Generic_Property_Throws_ParameterDefinitionException()
        {
            string[] cmdline = ["--numbers=1,2,3"];
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsGenericProperty>(cmdline));
        }

        /// <summary>
        /// Parser fails when encounter non-boolean switch parameter definition
        /// </summary>
        [TestMethod]
        public void Parse_NonBoolean_Switch_Throws_ParameterDefinitionException()
        {
            string[] cmdline = ["--enabled=1"];
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsBadSwitchParameter>(cmdline));
        }

        /// <summary>
        /// When incorrect value is supplied for enum based parameter
        /// error message should contain list of valid parameter values
        /// </summary>
        [TestMethod]
        public void Parse_Invalid_Enum_Value_Shows_Valid_Options()
        {
            ParameterException ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptions>(["--server=s", "--port=389", "--log=invalid"]));
            Assert.AreEqual("log", ex.ParameterName);
            Assert.Contains("Info, Debug, Warn, Error", ex.Message);

            ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptions>(["--server=s", "--port=389", "--role=master,broker,invalid"]));
            Assert.AreEqual("role", ex.ParameterName);
            Assert.Contains("Leader, Broker, Slave, Master", ex.Message);
        }

        /// <summary>
        /// Missing mandatory parameter "server"
        /// </summary>
        [TestMethod]
        public void Parse_Missing_Mandatory_Parameter_Throws_ParameterException()
        {
            string[] cmdline = ["--port=389", "--username=joe"];
            ParameterException ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptions>(cmdline));
            Assert.Contains("Missing mandatory parameter: server.", ex.Message);
        }

        [TestMethod]
        public void Parse_Short_Names_Only_Works_Correctly()
        {
            TestOptionsShortNameOnly options = ProgramOptions.Parse<TestOptionsShortNameOnly>(["-s", "mail.foo.com", "-p", "25"]);
            Assert.AreEqual("mail.foo.com", options.Server);
            Assert.AreEqual(25, options.Port);

            ParameterException ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptionsShortNameOnly>(["-?"]));
            Assert.Contains("-s  Server name", ex.Message);
            Assert.Contains("-p  Port number", ex.Message);
        }

        [TestMethod]
        public void Parse_Long_Names_Only_Works_Correctly()
        {
            TestOptionsLongNameOnly options = ProgramOptions.Parse<TestOptionsLongNameOnly>(["--server=mail.foo.com", "--port=25"]);
            Assert.AreEqual("mail.foo.com", options.Server);
            Assert.AreEqual(25, options.Port);

            ParameterException ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptionsLongNameOnly>(["-?"]));
            string usage = ex.Message;
            Assert.Contains("--server=FQDN  Server name", ex.Message);
            Assert.Contains("--port=VAL     Port number", ex.Message);
        }

        [TestMethod]
        public void Parse_Mixed_Short_And_Long_Names_Works_Correctly()
        {
            TestOptionsMixedNames options = ProgramOptions.Parse<TestOptionsMixedNames>(["-s", "mail.foo.com", "--port=25"]);
            Assert.AreEqual("mail.foo.com", options.Server);
            Assert.AreEqual(25, options.Port);

            ParameterException ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptionsMixedNames>(["-?"]));
            Assert.Contains("-s", ex.Message);
            Assert.Contains("--port=VAL", ex.Message);
        }

        /// <summary>
        /// Generates usage from parameter definitions when help is requested
        /// </summary>
        [TestMethod]
        public void Parse_Help_Request_Generates_Usage_Text()
        {
            string[][] helpLines = new string[][]
            {
                [],
                ["-?"],
                ["--?"],
                ["/?"],
                ["-h"],
                ["--h"],
                ["/h"],
                ["-help"],
                ["--help"],
                ["/help"]
            };

            foreach (string[] helpLine in helpLines)
            {
                IncorrectUsageException ex = Assert.Throws<IncorrectUsageException>(() => ProgramOptions.Parse<TestOptions>(helpLine));
                string usage = ex.Message;
                Debug.Write(usage);
                Assert.Contains("UnitTest for testing all parameter types", usage);
                Assert.Contains("Version: 1.0", usage);
                Assert.Contains("Mandatory Parameters:", usage);
                Assert.Contains("Optional Parameters:", usage);
                Assert.Contains("Switch Parameters:", usage);
            }
        }

        /// <summary>
        /// Throws exception when parameter defined without short and long names
        /// </summary>
        [TestMethod]
        public void Parse_Parameter_Without_Name_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsMissingName>(["--num=9"]));
        }

        /// <summary>
        /// Throws exception when short parameter name contains whitespace
        /// </summary>
        [TestMethod]
        public void Parse_Short_Name_With_Whitespace_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsWhiteSpaceShortName>(["-s", "blah"]));
        }

        /// <summary>
        /// Throws exception when long parameter name contains whitespace
        /// </summary>
        [TestMethod]
        public void Parse_Long_Name_With_Whitespace_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsWhiteSpaceLongName>(["--server:blah"]));
        }

        /// <summary>
        /// Throws exception when short and long parameter names are identical
        /// </summary>
        [TestMethod]
        public void Parse_Identical_Short_And_Long_Names_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsIdenticalNames>(["--server:blah"]));
        }

        /// <summary>
        /// Throws exception parameter doesn't have description
        /// </summary>
        [TestMethod]
        public void Parse_Missing_Description_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsMissingDescription>(["--server:blah"]));
        }

        /// <summary>
        /// Throws exception when switch parameter attribute applied to multivalued property
        /// </summary>
        [TestMethod]
        public void Parse_Multivalued_Switch_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsMultivaluedSwitch>(["--server:blah"]));
        }

        /// <summary>
        /// Throws exception when a mandatory or optional parameter doesn't have meta-value
        /// </summary>
        [TestMethod]
        public void Parse_Missing_MetaValue_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsMissingMetaValue>(["--server:blah"]));
        }

        /// <summary>
        /// Throws exception when a positional parameter doesn't have long name
        /// </summary>
        [TestMethod]
        public void Parse_Positional_Without_Long_Name_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsPositionalWithoutLongName>(["--server:blah"]));
        }

        /// <summary>
        /// Throws exception when there's two parameters with the same short name
        /// </summary>
        [TestMethod]
        public void Parse_Duplicate_Short_Name_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsDuplicateShortParameterName>(["--title=foo", "--text=bar"]));
        }

        /// <summary>
        /// Throws exception when there's two parameters with the same long name
        /// </summary>
        [TestMethod]
        public void Parse_Duplicate_Long_Name_Throws_ParameterDefinitionException()
        {
            Assert.Throws<ParameterDefinitionException>(() => ProgramOptions.Parse<TestOptionsDuplicateLongParameterName>(["--title=foo", "--text=bar"]));
        }

        /// <summary>
        /// Throws exception When same parameter is specified twice on command line
        /// </summary>
        [TestMethod]
        public void Parse_Duplicate_Parameter_On_Cmdline_Throws_ParameterException()
        {
                ParameterException ex = Assert.Throws<ParameterException>(() => ProgramOptions.Parse<TestOptions>(["--server=a", "--port=389", "--username=joe", "/p", "389"]));
                Assert.AreEqual("port", ex.ParameterName);
        }

        private static void AssertParametersAreEqual(TestOptions expected, TestOptions actual)
        {
            Assert.AreEqual(expected.Server, actual.Server);
            Assert.AreEqual(expected.Port, actual.Port);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.LogLevel, actual.LogLevel);
            Assert.AreEqual(expected.Timeout, actual.Timeout);
            Assert.AreEqual(expected.Retries, actual.Retries);
            Assert.IsTrue(expected.HomeLocation.Equals(actual.HomeLocation));
            Assert.AreEqual(expected.Interval, actual.Interval);
            Assert.AreEqual(expected.Overload, actual.Overload);

            Assert.AreEqual(expected.MagicNumbers is null, actual.MagicNumbers is null);
            if (expected.MagicNumbers is not null)
            {
                Assert.IsNotNull(actual.MagicNumbers);
                Assert.HasCount(expected.MagicNumbers.Length, actual.MagicNumbers);
                for (int i = 0; i < expected.MagicNumbers.Length; ++i)
                {
                    Assert.AreEqual(expected.MagicNumbers[i], actual.MagicNumbers[i]);
                }
            }

            Assert.AreEqual(expected.Countries is null, actual.Countries is null);
            if (expected.Countries is not null)
            {
                Assert.IsNotNull(actual.Countries);
                Assert.HasCount(expected.Countries.Length, actual.Countries);
                for (int i = 0; i < expected.Countries.Length; ++i)
                {
                    Assert.AreEqual(expected.Countries[i], actual.Countries[i]);
                }
            }

            Assert.AreEqual(expected.Roles is null, actual.Roles is null);
            if (expected.Roles is not null)
            {
                Assert.IsNotNull(actual.Roles);
                Assert.HasCount(expected.Roles.Length, actual.Roles);
                for (int i = 0; i < expected.Roles.Length; ++i)
                {
                    Assert.AreEqual(expected.Roles[i], actual.Roles[i]);
                }
            }
        }
    }
}

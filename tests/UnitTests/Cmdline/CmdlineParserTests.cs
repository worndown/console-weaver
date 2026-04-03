namespace UnitTests.Cmdline
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver.Cmdline;

    /// <summary>
    /// CmdlineParser test suite
    /// </summary>
    [TestClass]
    public class CmdlineParserTests
    {
        /// <summary>
        /// Null cmdline arguments
        /// </summary>
        [TestMethod]
        public void Parse_Null_Arguments_Throws_ArgumentNullException()
        {
            List<Parameter> parameters =
            [
                new MandatoryParameter("t", "timeout", "VAL", "Test parameter", false, 0),
                new MandatoryParameter("d", "delay", "VAL", "Test parameter", true, -1)
            ];

            CmdlineParser parser = new CmdlineParser(parameters);
            Assert.Throws<ArgumentNullException>(() => parser.Parse(null!));
        }

        /// <summary>
        /// Duplicate short parameter name
        /// </summary>
        [TestMethod]
        public void Constructor_Duplicate_Short_Name_Throws_ParameterDefinitionException()
        {
            List<Parameter> parameters =
            [
                new MandatoryParameter("e", "enable", "VAL", "Test parameter", false, 0),
                new MandatoryParameter("e", "medium", "VAL", "Test parameter", true, -1)
            ];

            Assert.Throws<ParameterDefinitionException>(() => new CmdlineParser(parameters));
        }

        /// <summary>
        /// Duplicate long parameter name
        /// </summary>
        [TestMethod]
        public void Constructor_Duplicate_Long_Name_Throws_ParameterDefinitionException()
        {
            List<Parameter> parameters =
            [
                new MandatoryParameter("d", "enable", "VAL", "Test parameter", false, 0),
                new MandatoryParameter("e", "enable", "VAL", "Test parameter", true, -1)
            ];

            Assert.Throws<ParameterDefinitionException>(() => new CmdlineParser(parameters));
        }

        /// <summary>
        /// Short parameter name matches another parameter's long name
        /// </summary>
        [TestMethod]
        public void Constructor_Short_Name_Matches_Other_Long_Name_Throws_ParameterDefinitionException()
        {
            List<Parameter> parameters =
            [
                new MandatoryParameter("d", "enable", "VAL", "Test parameter 1", false, 0),
                new MandatoryParameter("e", "d", "VAL", "Test parameter 2", true, -1)
            ];

            Assert.Throws<ParameterDefinitionException>(() => new CmdlineParser(parameters));
        }

        /// <summary>
        /// Long parameter name matches another parameter's short name
        /// </summary>
        [TestMethod]
        public void Constructor_Long_Name_Matches_Other_Short_Name_Throws_ParameterDefinitionException()
        {
            List<Parameter> parameters =
            [
                new MandatoryParameter("d", "e", "VAL", "Test parameter 1", false, 0),
                new MandatoryParameter("e", "enable", "VAL", "Test parameter 2", true, -1)
            ];

            Assert.Throws<ParameterDefinitionException>(() => new CmdlineParser(parameters));
        }

        /// <summary>
        /// Specifying value for a switch parameter causes exception
        /// </summary>
        [TestMethod]
        public void Parse_Switch_With_Value_Throws_ParameterException()
        {
            List<Parameter> parameters =
            [
                new MandatoryParameter("t", "target", "VAL", "Test parameter", false, 0),
                new SwitchParameter("l", "log", "Switch parameter")
            ];

            CmdlineParser parser = new CmdlineParser(parameters);
            Assert.Throws<ParameterException>(() => parser.Parse(["--target=abcdef", "--log=true"]));
        }

        /// <summary>
        /// Cmdline parser iterator test
        /// </summary>
        [TestMethod]
        public void GetEnumerator_Iterates_All_Parameters()
        {
            MandatoryParameter m1 = new MandatoryParameter("m1", "mandatory1", "VAL", "Mandatory parameter", false, 0);
            MandatoryParameter m2 = new MandatoryParameter("m2", "mandatory2", "VAL", "Mandatory parameter", true, 1);
            OptionalParameter o1 = new OptionalParameter("o1", "optional1", "VAL", "Optional parameter", "10");
            OptionalParameter o2 = new OptionalParameter("o2", "optional2", "VAL", "Optional parameter", "10", true);
            SwitchParameter s1 = new SwitchParameter("s1", "switch1", "Switch parameter");
            SwitchParameter s2 = new SwitchParameter("s2", "switch2", "Switch parameter");

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { m1, m2, o1, o2, s1, s2 });

            HashSet<string> set = new HashSet<string>();
            foreach (Parameter parameter in parser)
            {
                Assert.IsNotNull(parameter.LongName);
                Assert.IsTrue(set.Add(parameter.LongName));
            }

            Assert.HasCount(6, set);
            set.Clear();

            IEnumerator enumerator = ((IEnumerable)parser).GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is Parameter p && p.LongName is not null)
                {
                    Assert.IsTrue(set.Add(p.LongName));
                }
            }

            if (enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Assert.HasCount(6, set);
        }

        /// <summary>
        /// Tests subscript/indexer operator
        /// </summary>
        [TestMethod]
        public void Indexer_Returns_Parameter_By_Name_Or_Reference()
        {
            MandatoryParameter m1 = new MandatoryParameter("m1", "mandatory1", "VAL", "Mandatory parameter", false, 0);
            MandatoryParameter m2 = new MandatoryParameter("m2", "mandatory2", "VAL", "Mandatory parameter", true, 1);
            OptionalParameter o1 = new OptionalParameter("o1", "optional1", "VAL", "Optional parameter", "10");
            OptionalParameter o2 = new OptionalParameter("o2", "optional2", "VAL", "Optional parameter", "10", true);
            SwitchParameter s1 = new SwitchParameter("s1", "switch1", "Switch parameter");
            SwitchParameter s2 = new SwitchParameter("s2", "switch2", "Switch parameter");

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { m1, m2, o1, o2, s1, s2 });

            Assert.IsTrue(object.ReferenceEquals(m1, parser["m1"]));
            Assert.IsTrue(object.ReferenceEquals(m1, parser["mandatory1"]));
            Assert.IsTrue(object.ReferenceEquals(m1, parser[m1]));

            Assert.IsTrue(object.ReferenceEquals(m2, parser["m2"]));
            Assert.IsTrue(object.ReferenceEquals(m2, parser["mandatory2"]));
            Assert.IsTrue(object.ReferenceEquals(m2, parser[m2]));

            Assert.IsTrue(object.ReferenceEquals(o1, parser["o1"]));
            Assert.IsTrue(object.ReferenceEquals(o1, parser["optional1"]));
            Assert.IsTrue(object.ReferenceEquals(o1, parser[o1]));

            Assert.IsTrue(object.ReferenceEquals(o2, parser["o2"]));
            Assert.IsTrue(object.ReferenceEquals(o2, parser["optional2"]));
            Assert.IsTrue(object.ReferenceEquals(o2, parser[o2]));

            Assert.IsTrue(object.ReferenceEquals(s1, parser["s1"]));
            Assert.IsTrue(object.ReferenceEquals(s1, parser["switch1"]));
            Assert.IsTrue(object.ReferenceEquals(s1, parser[s1]));

            Assert.IsTrue(object.ReferenceEquals(s2, parser["s2"]));
            Assert.IsTrue(object.ReferenceEquals(s2, parser["switch2"]));
            Assert.IsTrue(object.ReferenceEquals(s2, parser[s2]));
        }

        /// <summary>
        /// Throws exception when unknown parameter name is specified
        /// </summary>
        [TestMethod]
        public void HasValue_Unknown_Name_Throws_ParameterException()
        {
            MandatoryParameter p = new MandatoryParameter("p", "param", "VAL", "Mandatory parameter", false, -1);
            CmdlineParser parser = new CmdlineParser(new List<Parameter> { p });

            parser.Parse(["/p", "55"]);
            ParameterException ex = Assert.Throws<ParameterException>(() => parser.HasValue("unknown"));
            Assert.AreEqual("Unknown parameter name: unknown", ex.Message);
        }

        /// <summary>
        /// Throws exception when unknown parameter name is specified
        /// </summary>
        [TestMethod]
        [DataRow("--unknown=55", "Unknown long parameter name. Parameter: unknown.")]
        [DataRow("-u", "Unknown short parameter name. Parameter: u.")]
        public void Parse_Unknown_Parameter_Name_Throws_ParameterException(string arg, string expectedMessage)
        {
            MandatoryParameter p = new MandatoryParameter("p", "param", "VAL", "Mandatory parameter", false, -1);
            CmdlineParser parser = new CmdlineParser(new List<Parameter> { p });

            string[] args = arg.Contains('=') ? [arg] : [arg, "55"];
            ParameterException ex = Assert.Throws<ParameterException>(() => parser.Parse(args));
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        /// <summary>
        /// Throws exception when incorrect position index is specified in parameter definition
        /// </summary>
        [TestMethod]
        public void Constructor_Invalid_Position_Throws_ParameterException()
        {
            ParameterException ex = Assert.Throws<ParameterException>(() =>
            {
                CmdlineParser _ = new CmdlineParser(
                    new List<Parameter>
                    {
                        new MandatoryParameter("s", "server", "VAL", "Server name", false, 0),
                        new MandatoryParameter("p", "port", "VAL", "Port number", false, 2)
                    });
            });

            Assert.AreEqual("Invalid parameter position 2 (should be 1). Parameter: port.", ex.Message);

            ex = Assert.Throws<ParameterException>(() =>
            {
                CmdlineParser _ = new CmdlineParser(
                    new List<Parameter>
                    {
                        new MandatoryParameter("s", "server", "VAL", "Server name", false, 1)
                    });
            });

            Assert.AreEqual("Invalid parameter position 1 (should be 0). Parameter: server.", ex.Message);
        }

        /// <summary>
        /// Throws exception when unexpected argument (in neither short or long format) is encountered
        /// </summary>
        [TestMethod]
        public void Parse_Unexpected_Argument_Throws_ParameterException()
        {
            MandatoryParameter p = new MandatoryParameter("p", "param", "VAL", "Mandatory parameter", false, -1);
            CmdlineParser parser = new CmdlineParser(new List<Parameter> { p });

            ParameterException ex = Assert.Throws<ParameterException>(() => parser.Parse(["--param=55", "unexpected"]));
            Assert.Contains("Unknown command line argument: unexpected.", ex.Message);
        }

        /// <summary>
        /// Validates that parser correctly counts number of specified parameters, sets default values etc.
        /// </summary>
        [TestMethod]
        public void Parse_Valid_Arguments_Sets_Properties_Correctly()
        {
            MandatoryParameter m1 = new MandatoryParameter("m1", "mandatory1", "VAL", "Mandatory parameter", false, 0);
            MandatoryParameter m2 = new MandatoryParameter("m2", "mandatory2", "VAL", "Mandatory parameter", true, 1);
            OptionalParameter o1 = new OptionalParameter("o1", "optional1", "VAL", "Optional parameter", "10");
            OptionalParameter o2 = new OptionalParameter("o2", "optional2", "VAL", "Optional parameter", "10", true);
            SwitchParameter s1 = new SwitchParameter("s1", "switch1", "Switch parameter");
            SwitchParameter s2 = new SwitchParameter("s2", "switch2", "Switch parameter");

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { m1, m2, o1, o2, s1, s2 });
            parser.Parse(["--switch1", "111", "-o1", "5", "-m2", "222"]);

            Assert.AreEqual(4, parser.ParameterCount);
            Assert.AreEqual(2, parser.MandatoryParameterCount);
            Assert.AreEqual(2, parser.SwitchParameterCount);

            Assert.IsTrue(parser.HasValue("m1"));
            Assert.AreEqual("111", parser["m1"].RawValue);
            Assert.IsFalse(parser["m1"].HasDefaultValue);
            Assert.IsTrue(parser["m1"].HasShortName);
            Assert.IsTrue(parser["m1"].HasLongName);

            Assert.IsTrue(parser.HasValue("m2"));
            Assert.AreEqual("222", parser["m2"].RawValue);
            Assert.IsFalse(parser["m2"].HasDefaultValue);
            Assert.IsTrue(parser["m2"].HasShortName);
            Assert.IsTrue(parser["m2"].HasLongName);

            Assert.IsTrue(parser.HasValue("optional1"));
            Assert.AreEqual("5", parser["optional1"].RawValue);
            Assert.IsTrue(parser["optional1"].HasDefaultValue);
            Assert.AreEqual("10", parser["optional1"].DefaultValue);
            Assert.IsTrue(parser["optional1"].HasShortName);
            Assert.IsTrue(parser["optional1"].HasLongName);

            Assert.IsFalse(parser.HasValue("o2"));
            Assert.IsNull(parser["o2"].RawValue);
            Assert.IsTrue(parser["o2"].HasDefaultValue);
            Assert.AreEqual("10", parser["o2"].DefaultValue);
            Assert.IsTrue(parser["o2"].HasShortName);
            Assert.IsTrue(parser["o2"].HasLongName);

            Assert.IsTrue(parser[s1].HasValue);
            Assert.AreEqual(0, parser[s1].RawValue?.Length ?? -1); // because switch was specified on command line
            Assert.IsFalse(parser[s1].HasDefaultValue);
            Assert.IsTrue(parser[s1].HasShortName);
            Assert.IsTrue(parser[s1].HasLongName);

            Assert.IsFalse(parser["s2"].HasValue);
            Assert.IsNull(parser["s2"].RawValue); // because switch was not specified on command line
            Assert.IsFalse(parser["s2"].HasDefaultValue);
            Assert.IsTrue(parser["s2"].HasShortName);
            Assert.IsTrue(parser["s2"].HasLongName);
        }

        /// <summary>
        /// Tests helper method detecting identical names in two parameter definitions
        /// </summary>
        [TestMethod]
        public void GetIdenticalParameterName_Finds_Matching_Names()
        {
            MandatoryParameter m1 = new MandatoryParameter("m1", "mandatory1", "VAL", "Mandatory parameter", false, 0);
            MandatoryParameter m2 = new MandatoryParameter("m2", "mandatory2", "VAL", "Mandatory parameter", true, 1);
            MandatoryParameter m3 = new MandatoryParameter("m3", "M1", "VAL", "Mandatory parameter", true, 1);
            OptionalParameter o1 = new OptionalParameter("o1", "mandatory1", "VAL", "Optional parameter", "10");

            Assert.AreEqual(string.Empty, CmdlineParser.GetIdenticalParameterName(m1, m2));
            Assert.AreEqual("m1", CmdlineParser.GetIdenticalParameterName(m1, m3));
            Assert.AreEqual("mandatory1", CmdlineParser.GetIdenticalParameterName(m1, o1));
        }

        /// <summary>
        /// Tests that CanSwitchToParameterSet returns true when no parameters are bound yet.
        /// </summary>
        [TestMethod]
        public void CanSwitchToParameterSet_No_Bound_Parameters_Returns_True()
        {
            // Create parameters belonging to different parameter sets
            MandatoryParameter setAParam = new MandatoryParameter("a", "alpha", "VAL", "Set A parameter", false, 0);
            setAParam.SetParameterSets(["SetA"]);

            MandatoryParameter setBParam = new MandatoryParameter("b", "beta", "VAL", "Set B parameter", false, 0);
            setBParam.SetParameterSets(["SetB"]);

            // Shared parameter belongs to both sets
            OptionalParameter sharedParam = new OptionalParameter("s", "shared", "VAL", "Shared parameter", "default");

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { setAParam, setBParam, sharedParam });

            // Get the parameter sets
            ParameterSet setA = new ParameterSet("SetA");
            setA.Add(setAParam);
            setA.Add(sharedParam);

            ParameterSet setB = new ParameterSet("SetB");
            setB.Add(setBParam);
            setB.Add(sharedParam);

            // Before parsing, no parameters are bound, so should be able to switch to any set
            Assert.IsTrue(parser.CanSwitchToParameterSet(setA));
            Assert.IsTrue(parser.CanSwitchToParameterSet(setB));
        }

        /// <summary>
        /// Tests that CanSwitchToParameterSet returns true when all bound parameters are compatible.
        /// </summary>
        [TestMethod]
        public void CanSwitchToParameterSet_All_Compatible_Returns_True()
        {
            // Create parameter belonging to SetA only
            MandatoryParameter setAParam = new MandatoryParameter("a", "alpha", "VAL", "Set A parameter", false, 0);
            setAParam.SetParameterSets(["SetA"]);

            // Shared parameter belongs to both sets (default parameter set means all sets)
            OptionalParameter sharedParam = new OptionalParameter("s", "shared", "VAL", "Shared parameter", "default");

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { setAParam, sharedParam });

            // Parse only the shared parameter
            parser.Parse(["--shared=value"]);

            // Create target set that contains sharedParam
            ParameterSet targetSet = new ParameterSet("SetA");
            targetSet.Add(setAParam);
            targetSet.Add(sharedParam);

            // Should be able to switch since sharedParam (the only bound parameter) is part of SetA
            Assert.IsTrue(parser.CanSwitchToParameterSet(targetSet));
        }

        /// <summary>
        /// Tests that CanSwitchToParameterSet returns false when some bound parameters are incompatible.
        /// </summary>
        [TestMethod]
        public void CanSwitchToParameterSet_Incompatible_Parameters_Returns_False()
        {
            // Create parameter belonging to SetA only
            MandatoryParameter setAParam = new MandatoryParameter("a", "alpha", "VAL", "Set A parameter", false, 0);
            setAParam.SetParameterSets(["SetA"]);

            // Create parameter belonging to SetB only
            MandatoryParameter setBParam = new MandatoryParameter("b", "beta", "VAL", "Set B parameter", false, 0);
            setBParam.SetParameterSets(["SetB"]);

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { setAParam, setBParam });

            // Parse SetA's parameter
            parser.Parse(["--alpha=value"]);

            // Create SetB (which doesn't contain setAParam)
            ParameterSet setB = new ParameterSet("SetB");
            setB.Add(setBParam);

            // Should NOT be able to switch since setAParam (bound) is NOT part of SetB
            Assert.IsFalse(parser.CanSwitchToParameterSet(setB));
        }

        /// <summary>
        /// Tests that CanSwitchToParameterSet throws on null parameter set.
        /// </summary>
        [TestMethod]
        public void CanSwitchToParameterSet_Null_Throws_ArgumentNullException()
        {
            MandatoryParameter p = new MandatoryParameter("p", "param", "VAL", "Parameter", false, 0);
            CmdlineParser parser = new CmdlineParser(new List<Parameter> { p });

            Assert.Throws<ArgumentNullException>(() => parser.CanSwitchToParameterSet(null!));
        }

        /// <summary>
        /// Tests parameter set switching when current set has no more positional parameters
        /// but another compatible set does.
        /// </summary>
        [TestMethod]
        public void Parse_Switches_To_Compatible_Set_When_Current_Set_Exhausted()
        {
            // Shared positional parameter at position 0 (belongs to both SetA and SetB)
            // This won't lock in the parameter set since it belongs to multiple sets
            MandatoryParameter sharedPos0 = new MandatoryParameter("a", "alpha", "VAL", "Shared position 0", false, 0);
            sharedPos0.SetParameterSets(["SetA", "SetB"]);

            // SetB-only positional parameter at position 1
            MandatoryParameter setBPos1 = new MandatoryParameter("b", "beta", "VAL", "Set B position 1", false, 1);
            setBPos1.SetParameterSets(["SetB"]);

            // Shared switch (belongs to all sets via default)
            SwitchParameter sharedSwitch = new SwitchParameter("s", "shared", "Shared switch");

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { sharedPos0, setBPos1, sharedSwitch });

            // Parse: --shared switch, then two positional arguments
            // First positional goes to sharedPos0 (compatible with both sets)
            // SetA has no more positional params, but SetB has position 1
            // Parser should switch to SetB to accommodate the second positional
            parser.Parse(["--shared", "first", "second"]);

            // Both positional parameters should have values assigned
            Assert.AreEqual("SetB", parser.ParameterSetName);
            Assert.IsTrue(parser["alpha"].HasValue);
            Assert.AreEqual("first", parser["alpha"].RawValue);
            Assert.IsTrue(parser["beta"].HasValue);
            Assert.AreEqual("second", parser["beta"].RawValue);
        }

        /// <summary>
        /// Tests that parameter set switching does not occur when a set-specific parameter has been bound
        /// (which locks in the parameter set selection).
        /// </summary>
        [TestMethod]
        public void Parse_Does_Not_Switch_When_Set_Is_Locked()
        {
            // SetA: has 1 positional parameter
            MandatoryParameter setAPos0 = new MandatoryParameter("a", "alpha", "VAL", "Set A position 0", false, 0);
            setAPos0.SetParameterSets(["SetA"]);

            // SetB: has 2 positional parameters
            MandatoryParameter setBPos0 = new MandatoryParameter("b", "beta", "VAL", "Set B position 0", false, 0);
            setBPos0.SetParameterSets(["SetB"]);

            MandatoryParameter setBPos1 = new MandatoryParameter("c", "gamma", "VAL", "Set B position 1", false, 1);
            setBPos1.SetParameterSets(["SetB"]);

            CmdlineParser parser = new CmdlineParser(new List<Parameter> { setAPos0, setBPos0, setBPos1 });

            // Parse: --alpha=first locks us into SetA, then second positional has nowhere to go
            ParameterException ex = Assert.Throws<ParameterException>(() => parser.Parse(["--alpha=first", "second"]));
            Assert.Contains("Unknown command line argument: second.", ex.Message);
        }
    }
}

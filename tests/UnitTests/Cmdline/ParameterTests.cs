namespace UnitTests.Cmdline
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver.Cmdline;

    /// <summary>
    /// Cmdline parameter tests
    /// </summary>
    [TestClass]
    public class ParameterTests
    {
        /// <summary>
        /// Tests parameter's Object.ToString() implementation.
        /// </summary>
        [TestMethod]
        public void Parameter_ToString_Returns_Preferred_Name()
        {
            MandatoryParameter m1 = new MandatoryParameter("m1", "mandatory1", "VAL", "Mandatory parameter 1", false, 0);
            MandatoryParameter m2 = new MandatoryParameter("m2", null, "VAL", "Mandatory parameter 2", true, -1);
            MandatoryParameter m3 = new MandatoryParameter(null, "mandatory3", "VAL", "Mandatory parameter 3", true, 1);
            OptionalParameter o1 = new OptionalParameter("o1", "optional1", "VAL", "Optional parameter 1");
            OptionalParameter o2 = new OptionalParameter("o2", null, "VAL", "Optional parameter 2");
            OptionalParameter o3 = new OptionalParameter(null, "optional3", "VAL", "Optional parameter 3");
            SwitchParameter s1 = new SwitchParameter("s1", "switch1", "Switch parameter 1");
            SwitchParameter s2 = new SwitchParameter("s2", null, "Switch parameter 2");
            SwitchParameter s3 = new SwitchParameter(null, "switch3", "Switch parameter 3");

            Assert.AreEqual("mandatory1", m1.ToString());
            Assert.AreEqual("m2", m2.ToString());
            Assert.AreEqual("mandatory3", m3.ToString());
            Assert.AreEqual("optional1", o1.ToString());
            Assert.AreEqual("o2", o2.ToString());
            Assert.AreEqual("optional3", o3.ToString());
            Assert.AreEqual("switch1", s1.ToString());
            Assert.AreEqual("s2", s2.ToString());
            Assert.AreEqual("switch3", s3.ToString());
        }

        /// <summary>
        /// Tests IEquitable implementation.
        /// Parameters considered equal if their names (long or short) are the same (case-insensitive).
        /// </summary>
        [TestMethod]
        public void Parameter_Equals_Compares_Names_Case_Insensitive()
        {
            MandatoryParameter m1 = new MandatoryParameter("m1", "mandatory1", "VAL", "Mandatory parameter 1", false, 0);
            MandatoryParameter m2 = new MandatoryParameter("m2", "mandatory2", "VAL", "Mandatory parameter 2", true, 1);
            MandatoryParameter m3 = new MandatoryParameter("m1", "mandatory3", "VAL", "Mandatory parameter 3", true, 1);
            OptionalParameter o4 = new OptionalParameter("o1", "mandatory1", "VAL", "Optional parameter 4");
            OptionalParameter o5 = new OptionalParameter("o5", "m1", "VAL", "Optional parameter 5");
            MandatoryParameter m6 = new MandatoryParameter("m6", null, "VAL", "Mandatory parameter 6", false, -1);
            MandatoryParameter m7 = new MandatoryParameter(null, "m6", "VAL", "Mandatory parameter 7", false, -1);

            Assert.IsFalse(m1.Equals(null));
            Assert.IsFalse(m1.Equals(m2));
            Assert.IsFalse(m2.Equals(m1));
            Assert.IsFalse(m2.Equals(o4));
            Assert.IsFalse(o4.Equals(m2));
            Assert.IsTrue(m1.Equals(m3));
            Assert.IsTrue(m1.Equals(o4));
            Assert.IsTrue(m6.Equals(m7));
            Assert.IsTrue(m7.Equals(m6));
            Assert.IsTrue(m1.Equals(o5));
        }

        /// <summary>
        /// Tests IComparable implementation
        /// </summary>
        [TestMethod]
        public void Parameter_CompareTo_Sorts_By_Name()
        {
            MandatoryParameter m1 = new MandatoryParameter("m1", "mandatory1", "VAL", "Mandatory parameter 1", false, 0);
            MandatoryParameter m2 = new MandatoryParameter("m2", "mandatory2", "VAL", "Mandatory parameter 2", true, 1);
            MandatoryParameter m3 = new MandatoryParameter("m1", "mandatory3", "VAL", "Mandatory parameter 3", true, 1);
            MandatoryParameter m4 = new MandatoryParameter(null, "Mandatory3", "VAL", "Mandatory parameter 4", true, 1);
            OptionalParameter o1 = new OptionalParameter("mandatory3", null, "VAL", "Optional parameter 1");
            OptionalParameter o2 = new OptionalParameter("o2", null, "VAL", "Optional parameter 2");

            Assert.AreEqual(0, m1.CompareTo(m1));
            Assert.AreEqual(1, m1.CompareTo(null));
            Assert.AreEqual(-1, m1.CompareTo(m2));
            Assert.AreEqual(1, m2.CompareTo(m1));

            Assert.AreEqual(0, m1.CompareTo(m3)); // both have "m1" as short name
            Assert.AreEqual(0, m3.CompareTo(m1)); // both have "m1" as short name

            Assert.AreEqual(0, m3.CompareTo(m4)); // both have "mandatory3" as long name
            Assert.AreEqual(0, m4.CompareTo(m3)); // both have "mandatory3" as long name

            Assert.AreEqual(0, m4.CompareTo(o1)); // both have "mandatory3" as long name
            Assert.AreEqual(0, o1.CompareTo(m4)); // both have "mandatory3" as long name

            Assert.AreEqual(-2, m4.CompareTo(o2)); // "Mandatory2" < "o2"
            Assert.AreEqual(2, o2.CompareTo(m4));  // "o2" > "Mandatory2"
        }

        [TestMethod]
        public void Parameter_GetValue_Parses_Complex_Type_With_Parse_Method()
        {
            MandatoryParameter p = new MandatoryParameter("p", "param", "VAL", "My parameter", false, -1);
            p.SetValue("8:12");

            // This should use TestClass.Parse method
            TestClass? t = p.GetValue(typeof(TestClass)) as TestClass;
            Assert.IsNotNull(t);
            Assert.AreEqual(8, t.A);
            Assert.AreEqual(12, t.B);
        }

        [TestMethod]
        public void Parameter_GetValue_Parses_Multiple_Complex_Values_As_Array()
        {
            MandatoryParameter p = new MandatoryParameter("p", "param", "VAL", "My parameter", true, -1);
            p.SetValue("8:12, 9:14");

            // This should use TestClass.Parse method
            TestClass[]? t = p.GetValue(typeof(TestClass[])) as TestClass[];
            Assert.IsNotNull(t);
            Assert.AreEqual(8, t[0].A);
            Assert.AreEqual(12, t[0].B);
            Assert.AreEqual(9, t[1].A);
            Assert.AreEqual(14, t[1].B);
        }

        internal class TestClass
        {
            public TestClass(int a, int b)
            {
                this.A = a;
                this.B = b;
            }

            public int A { get; }
            public int B { get; }

            public static TestClass Parse(string str)
            {
                string[] parts = str.Split(':');
                if (parts.Length != 2)
                {
                    throw new FormatException();
                }

                return new TestClass(int.Parse(parts[0]), int.Parse(parts[1]));
            }
        }
    }
}

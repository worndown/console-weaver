namespace UnitTests.Utils
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver.Utils;

    [TestClass]
    public class CommaSeparatedValuesTests
    {
        [TestMethod]
        public void Parse_Empty_String_Returns_Empty_Array()
        {
            string[] result = CommaSeparatedValues.Parse("");
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void Parse_Single_Value_Returns_Array_With_One_Element()
        {
            string[] result = CommaSeparatedValues.Parse("value");
            Assert.HasCount(1, result);
            Assert.AreEqual("value", result[0]);
        }

        [TestMethod]
        public void Parse_Multiple_Values_Returns_Correct_Array()
        {
            string[] result = CommaSeparatedValues.Parse("one,two,three");
            Assert.HasCount(3, result);
            Assert.AreEqual("one", result[0]);
            Assert.AreEqual("two", result[1]);
            Assert.AreEqual("three", result[2]);
        }

        [TestMethod]
        public void Parse_Single_Comma_Returns_Two_Empty_Strings()
        {
            string[] result = CommaSeparatedValues.Parse(",");
            Assert.HasCount(2, result);
            Assert.AreEqual(string.Empty, result[0]);
            Assert.AreEqual(string.Empty, result[1]);
        }

        [TestMethod]
        public void Parse_Leading_Comma_Returns_Empty_First_Element()
        {
            string[] result = CommaSeparatedValues.Parse(",value");
            Assert.HasCount(2, result);
            Assert.AreEqual(string.Empty, result[0]);
            Assert.AreEqual("value", result[1]);
        }

        [TestMethod]
        public void Parse_Trailing_Comma_Returns_Empty_Last_Element()
        {
            string[] result = CommaSeparatedValues.Parse("value,");
            Assert.HasCount(2, result);
            Assert.AreEqual("value", result[0]);
            Assert.AreEqual(string.Empty, result[1]);
        }

        [TestMethod]
        public void Parse_Quoted_Value_With_Comma_Returns_Single_Element()
        {
            string[] result = CommaSeparatedValues.Parse("\"value,with,commas\"");
            Assert.HasCount(1, result);
            Assert.AreEqual("value,with,commas", result[0]);
        }

        [TestMethod]
        public void Parse_Multiple_Quoted_Values_Returns_Correct_Array()
        {
            string[] result = CommaSeparatedValues.Parse("\"value one\",\"value two\"");
            Assert.HasCount(2, result);
            Assert.AreEqual("value one", result[0]);
            Assert.AreEqual("value two", result[1]);
        }

        [TestMethod]
        public void Parse_Escaped_Quotes_Returns_Correct_Value()
        {
            string[] result = CommaSeparatedValues.Parse("\"value with \"\"quotes\"\"\"");
            Assert.HasCount(1, result);
            Assert.AreEqual("value with \"quotes\"", result[0]);
        }

        [TestMethod]
        public void Parse_Mixed_Quoted_And_Unquoted_Returns_Correct_Array()
        {
            string[] result = CommaSeparatedValues.Parse("simple,\"quoted,value\",another");
            Assert.HasCount(3, result);
            Assert.AreEqual("simple", result[0]);
            Assert.AreEqual("quoted,value", result[1]);
            Assert.AreEqual("another", result[2]);
        }

        [TestMethod]
        public void Parse_Quote_In_Unquoted_Field_Throws_FormatException()
        {
            Assert.Throws<FormatException>(() => CommaSeparatedValues.Parse("val\"ue"));
        }

        [TestMethod]
        public void Parse_Single_Quote_Throws_FormatException()
        {
            Assert.Throws<FormatException>(() => CommaSeparatedValues.Parse("\""));
        }

        [TestMethod]
        public void Parse_Missing_Closing_Quote_Throws_FormatException()
        {
            Assert.Throws<FormatException>(() => CommaSeparatedValues.Parse("\"unclosed"));
        }

        [TestMethod]
        public void Parse_Unescaped_Quote_In_Quoted_Field_Throws_FormatException()
        {
            Assert.Throws<FormatException>(() => CommaSeparatedValues.Parse("\"value\"extra\""));
        }

        [TestMethod]
        public void Format_Empty_Array_Returns_Empty_String()
        {
            string result = CommaSeparatedValues.Format();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Format_Single_Value_Returns_Value()
        {
            string result = CommaSeparatedValues.Format("value");
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Format_Multiple_Values_Returns_Comma_Separated()
        {
            string result = CommaSeparatedValues.Format("one", "two", "three");
            Assert.AreEqual("one,two,three", result);
        }

        [TestMethod]
        public void Format_Value_With_Comma_Quotes_Value()
        {
            string result = CommaSeparatedValues.Format("value,with,commas");
            Assert.AreEqual("\"value,with,commas\"", result);
        }

        [TestMethod]
        public void Format_Value_With_Newline_Quotes_Value()
        {
            string result = CommaSeparatedValues.Format("value\nwith\nnewlines");
            Assert.AreEqual("\"value\nwith\nnewlines\"", result);
        }

        [TestMethod]
        public void Format_Value_With_Quotes_Escapes_And_Quotes()
        {
            string result = CommaSeparatedValues.Format("value with \"quotes\"");
            Assert.AreEqual("\"value with \"\"quotes\"\"\"", result);
        }

        [TestMethod]
        public void Format_Null_Value_Returns_Empty()
        {
            string result = CommaSeparatedValues.Format(new object?[] { null });
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Format_Mixed_Values_Returns_Correct_Format()
        {
            // The third value has one embedded quote which needs escaping
            // CSV format: wrap in quotes and double any embedded quotes
            string result = CommaSeparatedValues.Format("simple", "value,comma", "value\"quote");
            // Expected: simple,"value,comma","value""quote"
            string expected = "simple,\"value,comma\",\"value\"\"quote\"";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Format_Integer_Values_Returns_String_Representation()
        {
            string result = CommaSeparatedValues.Format(1, 2, 3);
            Assert.AreEqual("1,2,3", result);
        }

        [TestMethod]
        public void Format_IEnumerable_Works_Correctly()
        {
            List<object> values = new List<object> { "one", "two", "three" };
            string result = CommaSeparatedValues.Format(values);
            Assert.AreEqual("one,two,three", result);
        }
    }
}

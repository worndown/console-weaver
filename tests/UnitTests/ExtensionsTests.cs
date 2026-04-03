namespace UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver;

    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void Truncate_String_Shorter_Than_Length_Returns_Original()
        {
            string input = "short";
            string result = input.Truncate(10);
            Assert.AreEqual("short", result);
        }

        [TestMethod]
        public void Truncate_String_Equal_To_Length_Returns_Original()
        {
            string input = "exact";
            string result = input.Truncate(5);
            Assert.AreEqual("exact", result);
        }

        [TestMethod]
        public void Truncate_String_Longer_Than_Length_With_Ellipsis_Returns_Truncated()
        {
            string input = "This is a very long string";
            string result = input.Truncate(10, ellipsis: true);
            Assert.AreEqual("This is...", result);
            Assert.AreEqual(10, result.Length);
        }

        [TestMethod]
        public void Truncate_String_Longer_Than_Length_Without_Ellipsis_Returns_Truncated()
        {
            string input = "This is a very long string";
            string result = input.Truncate(10, ellipsis: false);
            Assert.AreEqual("This is a ", result);
            Assert.AreEqual(10, result.Length);
        }

        [TestMethod]
        public void Truncate_With_Length_Too_Small_For_Ellipsis_Throws_ArgumentException()
        {
            string input = "test";
            Assert.Throws<ArgumentException>(() => input.Truncate(3, ellipsis: true));
        }

        [TestMethod]
        public void Truncate_Without_Ellipsis_Allows_Any_Positive_Length()
        {
            string input = "test";
            string result = input.Truncate(2, ellipsis: false);
            Assert.AreEqual("te", result);
        }

        [TestMethod]
        public void Truncate_With_Zero_Length_Without_Ellipsis_Throws_ArgumentException()
        {
            string input = "test";
            Assert.Throws<ArgumentException>(() => input.Truncate(0, ellipsis: false));
        }
    }
}

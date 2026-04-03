namespace UnitTests.Utils
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver;
    using ConsoleWeaver.Utils;

    [TestClass]
    public class TableColumnDefinitionTests
    {
        [TestMethod]
        public void Constructor_Sets_Properties_Correctly()
        {
            TableColumnDefinition definition = new TableColumnDefinition("Foo");
            Assert.AreEqual("Foo", definition.Name);
            Assert.IsNull(definition.Format);
            Assert.AreEqual(int.MaxValue, definition.MaxWidth);
            Assert.AreEqual(3, definition.MaxValueLength);
            Assert.AreEqual(TableColumnDefinition.DefaultAlignment, definition.Alignment);

            definition = new TableColumnDefinition("Foo", "N0", Alignment.Center, 32);
            definition.UpdateMaxValueLength(256);
            Assert.AreEqual("Foo", definition.Name);
            Assert.AreEqual("N0", definition.Format);
            Assert.AreEqual(32, definition.MaxWidth);
            Assert.AreEqual(256, definition.MaxValueLength);
            Assert.AreEqual(Alignment.Center, definition.Alignment);
        }

        [TestMethod]
        public void Constructor_Width_Too_Small_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new TableColumnDefinition("Foo", null, Alignment.Center, 2);
            });
        }
    }
}

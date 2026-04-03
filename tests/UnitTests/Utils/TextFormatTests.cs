namespace UnitTests.Utils
{
    using System;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver;

    [TestClass]
    public class TextFormatTests
    {
        [TestMethod]
        [DataRow("fox", "  fox", 5, Alignment.Right)]
        [DataRow("fox", "fox  ", 5, Alignment.Left)]
        [DataRow("fox", " fox ", 5, Alignment.Center)]
        [DataRow("fox", "fox", 2, Alignment.Center)]
        public void AlignText_Aligns_Text_With_Padding(string input, string expected, int width, Alignment alignment)
        {
            string result = TextFormat.AlignText(input, alignment, width);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void WriteTextBox_Draws_Box_With_Alignment()
        {
            StringBuilder sb = new StringBuilder();

            string expected = $"+---+{Environment.NewLine}|fox|{Environment.NewLine}+---+";
            TextFormat.WriteTextBox(sb, ["fox"], Alignment.Center, 5);
            Assert.AreEqual(expected, sb.ToString());
            sb.Clear();

            expected = $"+-----+{Environment.NewLine}| fox |{Environment.NewLine}+-----+";
            TextFormat.WriteTextBox(sb, ["fox"], Alignment.Center, 7);
            Assert.AreEqual(expected, sb.ToString());
            sb.Clear();

            expected = $"+-----+{Environment.NewLine}|fox  |{Environment.NewLine}+-----+";
            TextFormat.WriteTextBox(sb, new[] { "fox" }, Alignment.Left, 7);
            Assert.AreEqual(expected, sb.ToString());
            sb.Clear();

            expected = $"+-----+{Environment.NewLine}|  fox|{Environment.NewLine}+-----+";
            TextFormat.WriteTextBox(sb, ["fox"], Alignment.Right, 7);
            Assert.AreEqual(expected, sb.ToString());
            sb.Clear();

            expected = $"+---------+{Environment.NewLine}|  crazy  |{Environment.NewLine}|   fox   |{Environment.NewLine}+---------+";
            TextFormat.WriteTextBox(sb, ["crazy", "fox"], Alignment.Center, 11);
            Assert.AreEqual(expected, sb.ToString());
            sb.Clear();

            expected = $"+---------+{Environment.NewLine}|crazy    |{Environment.NewLine}|fox      |{Environment.NewLine}+---------+";
            TextFormat.WriteTextBox(sb, ["crazy", "fox"], Alignment.Left, 11);
            Assert.AreEqual(expected, sb.ToString());
            sb.Clear();

            expected = $"+---------+{Environment.NewLine}|    crazy|{Environment.NewLine}|      fox|{Environment.NewLine}+---------+";
            TextFormat.WriteTextBox(sb, ["crazy", "fox"], Alignment.Right, 11);
            Assert.AreEqual(expected, sb.ToString());
            sb.Clear();
        }
    }
}

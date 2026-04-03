namespace UnitTests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ConsoleWeaver;
    using ConsoleWeaver.Utils;

    [TestClass]
    public class TableTests
    {
        private static readonly string TestTable =
            "+------------------------------------------------------------------+" + Environment.NewLine +
            "|                            Test table                            |" + Environment.NewLine +
            "+----------------------+------------+------------+-----------------+" + Environment.NewLine +
            "| Date                 |      Count |  Interest  | Notes           |" + Environment.NewLine +
            "+----------------------+------------+------------+-----------------+" + Environment.NewLine +
            "| 1969-04-15 20:20:20Z | 123,456.00 | 1,234.5679 | first bank l... |" + Environment.NewLine +
            "| 1969-04-15 20:20:20Z |  12,346.00 |  123.5679  | second bank ... |" + Environment.NewLine +
            "| 1969-04-15 20:20:20Z |   1,234.00 |  12.5600   | third bank l... |" + Environment.NewLine +
            "+----------------------+------------+------------+-----------------+" + Environment.NewLine +
            "| 1969-04-15 20:20:20Z |      12.00 |   1.5679   | fifth bank l... |" + Environment.NewLine +
            "| 1969-04-15 20:20:20Z | 135,568.00 |  245.5679  | sixth bank l... |" + Environment.NewLine +
            "| 1969-04-15 20:20:20Z | 146,679.00 | 3,456.5679 | seventh bank... |" + Environment.NewLine +
            "| 1969-04-15 20:20:20Z | 146,679.00 | 3,456.5679 | <null>          |" + Environment.NewLine +
            "| aaa                  |        bbb |    ccc     | ddd             |" + Environment.NewLine +
            "| True                 |    -128.00 |   0.0001   | Sunday          |" + Environment.NewLine +
            "+----------------------+------------+------------+-----------------+" + Environment.NewLine +
            "* Time is in UTC                                                    " + Environment.NewLine +
            "* For internal use only                                             " + Environment.NewLine;

        [TestMethod]
        public void FormatTable_Generates_Correct_Output()
        {
            Table table = new Table("Test table");
            table.NumericUseGroupSeparator = true;
            table.FloatingNumberPrecision = 4;
            table.TitleAlignment = Alignment.Center;
            table.Foreground = ConsoleColor.Gray;
            table.Background = ConsoleColor.DarkBlue;

            table.AddColumnDefinition("Date", "u", 80);
            table.AddColumnDefinition("Count", "N2", Alignment.Right);
            table.AddColumnDefinition("Interest", Alignment.Center);
            table.AddColumnDefinition("Notes", 15);
            table.Footnotes.Add("* Time is in UTC");
            table.Footnotes.Add("* For internal use only");

            DateTime dateTime = new DateTime(1969, 4, 15, 20, 20, 20);
            table.AddRow([dateTime, 123456, 1234.567890123, "first bank location"]);
            table.AddRow([dateTime, 12346, 123.567890123, "second bank location"]);
            table.AddRow([dateTime, 1234, 12.56, "third bank location"]);
            table.AddRowSeparator();
            table.AddRow([dateTime, 12, 1.567890123, "fifth bank location"]);
            table.AddRow([dateTime, 135568, 245.567890123, "sixth bank location"]);
            table.AddRow([dateTime, 146679, 3456.567890123, "seventh bank location"]);
            table.AddRow([dateTime, 146679, 3456.567890123, null]);
            table.AddRow(new List<string> { "aaa", "bbb", "ccc", "ddd" });
            table.AddRow(new List<object> { true, -128, 0.0001233333, DayOfWeek.Sunday });

            StringBuilder result = table.FormatTable();
            Assert.AreEqual(TestTable, result.ToString());
        }
    }
}

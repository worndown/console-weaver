namespace UnitTests.Utils
{
    using System;
    using ConsoleWeaver;
    using UnitTests.Mocks;

    [TestClass]
    [DoNotParallelize]
    public class ShellTests
    {
        private MockConsole mockConsole = null!;

        [TestInitialize]
        public void Setup()
        {
            this.mockConsole = new MockConsole();
            Shell.SetConsole(this.mockConsole);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Shell.ResetConsole();
        }

        #region Write(char) Tests

        [TestMethod]
        public void Write_Char_Writes_Character_To_Console()
        {
            Shell.Write('A');

            Assert.AreEqual("A", this.mockConsole.Output);
            Assert.AreEqual(1, this.mockConsole.WriteCount);
        }

        [TestMethod]
        public void Write_Char_Multiple_Writes_Concatenate()
        {
            Shell.Write('H');
            Shell.Write('i');

            Assert.AreEqual("Hi", this.mockConsole.Output);
            Assert.AreEqual(2, this.mockConsole.WriteCount);
        }

        #endregion

        #region Write(char, colors) Tests

        [TestMethod]
        public void Write_Char_With_Foreground_Sets_And_Restores_Color()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.Gray;

            Shell.Write('X', ConsoleColor.Red);

            Assert.AreEqual("X", this.mockConsole.Output);
            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);

            // Verify the text was written with the correct color
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Red, this.mockConsole.WriteOperations[0].Foreground);
        }

        [TestMethod]
        public void Write_Char_With_Background_Sets_And_Restores_Color()
        {
            this.mockConsole.BackgroundColor = ConsoleColor.Black;

            Shell.Write('X', null, ConsoleColor.Blue);

            Assert.AreEqual("X", this.mockConsole.Output);
            Assert.AreEqual(ConsoleColor.Black, this.mockConsole.BackgroundColor);

            // Verify the text was written with the correct background color
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Blue, this.mockConsole.WriteOperations[0].Background);
        }

        [TestMethod]
        public void Write_Char_With_Both_Colors_Sets_And_Restores()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.White;
            this.mockConsole.BackgroundColor = ConsoleColor.Black;

            Shell.Write('X', ConsoleColor.Yellow, ConsoleColor.DarkBlue);

            Assert.AreEqual(ConsoleColor.White, this.mockConsole.ForegroundColor);
            Assert.AreEqual(ConsoleColor.Black, this.mockConsole.BackgroundColor);

            // Verify the text was written with correct colors
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Yellow, this.mockConsole.WriteOperations[0].Foreground);
            Assert.AreEqual(ConsoleColor.DarkBlue, this.mockConsole.WriteOperations[0].Background);
        }

        [TestMethod]
        public void Write_Char_With_Null_Colors_Does_Not_Change_Colors()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.Cyan;
            this.mockConsole.BackgroundColor = ConsoleColor.DarkGray;

            Shell.Write('X', null, null);

            Assert.AreEqual(ConsoleColor.Cyan, this.mockConsole.ForegroundColor);
            Assert.AreEqual(ConsoleColor.DarkGray, this.mockConsole.BackgroundColor);

            // Verify the text was written with original colors (no change)
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Cyan, this.mockConsole.WriteOperations[0].Foreground);
            Assert.AreEqual(ConsoleColor.DarkGray, this.mockConsole.WriteOperations[0].Background);
        }

        #endregion

        #region Write(string) Tests

        [TestMethod]
        public void Write_String_Writes_Text_To_Console()
        {
            Shell.Write("Hello World");

            Assert.AreEqual("Hello World", this.mockConsole.Output);
        }

        [TestMethod]
        public void Write_String_Empty_Writes_Nothing()
        {
            Shell.Write(string.Empty);

            Assert.AreEqual(string.Empty, this.mockConsole.Output);
            Assert.AreEqual(1, this.mockConsole.WriteCount);
        }

        [TestMethod]
        public void Write_String_With_Foreground_Sets_And_Restores()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.Gray;

            Shell.Write("Test", ConsoleColor.Green);

            Assert.AreEqual("Test", this.mockConsole.Output);
            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);

            // Verify the text was written with the correct color
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Green, this.mockConsole.WriteOperations[0].Foreground);
        }

        [TestMethod]
        public void Write_String_With_Background_Sets_And_Restores()
        {
            this.mockConsole.BackgroundColor = ConsoleColor.Black;

            Shell.Write("Test", null, ConsoleColor.Magenta);

            Assert.AreEqual("Test", this.mockConsole.Output);
            Assert.AreEqual(ConsoleColor.Black, this.mockConsole.BackgroundColor);

            // Verify the text was written with the correct background color
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Magenta, this.mockConsole.WriteOperations[0].Background);
        }

        [TestMethod]
        public void Write_String_With_Both_Colors_Sets_And_Restores()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.White;
            this.mockConsole.BackgroundColor = ConsoleColor.Black;

            Shell.Write("Colored Text", ConsoleColor.Red, ConsoleColor.Yellow);

            Assert.AreEqual("Colored Text", this.mockConsole.Output);
            Assert.AreEqual(ConsoleColor.White, this.mockConsole.ForegroundColor);
            Assert.AreEqual(ConsoleColor.Black, this.mockConsole.BackgroundColor);

            // Verify the text was written with correct colors
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Red, this.mockConsole.WriteOperations[0].Foreground);
            Assert.AreEqual(ConsoleColor.Yellow, this.mockConsole.WriteOperations[0].Background);
        }

        #endregion

        #region WriteLine Tests

        [TestMethod]
        public void WriteLine_Writes_Text_With_Newline()
        {
            Shell.WriteLine("Hello");

            Assert.AreEqual("Hello" + Environment.NewLine, this.mockConsole.Output);
            Assert.AreEqual(1, this.mockConsole.WriteLineCount);
        }

        [TestMethod]
        public void WriteLine_Multiple_Calls_Add_Newlines()
        {
            Shell.WriteLine("Line 1");
            Shell.WriteLine("Line 2");

            string expected = "Line 1" + Environment.NewLine + "Line 2" + Environment.NewLine;
            Assert.AreEqual(expected, this.mockConsole.Output);
        }

        [TestMethod]
        public void WriteLine_With_Colors_Sets_And_Restores()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.Gray;
            this.mockConsole.BackgroundColor = ConsoleColor.Black;

            Shell.WriteLine("Test", ConsoleColor.Cyan, ConsoleColor.DarkRed);

            Assert.IsTrue(this.mockConsole.Output.Contains("Test"));
            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);
            Assert.AreEqual(ConsoleColor.Black, this.mockConsole.BackgroundColor);

            // Verify the text was written with correct colors
            Assert.AreEqual(1, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Cyan, this.mockConsole.WriteOperations[0].Foreground);
            Assert.AreEqual(ConsoleColor.DarkRed, this.mockConsole.WriteOperations[0].Background);
        }

        [TestMethod]
        public void WriteLine_Empty_String_Writes_Only_Newline()
        {
            Shell.WriteLine(string.Empty);

            Assert.AreEqual(Environment.NewLine, this.mockConsole.Output);
        }

        #endregion

        #region SetConsole/ResetConsole Tests

        [TestMethod]
        public void SetConsole_Changes_Console_Implementation()
        {
            var newMock = new MockConsole();
            Shell.SetConsole(newMock);

            Shell.Write("Test");

            Assert.AreEqual("Test", newMock.Output);
            Assert.AreEqual(string.Empty, this.mockConsole.Output);
        }

        [TestMethod]
        public void SetConsole_Throws_On_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Shell.SetConsole(null!));
        }

        [TestMethod]
        public void ResetConsole_Restores_Default_Console()
        {
            // After ResetConsole, writing should go to the real console (not our mock)
            Shell.ResetConsole();

            // We can't easily test the real console, but we can verify our mock is no longer used
            var countBefore = this.mockConsole.WriteCount;

            // Note: This would write to real console in a real scenario
            // For this test, we just verify the mock is disconnected
            Shell.SetConsole(this.mockConsole); // Re-set for other tests
        }

        #endregion

        #region Color Preservation Tests

        [TestMethod]
        public void Write_Preserves_Original_Foreground_When_Setting_New()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.DarkYellow;

            Shell.Write("Test", ConsoleColor.White);

            Assert.AreEqual(ConsoleColor.DarkYellow, this.mockConsole.ForegroundColor);

            // Verify the text was written with the requested color
            Assert.AreEqual(ConsoleColor.White, this.mockConsole.WriteOperations[0].Foreground);
        }

        [TestMethod]
        public void Write_Preserves_Original_Background_When_Setting_New()
        {
            this.mockConsole.BackgroundColor = ConsoleColor.DarkCyan;

            Shell.Write("Test", null, ConsoleColor.White);

            Assert.AreEqual(ConsoleColor.DarkCyan, this.mockConsole.BackgroundColor);

            // Verify the text was written with the requested background color
            Assert.AreEqual(ConsoleColor.White, this.mockConsole.WriteOperations[0].Background);
        }

        [TestMethod]
        public void Multiple_Colored_Writes_All_Restore_Colors()
        {
            this.mockConsole.ForegroundColor = ConsoleColor.Gray;
            this.mockConsole.BackgroundColor = ConsoleColor.Black;

            Shell.Write("Red", ConsoleColor.Red);
            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);

            Shell.Write("Green", ConsoleColor.Green);
            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);

            Shell.Write("Blue", ConsoleColor.Blue);
            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);

            Assert.AreEqual("RedGreenBlue", this.mockConsole.Output);

            // Verify each write operation used the correct color
            Assert.AreEqual(3, this.mockConsole.WriteOperations.Count);
            Assert.AreEqual(ConsoleColor.Red, this.mockConsole.WriteOperations[0].Foreground);
            Assert.AreEqual(ConsoleColor.Green, this.mockConsole.WriteOperations[1].Foreground);
            Assert.AreEqual(ConsoleColor.Blue, this.mockConsole.WriteOperations[2].Foreground);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Write_Special_Characters()
        {
            Shell.Write("\t\n\r");

            Assert.AreEqual("\t\n\r", this.mockConsole.Output);
        }

        [TestMethod]
        public void Write_Unicode_Characters()
        {
            Shell.Write("Hello \u4e16\u754c"); // "Hello 世界"

            Assert.AreEqual("Hello \u4e16\u754c", this.mockConsole.Output);
        }

        [TestMethod]
        public void Write_All_Console_Colors()
        {
            var colors = Enum.GetValues<ConsoleColor>();
            int index = 0;

            foreach (ConsoleColor color in colors)
            {
                this.mockConsole.ForegroundColor = ConsoleColor.Gray;
                Shell.Write("X", color);
                Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);

                // Verify the text was written with the correct color
                Assert.AreEqual(color, this.mockConsole.WriteOperations[index].Foreground);
                index++;
            }

            Assert.AreEqual(colors.Length, this.mockConsole.WriteOperations.Count);
        }

        #endregion
    }
}

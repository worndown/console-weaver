namespace UnitTests.Utils
{
    using ConsoleWeaver;
    using ConsoleWeaver.Utils;
    using System;
    using UnitTests.Mocks;

    [TestClass]
    [DoNotParallelize]
    public class PromptTests
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

        #region ResponseOptions Tests

        [TestMethod]
        public void ResponseOptions_YesNo_Contains_Yes_And_No()
        {
            ResponseOptions options = ResponseOptions.YesNo;
            Assert.AreEqual(ResponseOptions.Yes, options & ResponseOptions.Yes);
            Assert.AreEqual(ResponseOptions.No, options & ResponseOptions.No);
            Assert.AreNotEqual(ResponseOptions.Cancel, options & ResponseOptions.Cancel);
        }

        [TestMethod]
        public void ResponseOptions_YesNoCancel_Contains_Yes_No_And_Cancel()
        {
            ResponseOptions options = ResponseOptions.YesNoCancel;
            Assert.AreEqual(ResponseOptions.Yes, options & ResponseOptions.Yes);
            Assert.AreEqual(ResponseOptions.No, options & ResponseOptions.No);
            Assert.AreEqual(ResponseOptions.Cancel, options & ResponseOptions.Cancel);
        }

        [TestMethod]
        public void ResponseOptions_All_Contains_All_Options()
        {
            ResponseOptions options = ResponseOptions.All;
            Assert.AreEqual(ResponseOptions.Yes, options & ResponseOptions.Yes);
            Assert.AreEqual(ResponseOptions.YesToAll, options & ResponseOptions.YesToAll);
            Assert.AreEqual(ResponseOptions.No, options & ResponseOptions.No);
            Assert.AreEqual(ResponseOptions.NoToAll, options & ResponseOptions.NoToAll);
            Assert.AreEqual(ResponseOptions.Cancel, options & ResponseOptions.Cancel);
        }

        [TestMethod]
        public void Response_Enum_Values_Match_ResponseOptions()
        {
            Assert.AreEqual((int)ResponseOptions.Yes, (int)Response.Yes);
            Assert.AreEqual((int)ResponseOptions.YesToAll, (int)Response.YesToAll);
            Assert.AreEqual((int)ResponseOptions.No, (int)Response.No);
            Assert.AreEqual((int)ResponseOptions.NoToAll, (int)Response.NoToAll);
            Assert.AreEqual((int)ResponseOptions.Cancel, (int)Response.Cancel);
        }

        #endregion

        #region GetChoice Tests

        [TestMethod]
        public void GetChoice_Returns_Selected_Option()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "Option A", "Option B", "Option C" };
            this.mockConsole.QueueReadLine("2");

            string result = prompt.GetChoiceCore("Select:", choices);

            Assert.AreEqual("Option B", result);
        }

        [TestMethod]
        public void GetChoice_Displays_All_Options()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "First", "Second", "Third" };
            this.mockConsole.QueueReadLine("1");

            prompt.GetChoiceCore("Select:", choices);

            Assert.IsTrue(this.mockConsole.Output.Contains("First"));
            Assert.IsTrue(this.mockConsole.Output.Contains("Second"));
            Assert.IsTrue(this.mockConsole.Output.Contains("Third"));
        }

        [TestMethod]
        public void GetChoice_Shows_Prompt_Text()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "A", "B" };
            this.mockConsole.QueueReadLine("1");

            prompt.GetChoiceCore("Please choose:", choices);

            Assert.IsTrue(this.mockConsole.Output.Contains("Please choose:"));
        }

        [TestMethod]
        public void GetChoice_Retries_On_Invalid_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "A", "B" };
            this.mockConsole.QueueReadLines("invalid", "0", "3", "1"); // First 3 are invalid

            string result = prompt.GetChoiceCore("Select:", choices);

            Assert.AreEqual("A", result);
            Assert.AreEqual(4, this.mockConsole.ReadLineCount);
        }

        [TestMethod]
        public void GetChoice_Returns_Last_Option()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "First", "Last" };
            this.mockConsole.QueueReadLine("2");

            string result = prompt.GetChoiceCore("Select:", choices);

            Assert.AreEqual("Last", result);
        }

        [TestMethod]
        public void GetChoice_Throws_On_Null_Choices()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Prompt.GetChoice<string>("Select:", null!));
        }

        [TestMethod]
        public void GetChoice_Throws_On_Duplicate_Choices()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "Same", "Same" };

            Assert.Throws<ArgumentException>(() =>
                prompt.GetChoiceCore("Select:", choices));
        }

        [TestMethod]
        public void GetChoice_Handles_Case_Insensitive_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "A", "B" };
            this.mockConsole.QueueReadLine("  1  "); // With whitespace

            string result = prompt.GetChoiceCore("Select:", choices);

            Assert.AreEqual("A", result);
        }

        #endregion

        #region GetResponse Tests

        [TestMethod]
        public void GetResponse_Returns_Yes_On_Y_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("Y");

            Response result = prompt.GetResponseCore("Continue?", Response.No, ResponseOptions.YesNo);

            Assert.AreEqual(Response.Yes, result);
        }

        [TestMethod]
        public void GetResponse_Returns_Yes_On_YES_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("YES");

            Response result = prompt.GetResponseCore("Continue?", Response.No, ResponseOptions.YesNo);

            Assert.AreEqual(Response.Yes, result);
        }

        [TestMethod]
        public void GetResponse_Returns_No_On_N_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("N");

            Response result = prompt.GetResponseCore("Continue?", Response.Yes, ResponseOptions.YesNo);

            Assert.AreEqual(Response.No, result);
        }

        [TestMethod]
        public void GetResponse_Returns_No_On_NO_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("NO");

            Response result = prompt.GetResponseCore("Continue?", Response.Yes, ResponseOptions.YesNo);

            Assert.AreEqual(Response.No, result);
        }

        [TestMethod]
        public void GetResponse_Returns_Default_On_Empty_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("");

            Response result = prompt.GetResponseCore("Continue?", Response.Yes, ResponseOptions.YesNo);

            Assert.AreEqual(Response.Yes, result);
        }

        [TestMethod]
        public void GetResponse_Returns_Default_On_Null_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine(null);

            Response result = prompt.GetResponseCore("Continue?", Response.No, ResponseOptions.YesNo);

            Assert.AreEqual(Response.No, result);
        }

        [TestMethod]
        public void GetResponse_Returns_Cancel_On_C_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("C");

            Response result = prompt.GetResponseCore("Continue?", Response.Yes, ResponseOptions.YesNoCancel);

            Assert.AreEqual(Response.Cancel, result);
        }

        [TestMethod]
        public void GetResponse_Returns_YesToAll_On_A_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("A");

            Response result = prompt.GetResponseCore("Continue?", Response.Yes, ResponseOptions.All);

            Assert.AreEqual(Response.YesToAll, result);
        }

        [TestMethod]
        public void GetResponse_Returns_NoToAll_On_L_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("L");

            Response result = prompt.GetResponseCore("Continue?", Response.Yes, ResponseOptions.All);

            Assert.AreEqual(Response.NoToAll, result);
        }

        [TestMethod]
        public void GetResponse_Case_Insensitive()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("y");

            Response result = prompt.GetResponseCore("Continue?", Response.No, ResponseOptions.YesNo);

            Assert.AreEqual(Response.Yes, result);
        }

        [TestMethod]
        public void GetResponse_Retries_On_Invalid_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLines("X", "invalid", "Y");

            Response result = prompt.GetResponseCore("Continue?", Response.No, ResponseOptions.YesNo);

            Assert.AreEqual(Response.Yes, result);
            Assert.AreEqual(3, this.mockConsole.ReadLineCount);
        }

        [TestMethod]
        public void GetResponse_Throws_When_Default_Not_In_Options()
        {
            Assert.Throws<ArgumentException>(() =>
                Prompt.GetResponse("Continue?", Response.Cancel, ResponseOptions.YesNo));
        }

        [TestMethod]
        public void GetResponse_Throws_On_Empty_Prompt()
        {
            Assert.Throws<ArgumentException>(() =>
                Prompt.GetResponse("", Response.Yes, ResponseOptions.Yes));
        }

        [TestMethod]
        public void GetResponse_Throws_On_Null_Prompt()
        {
            Assert.Throws<ArgumentException>(() =>
                Prompt.GetResponse(null!, Response.Yes, ResponseOptions.Yes));
        }

        [TestMethod]
        public void GetResponse_Shows_Prompt_Text()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("Y");

            prompt.GetResponseCore("Do you want to continue?", Response.Yes, ResponseOptions.YesNo);

            Assert.IsTrue(this.mockConsole.Output.Contains("Do you want to continue?"));
        }

        #endregion

        #region GetString Tests

        [TestMethod]
        public void GetString_Returns_User_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("Hello World");

            string? result = prompt.GetStringCore("Enter text");

            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void GetString_Returns_Default_On_Empty_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("");

            string? result = prompt.GetStringCore("Enter text", "default value");

            Assert.AreEqual("default value", result);
        }

        [TestMethod]
        public void GetString_Returns_Default_On_Null_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine(null);

            string? result = prompt.GetStringCore("Enter text", "default");

            Assert.AreEqual("default", result);
        }

        [TestMethod]
        public void GetString_Returns_Null_When_No_Default_And_Empty_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("");

            string? result = prompt.GetStringCore("Enter text");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetString_Shows_Prompt_With_Default()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("test");

            prompt.GetStringCore("Enter name", "John");

            Assert.IsTrue(this.mockConsole.Output.Contains("Enter name"));
            Assert.IsTrue(this.mockConsole.Output.Contains("John"));
        }

        [TestMethod]
        public void GetString_Returns_Trimmed_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("  spaces  ");

            string? result = prompt.GetStringCore("Enter text");

            // Note: GetString doesn't trim by default
            Assert.AreEqual("  spaces  ", result);
        }

        #endregion

        #region GetInteger Tests

        [TestMethod]
        public void GetInteger_Returns_Parsed_Integer()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("42");

            int result = prompt.GetIntegerCore("Enter number");

            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void GetInteger_Returns_Negative_Integer()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("-100");

            int result = prompt.GetIntegerCore("Enter number");

            Assert.AreEqual(-100, result);
        }

        [TestMethod]
        public void GetInteger_Returns_Default_On_Empty_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("");

            int result = prompt.GetIntegerCore("Enter number", 99);

            Assert.AreEqual(99, result);
        }

        [TestMethod]
        public void GetInteger_Retries_On_Invalid_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLines("abc", "12.5", "42");

            int result = prompt.GetIntegerCore("Enter number");

            Assert.AreEqual(42, result);
            Assert.AreEqual(3, this.mockConsole.ReadLineCount);
        }

        [TestMethod]
        public void GetInteger_Shows_Error_On_Invalid_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLines("invalid", "42");

            prompt.GetIntegerCore("Enter number");

            Assert.IsTrue(this.mockConsole.Output.Contains("Invalid integer"));
        }

        [TestMethod]
        public void GetInteger_Shows_Default_In_Prompt()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("10");

            prompt.GetIntegerCore("Enter number", 5);

            Assert.IsTrue(this.mockConsole.Output.Contains("5"));
        }

        [TestMethod]
        public void GetInteger_Handles_Zero()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("0");

            int result = prompt.GetIntegerCore("Enter number");

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetInteger_Retries_On_Empty_Without_Default()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLines("", "", "5");

            int result = prompt.GetIntegerCore("Enter number");

            Assert.AreEqual(5, result);
            Assert.AreEqual(3, this.mockConsole.ReadLineCount);
        }

        #endregion

        #region GetPassword Tests

        [TestMethod]
        public void GetPassword_Returns_Entered_Password()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueCharKey('p');
            this.mockConsole.QueueCharKey('a');
            this.mockConsole.QueueCharKey('s');
            this.mockConsole.QueueCharKey('s');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            string result = prompt.GetPasswordCore("Password");

            Assert.AreEqual("pass", result);
        }

        [TestMethod]
        public void GetPassword_Masks_Input_With_Asterisks()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueCharKey('a');
            this.mockConsole.QueueCharKey('b');
            this.mockConsole.QueueCharKey('c');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            prompt.GetPasswordCore("Password");

            // Should have written 3 asterisks
            Assert.IsTrue(this.mockConsole.Output.Contains("***"));
        }

        [TestMethod]
        public void GetPassword_Handles_Backspace()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueCharKey('a');
            this.mockConsole.QueueCharKey('b');
            this.mockConsole.QueueReadKey(ConsoleKey.Backspace);
            this.mockConsole.QueueCharKey('c');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            string result = prompt.GetPasswordCore("Password");

            Assert.AreEqual("ac", result);
        }

        [TestMethod]
        public void GetPassword_Handles_Escape_Clears_Input()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueCharKey('a');
            this.mockConsole.QueueCharKey('b');
            this.mockConsole.QueueCharKey('c');
            this.mockConsole.QueueReadKey(ConsoleKey.Escape);
            this.mockConsole.QueueCharKey('x');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            string result = prompt.GetPasswordCore("Password");

            Assert.AreEqual("x", result);
        }

        [TestMethod]
        public void GetPassword_Returns_Empty_On_Immediate_Enter()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            string result = prompt.GetPasswordCore("Password");

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void GetPassword_Shows_Prompt()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            prompt.GetPasswordCore("Enter your password");

            Assert.IsTrue(this.mockConsole.Output.Contains("Enter your password"));
        }

        [TestMethod]
        public void GetPassword_Ignores_Alt_Modifier()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadKey(ConsoleKey.A, 'a', alt: true);
            this.mockConsole.QueueCharKey('b');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            string result = prompt.GetPasswordCore("Password");

            Assert.AreEqual("b", result); // Alt+A should be ignored
        }

        [TestMethod]
        public void GetPassword_Ignores_Control_Modifier()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadKey(ConsoleKey.A, 'a', control: true);
            this.mockConsole.QueueCharKey('x');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            string result = prompt.GetPasswordCore("Password");

            Assert.AreEqual("x", result); // Ctrl+A should be ignored
        }

        [TestMethod]
        public void GetPassword_Backspace_At_Start_Does_Nothing()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadKey(ConsoleKey.Backspace);
            this.mockConsole.QueueReadKey(ConsoleKey.Backspace);
            this.mockConsole.QueueCharKey('a');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            string result = prompt.GetPasswordCore("Password");

            Assert.AreEqual("a", result);
        }

        #endregion

        #region GetPasswordSecure Tests

        [TestMethod]
        public void GetPasswordSecure_Returns_SecureString()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueCharKey('s');
            this.mockConsole.QueueCharKey('e');
            this.mockConsole.QueueCharKey('c');
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            var result = prompt.GetPasswordSecureCore("Password");

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result.IsReadOnly());
        }

        [TestMethod]
        public void GetPasswordSecure_Handles_Backspace()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueCharKey('a');
            this.mockConsole.QueueCharKey('b');
            this.mockConsole.QueueReadKey(ConsoleKey.Backspace);
            this.mockConsole.QueueReadKey(ConsoleKey.Enter);

            var result = prompt.GetPasswordSecureCore("Password");

            Assert.AreEqual(1, result.Length);
        }

        #endregion

        #region Prompt Instance Tests

        [TestMethod]
        public void Prompt_Constructor_Accepts_IConsole()
        {
            var console = new MockConsole();
            var prompt = new Prompt(console);

            console.QueueReadLine("test");
            string? result = prompt.GetStringCore("Enter:");

            Assert.AreEqual("test", result);
        }

        [TestMethod]
        public void Prompt_Constructor_Throws_On_Null_Console()
        {
            Assert.Throws<ArgumentNullException>(() => new Prompt(null!));
        }

        [TestMethod]
        public void Prompt_Default_Constructor_Uses_Shell_Console()
        {
            // This test verifies the default constructor works
            var prompt = new Prompt();
            this.mockConsole.QueueReadLine("test");

            string? result = prompt.GetStringCore("Enter:");

            Assert.AreEqual("test", result);
        }

        #endregion

        #region Color Tests

        [TestMethod]
        public void GetChoice_Respects_Prompt_Colors()
        {
            var prompt = new Prompt(this.mockConsole);
            var choices = new[] { "A" };
            this.mockConsole.QueueReadLine("1");
            this.mockConsole.ForegroundColor = ConsoleColor.Gray;

            prompt.GetChoiceCore("Select:", choices, ConsoleColor.Red, ConsoleColor.Blue);

            // Colors should be restored after the call
            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);
        }

        [TestMethod]
        public void GetString_Respects_Prompt_Colors()
        {
            var prompt = new Prompt(this.mockConsole);
            this.mockConsole.QueueReadLine("test");
            this.mockConsole.ForegroundColor = ConsoleColor.Gray;

            prompt.GetStringCore("Enter:", null, ConsoleColor.Yellow);

            Assert.AreEqual(ConsoleColor.Gray, this.mockConsole.ForegroundColor);
        }

        #endregion
    }
}

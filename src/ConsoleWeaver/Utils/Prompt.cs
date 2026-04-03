namespace ConsoleWeaver.Utils
{
    using ConsoleWeaver;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security;
    using System.Text;

    /// <summary>
    /// Response options.
    /// </summary>
    [Flags]
    public enum ResponseOptions
    {
        Yes = 1,
        YesToAll = 2,
        No = 4,
        NoToAll = 8,
        Cancel = 16,
        YesNo = Yes | No,
        YesNoCancel = YesNo | Cancel,
        All = YesNoCancel | YesToAll | NoToAll
    }

    /// <summary>
    /// Console responses.
    /// </summary>
    public enum Response
    {
        Yes = ResponseOptions.Yes,
        YesToAll = ResponseOptions.YesToAll,
        No = ResponseOptions.No,
        NoToAll = ResponseOptions.NoToAll,
        Cancel = ResponseOptions.Cancel,
    }

    /// <summary>
    /// Console input helper.
    /// </summary>
    public class Prompt
    {
        private const char Whitespace = ' ';
        private const char Backspace = '\b';

        private static readonly Dictionary<Response, Tuple<string, string>> ResponsePrompt = new Dictionary<Response, Tuple<string, string>>
        {
            { Response.Yes, new Tuple<string, string>("[Y] Yes", "Y") },
            { Response.YesToAll, new Tuple<string, string>("[A] Yes to All", "A") },
            { Response.No, new Tuple<string, string>("[N] No", "N") },
            { Response.NoToAll, new Tuple<string, string>("[L] No to All", "L") },
            { Response.Cancel, new Tuple<string, string>("[C] Cancel", "C") }
        };

        private readonly IConsole console;

        /// <summary>
        /// Initializes a new instance of the Prompt class using the default console.
        /// </summary>
        public Prompt()
            : this(Shell.Console)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Prompt class.
        /// </summary>
        /// <param name="console">Console implementation to use.</param>
        public Prompt(IConsole console)
        {
            this.console = Precondition.CheckNotNull(console, nameof(console));
        }

        /// <summary>
        /// Displays prompt and list of options to select from.
        /// </summary>
        /// <typeparam name="T">Option type.</typeparam>
        /// <param name="prompt">Prompt to display.</param>
        /// <param name="choice">List of options to select from.</param>
        /// <param name="promptForeground">Prompt foreground.</param>
        /// <param name="promptBackground">Prompt background.</param>
        /// <returns>Selected option.</returns>
        public static T GetChoice<T>(
            string prompt,
            IEnumerable<T> choice,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            return new Prompt().GetChoiceCore(prompt, choice, promptForeground, promptBackground);
        }

        /// <summary>
        /// Displays prompt and list of options to select from.
        /// </summary>
        /// <typeparam name="T">Option type.</typeparam>
        /// <param name="prompt">Prompt to display.</param>
        /// <param name="choice">List of options to select from.</param>
        /// <param name="promptForeground">Prompt foreground.</param>
        /// <param name="promptBackground">Prompt background.</param>
        /// <returns>Selected option.</returns>
        public T GetChoiceCore<T>(
            string prompt,
            IEnumerable<T> choice,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            int optionCount = 0;
            Dictionary<int, T> choiceTable = new Dictionary<int, T>();
            foreach (T option in Precondition.CheckNotNull(choice, nameof(choice)))
            {
                if (choiceTable.ContainsValue(option))
                {
                    throw new ArgumentException($"Duplicate choice option '{option}'.", nameof(choice));
                }

                choiceTable.Add(++optionCount, option);
            }

            int width = (int)Math.Floor(Math.Log10(optionCount)) + 1;

            Shell.WriteLine(prompt, promptForeground, promptBackground);
            foreach (KeyValuePair<int, T> pair in choiceTable)
            {
                Shell.WriteLine($" [{pair.Key.ToString(CultureInfo.InvariantCulture).PadLeft(width)}] {pair.Value}");
            }

            do
            {
                Shell.Write($"Select one (1-{optionCount}):", promptForeground, promptBackground);
                Shell.Write(Whitespace);

                string? response;
                using (new CursorState(this.console, true))
                {
                    response = this.console.ReadLine()?.ToUpper(CultureInfo.CurrentCulture).Trim();
                }

                if (!string.IsNullOrEmpty(response) &&
                    int.TryParse(response, out int key) &&
                    choiceTable.TryGetValue(key, out var selection))
                {
                    return selection;
                }
            }
            while (true);
        }

        /// <summary>
        /// Gets "yes", "no", "yes to all", "no to all", "cancel" response from user.
        /// </summary>
        /// <param name="prompt">Prompt to display.</param>
        /// <param name="defaultResponse">Default response option.</param>
        /// <param name="responseOptions">Response options available to user.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>Selected response.</returns>
        public static Response GetResponse(
            string prompt,
            Response defaultResponse,
            ResponseOptions responseOptions,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            return new Prompt().GetResponseCore(prompt, defaultResponse, responseOptions, promptForeground, promptBackground);
        }

        /// <summary>
        /// Gets "yes", "no", "yes to all", "no to all", "cancel" response from user.
        /// </summary>
        /// <param name="prompt">Prompt to display.</param>
        /// <param name="defaultResponse">Default response option.</param>
        /// <param name="responseOptions">Response options available to user.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>Selected response.</returns>
        public Response GetResponseCore(
            string prompt,
            Response defaultResponse,
            ResponseOptions responseOptions,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            Precondition.CheckNotNullOrEmpty(prompt, nameof(prompt));
            if ((responseOptions & (ResponseOptions)defaultResponse) != (ResponseOptions)defaultResponse)
            {
                throw new ArgumentException($"Response options don't contain default response '{defaultResponse}'.", nameof(responseOptions));
            }

            Shell.WriteLine(prompt, promptForeground, promptBackground);

            do
            {
                WriteResponseOptions(responseOptions, defaultResponse);

                string? response;
                using (new CursorState(this.console, true))
                {
                    response = this.console.ReadLine()?.ToUpper().Trim();
                }

                if (string.IsNullOrEmpty(response))
                {
                    return defaultResponse;
                }

                if ((responseOptions & ResponseOptions.Yes) == ResponseOptions.Yes &&
                    (response == ResponsePrompt[Response.Yes].Item2 || response == "YES"))
                {
                    return Response.Yes;
                }

                if ((responseOptions & ResponseOptions.YesToAll) == ResponseOptions.YesToAll &&
                    response == ResponsePrompt[Response.YesToAll].Item2)
                {
                    return Response.YesToAll;
                }

                if ((responseOptions & ResponseOptions.No) == ResponseOptions.No &&
                    (response == ResponsePrompt[Response.No].Item2 || response == "NO"))
                {
                    return Response.No;
                }

                if ((responseOptions & ResponseOptions.NoToAll) == ResponseOptions.NoToAll &&
                    response == ResponsePrompt[Response.NoToAll].Item2)
                {
                    return Response.NoToAll;
                }

                if ((responseOptions & ResponseOptions.Cancel) == ResponseOptions.Cancel &&
                    response == ResponsePrompt[Response.Cancel].Item2)
                {
                    return Response.Cancel;
                }
            }
            while (true);
        }

        /// <summary>
        /// Gets a string response from console after displaying specified prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="defaultValue">Default response to use if used hits enter.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>String response provided by user.</returns>
        public static string? GetString(
            string prompt,
            string? defaultValue = null,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            return new Prompt().GetStringCore(prompt, defaultValue, promptForeground, promptBackground);
        }

        /// <summary>
        /// Gets a string response from console after displaying specified prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="defaultValue">Default response to use if used hits enter.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>String response provided by user.</returns>
        public string? GetStringCore(
            string prompt,
            string? defaultValue = null,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            prompt = defaultValue != null ? $"{prompt} (default is \"{defaultValue}\"):" : $"{prompt}:";
            Shell.Write(prompt, promptForeground, promptBackground);
            Shell.Write(Whitespace);

            string? response;
            using (new CursorState(this.console, true))
            {
                response = this.console.ReadLine();
            }

            return !string.IsNullOrEmpty(response) ? response : defaultValue;
        }

        /// <summary>
        /// Gets an integer response from the console after displaying specified prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="defaultResponse">The default response value.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>Integer value entered in console.</returns>
        public static int GetInteger(
            string prompt,
            int? defaultResponse = null,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            return new Prompt().GetIntegerCore(prompt, defaultResponse, promptForeground, promptBackground);
        }

        /// <summary>
        /// Gets an integer response from the console after displaying specified prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="defaultResponse">The default response value.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>Integer value entered in console.</returns>
        public int GetIntegerCore(
            string prompt,
            int? defaultResponse = null,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            do
            {
                Shell.Write(
                    defaultResponse.HasValue
                        ? $"{prompt} (default is \"{defaultResponse.Value}\"):"
                        : $"{prompt}:",
                    promptForeground,
                    promptBackground);
                Shell.Write(Whitespace);

                string? response;
                using (new CursorState(this.console, true))
                {
                    response = this.console.ReadLine()?.Trim();
                }

                if (string.IsNullOrEmpty(response))
                {
                    if (defaultResponse.HasValue)
                    {
                        return defaultResponse.Value;
                    }

                    continue;
                }

                if (int.TryParse(response, out int result))
                {
                    return result;
                }

                Shell.WriteLine($"Invalid integer value '{response}'.");
            }
            while (true);
        }

        /// <summary>
        /// Gets a response that contains a password.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>The password in plaintext.</returns>
        public static string GetPassword(
            string prompt,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            return new Prompt().GetPasswordCore(prompt, promptForeground, promptBackground);
        }

        /// <summary>
        /// Gets a response that contains a password.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>The password in plaintext.</returns>
        public string GetPasswordCore(
            string prompt,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            StringBuilder resp = new StringBuilder();

            foreach (char key in this.ReadPasswordLine(prompt, promptForeground, promptBackground))
            {
                switch (key)
                {
                    case Backspace:
                        resp.Remove(resp.Length - 1, 1);
                        break;
                    default:
                        resp.Append(key);
                        break;
                }
            }

            return resp.ToString();
        }

        /// <summary>
        /// Gets a response that contains a password.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>The password as secure string.</returns>
        public static SecureString GetPasswordSecure(
            string prompt,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            return new Prompt().GetPasswordSecureCore(prompt, promptForeground, promptBackground);
        }

        /// <summary>
        /// Gets a response that contains a password.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>The password as secure string.</returns>
        public SecureString GetPasswordSecureCore(
            string prompt,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            SecureString secureString = new SecureString();

            foreach (char key in this.ReadPasswordLine(prompt, promptForeground, promptBackground))
            {
                switch (key)
                {
                    case Backspace:
                        secureString.RemoveAt(secureString.Length - 1);
                        break;
                    default:
                        secureString.AppendChar(key);
                        break;
                }
            }

            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Prompts the user for a password and yields each key as the user inputs.
        /// The input is masked so the password is not shown in the console.
        /// </summary>
        /// <param name="prompt">The prompt to display. A whitespace is added to the supplied prompt.</param>
        /// <param name="promptForeground">Prompt foreground color.</param>
        /// <param name="promptBackground">Prompt background color.</param>
        /// <returns>A stream of characters as input by the user including Backspace for deletions.</returns>
        private IEnumerable<char> ReadPasswordLine(
            string prompt,
            ConsoleColor? promptForeground = null,
            ConsoleColor? promptBackground = null)
        {
            const string whiteOut = "\b \b";
            Shell.Write($"{prompt}:", promptForeground, promptBackground);
            Shell.Write(Whitespace);
            const ConsoleModifiers consoleModifiers = ConsoleModifiers.Alt | ConsoleModifiers.Control;
            ConsoleKeyInfo keyInfo;
            int readChars = 0;

            do
            {
                using (new CursorState(this.console, true))
                {
                    // Intercept key so it is not displayed in console
                    keyInfo = this.console.ReadKey(true);
                }

                if ((keyInfo.Modifiers & consoleModifiers) != 0)
                {
                    continue;
                }

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        this.console.WriteLine();
                        break;
                    case ConsoleKey.Backspace:
                        if (readChars > 0)
                        {
                            this.console.Write(whiteOut);
                            --readChars;
                            yield return Backspace;
                        }
                        break;
                    case ConsoleKey.Escape:
                        // Reset the password
                        while (readChars > 0)
                        {
                            this.console.Write(whiteOut);
                            yield return Backspace;
                            --readChars;
                        }
                        break;
                    default:
                        readChars++;
                        this.console.Write('*');
                        yield return keyInfo.KeyChar;
                        break;
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);
        }

        /// <summary>
        /// Writes response options prompt to standard output.
        /// </summary>
        /// <param name="responseOptions">Response options.</param>
        /// <param name="defaultResponse">Default response.</param>
        private static void WriteResponseOptions(ResponseOptions responseOptions, Response defaultResponse)
        {
            foreach (object value in Enum.GetValues(typeof(ResponseOptions)))
            {
                ResponseOptions option = (ResponseOptions)value;
                Response response = (Response)option;

                if ((responseOptions & option) == option)
                {
                    if (response == defaultResponse)
                    {
                        Shell.Write(ResponsePrompt[response].Item1, ConsoleColor.Yellow);
                        Shell.Write(' ');
                    }
                    else if (ResponsePrompt.ContainsKey(response))
                    {
                        Shell.Write(ResponsePrompt[response].Item1);
                        Shell.Write(Whitespace);
                    }
                }
            }

            Shell.Write($"(default is \"{ResponsePrompt[defaultResponse].Item2}\"): ");
        }

        /// <summary>
        /// Changes cursor visibility and restores original state upon disposal.
        /// Currently only works on Windows - noop on other platforms.
        /// </summary>
        private readonly struct CursorState : IDisposable
        {
            private readonly IConsole console;
            private readonly bool original;
            private readonly bool restore;

            public CursorState(IConsole console, bool visible = true)
            {
                this.console = console;

                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        original = console.CursorVisible;
                    }
                    catch
                    {
                        // System.PlatformNotSupportedException is possible.
                        // Assuming the cursor was visible.
                        original = true;
                    }

                    restore = visible != original;
                    if (restore)
                    {
                        TrySetVisible(console, visible);
                    }
                }
                else
                {
                    // TODO: Add non-Windows support
                    original = false;
                    restore = false;
                }
            }

            private static void TrySetVisible(IConsole console, bool visible)
            {
                try
                {
                    console.CursorVisible = visible;
                }
                catch
                {
                    // Setting cursor may fail if output is piped or permission is denied.
                }
            }

            public void Dispose()
            {
                if (restore)
                {
                    TrySetVisible(this.console, original);
                }
            }
        }
    }
}

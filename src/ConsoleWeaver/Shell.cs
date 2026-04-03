namespace ConsoleWeaver
{
    using System;

    /// <summary>
    /// Console output helper.
    /// </summary>
    public static class Shell
    {
        private static IConsole console = SystemConsole.Instance;

        /// <summary>
        /// Gets the current console instance.
        /// </summary>
        internal static IConsole Console => console;

        /// <summary>
        /// Sets the console implementation to use. Primarily for testing.
        /// </summary>
        /// <param name="newConsole">The console implementation to use.</param>
        public static void SetConsole(IConsole newConsole)
        {
            console = Precondition.CheckNotNull(newConsole, nameof(newConsole));
        }

        /// <summary>
        /// Resets the console to the default system console.
        /// </summary>
        public static void ResetConsole()
        {
            console = SystemConsole.Instance;
        }

        /// <summary>
        /// Writes character to standard output.
        /// </summary>
        /// <param name="ch">Character to write.</param>
        public static void Write(char ch)
        {
            console.Write(ch);
        }

        /// <summary>
        /// Writes character to standard output using specified colors.
        /// </summary>
        /// <param name="ch">Character to write.</param>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        public static void Write(char ch, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            using (ConsoleSettings settings = new ConsoleSettings(console, foreground, background))
            {
                console.Write(ch);
            }
        }

        /// <summary>
        /// Writes text to console.
        /// </summary>
        /// <param name="value">Text to write.</param>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        public static void Write(string value, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            using (ConsoleSettings settings = new ConsoleSettings(console, foreground, background))
            {
                console.Write(value);
            }
        }

        /// <summary>
        /// Writes text to console.
        /// </summary>
        /// <param name="value">Text to write.</param>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        public static void WriteLine(string value, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            using (ConsoleSettings settings = new ConsoleSettings(console, foreground, background))
            {
                console.WriteLine(value);
            }
        }

        /// <summary>
        /// Sets and restores console colors.
        /// </summary>
        private readonly struct ConsoleSettings : IDisposable
        {
            private readonly IConsole console;
            private readonly ConsoleColor? originalForeground;
            private readonly ConsoleColor? originalBackground;

            public ConsoleSettings(IConsole console, ConsoleColor? foreground, ConsoleColor? background)
            {
                this.console = console;

                if (foreground.HasValue)
                {
                    this.originalForeground = console.ForegroundColor;
                    console.ForegroundColor = foreground.Value;
                }
                else
                {
                    this.originalForeground = null;
                }

                if (background.HasValue)
                {
                    this.originalBackground = console.BackgroundColor;
                    console.BackgroundColor = background.Value;
                }
                else
                {
                    this.originalBackground = null;
                }
            }

            public void Dispose()
            {
                if (this.originalForeground.HasValue)
                {
                    this.console.ForegroundColor = this.originalForeground.Value;
                }

                if (this.originalBackground.HasValue)
                {
                    this.console.BackgroundColor = this.originalBackground.Value;
                }
            }
        }
    }
}

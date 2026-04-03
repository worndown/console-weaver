namespace ConsoleWeaver
{
    using System;

    /// <summary>
    /// Default implementation of <see cref="IConsole"/> that wraps <see cref="System.Console"/>.
    /// </summary>
    public sealed class SystemConsole : IConsole
    {
        /// <summary>
        /// Singleton instance of the system console.
        /// </summary>
        public static readonly SystemConsole Instance = new SystemConsole();

        /// <summary>
        /// Prevents external instantiation.
        /// </summary>
        private SystemConsole()
        {
        }

        /// <inheritdoc/>
        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        /// <inheritdoc/>
        public ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        /// <inheritdoc/>
        public bool CursorVisible
        {
            get
            {
                if (OperatingSystem.IsWindows())
                {
                    return Console.CursorVisible;
                }

                return true; // Default for non-Windows platforms
            }
            set
            {
                if (OperatingSystem.IsWindows())
                {
                    Console.CursorVisible = value;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsOutputRedirected => Console.IsOutputRedirected;

        /// <inheritdoc/>
        public void Write(char value)
        {
            Console.Write(value);
        }

        /// <inheritdoc/>
        public void Write(string? value)
        {
            Console.Write(value);
        }

        /// <inheritdoc/>
        public void WriteLine()
        {
            Console.WriteLine();
        }

        /// <inheritdoc/>
        public void WriteLine(string? value)
        {
            Console.WriteLine(value);
        }

        /// <inheritdoc/>
        public string? ReadLine()
        {
            return Console.ReadLine();
        }

        /// <inheritdoc/>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }
    }
}

namespace UnitTests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ConsoleWeaver;

    /// <summary>
    /// Mock console implementation for testing.
    /// Captures all output and allows simulating user input.
    /// </summary>
    public class MockConsole : IConsole
    {
        private readonly Queue<string?> readLineResponses = new Queue<string?>();
        private readonly Queue<ConsoleKeyInfo> readKeyResponses = new Queue<ConsoleKeyInfo>();
        private readonly StringBuilder outputBuilder = new StringBuilder();
        private readonly List<WriteOperation> writeOperations = new List<WriteOperation>();
        private readonly List<ConsoleColor> foregroundColorChanges = new List<ConsoleColor>();
        private readonly List<ConsoleColor> backgroundColorChanges = new List<ConsoleColor>();

        private ConsoleColor foregroundColor = ConsoleColor.Gray;
        private ConsoleColor backgroundColor = ConsoleColor.Black;

        /// <summary>
        /// Gets all output written to the console.
        /// </summary>
        public string Output => this.outputBuilder.ToString();

        /// <summary>
        /// Gets the list of write operations with their associated colors.
        /// </summary>
        public IReadOnlyList<WriteOperation> WriteOperations => this.writeOperations;

        /// <summary>
        /// Gets the history of foreground color changes in order.
        /// </summary>
        public IReadOnlyList<ConsoleColor> ForegroundColorChanges => this.foregroundColorChanges;

        /// <summary>
        /// Gets the history of background color changes in order.
        /// </summary>
        public IReadOnlyList<ConsoleColor> BackgroundColorChanges => this.backgroundColorChanges;

        /// <summary>
        /// Gets the number of times Write was called.
        /// </summary>
        public int WriteCount { get; private set; }

        /// <summary>
        /// Gets the number of times WriteLine was called.
        /// </summary>
        public int WriteLineCount { get; private set; }

        /// <summary>
        /// Gets the number of times ReadLine was called.
        /// </summary>
        public int ReadLineCount { get; private set; }

        /// <summary>
        /// Gets the number of times ReadKey was called.
        /// </summary>
        public int ReadKeyCount { get; private set; }

        /// <inheritdoc/>
        public ConsoleColor ForegroundColor
        {
            get => this.foregroundColor;
            set
            {
                this.foregroundColorChanges.Add(value);
                this.foregroundColor = value;
            }
        }

        /// <inheritdoc/>
        public ConsoleColor BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                this.backgroundColorChanges.Add(value);
                this.backgroundColor = value;
            }
        }

        /// <inheritdoc/>
        public bool CursorVisible { get; set; } = true;

        /// <inheritdoc/>
        public bool IsOutputRedirected { get; set; }

        /// <summary>
        /// Queues a response for the next ReadLine call.
        /// </summary>
        public void QueueReadLine(string? response)
        {
            this.readLineResponses.Enqueue(response);
        }

        /// <summary>
        /// Queues multiple responses for ReadLine calls.
        /// </summary>
        public void QueueReadLines(params string?[] responses)
        {
            foreach (var response in responses)
            {
                this.readLineResponses.Enqueue(response);
            }
        }

        /// <summary>
        /// Queues a key response for the next ReadKey call.
        /// </summary>
        public void QueueReadKey(ConsoleKey key, char keyChar = '\0', bool shift = false, bool alt = false, bool control = false)
        {
            this.readKeyResponses.Enqueue(new ConsoleKeyInfo(keyChar == '\0' ? (char)key : keyChar, key, shift, alt, control));
        }

        /// <summary>
        /// Queues a character key response.
        /// </summary>
        public void QueueCharKey(char c)
        {
            this.readKeyResponses.Enqueue(new ConsoleKeyInfo(c, (ConsoleKey)char.ToUpper(c), false, false, false));
        }

        /// <summary>
        /// Clears all captured output and resets counters.
        /// </summary>
        public void Clear()
        {
            this.outputBuilder.Clear();
            this.writeOperations.Clear();
            this.foregroundColorChanges.Clear();
            this.backgroundColorChanges.Clear();
            this.WriteCount = 0;
            this.WriteLineCount = 0;
            this.ReadLineCount = 0;
            this.ReadKeyCount = 0;
        }

        /// <inheritdoc/>
        public void Write(char value)
        {
            this.WriteCount++;
            this.outputBuilder.Append(value);
            this.writeOperations.Add(new WriteOperation(value.ToString(), this.foregroundColor, this.backgroundColor));
        }

        /// <inheritdoc/>
        public void Write(string? value)
        {
            this.WriteCount++;
            this.outputBuilder.Append(value);
            this.writeOperations.Add(new WriteOperation(value ?? string.Empty, this.foregroundColor, this.backgroundColor));
        }

        /// <inheritdoc/>
        public void WriteLine()
        {
            this.WriteLineCount++;
            this.outputBuilder.AppendLine();
            this.writeOperations.Add(new WriteOperation(Environment.NewLine, this.foregroundColor, this.backgroundColor));
        }

        /// <inheritdoc/>
        public void WriteLine(string? value)
        {
            this.WriteLineCount++;
            this.outputBuilder.AppendLine(value);
            this.writeOperations.Add(new WriteOperation((value ?? string.Empty) + Environment.NewLine, this.foregroundColor, this.backgroundColor));
        }

        /// <inheritdoc/>
        public string? ReadLine()
        {
            this.ReadLineCount++;
            if (this.readLineResponses.Count == 0)
            {
                throw new InvalidOperationException("No ReadLine response queued. Call QueueReadLine() first.");
            }

            return this.readLineResponses.Dequeue();
        }

        /// <inheritdoc/>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            this.ReadKeyCount++;
            if (this.readKeyResponses.Count == 0)
            {
                throw new InvalidOperationException("No ReadKey response queued. Call QueueReadKey() first.");
            }

            return this.readKeyResponses.Dequeue();
        }

        /// <summary>
        /// Represents a single write operation with its associated colors.
        /// </summary>
        public readonly record struct WriteOperation(string Text, ConsoleColor Foreground, ConsoleColor Background);
    }
}

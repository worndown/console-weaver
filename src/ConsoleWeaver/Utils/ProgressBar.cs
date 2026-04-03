namespace ConsoleWeaver.Utils
{
    using System;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// ASCII progress bar for console applications.
    /// </summary>
    public class ProgressBar : IDisposable, IProgress<double>
    {
        private const int MinBarSize = 3;
        private const int MaxBarSize = 100;
        private const int DefaultBarSize = 20;
        private const string Animation = @"|/-\";
        private readonly object sync = new object();
        private readonly Timer timer;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private readonly StringBuilder outputBuilder = new StringBuilder();
        private readonly ConsoleColor? foreground;
        private readonly ConsoleColor? background;
        private readonly IConsole console;
        private string currentText = string.Empty;
        private double currentProgress;
        private int currentAnimationIndex;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the ProgressBar class.
        /// </summary>
        public ProgressBar()
            : this(DefaultBarSize, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ProgressBar class.
        /// </summary>
        /// <param name="size">Progress bar size in characters.</param>
        public ProgressBar(int size)
            : this(size, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ProgressBar class.
        /// </summary>
        /// <param name="size">Progress bar size in characters.</param>
        /// <param name="autoClear">Clear progress bar when complete.</param>
        /// <param name="foreground">Console foreground color.</param>
        /// <param name="background">Console background color.</param>
        public ProgressBar(int size, bool autoClear, ConsoleColor? foreground = null, ConsoleColor? background = null)
            : this(size, autoClear, foreground, background, Shell.Console)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ProgressBar class.
        /// </summary>
        /// <param name="size">Progress bar size in characters.</param>
        /// <param name="autoClear">Clear progress bar when complete.</param>
        /// <param name="foreground">Console foreground color.</param>
        /// <param name="background">Console background color.</param>
        /// <param name="console">Console implementation to use.</param>
        public ProgressBar(int size, bool autoClear, ConsoleColor? foreground, ConsoleColor? background, IConsole console)
        {
            this.Size = Precondition.CheckInRange(size, MinBarSize, MaxBarSize, nameof(size));
            this.AutoClear = autoClear;
            this.foreground = foreground;
            this.background = background;
            this.console = Precondition.CheckNotNull(console, nameof(console));
            this.timer = new Timer(this.TimerCallback);

            // A progress bar is only for the display in a console window.
            // We won't print anything if console output is redirected.
            if (!this.console.IsOutputRedirected)
            {
                this.ResetTimer();
            }
        }

        /// <summary>
        /// Gets progress bar size in characters.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Whether progress bar should be cleared at the end.
        /// </summary>
        public bool AutoClear { get; }

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        public void Report(double value)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref this.currentProgress, value);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs object disposal.
        /// </summary>
        /// <param name="disposing">Whether object is being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this.sync)
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.timer.Dispose();
                    this.disposed = true;

                    if (this.AutoClear)
                    {
                        this.ClearText(this.currentText.Length);
                    }
                    else
                    {
                        // It is possible we stopped the timer before 100% was printed.
                        // This is not nice and looks like incomplete progress. Fix it.
                        if (!this.currentText.Contains("100%", StringComparison.Ordinal) &&
                            Math.Abs(this.currentProgress - 1.0) < double.Epsilon)
                        {
                            this.TimerCallback(null);
                        }

                        // Clear animation character since it looks ugly.
                        this.ClearText(1);
                    }
                }
            }
        }

        /// <summary>
        /// Handled timer callbacks.
        /// </summary>
        /// <param name="state">Unused state object.</param>
        private void TimerCallback(object? state)
        {
            lock (this.sync)
            {
                int progressBlockCount = (int)(this.currentProgress * Size);
                int percent = (int)(this.currentProgress * 100);
                string text = string.Format(
                    "[{0}{1}] {2,3}% {3}",
                    new string('#', progressBlockCount),
                    new string(' ', Size - progressBlockCount),
                    percent,
                    Animation[currentAnimationIndex++ % Animation.Length]);

                this.UpdateText(text);
                this.ResetTimer();
            }
        }

        /// <summary>
        /// Updates progress text in the console.
        /// </summary>
        /// <param name="text">New text to output.</param>
        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(this.currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == this.currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            this.outputBuilder.Clear();
            this.outputBuilder.Append('\b', this.currentText.Length - commonPrefixLength);

            // Output new suffix
            this.outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = this.currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                this.outputBuilder.Append(' ', overlapCount);
                this.outputBuilder.Append('\b', overlapCount);
            }

            Shell.Write(this.outputBuilder.ToString(), foreground, background);
            this.currentText = text;
        }

        /// <summary>
        /// Clears text and restores background.
        /// </summary>
        /// <param name="length">Length of text to clear</param>
        private void ClearText(int length)
        {
            this.outputBuilder.Clear();
            this.outputBuilder.Append('\b', length);
            this.outputBuilder.Append(' ', length);
            this.outputBuilder.Append('\b', length);
            this.console.Write(this.outputBuilder.ToString());
        }

        /// <summary>
        /// Resets timer.
        /// </summary>
        private void ResetTimer()
        {
            if (!this.disposed)
            {
                this.timer.Change(this.animationInterval, TimeSpan.FromMilliseconds(-1));
            }
        }
    }
}

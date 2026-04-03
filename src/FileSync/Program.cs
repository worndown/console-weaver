namespace FileSync
{
    using ConsoleWeaver;
    using ConsoleWeaver.Cmdline;
    using ConsoleWeaver.Utils;

    internal class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                // Parse command-line arguments into strongly-typed options
                FileSyncOptions options = ProgramOptions.Parse<FileSyncOptions>(args);

                // Display parsed options
                DisplayOptions(options);

                // Simulate the operation
                if (options.BackupMode || options.ParameterSetName == "Backup")
                {
                    RunBackupOperation(options);
                }
                else
                {
                    RunSyncOperation(options);
                }

                return 0;
            }
            catch (IncorrectUsageException ex)
            {
                // Display usage information (automatically formatted by ConsoleWeaver)
                Shell.WriteLine(ex.Message, ConsoleColor.Yellow);
                return 1;
            }
            catch (ParameterException ex)
            {
                Shell.WriteLine(ex.Message, ConsoleColor.Red);
                return 2;
            }
            catch (Exception ex)
            {
                Shell.WriteLine($"Error: {ex.Message}", ConsoleColor.Red);
                if (args.Contains("-v") || args.Contains("--verbose"))
                {
                    Shell.WriteLine(ex.StackTrace ?? string.Empty, ConsoleColor.Red);
                }
                return 3;
            }
        }

        /// <summary>
        /// Displays the parsed options in a formatted table.
        /// </summary>
        private static void DisplayOptions(FileSyncOptions options)
        {
            var table = new Table("Parsed Options")
            {
                Foreground = ConsoleColor.Cyan
            };

            table.AddColumnDefinition("Option", Alignment.Left);
            table.AddColumnDefinition("Value", Alignment.Left);

            table.AddRow(["Parameter Set", options.ParameterSetName]);
            table.AddRow(["Source Path", options.SourcePath]);

            if (!string.IsNullOrEmpty(options.DestinationPath))
            {
                table.AddRow(["Destination Path", options.DestinationPath]);
            }

            if (!string.IsNullOrEmpty(options.ArchivePath))
            {
                table.AddRow(["Archive Path", options.ArchivePath]);
                table.AddRow(["Compression Level", options.CompressionLevel]);
            }

            table.AddRowSeparator();
            table.AddRow(["Filter Pattern", options.FilterPattern]);
            table.AddRow(["Max File Size (MB)", options.MaxFileSizeMB]);
            table.AddRow(["Log File", options.LogFilePath ?? "(none)"]);

            table.AddRowSeparator();
            table.AddRow(["Verbose", options.Verbose ? "Yes" : "No"]);
            table.AddRow(["Recursive", options.Recursive ? "Yes" : "No"]);
            table.AddRow(["Overwrite", options.Overwrite ? "Yes" : "No"]);
            table.AddRow(["Dry Run", options.DryRun ? "Yes" : "No"]);

            Console.WriteLine();
            table.ConsoleWrite();
            Console.WriteLine();
        }

        /// <summary>
        /// Simulates a file synchronization operation.
        /// </summary>
        private static void RunSyncOperation(FileSyncOptions options)
        {
            if (options.DryRun)
            {
                Shell.WriteLine("[DRY RUN] No files will be modified.", ConsoleColor.Yellow);
            }

            // Get files from source directory
            var searchOption = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(options.SourcePath, options.FilterPattern, searchOption);

            if (options.Verbose)
            {
                Shell.WriteLine($"Found {files.Length} file(s) matching '{options.FilterPattern}'", ConsoleColor.Gray);
            }

            // Simulate processing with progress bar
            Shell.WriteLine("Starting sync operation...", ConsoleColor.Green);
            using (var progress = new ProgressBar(50, autoClear: false))
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    var fileInfo = new FileInfo(file);

                    // Skip files exceeding max size
                    if (fileInfo.Length > options.MaxFileSizeMB * 1024 * 1024)
                    {
                        continue;
                    }

                    // Simulate file copy
                    Thread.Sleep(50);
                    progress.Report((double)(i + 1) / files.Length);
                }
            }

            Console.WriteLine();
            Shell.WriteLine($"Sync completed: {files.Length} file(s) processed.", ConsoleColor.Green);
        }

        /// <summary>
        /// Simulates a backup archive operation.
        /// </summary>
        private static void RunBackupOperation(FileSyncOptions options)
        {
            if (options.DryRun)
            {
                Shell.WriteLine("[DRY RUN] No archive will be created.", ConsoleColor.Yellow);
            }

            // Get files from source directory
            var searchOption = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(options.SourcePath, options.FilterPattern, searchOption);

            if (options.Verbose)
            {
                Shell.WriteLine($"Found {files.Length} file(s) to archive", ConsoleColor.Gray);
                Shell.WriteLine($"Compression level: {options.CompressionLevel}", ConsoleColor.Gray);
            }

            // Simulate archiving with progress bar
            long totalSize = 0;
            Shell.WriteLine("Starting backup operation...", ConsoleColor.Green);
            using (var progress = new ProgressBar(30, autoClear: false, ConsoleColor.Cyan))
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    var fileInfo = new FileInfo(file);

                    // Skip files exceeding max size
                    if (fileInfo.Length > options.MaxFileSizeMB * 1024 * 1024)
                    {
                        continue;
                    }

                    totalSize += fileInfo.Length;

                    // Simulate compression (slower for higher compression levels)
                    Thread.Sleep(20 + options.CompressionLevel * 5);
                    progress.Report((double)(i + 1) / files.Length);
                }
            }

            Console.WriteLine();
            Shell.WriteLine($"Backup completed: {files.Length} file(s), {totalSize / 1024.0:N0} KB total", ConsoleColor.Green);

            if (!options.DryRun)
            {
                Shell.WriteLine($"Archive created: {options.ArchivePath}", ConsoleColor.Green);
            }
        }
    }
}

namespace FileSync
{
    using ConsoleWeaver;
    using ConsoleWeaver.Cmdline;

    /// <summary>
    /// Command-line options for the FileSync utility.
    /// Demonstrates all parameter types and parameter sets.
    /// </summary>
    /// <remarks>
    /// This utility supports two modes of operation:
    ///
    /// 1. Sync Mode (-sync): Synchronize files between two directories
    ///    filesync -sync source_dir destination_dir [options]
    ///
    /// 2. Backup Mode (-backup): Create a backup archive of a directory
    ///    filesync -backup source_dir -archive backup.zip [options]
    /// </remarks>
    public class FileSyncOptions : OptionsBase
    {
        public FileSyncOptions()
        {
            this.ProgramDescription.Add("FileSync - A file synchronization and backup utility");
            this.ProgramDescription.Add("Synchronize directories or create backup archives");
            this.HeaderAlignment = Alignment.Center;

            this.Examples.Add("filesync --sync C:\\Source C:\\Dest --recursive --verbose");
            this.Examples.Add("filesync --backup C:\\Source --archive D:\\backup.zip --filter=*.docx");
            this.Examples.Add("filesync C:\\Source C:\\Dest --overwrite --log=sync.log");

            this.Remarks.Add("Use --sync for directory synchronization or --backup for archive creation.");
            this.Remarks.Add("When neither --sync nor --backup is specified, sync mode is assumed.");
        }

        // ==================== Parameter Set Switches ====================

        /// <summary>
        /// Enable sync mode - synchronize files between directories.
        /// </summary>
        [SwitchParameter(
            ShortName = "s",
            LongName = "sync",
            Description = "Synchronize files between source and destination directories")]
        [ParameterSet("Sync")]
        public bool SyncMode { get; set; }

        /// <summary>
        /// Enable backup mode - create archive from source directory.
        /// </summary>
        [SwitchParameter(
            ShortName = "b",
            LongName = "backup",
            Description = "Create a backup archive from the source directory")]
        [ParameterSet("Backup")]
        public bool BackupMode { get; set; }

        // ==================== Mandatory Parameters ====================

        /// <summary>
        /// Source directory path (positional parameter, position 0).
        /// Required in both Sync and Backup modes.
        /// </summary>
        [MandatoryParameter(
            LongName = "source",
            Position = 0,
            MetaValue = "PATH",
            Description = "Source directory path")]
        [ParameterSet("Sync")]
        [ParameterSet("Backup")]
        public string SourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Destination directory path (positional parameter, position 1).
        /// Required only in Sync mode.
        /// </summary>
        [MandatoryParameter(
            LongName = "destination",
            Position = 1,
            MetaValue = "PATH",
            Description = "Destination directory path for synchronization")]
        [ParameterSet("Sync")]
        public string DestinationPath { get; set; } = string.Empty;

        /// <summary>
        /// Archive file path for backup mode.
        /// Required only in Backup mode.
        /// </summary>
        [MandatoryParameter(
            ShortName = "a",
            LongName = "archive",
            MetaValue = "FILE",
            Description = "Output archive file path (.zip)")]
        [ParameterSet("Backup")]
        public string ArchivePath { get; set; } = string.Empty;

        // ==================== Optional Parameters ====================

        /// <summary>
        /// File filter pattern (e.g., "*.txt", "*.cs").
        /// </summary>
        [OptionalParameter(
            ShortName = "f",
            LongName = "filter",
            MetaValue = "PATTERN",
            Description = "File filter pattern (e.g., *.txt, *.cs)",
            DefaultValue = "*.*")]
        public string FilterPattern { get; set; } = "*.*";

        /// <summary>
        /// Log file path for operation logging.
        /// </summary>
        [OptionalParameter(
            ShortName = "l",
            LongName = "log",
            MetaValue = "FILE",
            Description = "Log file path for operation details")]
        public string? LogFilePath { get; set; }

        /// <summary>
        /// Maximum file size in megabytes.
        /// </summary>
        [OptionalParameter(
            ShortName = "m",
            LongName = "maxsize",
            MetaValue = "MB",
            Description = "Maximum file size to process in megabytes",
            DefaultValue = "100")]
        public int MaxFileSizeMB { get; set; } = 100;

        /// <summary>
        /// Compression level for backup archives (0-9).
        /// </summary>
        [OptionalParameter(
            ShortName = "c",
            LongName = "compression",
            MetaValue = "LEVEL",
            Description = "Compression level 0-9 (0=store, 9=maximum)",
            DefaultValue = "6")]
        [ParameterSet("Backup")]
        public int CompressionLevel { get; set; } = 6;

        // ==================== Switch Parameters ====================

        /// <summary>
        /// Enable verbose output.
        /// </summary>
        [SwitchParameter(
            ShortName = "v",
            LongName = "verbose",
            Description = "Enable verbose output")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Process subdirectories recursively.
        /// </summary>
        [SwitchParameter(
            ShortName = "r",
            LongName = "recursive",
            Description = "Process subdirectories recursively")]
        public bool Recursive { get; set; }

        /// <summary>
        /// Overwrite existing files without prompting.
        /// </summary>
        [SwitchParameter(
            ShortName = "o",
            LongName = "overwrite",
            Description = "Overwrite existing files without prompting")]
        public bool Overwrite { get; set; }

        /// <summary>
        /// Perform a dry run without making changes.
        /// </summary>
        [SwitchParameter(
            ShortName = "n",
            LongName = "dryrun",
            Description = "Perform a dry run without making actual changes")]
        public bool DryRun { get; set; }

        /// <summary>
        /// Validates the parsed options.
        /// </summary>
        protected override void InternalValidate()
        {
            // Validate source path exists
            if (!Directory.Exists(this.SourcePath))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {SourcePath}");
            }

            if (this.SyncMode || this.ParameterSetName == "Sync")
            {
                // Check source and destination are not the same
                string fullSource = Path.GetFullPath(this.SourcePath);
                string fullDest = Path.GetFullPath(this.DestinationPath);
                if (string.Equals(fullSource, fullDest, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Source and destination directories cannot be the same.");
                }
            }

            // Validate archive path for backup mode
            if (this.BackupMode || this.ParameterSetName == "Backup")
            {
                if (!this.ArchivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Archive file must have .zip extension.");
                }

                if (File.Exists(this.ArchivePath) && !this.Overwrite)
                {
                    throw new IOException($"Archive file already exists: {this.ArchivePath}. Use -overwrite to replace.");
                }
            }

            // Validate compression level
            if (CompressionLevel < 0 || CompressionLevel > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(CompressionLevel), "Compression level must be between 0 and 9.");
            }

            // Validate max file size
            if (MaxFileSizeMB <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxFileSizeMB), "Maximum file size must be greater than 0.");
            }
        }
    }
}

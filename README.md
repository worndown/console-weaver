# ConsoleWeaver

[![NuGet](https://img.shields.io/nuget/v/ConsoleWeaver.svg)](https://www.nuget.org/packages/ConsoleWeaver/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build](https://github.com/worndown/console-weaver/actions/workflows/build.yml/badge.svg)](https://github.com/worndown/console-weaver/actions/workflows/build.yml)

A .NET utility library for building console applications with command-line argument parsing, progress bars, formatted tables, interactive prompts, and testable console abstractions.

## Features

- **Command-Line Parsing** - Attribute-based argument parsing with support for mandatory, optional, and switch parameters
- **Progress Bar** - Animated ASCII progress bar with color support
- **Table Output** - Formatted ASCII tables with configurable columns, alignment, and CSV export
- **Interactive Prompts** - User input helpers for Yes/No responses, choices, strings, integers, and passwords
- **Console Abstraction** - `IConsole` interface for testable console applications

## Installation

```bash
dotnet add package ConsoleWeaver
```

## Quick Start

### Command-Line Argument Parsing

Define your options by inheriting from `OptionsBase` and decorating properties with parameter attributes:

```csharp
using ConsoleWeaver.Cmdline;

public class MyOptions : OptionsBase
{
    [MandatoryParameter(ShortName = "i", LongName = "input",
        MetaValue = "FILE", Description = "Input file path")]
    public string InputFile { get; set; } = string.Empty;

    [OptionalParameter(ShortName = "o", LongName = "output",
        MetaValue = "FILE", Description = "Output file path",
        DefaultValue = "output.txt")]
    public string OutputFile { get; set; } = string.Empty;

    [SwitchParameter(ShortName = "v", LongName = "verbose",
        Description = "Enable verbose output")]
    public bool Verbose { get; set; }

    protected override void InternalValidate()
    {
        // Custom validation logic
        if (!File.Exists(InputFile))
            throw new FileNotFoundException("Input file not found", InputFile);
    }
}

// In your Main method:
try
{
    var options = ProgramOptions.Parse<MyOptions>(args);
    Console.WriteLine($"Processing {options.InputFile}...");
}
catch (IncorrectUsageException ex)
{
    // Display usage help
    Console.WriteLine(ex.Message);
}
```

Running `myapp --help` or `myapp -?` automatically displays formatted usage information.

### Progress Bar

Display an animated progress bar for long-running operations:

```csharp
using ConsoleWeaver.Utils;

using (var progress = new ProgressBar(size: 30, autoClear: false))
{
    for (int i = 0; i <= 100; i++)
    {
        progress.Report(i / 100.0);
        Thread.Sleep(50);
    }
}
// Output: [##############################] 100% |
```

The progress bar supports custom colors and implements `IProgress<double>`:

```csharp
using (var progress = new ProgressBar(25, true, ConsoleColor.Green, ConsoleColor.Black))
{
    await ProcessFilesAsync(progress);
}
```

### Table Output

Create formatted ASCII tables:

```csharp
using ConsoleWeaver.Utils;

var table = new Table("Sales Report");
table.AddColumnDefinition("Product", Alignment.Left);
table.AddColumnDefinition("Quantity", Alignment.Right);
table.AddColumnDefinition("Price", "C2", Alignment.Right);

table.AddRow(new object[] { "Widget", 150, 29.99 });
table.AddRow(new object[] { "Gadget", 75, 49.99 });
table.AddRowSeparator();
table.AddRow(new object[] { "Total", 225, 7248.75 });

table.Footnotes.Add("* Prices in USD");
table.ConsoleWrite();
```

Output:
```
+-----------------------------+
|        Sales Report         |
+----------+----------+-------+
| Product  | Quantity | Price |
+----------+----------+-------+
| Widget   |      150 | $29.99|
| Gadget   |       75 | $49.99|
+----------+----------+-------+
| Total    |      225 |$7248.75|
+----------+----------+-------+
* Prices in USD
```

Export to CSV:
```csharp
table.ExportCsv("report.csv", append: false);
```

### Interactive Prompts

Collect user input with various prompt types:

```csharp
using ConsoleWeaver.Utils;

// Yes/No/Cancel prompt
var response = Prompt.GetResponse(
    "Do you want to continue?",
    defaultResponse: Response.Yes,
    responseOptions: ResponseOptions.YesNoCancel);

if (response == Response.Cancel)
    return;

// Choice selection
var format = Prompt.GetChoice(
    "Select output format:",
    new[] { "JSON", "XML", "CSV" });

// String input with default
string? name = Prompt.GetString("Enter your name", defaultValue: "Anonymous");

// Integer input
int count = Prompt.GetInteger("Enter count", defaultResponse: 10);

// Secure password input (masked)
string password = Prompt.GetPassword("Enter password");
```

### Console Abstraction

Use `IConsole` for testable console applications:

```csharp
using ConsoleWeaver;

public class MyService
{
    private readonly IConsole _console;

    public MyService(IConsole console)
    {
        _console = console;
    }

    public void Run()
    {
        _console.WriteLine("Hello, World!");
    }
}

// Production usage with SystemConsole
var service = new MyService(SystemConsole.Instance);

// Or use the Shell helper with colors
Shell.WriteLine("Success!", ConsoleColor.Green);
Shell.WriteLine("Error!", ConsoleColor.Red);
```

For unit testing, implement `IConsole` with a mock or use the `MockConsole` pattern from the test project.

## Command-Line Parser Features

### Parameter Types

| Attribute | Description |
|-----------|-------------|
| `MandatoryParameter` | Required parameter that must be provided |
| `OptionalParameter` | Optional parameter with optional default value |
| `SwitchParameter` | Boolean flag (true when present) |

### Parameter Syntax

Parameters can be specified using short or long names:

```bash
myapp -i input.txt -o output.txt -v
myapp --input input.txt --output output.txt --verbose
myapp -i input.txt --verbose  # Mixed syntax
```

### Positional Parameters

Use the `Position` property for positional arguments (a name is still required):

```csharp
[MandatoryParameter(LongName = "source", Position = 0,
    MetaValue = "SOURCE", Description = "Source file")]
public string Source { get; set; } = string.Empty;

[MandatoryParameter(LongName = "destination", Position = 1,
    MetaValue = "DESTINATION", Description = "Destination file")]
public string Destination { get; set; } = string.Empty;
```

```bash
myapp source.txt destination.txt
```

### Multi-Valued Parameters

Use array properties for parameters that accept multiple values:

```csharp
[MandatoryParameter(ShortName = "f", LongName = "files",
    MetaValue = "FILE", Description = "Input files")]
public string[] Files { get; set; } = Array.Empty<string>();
```

```bash
myapp -f file1.txt -f file2.txt -f file3.txt
```

### Parameter Sets

Define mutually exclusive parameter groups:

```csharp
[MandatoryParameter(ShortName = "u", LongName = "username")]
[ParameterSet("Credentials")]
public string Username { get; set; } = string.Empty;

[MandatoryParameter(ShortName = "p", LongName = "password")]
[ParameterSet("Credentials")]
public string Password { get; set; } = string.Empty;

[MandatoryParameter(ShortName = "t", LongName = "token")]
[ParameterSet("Token")]
public string Token { get; set; } = string.Empty;
```

### Type Conversion

The parser automatically converts string values to property types using a converter chain:

1. **Enum values** - Parse enum names (case-insensitive)
2. **Parse method** - Types with `static T Parse(string)` method
3. **Constructor** - Types with `T(string)` constructor
4. **ChangeType** - Standard type conversion

## Example Application

The repository includes a complete example application (`ExampleApp`) demonstrating all command-line parser features:

- **Parameter Sets**: Two modes of operation (Sync and Backup)
- **Mandatory Parameters**: Source path (positional), destination path, archive path
- **Optional Parameters**: Filter pattern, log file, max file size, compression level
- **Switch Parameters**: Verbose, recursive, overwrite, dry-run
- **Validation**: Custom validation in `InternalValidate()` method

### Running the Example

```bash
# Show usage help
dotnet run --project src/ExampleApp/ExampleApp.csproj -- --help

# Sync mode: synchronize directories
dotnet run --project src/ExampleApp/ExampleApp.csproj -- --sync ./src ./backup --recursive --verbose

# Backup mode: create archive
dotnet run --project src/ExampleApp/ExampleApp.csproj -- --backup ./src --archive=backup.zip --compression=9

# Dry run (simulate without changes)
dotnet run --project src/ExampleApp/ExampleApp.csproj -- ./src ./dest --dryrun --filter=*.cs
```

See [`src/ExampleApp/FileSyncOptions.cs`](src/ExampleApp/FileSyncOptions.cs) for the complete options class implementation.

## Building from Source

```bash
# Clone the repository
git clone https://github.com/worndown/console-weaver.git
cd console-weaver

# Build the solution
dotnet build src/Solution.slnx

# Run tests
dotnet test src/Solution.slnx

# Run example application
dotnet run --project src/ExampleApp/ExampleApp.csproj -- --help

# Create NuGet package
dotnet pack src/ConsoleWeaver/ConsoleWeaver.csproj -c Release
```

## Requirements

- .NET 10.0 or later

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

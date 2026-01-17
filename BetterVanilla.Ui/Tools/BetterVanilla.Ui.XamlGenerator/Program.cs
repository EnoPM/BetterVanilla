using System.Text.Json;
using BetterVanilla.Ui.XamlGenerator;
using BetterVanilla.Ui.XamlGenerator.Models;

// Parse command line arguments
if (args.Length < 2)
{
    Console.WriteLine("Usage: bvui-gen <input-file-or-dir> <output-dir> [--aliases <aliases-file>]");
    Console.WriteLine();
    Console.WriteLine("Arguments:");
    Console.WriteLine("  <input-file-or-dir>  Path to .bvui.xml file or directory containing them");
    Console.WriteLine("  <output-dir>         Directory to write generated .g.cs files");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --aliases <file>     Path to ui-aliases.json file");
    Console.WriteLine("  --verbose            Enable verbose output");
    Console.WriteLine();
    Console.WriteLine("Example:");
    Console.WriteLine("  bvui-gen ./Views ./obj/Generated --aliases ./ui-aliases.json");
    return 1;
}

var inputPath = args[0];
var outputDir = args[1];
var aliasesPath = "ui-aliases.json";
var verbose = false;

// Parse optional arguments
for (var i = 2; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--aliases" when i + 1 < args.Length:
            aliasesPath = args[++i];
            break;
        case "--verbose":
            verbose = true;
            break;
    }
}

// Load aliases configuration
AliasConfig aliasConfig;
if (File.Exists(aliasesPath))
{
    var json = await File.ReadAllTextAsync(aliasesPath);
    aliasConfig = JsonSerializer.Deserialize<AliasConfig>(json) ?? new AliasConfig();
    if (verbose)
    {
        Console.WriteLine($"Loaded {aliasConfig.Aliases.Count} aliases from {aliasesPath}");
    }
}
else
{
    aliasConfig = new AliasConfig();
    if (verbose)
    {
        Console.WriteLine($"Warning: Aliases file not found at {aliasesPath}, using defaults");
    }
}

// Ensure output directory exists
Directory.CreateDirectory(outputDir);

// Find input files
var inputFiles = new List<string>();
if (File.Exists(inputPath))
{
    inputFiles.Add(inputPath);
}
else if (Directory.Exists(inputPath))
{
    inputFiles.AddRange(Directory.GetFiles(inputPath, "*.bvui.xml", SearchOption.AllDirectories));
}
else
{
    Console.Error.WriteLine($"Error: Input path not found: {inputPath}");
    return 1;
}

if (inputFiles.Count == 0)
{
    Console.WriteLine("No .bvui.xml files found");
    return 0;
}

if (verbose)
{
    Console.WriteLine($"Found {inputFiles.Count} BVUI file(s) to process");
}

// Process each file
var parser = new XamlParser();
var generator = new CodeGenerator(aliasConfig);
var errorCount = 0;

foreach (var inputFile in inputFiles)
{
    try
    {
        if (verbose)
        {
            Console.WriteLine($"Processing: {inputFile}");
        }

        // Parse the BVUI file
        var definition = parser.Parse(inputFile);

        // Generate code
        var code = generator.Generate(definition);

        // Determine output file path
        var relativePath = inputPath.Length > 0 && Directory.Exists(inputPath)
            ? Path.GetRelativePath(inputPath, inputFile)
            : Path.GetFileName(inputFile);

        var outputFileName = Path.GetFileNameWithoutExtension(relativePath);
        if (outputFileName.EndsWith(".bvui", StringComparison.OrdinalIgnoreCase))
        {
            outputFileName = outputFileName[..^5];
        }

        var outputPath = Path.Combine(outputDir, $"{outputFileName}.g.cs");

        // Ensure subdirectory exists
        var outputSubDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputSubDir))
        {
            Directory.CreateDirectory(outputSubDir);
        }

        // Write generated code
        await File.WriteAllTextAsync(outputPath, code);

        Console.WriteLine($"Generated: {outputPath}");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error processing {inputFile}: {ex.Message}");
        if (verbose)
        {
            Console.Error.WriteLine(ex.StackTrace);
        }
        errorCount++;
    }
}

Console.WriteLine();
Console.WriteLine($"Processed {inputFiles.Count} file(s), {errorCount} error(s)");

return errorCount > 0 ? 1 : 0;

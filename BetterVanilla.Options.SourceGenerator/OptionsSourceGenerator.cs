using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BetterVanilla.Options.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class OptionsSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all .options.xml files
        var optionsFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".options.xml", StringComparison.OrdinalIgnoreCase));

        // Generate source for each options file
        context.RegisterSourceOutput(optionsFiles, static (spc, optionsFile) =>
        {
            GenerateSource(spc, optionsFile);
        });
    }

    private static void GenerateSource(SourceProductionContext context, AdditionalText optionsFile)
    {
        var text = optionsFile.GetText(context.CancellationToken);
        if (text == null)
            return;

        try
        {
            // Parse the options XML
            var parser = new OptionsParser();
            var definition = parser.Parse(text.ToString(), optionsFile.Path);

            // Generate code
            var generator = new OptionsCodeGenerator();
            var code = generator.Generate(definition);

            // Add the source
            var hintName = GetHintName(optionsFile.Path, definition.TypeName);
            context.AddSource(hintName, SourceText.From(code, Encoding.UTF8));
        }
        catch (Exception ex)
        {
            // Report diagnostic for parsing errors
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "OPT001",
                    "Options Parsing Error",
                    "Error parsing {0}: {1}",
                    "BetterVanilla.Options.Core",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                Location.None,
                optionsFile.Path,
                ex.Message);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static string GetHintName(string filePath, string typeName)
    {
        // Use the type name if available, otherwise derive from file path
        if (!string.IsNullOrEmpty(typeName))
            return $"{typeName}.g.cs";

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        if (fileName.EndsWith(".options", StringComparison.OrdinalIgnoreCase))
            fileName = fileName.Substring(0, fileName.Length - 8);

        return $"{fileName}.g.cs";
    }
}

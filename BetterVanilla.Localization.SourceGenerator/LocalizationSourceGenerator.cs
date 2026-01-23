using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BetterVanilla.Localization.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class LocalizationSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all .loc.xml files
        var localizationFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".loc.xml", StringComparison.OrdinalIgnoreCase));

        // Generate source for each localization file
        context.RegisterSourceOutput(localizationFiles, static (spc, locFile) =>
        {
            GenerateSource(spc, locFile);
        });
    }

    private static void GenerateSource(SourceProductionContext context, AdditionalText locFile)
    {
        var text = locFile.GetText(context.CancellationToken);
        if (text == null)
            return;

        try
        {
            // Parse the localization XML
            var parser = new LocalizationParser();
            var definition = parser.Parse(text.ToString(), locFile.Path);

            // Generate code
            var generator = new LocalizationCodeGenerator();
            var code = generator.Generate(definition);

            // Add the source
            var hintName = GetHintName(locFile.Path, definition.TypeName);
            context.AddSource(hintName, SourceText.From(code, Encoding.UTF8));
        }
        catch (Exception ex)
        {
            // Report diagnostic for parsing errors
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "LOC001",
                    "Localization Parsing Error",
                    "Error parsing {0}: {1}",
                    "BetterVanilla.Localization",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                Location.None,
                locFile.Path,
                ex.Message);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static string GetHintName(string filePath, string typeName)
    {
        if (!string.IsNullOrEmpty(typeName))
            return $"{typeName}.g.cs";

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        if (fileName.EndsWith(".loc", StringComparison.OrdinalIgnoreCase))
            fileName = fileName[..^4];

        return $"{fileName}.g.cs";
    }
}

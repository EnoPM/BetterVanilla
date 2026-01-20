using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using BetterVanilla.Ui.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BetterVanilla.Ui.SourceGenerator;

/// <summary>
/// Roslyn Source Generator that generates C# code from .ui.xml files.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class UiSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all .ui.xml files
        var uiFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".ui.xml", StringComparison.OrdinalIgnoreCase));

        // Find the ui-aliases.json file
        var aliasFile = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith("ui-aliases.json", StringComparison.OrdinalIgnoreCase))
            .Collect();

        // Combine ui files with alias config
        var combined = uiFiles.Combine(aliasFile);

        // Generate source for each ui file
        context.RegisterSourceOutput(combined, static (spc, source) =>
        {
            var (uiFile, aliasFiles) = source;

            // Load alias config
            var aliasConfig = LoadAliasConfig(aliasFiles);

            // Generate code
            GenerateSource(spc, uiFile, aliasConfig);
        });
    }

    private static AliasConfig LoadAliasConfig(ImmutableArray<AdditionalText> aliasFiles)
    {
        if (aliasFiles.IsDefaultOrEmpty)
            return new AliasConfig();

        var aliasFile = aliasFiles[0];
        var text = aliasFile.GetText();
        if (text == null)
            return new AliasConfig();

        try
        {
            return AliasConfigParser.Parse(text.ToString());
        }
        catch
        {
            return new AliasConfig();
        }
    }

    private static void GenerateSource(SourceProductionContext context, AdditionalText uiFile, AliasConfig aliasConfig)
    {
        var text = uiFile.GetText(context.CancellationToken);
        if (text == null)
            return;

        try
        {
            // Parse the UI XML
            var parser = new XamlParser();
            var definition = parser.Parse(text.ToString(), uiFile.Path);

            // Generate code
            var generator = new CodeGenerator(aliasConfig);
            var code = generator.Generate(definition);

            // Add the source
            var hintName = GetHintName(uiFile.Path, definition.TypeName);
            context.AddSource(hintName, SourceText.From(code, Encoding.UTF8));
        }
        catch (Exception ex)
        {
            // Report diagnostic for parsing errors
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "UI001",
                    "UI Parsing Error",
                    "Error parsing {0}: {1}",
                    "BetterVanilla.Ui",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                Location.None,
                uiFile.Path,
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
        if (fileName.EndsWith(".ui", StringComparison.OrdinalIgnoreCase))
            fileName = fileName.Substring(0, fileName.Length - 3);

        return $"{fileName}.g.cs";
    }
}

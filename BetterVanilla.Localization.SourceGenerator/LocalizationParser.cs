using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BetterVanilla.Localization.SourceGenerator.Models;

namespace BetterVanilla.Localization.SourceGenerator;

public sealed class LocalizationParser
{
    private const string LocalizationNamespace = "http://schemas.bettervanilla.localization/2025";
    private const string XamlNamespace = "http://schemas.bettervanilla.localization/2025/xaml";
    private const string MsXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    public LocalizationDefinition Parse(string xmlContent, string filePath)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xmlContent);

        var definition = new LocalizationDefinition
        {
            SourceFile = filePath
        };

        if (doc.DocumentElement == null)
        {
            throw new InvalidOperationException($"Empty XML document: {filePath}");
        }

        // Parse x:Class
        var classAttr = doc.DocumentElement.GetAttributeNode("Class", XamlNamespace)
                        ?? doc.DocumentElement.GetAttributeNode("Class", MsXamlNamespace)
                        ?? doc.DocumentElement.GetAttributeNode("x:Class");

        if (classAttr != null)
        {
            definition.ClassName = classAttr.Value;
            var lastDot = definition.ClassName.LastIndexOf('.');
            if (lastDot > 0)
            {
                definition.Namespace = definition.ClassName[..lastDot];
                definition.TypeName = definition.ClassName[(lastDot + 1)..];
            }
            else
            {
                definition.TypeName = definition.ClassName;
            }
        }
        else
        {
            // Derive from file name
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.EndsWith(".loc", StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName[..^4];
            }
            definition.TypeName = fileName;
        }

        // Parse DefaultLanguage attribute
        var defaultLangAttr = doc.DocumentElement.GetAttribute("DefaultLanguage");
        if (!string.IsNullOrEmpty(defaultLangAttr))
        {
            definition.DefaultLanguage = defaultLangAttr;
        }

        // Collect all languages from Key elements
        var languagesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Parse child elements (Key entries)
        foreach (XmlNode child in doc.DocumentElement.ChildNodes)
        {
            if (child is not XmlElement element || element.LocalName != "Key") continue;

            var entry = ParseEntry(element, languagesSet);
            if (entry != null)
            {
                definition.Entries.Add(entry);
            }
        }

        // Add collected languages in sorted order
        var sortedLanguages = new List<string>(languagesSet);
        sortedLanguages.Sort(StringComparer.OrdinalIgnoreCase);

        // Ensure default language is first
        if (sortedLanguages.Remove(definition.DefaultLanguage))
        {
            sortedLanguages.Insert(0, definition.DefaultLanguage);
        }

        definition.Languages.AddRange(sortedLanguages);

        return definition;
    }

    private static TranslationEntry? ParseEntry(XmlElement element, HashSet<string> languagesSet)
    {
        var entry = new TranslationEntry();

        // Parse x:Name or Name attribute
        var nameAttr = element.GetAttributeNode("Name", XamlNamespace)
                       ?? element.GetAttributeNode("Name", MsXamlNamespace)
                       ?? element.GetAttributeNode("x:Name")
                       ?? element.GetAttributeNode("Name");

        if (nameAttr != null)
        {
            entry.Key = nameAttr.Value;
        }
        else
        {
            return null; // Key without name is invalid
        }

        // Parse language children (e.g., <En>Hello</En>, <Fr>Bonjour</Fr>)
        foreach (XmlNode langChild in element.ChildNodes)
        {
            if (langChild is not XmlElement langElement) continue;

            var langCode = langElement.LocalName;
            var translation = langElement.InnerText;

            entry.Translations[langCode] = translation;
            languagesSet.Add(langCode);
        }

        return entry.Translations.Count > 0 ? entry : null;
    }
}

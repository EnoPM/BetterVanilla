using System;
using System.IO;
using System.Xml;
using BetterVanilla.Options.SourceGenerator.Models;

namespace BetterVanilla.Options.SourceGenerator;

public sealed class OptionsParser
{
    private const string OptionsNamespace = "http://schemas.bettervanilla.options/2025";
    private const string XamlNamespace = "http://schemas.bettervanilla.options/2025/xaml";
    private const string MsXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    public OptionsDefinition Parse(string xmlContent, string filePath)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xmlContent);

        var definition = new OptionsDefinition
        {
            SourceFile = filePath
        };

        if (doc.DocumentElement == null)
        {
            throw new InvalidOperationException($"Empty XML document: {filePath}");
        }

        // Parse x:Class (support both Microsoft and Options namespaces)
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
            if (fileName.EndsWith(".options", StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName[..^8];
            }
            definition.TypeName = fileName;
        }

        // Parse Version attribute
        var versionAttr = doc.DocumentElement.GetAttribute("Version");
        if (!string.IsNullOrEmpty(versionAttr) && int.TryParse(versionAttr, out var version))
        {
            definition.Version = version;
        }

        // Parse child elements (options)
        foreach (XmlNode child in doc.DocumentElement.ChildNodes)
        {
            if (child is not XmlElement element) continue;
            var option = ParseOption(element);
            if (option != null)
            {
                definition.Options.Add(option);
            }
        }

        return definition;
    }

    private static OptionEntry? ParseOption(XmlElement element)
    {
        var typeName = element.LocalName;
        if (!TryParseOptionType(typeName, out var optionType))
        {
            return null;
        }

        var option = new OptionEntry
        {
            Type = optionType
        };

        // Parse x:Name
        var nameAttr = element.GetAttributeNode("Name", XamlNamespace)
                       ?? element.GetAttributeNode("Name", MsXamlNamespace)
                       ?? element.GetAttributeNode("x:Name");

        if (nameAttr != null)
        {
            option.Name = nameAttr.Value;
        }

        // Parse common attributes
        foreach (XmlAttribute attr in element.Attributes)
        {
            var name = attr.LocalName;
            var value = attr.Value;

            switch (name)
            {
                case "Default":
                    option.Default = value;
                    break;
                case "Min":
                    option.Min = value;
                    break;
                case "Max":
                    option.Max = value;
                    break;
                case "MaxLength":
                    if (int.TryParse(value, out var maxLength))
                        option.MaxLength = maxLength;
                    break;
                case "Type":
                    option.EnumType = value;
                    break;
            }
        }

        return option;
    }

    private static bool TryParseOptionType(string typeName, out OptionEntryType optionType)
    {
        switch (typeName)
        {
            case "BoolOption":
                optionType = OptionEntryType.Bool;
                return true;
            case "IntOption":
                optionType = OptionEntryType.Int;
                return true;
            case "FloatOption":
                optionType = OptionEntryType.Float;
                return true;
            case "StringOption":
                optionType = OptionEntryType.String;
                return true;
            case "EnumOption":
                optionType = OptionEntryType.Enum;
                return true;
            case "ColorOption":
                optionType = OptionEntryType.Color;
                return true;
            case "Vector2Option":
                optionType = OptionEntryType.Vector2;
                return true;
            default:
                optionType = default;
                return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BetterVanilla.Ui.SourceGenerator.Models;

namespace BetterVanilla.Ui.SourceGenerator;

/// <summary>
/// Parses .ui.xml content into ViewDefinition objects.
/// </summary>
public sealed class XamlParser
{
    private const string MsXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    private const string UiXamlNamespace = "http://schemas.bettervanilla.ui/2025/xaml";
    private const string UiNamespace = "http://schemas.bettervanilla.ui/2025";

    private static readonly HashSet<string> KnownEvents = new HashSet<string>
    {
        "Click", "Clicked", "ValueChanged", "IsOnChanged",
        "TextChanged", "SelectionChanged", "Submitted"
    };

    private static readonly HashSet<string> LayoutAttributes = new HashSet<string>
    {
        "Width", "Height", "MinWidth", "MinHeight", "MaxWidth", "MaxHeight",
        "FlexibleWidth", "FlexibleHeight",
        "Margin", "HorizontalAlignment", "VerticalAlignment"
    };

    public ViewDefinition Parse(string xmlContent, string filePath)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xmlContent);

        var definition = new ViewDefinition
        {
            SourceFile = filePath
        };

        if (doc.DocumentElement == null)
        {
            throw new InvalidOperationException($"Empty XML document: {filePath}");
        }

        // Parse x:Class (support both Microsoft and UI namespaces)
        var classAttr = doc.DocumentElement.GetAttributeNode("Class", UiXamlNamespace)
                        ?? doc.DocumentElement.GetAttributeNode("Class", MsXamlNamespace)
                        ?? doc.DocumentElement.GetAttributeNode("x:Class");

        if (classAttr != null)
        {
            definition.ClassName = classAttr.Value;
            var lastDot = definition.ClassName.LastIndexOf('.');
            if (lastDot > 0)
            {
                definition.Namespace = definition.ClassName.Substring(0, lastDot);
                definition.TypeName = definition.ClassName.Substring(lastDot + 1);
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
            if (fileName.EndsWith(".ui", StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName.Substring(0, fileName.Length - 3);
            }
            definition.TypeName = fileName;
        }

        // Parse root element
        definition.RootElement = ParseElement(doc.DocumentElement);

        // Collect all named elements
        CollectNamedElements(definition.RootElement, definition.NamedElements);

        return definition;
    }

    private ViewElement ParseElement(XmlElement xmlElement)
    {
        var element = new ViewElement
        {
            TagName = xmlElement.LocalName
        };

        // Parse attributes
        foreach (XmlAttribute attr in xmlElement.Attributes)
        {
            var name = attr.LocalName;
            var value = attr.Value;

            // Handle x:Name (support both Microsoft and UI namespaces)
            if ((attr.NamespaceURI == UiXamlNamespace || attr.NamespaceURI == MsXamlNamespace) && name == "Name" || attr.Name == "x:Name")
            {
                element.Name = value;
                continue;
            }

            // Skip x:Class, xmlns declarations, and xsi attributes
            if (name == "Class" || name.StartsWith("xmlns") || attr.Prefix == "xmlns" || attr.Prefix == "xsi")
            {
                continue;
            }

            // Handle Alias
            if (name == "Alias")
            {
                element.Alias = value;
                continue;
            }

            // Check if it's a layout attribute
            if (LayoutAttributes.Contains(name))
            {
                ParseLayoutAttribute(element.Layout, name, value);
                continue;
            }

            // Check if it's a binding expression
            if (IsBindingExpression(value))
            {
                var binding = ParseBinding(value, name);
                if (binding != null)
                {
                    element.Bindings[name] = binding;
                }
                continue;
            }

            // Check if it's an event handler
            if (KnownEvents.Contains(name) || name.EndsWith("Changed") || name.StartsWith("On"))
            {
                element.EventHandlers[name] = value;
                continue;
            }

            // Handle Source attribute on Image elements specially - convert to SourceInfo
            // This ensures SourceAssembly is set correctly when loading embedded resources
            if (name == "Source" && element.TagName == "Image")
            {
                element.Source ??= new SourceInfo();
                element.Source.EmbeddedResource = value;
                continue;
            }

            // Literal property value
            element.LiteralProperties[name] = value;
        }

        // Parse child elements
        foreach (XmlNode child in xmlElement.ChildNodes)
        {
            if (child is XmlElement childElement)
            {
                // Check if it's a property element
                if (childElement.LocalName.Contains("."))
                {
                    var parts = childElement.LocalName.Split(new[] { '.' }, 2);
                    if (parts.Length == 2 && parts[0] == element.TagName)
                    {
                        var propertyName = parts[1];
                        switch (propertyName)
                        {
                            case "TextStyle":
                                element.TextStyle = ParseTextStyle(childElement);
                                break;
                            case "PlaceholderStyle":
                                element.PlaceholderStyle = ParseTextStyle(childElement);
                                break;
                            case "LayoutGroup":
                                element.LayoutGroup = ParseLayoutGroup(childElement);
                                break;
                            case "Image":
                                element.Image = ParseImage(childElement);
                                break;
                            case "Source":
                                element.Source = ParseSource(childElement);
                                break;
                        }
                        continue;
                    }
                }

                element.Children.Add(ParseElement(childElement));
            }
        }

        return element;
    }

    private static TextStyleInfo ParseTextStyle(XmlElement xmlElement)
    {
        var style = new TextStyleInfo();

        foreach (XmlAttribute attr in xmlElement.Attributes)
        {
            var name = attr.LocalName;
            var value = attr.Value;

            switch (name)
            {
                case "FontSize":
                    if (float.TryParse(value, out var fontSize))
                        style.FontSize = fontSize;
                    break;
                case "TextColor":
                    style.TextColor = value;
                    break;
                case "TextAlignment":
                    style.TextAlignment = value;
                    break;
                case "FontStyle":
                    style.FontStyle = value;
                    break;
                case "CharacterSpacing":
                    if (float.TryParse(value, out var charSpacing))
                        style.CharacterSpacing = charSpacing;
                    break;
                case "LineSpacing":
                    if (float.TryParse(value, out var lineSpacing))
                        style.LineSpacing = lineSpacing;
                    break;
                case "WordSpacing":
                    if (float.TryParse(value, out var wordSpacing))
                        style.WordSpacing = wordSpacing;
                    break;
                case "WordWrapping":
                    if (bool.TryParse(value, out var wordWrapping))
                        style.WordWrapping = wordWrapping;
                    break;
                case "TextOverflow":
                    style.TextOverflow = value;
                    break;
                case "RichText":
                    if (bool.TryParse(value, out var richText))
                        style.RichText = richText;
                    break;
                case "AutoSize":
                    if (bool.TryParse(value, out var autoSize))
                        style.AutoSize = autoSize;
                    break;
                case "MinFontSize":
                    if (float.TryParse(value, out var minFontSize))
                        style.MinFontSize = minFontSize;
                    break;
                case "MaxFontSize":
                    if (float.TryParse(value, out var maxFontSize))
                        style.MaxFontSize = maxFontSize;
                    break;
                case "TextMargin":
                    style.TextMargin = value;
                    break;
            }
        }

        return style;
    }

    private static void ParseLayoutAttribute(LayoutInfo layout, string name, string value)
    {
        switch (name)
        {
            case "Width":
                layout.Width = value;
                break;
            case "Height":
                layout.Height = value;
                break;
            case "MinWidth":
                layout.MinWidth = value;
                break;
            case "MinHeight":
                layout.MinHeight = value;
                break;
            case "MaxWidth":
                layout.MaxWidth = value;
                break;
            case "MaxHeight":
                layout.MaxHeight = value;
                break;
            case "FlexibleWidth":
                if (float.TryParse(value, out var flexWidth))
                    layout.FlexibleWidth = flexWidth;
                break;
            case "FlexibleHeight":
                if (float.TryParse(value, out var flexHeight))
                    layout.FlexibleHeight = flexHeight;
                break;
            case "Margin":
                layout.Margin = value;
                break;
            case "HorizontalAlignment":
                layout.HorizontalAlignment = value;
                break;
            case "VerticalAlignment":
                layout.VerticalAlignment = value;
                break;
        }
    }

    private static LayoutGroupInfo ParseLayoutGroup(XmlElement xmlElement)
    {
        var layoutGroup = new LayoutGroupInfo();

        foreach (XmlAttribute attr in xmlElement.Attributes)
        {
            var name = attr.LocalName;
            var value = attr.Value;

            switch (name)
            {
                case "Orientation":
                    layoutGroup.Orientation = value;
                    break;
                case "Spacing":
                    if (float.TryParse(value, out var spacing))
                        layoutGroup.Spacing = spacing;
                    break;
                case "Padding":
                    layoutGroup.Padding = value;
                    break;
                case "ChildAlignment":
                    layoutGroup.ChildAlignment = value;
                    break;
                case "ChildControlWidth":
                    if (bool.TryParse(value, out var ccw))
                        layoutGroup.ChildControlWidth = ccw;
                    break;
                case "ChildControlHeight":
                    if (bool.TryParse(value, out var cch))
                        layoutGroup.ChildControlHeight = cch;
                    break;
                case "ChildForceExpandWidth":
                    if (bool.TryParse(value, out var cfew))
                        layoutGroup.ChildForceExpandWidth = cfew;
                    break;
                case "ChildForceExpandHeight":
                    if (bool.TryParse(value, out var cfeh))
                        layoutGroup.ChildForceExpandHeight = cfeh;
                    break;
                case "ReverseArrangement":
                    if (bool.TryParse(value, out var ra))
                        layoutGroup.ReverseArrangement = ra;
                    break;
            }
        }

        return layoutGroup;
    }

    private static ImageInfo ParseImage(XmlElement xmlElement)
    {
        var image = new ImageInfo();

        foreach (XmlAttribute attr in xmlElement.Attributes)
        {
            var name = attr.LocalName;
            var value = attr.Value;

            switch (name)
            {
                case "Color":
                    image.Color = value;
                    break;
                case "PreserveAspect":
                    if (bool.TryParse(value, out var preserveAspect))
                        image.PreserveAspect = preserveAspect;
                    break;
                case "ImageType":
                    image.ImageType = value;
                    break;
            }
        }

        // Parse nested Image.Source element
        foreach (XmlNode child in xmlElement.ChildNodes)
        {
            if (child is XmlElement sourceElement && sourceElement.LocalName == "Image.Source")
            {
                image.Source = ParseSource(sourceElement);
                break;
            }
        }

        return image;
    }

    private static SourceInfo ParseSource(XmlElement xmlElement)
    {
        var source = new SourceInfo();

        foreach (XmlAttribute attr in xmlElement.Attributes)
        {
            var name = attr.LocalName;
            var value = attr.Value;

            switch (name)
            {
                case "EmbeddedResource":
                    source.EmbeddedResource = value;
                    break;
                case "Url":
                    source.Url = value;
                    break;
                case "FilePath":
                    source.FilePath = value;
                    break;
                case "PixelsPerUnit":
                    if (float.TryParse(value, out var pixelsPerUnit))
                        source.PixelsPerUnit = pixelsPerUnit;
                    break;
                case "Pivot":
                    source.Pivot = value;
                    break;
                case "WrapMode":
                    source.WrapMode = value;
                    break;
                case "FilterMode":
                    source.FilterMode = value;
                    break;
                case "GenerateMipmaps":
                    if (bool.TryParse(value, out var generateMipmaps))
                        source.GenerateMipmaps = generateMipmaps;
                    break;
            }
        }

        return source;
    }

    private static bool IsBindingExpression(string value)
    {
        var trimmed = value.Trim();
        return trimmed.StartsWith("{Binding", StringComparison.OrdinalIgnoreCase)
               && trimmed.EndsWith("}");
    }

    private static BindingInfo? ParseBinding(string expression, string targetProperty)
    {
        var trimmed = expression.Trim();
        if (!trimmed.StartsWith("{Binding", StringComparison.OrdinalIgnoreCase))
            return null;

        var binding = new BindingInfo { TargetProperty = targetProperty };

        // Remove {Binding and }
        var content = trimmed.Substring(8, trimmed.Length - 9).Trim();

        // Parse the content
        var parts = SplitBindingContent(content);
        var isFirstPart = true;

        foreach (var part in parts)
        {
            var trimmedPart = part.Trim();
            if (string.IsNullOrEmpty(trimmedPart))
                continue;

            // First part without '=' is the path
            if (isFirstPart && !trimmedPart.Contains("="))
            {
                binding.Path = trimmedPart;
                isFirstPart = false;
                continue;
            }

            isFirstPart = false;

            // Parse key=value pairs
            var eqIndex = trimmedPart.IndexOf('=');
            if (eqIndex <= 0)
                continue;

            var key = trimmedPart.Substring(0, eqIndex).Trim();
            var value = trimmedPart.Substring(eqIndex + 1).Trim();

            switch (key.ToLowerInvariant())
            {
                case "path":
                    binding.Path = value;
                    break;
                case "mode":
                    binding.Mode = value;
                    break;
                case "converter":
                    binding.Converter = value;
                    break;
                case "converterparameter":
                    binding.ConverterParameter = value;
                    break;
                case "stringformat":
                    binding.StringFormat = value;
                    break;
                case "fallbackvalue":
                    binding.FallbackValue = value;
                    break;
            }
        }

        return string.IsNullOrEmpty(binding.Path) ? null : binding;
    }

    private static List<string> SplitBindingContent(string content)
    {
        var parts = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        foreach (var c in content)
        {
            if (c == '\'' || c == '"')
            {
                inQuotes = !inQuotes;
                current.Append(c);
            }
            else if (c == ',' && !inQuotes)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            parts.Add(current.ToString());

        return parts;
    }

    private static void CollectNamedElements(ViewElement element, List<ViewElement> namedElements)
    {
        if (!string.IsNullOrEmpty(element.Name))
        {
            namedElements.Add(element);
        }

        foreach (var child in element.Children)
        {
            CollectNamedElements(child, namedElements);
        }
    }
}

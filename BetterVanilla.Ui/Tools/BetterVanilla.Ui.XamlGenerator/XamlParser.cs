using System.Xml;
using BetterVanilla.Ui.XamlGenerator.Models;

namespace BetterVanilla.Ui.XamlGenerator;

/// <summary>
/// Parses .bvui.xml files into ViewDefinition objects.
/// </summary>
public sealed partial class XamlParser
{
    private const string MsXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    private const string BvuiXamlNamespace = "http://schemas.bettervanilla.ui/2025/xaml";
    private const string BvuiNamespace = "http://schemas.bettervanilla.ui/2025";

    // Known event names that should be treated as event handlers
    private static readonly HashSet<string> KnownEvents =
    [
        "Click", "Clicked", "ValueChanged", "IsOnChanged",
        "TextChanged", "SelectionChanged", "Submitted"
    ];

    // Known layout attributes
    private static readonly HashSet<string> LayoutAttributes =
    [
        "Width", "Height", "MinWidth", "MinHeight", "MaxWidth", "MaxHeight",
        "FlexibleWidth", "FlexibleHeight",
        "Margin", "Padding", "HorizontalAlignment", "VerticalAlignment",
        // LayoutGroup attributes (for Panel)
        "Background", "Orientation", "Spacing", "ChildAlignment", "ChildControlWidth", "ChildControlHeight",
        "ChildForceExpandWidth", "ChildForceExpandHeight", "ReverseArrangement"
    ];

    public ViewDefinition Parse(string filePath)
    {
        var doc = new XmlDocument();
        doc.Load(filePath);

        var definition = new ViewDefinition
        {
            SourceFile = filePath
        };

        if (doc.DocumentElement == null)
        {
            throw new InvalidOperationException($"Empty XML document: {filePath}");
        }

        // Parse x:Class (support both Microsoft and BVUI namespaces)
        var classAttr = doc.DocumentElement.GetAttributeNode("Class", BvuiXamlNamespace)
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
            if (fileName.EndsWith(".bvui", StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName[..^5];
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

            // Handle x:Name (support both Microsoft and BVUI namespaces)
            if ((attr.NamespaceURI is BvuiXamlNamespace or MsXamlNamespace && name == "Name") || attr.Name == "x:Name")
            {
                element.Name = value;
                continue;
            }

            // Skip x:Class, xmlns declarations (both xmlns="..." and xmlns:prefix="..."), and xsi attributes
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

            // Literal property value
            element.LiteralProperties[name] = value;
        }

        // Parse child elements
        foreach (XmlNode child in xmlElement.ChildNodes)
        {
            if (child is XmlElement childElement)
            {
                // Check if it's a property element like <Button.TextStyle> or <InputField.PlaceholderStyle>
                if (childElement.LocalName.Contains('.'))
                {
                    var parts = childElement.LocalName.Split('.', 2);
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
                if (float.TryParse(value, out var width))
                    layout.Width = width;
                break;
            case "Height":
                if (float.TryParse(value, out var height))
                    layout.Height = height;
                break;
            case "MinWidth":
                if (float.TryParse(value, out var minWidth))
                    layout.MinWidth = minWidth;
                break;
            case "MinHeight":
                if (float.TryParse(value, out var minHeight))
                    layout.MinHeight = minHeight;
                break;
            case "MaxWidth":
                if (float.TryParse(value, out var maxWidth))
                    layout.MaxWidth = maxWidth;
                break;
            case "MaxHeight":
                if (float.TryParse(value, out var maxHeight))
                    layout.MaxHeight = maxHeight;
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
            case "Padding":
                layout.Padding = value;
                break;
            case "HorizontalAlignment":
                layout.HorizontalAlignment = value;
                break;
            case "VerticalAlignment":
                layout.VerticalAlignment = value;
                break;
            // LayoutGroup properties
            case "Background":
                layout.Background = value;
                break;
            case "Orientation":
                layout.Orientation = value;
                break;
            case "Spacing":
                if (float.TryParse(value, out var spacing))
                    layout.Spacing = spacing;
                break;
            case "ChildAlignment":
                layout.ChildAlignment = value;
                break;
            case "ChildControlWidth":
                if (bool.TryParse(value, out var ccw))
                    layout.ChildControlWidth = ccw;
                break;
            case "ChildControlHeight":
                if (bool.TryParse(value, out var cch))
                    layout.ChildControlHeight = cch;
                break;
            case "ChildForceExpandWidth":
                if (bool.TryParse(value, out var cfew))
                    layout.ChildForceExpandWidth = cfew;
                break;
            case "ChildForceExpandHeight":
                if (bool.TryParse(value, out var cfeh))
                    layout.ChildForceExpandHeight = cfeh;
                break;
            case "ReverseArrangement":
                if (bool.TryParse(value, out var ra))
                    layout.ReverseArrangement = ra;
                break;
        }
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
        var content = trimmed[8..^1].Trim();

        // Parse the content
        var parts = SplitBindingContent(content);
        var isFirstPart = true;

        foreach (var part in parts)
        {
            var trimmedPart = part.Trim();
            if (string.IsNullOrEmpty(trimmedPart))
                continue;

            // First part without '=' is the path
            if (isFirstPart && !trimmedPart.Contains('='))
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

            var key = trimmedPart[..eqIndex].Trim();
            var value = trimmedPart[(eqIndex + 1)..].Trim();

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
            if (c is '\'' or '"')
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

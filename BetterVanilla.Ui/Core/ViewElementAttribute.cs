using System;

namespace BetterVanilla.Ui.Core;

/// <summary>
/// Marks a property as a view element that will be populated by the view initialization.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ViewElementAttribute : Attribute
{
    /// <summary>
    /// The name of the element in the view (x:Name).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Whether this element is required (throws if not found).
    /// </summary>
    public bool Required { get; set; } = true;

    public ViewElementAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// Specifies the alias path for loading a prefab from an AssetBundle.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class AliasAttribute : Attribute
{
    /// <summary>
    /// The alias path (e.g., "Controls/Toggle").
    /// </summary>
    public string Path { get; }

    public AliasAttribute(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }
}

/// <summary>
/// Marks a method as an event handler for a view element.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ViewEventAttribute : Attribute
{
    /// <summary>
    /// The name of the element to bind to.
    /// </summary>
    public string ElementName { get; }

    /// <summary>
    /// The name of the event to handle (e.g., "Click", "ValueChanged").
    /// </summary>
    public string EventName { get; }

    public ViewEventAttribute(string elementName, string eventName)
    {
        ElementName = elementName ?? throw new ArgumentNullException(nameof(elementName));
        EventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
    }
}

/// <summary>
/// Marks a class as a view with the specified BVUI file.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewAttribute : Attribute
{
    /// <summary>
    /// The path to the .bvui.xml file.
    /// </summary>
    public string BvuiFile { get; }

    public ViewAttribute(string bvuiFile)
    {
        BvuiFile = bvuiFile ?? throw new ArgumentNullException(nameof(bvuiFile));
    }
}

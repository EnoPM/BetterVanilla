using System;
using BetterVanilla.Ui.Binding;
using UnityEngine;

namespace BetterVanilla.Ui.Core;

/// <summary>
/// Interface for UI controls that can be used in views.
/// </summary>
public interface IViewControl : IDisposable
{
    /// <summary>
    /// The name of this control (x:Name in XAML).
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// The underlying Unity GameObject.
    /// </summary>
    GameObject GameObject { get; }

    /// <summary>
    /// The RectTransform of this control.
    /// </summary>
    RectTransform RectTransform { get; }

    /// <summary>
    /// Whether the control is visible.
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// Whether the control is enabled/interactive.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets a bindable property by name.
    /// </summary>
    IBindableProperty? GetBindableProperty(string propertyName);

    /// <summary>
    /// Initializes the control after creation.
    /// </summary>
    void Initialize();
}

/// <summary>
/// Interface for controls that can contain child controls.
/// </summary>
public interface IContainerControl : IViewControl
{
    /// <summary>
    /// Adds a child control to this container.
    /// </summary>
    void AddChild(IViewControl child);

    /// <summary>
    /// Removes a child control from this container.
    /// </summary>
    void RemoveChild(IViewControl child);

    /// <summary>
    /// Gets all child controls.
    /// </summary>
    IViewControl[] GetChildren();
}

/// <summary>
/// Interface for controls that have a text property.
/// </summary>
public interface ITextControl : IViewControl
{
    /// <summary>
    /// The text content of the control.
    /// </summary>
    string Text { get; set; }
}

/// <summary>
/// Interface for controls that support text styling (TMP properties).
/// </summary>
public interface ITextStyleControl : ITextControl
{
    /// <summary>
    /// Font size in points.
    /// </summary>
    float FontSize { get; set; }

    /// <summary>
    /// Text color.
    /// </summary>
    Color TextColor { get; set; }

    /// <summary>
    /// Text alignment options.
    /// </summary>
    TMPro.TextAlignmentOptions TextAlignment { get; set; }

    /// <summary>
    /// Font style (Bold, Italic, etc.).
    /// </summary>
    TMPro.FontStyles FontStyle { get; set; }

    /// <summary>
    /// Character spacing (in em units).
    /// </summary>
    float CharacterSpacing { get; set; }

    /// <summary>
    /// Line spacing adjustment.
    /// </summary>
    float LineSpacing { get; set; }

    /// <summary>
    /// Word spacing adjustment.
    /// </summary>
    float WordSpacing { get; set; }

    /// <summary>
    /// Whether word wrapping is enabled.
    /// </summary>
    bool WordWrapping { get; set; }

    /// <summary>
    /// Text overflow mode.
    /// </summary>
    TMPro.TextOverflowModes TextOverflow { get; set; }

    /// <summary>
    /// Whether rich text tags are enabled.
    /// </summary>
    bool RichText { get; set; }

    /// <summary>
    /// Whether auto-sizing is enabled.
    /// </summary>
    bool AutoSize { get; set; }

    /// <summary>
    /// Minimum font size for auto-sizing.
    /// </summary>
    float MinFontSize { get; set; }

    /// <summary>
    /// Maximum font size for auto-sizing.
    /// </summary>
    float MaxFontSize { get; set; }

    /// <summary>
    /// Text margins (left, top, right, bottom).
    /// </summary>
    Vector4 TextMargin { get; set; }
}

/// <summary>
/// Interface for controls that have a styleable text label (Button, Toggle, Slider).
/// Properties are prefixed with "Label" to distinguish from the control's main properties.
/// </summary>
public interface ILabelStyleControl : ITextControl
{
    float LabelFontSize { get; set; }
    Color LabelTextColor { get; set; }
    TMPro.TextAlignmentOptions LabelTextAlignment { get; set; }
    TMPro.FontStyles LabelFontStyle { get; set; }
    float LabelCharacterSpacing { get; set; }
    float LabelLineSpacing { get; set; }
    float LabelWordSpacing { get; set; }
    bool LabelWordWrapping { get; set; }
    TMPro.TextOverflowModes LabelTextOverflow { get; set; }
    bool LabelRichText { get; set; }
    bool LabelAutoSize { get; set; }
    float LabelMinFontSize { get; set; }
    float LabelMaxFontSize { get; set; }
    Vector4 LabelTextMargin { get; set; }
}

/// <summary>
/// Interface for controls that have a styleable placeholder text (InputField).
/// Properties are prefixed with "Placeholder" to distinguish from input text properties.
/// </summary>
public interface IPlaceholderStyleControl : IViewControl
{
    float PlaceholderFontSize { get; set; }
    Color PlaceholderTextColor { get; set; }
    TMPro.TextAlignmentOptions PlaceholderTextAlignment { get; set; }
    TMPro.FontStyles PlaceholderFontStyle { get; set; }
    float PlaceholderCharacterSpacing { get; set; }
    float PlaceholderLineSpacing { get; set; }
    float PlaceholderWordSpacing { get; set; }
    bool PlaceholderWordWrapping { get; set; }
    TMPro.TextOverflowModes PlaceholderTextOverflow { get; set; }
    bool PlaceholderRichText { get; set; }
    bool PlaceholderAutoSize { get; set; }
    float PlaceholderMinFontSize { get; set; }
    float PlaceholderMaxFontSize { get; set; }
    Vector4 PlaceholderTextMargin { get; set; }
}

/// <summary>
/// Interface for controls that can be clicked.
/// </summary>
public interface IClickableControl : IViewControl
{
    /// <summary>
    /// Event raised when the control is clicked.
    /// </summary>
    event Action? Clicked;
}

/// <summary>
/// Interface for controls that have a value.
/// </summary>
public interface IValueControl<T> : IViewControl
{
    /// <summary>
    /// The current value of the control.
    /// </summary>
    T Value { get; set; }

    /// <summary>
    /// Event raised when the value changes.
    /// </summary>
    event Action<T>? ValueChanged;
}

/// <summary>
/// Interface for controls that support Unity button color transitions (ColorBlock).
/// </summary>
public interface IButtonColorsControl : IClickableControl
{
    /// <summary>
    /// Color when the button is in its normal state.
    /// </summary>
    Color NormalColor { get; set; }

    /// <summary>
    /// Color when the button is highlighted (hovered).
    /// </summary>
    Color HighlightedColor { get; set; }

    /// <summary>
    /// Color when the button is pressed.
    /// </summary>
    Color PressedColor { get; set; }

    /// <summary>
    /// Color when the button is selected.
    /// </summary>
    Color SelectedColor { get; set; }

    /// <summary>
    /// Color when the button is disabled.
    /// </summary>
    Color DisabledColor { get; set; }

    /// <summary>
    /// Multiplier applied to the colors (default: 1).
    /// </summary>
    float ColorMultiplier { get; set; }

    /// <summary>
    /// Duration of the color transition in seconds.
    /// </summary>
    float FadeDuration { get; set; }
}

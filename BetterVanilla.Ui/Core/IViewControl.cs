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

using System;
using System.Collections.Generic;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using BetterVanilla.Ui.Helpers;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Base class for all UI controls.
/// </summary>
public abstract class BaseControl : MonoBehaviour, IViewControl
{
    private readonly Dictionary<string, IBindableProperty> _bindableProperties = new();
    protected readonly CompositeDisposable Disposables = new();

    private string _name = string.Empty;
    private bool _isEnabled = true;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public GameObject GameObject => gameObject;

    public RectTransform RectTransform => (RectTransform)transform;

    public bool IsVisible
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public virtual bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            OnEnabledChanged(value);
        }
    }

    protected virtual void Awake()
    {
        RegisterBindableProperties();
    }

    protected virtual void OnDestroy()
    {
        Dispose();
    }

    public virtual void Initialize()
    {
    }

    public IBindableProperty? GetBindableProperty(string propertyName)
    {
        return _bindableProperties.TryGetValue(propertyName, out var prop) ? prop : null;
    }

    protected void RegisterBindableProperty(string name, IBindableProperty property)
    {
        _bindableProperties[name] = property;
    }

    protected BindableProperty<T> CreateBindableProperty<T>(
        string name,
        Func<T> getter,
        Action<T> setter)
    {
        var property = new DelegateBindableProperty<T>(getter, setter);
        RegisterBindableProperty(name, property);
        return property;
    }

    protected virtual void RegisterBindableProperties()
    {
        // Override in derived classes to register bindable properties
    }

    protected virtual void OnEnabledChanged(bool enabled)
    {
        // Override in derived classes to handle enabled state changes
    }

    public virtual void Dispose()
    {
        Disposables.Dispose();
        _bindableProperties.Clear();
    }
}

/// <summary>
/// A bindable property backed by getter/setter delegates.
/// </summary>
internal sealed class DelegateBindableProperty<T> : BindableProperty<T>
{
    private readonly Func<T> _getter;
    private readonly Action<T> _setter;

    public DelegateBindableProperty(Func<T> getter, Action<T> setter)
    {
        _getter = getter;
        _setter = setter;
    }

    public new T? Value
    {
        get => _getter();
        set
        {
            if (value != null)
            {
                _setter(value);
            }
        }
    }

    public new object? GetValue() => _getter();

    public new void SetValue(object? value)
    {
        if (value is T typedValue)
        {
            _setter(typedValue);
        }
    }
}

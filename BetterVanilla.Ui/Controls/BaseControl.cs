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

    public string Name
    {
        get => field;
        set => field = value;
    } = string.Empty;

    public GameObject GameObject => gameObject;

    public RectTransform RectTransform => transform.Cast<RectTransform>();

    #region Layout Properties

    /// <summary>
    /// Width in pixels. -1 for auto.
    /// </summary>
    public float? Width
    {
        get;
        set
        {
            field = value;
            ApplySize();
        }
    }

    /// <summary>
    /// Height in pixels. -1 for auto.
    /// </summary>
    public float? Height
    {
        get;
        set
        {
            field = value;
            ApplySize();
        }
    }

    /// <summary>
    /// Minimum width in pixels.
    /// </summary>
    public float? MinWidth
    {
        get;
        set
        {
            field = value;
            ApplyMinSize();
        }
    }

    /// <summary>
    /// Minimum height in pixels.
    /// </summary>
    public float? MinHeight
    {
        get;
        set
        {
            field = value;
            ApplyMinSize();
        }
    }

    /// <summary>
    /// Maximum width in pixels.
    /// </summary>
    public float? MaxWidth { get; set; }

    /// <summary>
    /// Maximum height in pixels.
    /// </summary>
    public float? MaxHeight { get; set; }

    /// <summary>
    /// Flexible width for LayoutGroup expansion. Higher values = more expansion.
    /// </summary>
    public float? FlexibleWidth
    {
        get;
        set
        {
            field = value;
            ApplyFlexibleSize();
        }
    }

    /// <summary>
    /// Flexible height for LayoutGroup expansion. Higher values = more expansion.
    /// </summary>
    public float? FlexibleHeight
    {
        get;
        set
        {
            field = value;
            ApplyFlexibleSize();
        }
    }

    /// <summary>
    /// External margin (spacing outside the control).
    /// </summary>
    public Thickness? Margin
    {
        get;
        set
        {
            field = value;
            ApplyMargin();
        }
    }

    /// <summary>
    /// Horizontal alignment within parent. None = keep prefab's default.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment
    {
        get;
        set
        {
            field = value;
            ApplyAlignment();
        }
    } = HorizontalAlignment.None;

    /// <summary>
    /// Vertical alignment within parent. None = keep prefab's default.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
        get;
        set
        {
            field = value;
            ApplyAlignment();
        }
    } = VerticalAlignment.None;

    private void ApplySize()
    {
        RectTransform.SetPreferredSize(Width, Height);
    }

    private void ApplyMinSize()
    {
        RectTransform.SetMinSize(MinWidth, MinHeight);
    }

    private void ApplyMargin()
    {
        if (Margin.HasValue)
        {
            RectTransform.SetMargin(Margin.Value);
        }
    }

    private void ApplyAlignment()
    {
        RectTransform.SetAlignment(HorizontalAlignment, VerticalAlignment);
    }

    private void ApplyFlexibleSize()
    {
        RectTransform.SetFlexibleSize(FlexibleWidth, FlexibleHeight);
    }

    /// <summary>
    /// Applies all layout properties at once.
    /// </summary>
    public void ApplyLayout()
    {
        Helpers.UiDebugger.LogControlWithParent(gameObject, $"[BEFORE] '{Name}' W={Width} H={Height} HA={HorizontalAlignment} VA={VerticalAlignment}");

        // Order matters: alignment sets anchors, which affects how sizeDelta is interpreted
        ApplyAlignment();
        ApplySize();
        ApplyMinSize();
        ApplyFlexibleSize();
        ApplyMargin();

        Helpers.UiDebugger.LogControlWithParent(gameObject, $"[AFTER] '{Name}'");
    }

    #endregion

    private readonly BindableProperty<bool> _isVisibleProperty = new();
    private readonly BindableProperty<bool> _isEnabledProperty = new();

    public bool IsVisible
    {
        get => gameObject.activeSelf;
        set
        {
            gameObject.SetActive(value);
            _isVisibleProperty.Value = value;
        }
    }

    public virtual bool IsEnabled
    {
        get => field;
        set
        {
            field = value;
            _isEnabledProperty.Value = value;
            OnEnabledChanged(value);
        }
    } = true;

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
        return _bindableProperties.GetValueOrDefault(propertyName);
    }

    protected void RegisterBindableProperty(string propertyName, IBindableProperty property)
    {
        _bindableProperties[propertyName] = property;
    }

    protected BindableProperty<T> CreateBindableProperty<T>(
        string propertyName,
        Func<T> getter,
        Action<T> setter)
    {
        var property = new DelegateBindableProperty<T>(getter, setter);
        RegisterBindableProperty(propertyName, property);
        return property;
    }

    protected virtual void RegisterBindableProperties()
    {
        // Register common bindable properties
        RegisterBindableProperty("IsVisible", _isVisibleProperty);
        RegisterBindableProperty("IsEnabled", _isEnabledProperty);

        // Sync bindable property changes to actual properties
        _isVisibleProperty.ValueChanged += value =>
        {
            if (value is bool boolValue)
            {
                gameObject.SetActive(boolValue);
            }
        };

        _isEnabledProperty.ValueChanged += value =>
        {
            if (value is bool boolValue)
            {
                IsEnabled = boolValue;
            }
        };
    }

    protected virtual void OnEnabledChanged(bool state)
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

using System;
using System.Collections;
using System.Collections.Generic;
using BetterVanilla.Ui.Extensions;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using BetterVanilla.Ui.Helpers;
using EnoUnityLoader.Il2Cpp.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Base class for all UI controls.
/// </summary>
public abstract class BaseControl : MonoBehaviour, IViewControl
{
    private readonly Dictionary<string, IBindableProperty> _bindableProperties = new();
    protected CompositeDisposable Disposables { get; } = new();

    public string Name { get; set; } = string.Empty;

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
    public float? MaxWidth
    {
        get;
        set
        {
            field = value;
            ApplyMaxSize();
        }
    }

    /// <summary>
    /// Maximum height in pixels.
    /// </summary>
    public float? MaxHeight
    {
        get;
        set
        {
            field = value;
            ApplyMaxSize();
        }
    }

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

    private void ApplyMaxSize()
    {
        RectTransform.SetMaxSize(MaxWidth, MaxHeight);
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

    #region Percentage-based dimensions

    // Pending percentage values (null = not set as percentage)
    private float? _pendingWidthPercent;
    private float? _pendingHeightPercent;
    private float? _pendingMinWidthPercent;
    private float? _pendingMinHeightPercent;
    private float? _pendingMaxWidthPercent;
    private float? _pendingMaxHeightPercent;
    private Coroutine? _percentageCoroutine;

    /// <summary>
    /// Gets the reference size for percentage calculations.
    /// Uses parent RectTransform size if available, otherwise uses screen size.
    /// </summary>
    private Vector2 GetReferenceSize()
    {
        var parentRect = transform.parent?.TryCast<RectTransform>();
        if (parentRect == null)
        {
            return new Vector2(Screen.width, Screen.height);
        }
        // Force layout rebuild to ensure parent has correct size
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        return parentRect.rect.size;
        // No parent - use screen size
    }

    /// <summary>
    /// Sets width as a percentage of parent (or screen if no parent).
    /// The calculation is deferred until the layout is stabilized.
    /// </summary>
    /// <param name="percent">Percentage value (e.g., 50 for 50%)</param>
    public void SetWidthPercent(float percent)
    {
        _pendingWidthPercent = percent;
        SchedulePercentageApplication();
    }

    /// <summary>
    /// Sets height as a percentage of parent (or screen if no parent).
    /// The calculation is deferred until the layout is stabilized.
    /// </summary>
    /// <param name="percent">Percentage value (e.g., 50 for 50%)</param>
    public void SetHeightPercent(float percent)
    {
        _pendingHeightPercent = percent;
        SchedulePercentageApplication();
    }

    /// <summary>
    /// Sets minimum width as a percentage of parent (or screen if no parent).
    /// The calculation is deferred until the layout is stabilized.
    /// </summary>
    /// <param name="percent">Percentage value (e.g., 50 for 50%)</param>
    public void SetMinWidthPercent(float percent)
    {
        _pendingMinWidthPercent = percent;
        SchedulePercentageApplication();
    }

    /// <summary>
    /// Sets minimum height as a percentage of parent (or screen if no parent).
    /// The calculation is deferred until the layout is stabilized.
    /// </summary>
    /// <param name="percent">Percentage value (e.g., 50 for 50%)</param>
    public void SetMinHeightPercent(float percent)
    {
        _pendingMinHeightPercent = percent;
        SchedulePercentageApplication();
    }

    /// <summary>
    /// Sets maximum width as a percentage of parent (or screen if no parent).
    /// The calculation is deferred until the layout is stabilized.
    /// </summary>
    /// <param name="percent">Percentage value (e.g., 50 for 50%)</param>
    public void SetMaxWidthPercent(float percent)
    {
        _pendingMaxWidthPercent = percent;
        SchedulePercentageApplication();
    }

    /// <summary>
    /// Sets maximum height as a percentage of parent (or screen if no parent).
    /// The calculation is deferred until the layout is stabilized.
    /// </summary>
    /// <param name="percent">Percentage value (e.g., 50 for 50%)</param>
    public void SetMaxHeightPercent(float percent)
    {
        _pendingMaxHeightPercent = percent;
        SchedulePercentageApplication();
    }

    /// <summary>
    /// Schedules the percentage application to run after layout stabilization.
    /// </summary>
    private void SchedulePercentageApplication()
    {
        if (_percentageCoroutine != null) return;
        _percentageCoroutine = this.StartCoroutine(CoApplyPercentages());
    }

    /// <summary>
    /// Coroutine that waits for layout to stabilize, then applies percentage-based dimensions.
    /// </summary>
    private IEnumerator CoApplyPercentages()
    {
        // Wait for end of frame to ensure all layouts are calculated
        yield return new WaitForEndOfFrame();

        // Wait one more frame to ensure parent layouts are fully resolved
        yield return null;

        ApplyPendingPercentages();
        _percentageCoroutine = null;
    }

    /// <summary>
    /// Applies all pending percentage-based dimensions.
    /// </summary>
    private void ApplyPendingPercentages()
    {
        var refSize = GetReferenceSize();

        if (_pendingWidthPercent.HasValue)
        {
            Width = refSize.x * (_pendingWidthPercent.Value / 100f);
            _pendingWidthPercent = null;
        }

        if (_pendingHeightPercent.HasValue)
        {
            Height = refSize.y * (_pendingHeightPercent.Value / 100f);
            _pendingHeightPercent = null;
        }

        if (_pendingMinWidthPercent.HasValue)
        {
            MinWidth = refSize.x * (_pendingMinWidthPercent.Value / 100f);
            _pendingMinWidthPercent = null;
        }

        if (_pendingMinHeightPercent.HasValue)
        {
            MinHeight = refSize.y * (_pendingMinHeightPercent.Value / 100f);
            _pendingMinHeightPercent = null;
        }

        if (_pendingMaxWidthPercent.HasValue)
        {
            MaxWidth = refSize.x * (_pendingMaxWidthPercent.Value / 100f);
            _pendingMaxWidthPercent = null;
        }

        if (_pendingMaxHeightPercent.HasValue)
        {
            MaxHeight = refSize.y * (_pendingMaxHeightPercent.Value / 100f);
            _pendingMaxHeightPercent = null;
        }

        // Re-apply layout after percentage calculations
        ApplyLayout();
    }

    #endregion

    /// <summary>
    /// Applies all layout properties at once.
    /// </summary>
    public void ApplyLayout()
    {
        UiDebugger.LogControlWithParent(gameObject, $"[BEFORE] '{Name}' W={Width} H={Height} HA={HorizontalAlignment} VA={VerticalAlignment}");

        // Order matters: alignment sets anchors, which affects how sizeDelta is interpreted
        ApplyAlignment();
        ApplySize();
        ApplyMinSize();
        ApplyMaxSize();
        ApplyFlexibleSize();
        ApplyMargin();

        UiDebugger.LogControlWithParent(gameObject, $"[AFTER] '{Name}'");
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
        get;
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
        RegisterBindableProperty(nameof(IsVisible), _isVisibleProperty);
        RegisterBindableProperty(nameof(IsEnabled), _isEnabledProperty);

        // Sync initial values with actual property values
        // This ensures bindings work correctly even when the bound value equals the default
        _isVisibleProperty.Value = IsVisible;
        _isEnabledProperty.Value = IsEnabled;

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
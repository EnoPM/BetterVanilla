using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using BetterVanilla.Ui.Binding.Converters;
using BetterVanilla.Ui.Helpers;

namespace BetterVanilla.Ui.Binding;

/// <summary>
/// Manages data bindings between view models and UI controls.
/// </summary>
public sealed class BindingEngine : IDisposable
{
    private readonly List<ActiveBinding> _activeBindings = [];
    private readonly Dictionary<string, IValueConverter> _converters = new(StringComparer.OrdinalIgnoreCase);
    private object? _dataContext;
    private bool _disposed;

    public BindingEngine()
    {
        RegisterDefaultConverters();
    }

    /// <summary>
    /// Gets or sets the data context (typically a ViewModel).
    /// </summary>
    public object? DataContext
    {
        get => _dataContext;
        set
        {
            if (_dataContext == value)
                return;

            // Unsubscribe from old context
            if (_dataContext is INotifyPropertyChanged oldNotify)
            {
                oldNotify.PropertyChanged -= OnSourcePropertyChanged;
            }

            _dataContext = value;

            // Subscribe to new context
            if (_dataContext is INotifyPropertyChanged newNotify)
            {
                newNotify.PropertyChanged += OnSourcePropertyChanged;
            }

            // Refresh all bindings
            RefreshAllBindings();
        }
    }

    /// <summary>
    /// Registers a converter for use in bindings.
    /// </summary>
    public void RegisterConverter(string name, IValueConverter converter)
    {
        _converters[name] = converter;
    }

    /// <summary>
    /// Gets a registered converter by name.
    /// </summary>
    public IValueConverter? GetConverter(string name)
    {
        return _converters.TryGetValue(name, out var converter) ? converter : null;
    }

    /// <summary>
    /// Creates and activates a binding between source and target.
    /// </summary>
    public IDisposable Bind(BindingDefinition definition, IBindableProperty target)
    {
        var binding = new ActiveBinding(this, definition, target);
        _activeBindings.Add(binding);

        // Initial sync
        binding.UpdateTarget();

        return new ActionDisposable(() =>
        {
            binding.Dispose();
            _activeBindings.Remove(binding);
        });
    }

    /// <summary>
    /// Creates a binding from a binding expression.
    /// </summary>
    public IDisposable Bind(BindingExpression expression, string targetProperty, IBindableProperty target)
    {
        var definition = new BindingDefinition(expression.Path, targetProperty)
        {
            Mode = expression.Mode,
            StringFormat = expression.StringFormat
        };

        if (!string.IsNullOrEmpty(expression.ConverterName))
        {
            definition.Converter = GetConverter(expression.ConverterName);
            definition.ConverterParameter = expression.ConverterParameter;
        }

        return Bind(definition, target);
    }

    /// <summary>
    /// Refreshes all active bindings.
    /// </summary>
    public void RefreshAllBindings()
    {
        foreach (var binding in _activeBindings)
        {
            binding.UpdateTarget();
        }
    }

    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        foreach (var binding in _activeBindings)
        {
            if (binding.IsAffectedByProperty(e.PropertyName))
            {
                binding.UpdateTarget();
            }
        }
    }

    private void RegisterDefaultConverters()
    {
        RegisterConverter("BoolToVisibility", BoolToVisibilityConverter.Instance);
        RegisterConverter("BoolToAlpha", BoolToAlphaConverter.Instance);
        RegisterConverter("StringFormat", StringFormatConverter.Instance);
        RegisterConverter("IntToString", IntToStringConverter.Instance);
        RegisterConverter("FloatToString", FloatToStringConverter.Instance);
        RegisterConverter("EnumToString", EnumToStringConverter.Instance);
        RegisterConverter("EnumToInt", EnumToIntConverter.Instance);
        RegisterConverter("EnumToBool", EnumToBoolConverter.Instance);
        RegisterConverter("PassThrough", PassThroughConverter.Instance);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        foreach (var binding in _activeBindings)
        {
            binding.Dispose();
        }
        _activeBindings.Clear();

        if (_dataContext is INotifyPropertyChanged notify)
        {
            notify.PropertyChanged -= OnSourcePropertyChanged;
        }
    }
}

/// <summary>
/// Interface for properties that can be bound.
/// </summary>
public interface IBindableProperty
{
    /// <summary>
    /// Gets the current value of the property.
    /// </summary>
    object? GetValue();

    /// <summary>
    /// Sets the value of the property.
    /// </summary>
    void SetValue(object? value);

    /// <summary>
    /// Event raised when the property value changes (for TwoWay binding).
    /// </summary>
    event Action<object?>? ValueChanged;
}

/// <summary>
/// Represents an active binding between a source and target.
/// </summary>
internal sealed class ActiveBinding : IDisposable
{
    private readonly BindingEngine _engine;
    private readonly BindingDefinition _definition;
    private readonly IBindableProperty _target;
    private readonly string[] _pathParts;
    private bool _isUpdating;
    private bool _disposed;

    public ActiveBinding(BindingEngine engine, BindingDefinition definition, IBindableProperty target)
    {
        _engine = engine;
        _definition = definition;
        _target = target;
        _pathParts = definition.SourcePath.Split('.');

        // Subscribe to target changes for TwoWay binding
        if (definition.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource)
        {
            _target.ValueChanged += OnTargetValueChanged;
        }
    }

    public bool IsAffectedByProperty(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return true; // Refresh all

        // Check if this binding's path starts with or equals the changed property
        return _pathParts.Length > 0 &&
               string.Equals(_pathParts[0], propertyName, StringComparison.Ordinal);
    }

    public void UpdateTarget()
    {
        if (_disposed || _isUpdating)
            return;

        if (_definition.Mode == BindingMode.OneWayToSource)
            return;

        _isUpdating = true;
        try
        {
            var value = GetSourceValue();
            value = ApplyConverter(value, toTarget: true);
            value = ApplyStringFormat(value);
            _target.SetValue(value);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void UpdateSource(object? value)
    {
        if (_disposed || _isUpdating)
            return;

        if (_definition.Mode is not (BindingMode.TwoWay or BindingMode.OneWayToSource))
            return;

        _isUpdating = true;
        try
        {
            value = ApplyConverter(value, toTarget: false);
            SetSourceValue(value);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void OnTargetValueChanged(object? newValue)
    {
        if (_definition.UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
        {
            UpdateSource(newValue);
        }
    }

    private object? GetSourceValue()
    {
        var current = _engine.DataContext;

        foreach (var part in _pathParts)
        {
            if (current == null)
                return _definition.FallbackValue;

            var prop = current.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return _definition.FallbackValue;

            current = prop.GetValue(current);
        }

        return current ?? _definition.FallbackValue;
    }

    private void SetSourceValue(object? value)
    {
        var current = _engine.DataContext;
        if (current == null)
            return;

        // Navigate to parent of target property
        for (var i = 0; i < _pathParts.Length - 1; i++)
        {
            var prop = current.GetType().GetProperty(_pathParts[i], BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return;

            current = prop.GetValue(current);
            if (current == null)
                return;
        }

        // Set the final property
        var finalProp = current.GetType().GetProperty(
            _pathParts[^1],
            BindingFlags.Public | BindingFlags.Instance);

        if (finalProp?.CanWrite == true)
        {
            // Convert value to target type if needed
            var convertedValue = ConvertToType(value, finalProp.PropertyType);
            finalProp.SetValue(current, convertedValue);
        }
    }

    private object? ApplyConverter(object? value, bool toTarget)
    {
        if (_definition.Converter == null)
            return value;

        return toTarget
            ? _definition.Converter.Convert(value, typeof(object), _definition.ConverterParameter)
            : _definition.Converter.ConvertBack(value, typeof(object), _definition.ConverterParameter);
    }

    private object? ApplyStringFormat(object? value)
    {
        if (string.IsNullOrEmpty(_definition.StringFormat))
            return value;

        return string.Format(_definition.StringFormat, value);
    }

    private static object? ConvertToType(object? value, Type targetType)
    {
        if (value == null)
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        if (targetType.IsInstanceOfType(value))
            return value;

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            return Convert.ChangeType(value, underlyingType);
        }
        catch
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_definition.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource)
        {
            _target.ValueChanged -= OnTargetValueChanged;
        }
    }
}

/// <summary>
/// A simple bindable property wrapper.
/// </summary>
public class BindableProperty<T> : IBindableProperty
{
    private T? _value;

    public T? Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    public event Action<object?>? ValueChanged;

    public object? GetValue() => _value;

    public void SetValue(object? value)
    {
        Value = value is T typedValue ? typedValue : default;
    }
}

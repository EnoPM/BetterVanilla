using System;
using System.Collections.Generic;
using System.Reflection;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Controls;
using BetterVanilla.Ui.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Core;

/// <summary>
/// Base class for all views. Generated code derives from this class.
/// </summary>
public abstract class BaseView : MonoBehaviour, IDisposable
{
    // Use lazy initialization for IL2CPP compatibility (field initializers can fail on MonoBehaviour)
    private Dictionary<string, IViewControl>? _namedElements;
    private BindingEngine? _bindingEngine;
    private CompositeDisposable? _disposables;

    private Dictionary<string, IViewControl> NamedElements => _namedElements ??= new();
    protected BindingEngine BindingEngine => _bindingEngine ??= new();
    protected CompositeDisposable Disposables => _disposables ??= new();

    private bool _isInitialized;

    /// <summary>
    /// The data context (ViewModel) for this view.
    /// </summary>
    public object? DataContext
    {
        get;
        set
        {
            field = value;
            BindingEngine.DataContext = value;
            OnDataContextChanged(value);
        }
    }

    /// <summary>
    /// Whether the view has been initialized.
    /// </summary>
    public bool IsInitialized => _isInitialized;

    protected virtual void Awake()
    {
        InitializeComponent();
    }

    protected virtual void OnDestroy()
    {
        Dispose();
    }

    /// <summary>
    /// Initializes the view components. Called automatically in Awake.
    /// Override in generated code to set up elements.
    /// </summary>
    protected virtual void InitializeComponent()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        // Find and register elements marked with ViewElementAttribute
        RegisterAttributeElements();

        // Set up bindings defined in generated code
        SetupBindings();

        // Set up event handlers
        SetupEventHandlers();

        // Force layout rebuild after all elements are set up
        ForceLayoutRebuild();

        // Custom initialization
        OnInitialized();
    }

    /// <summary>
    /// Forces a layout rebuild on all layout groups in this view.
    /// </summary>
    protected void ForceLayoutRebuild()
    {
        var rectTransform = transform.Cast<RectTransform>();
        if (rectTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    /// <summary>
    /// Registers an element by name.
    /// </summary>
    protected void RegisterElement(string elementName, IViewControl control)
    {
        NamedElements[elementName] = control;
        control.Name = elementName;
    }

    /// <summary>
    /// Gets a named element.
    /// </summary>
    protected T? GetElement<T>(string elementName) where T : class, IViewControl
    {
        return NamedElements.TryGetValue(elementName, out var element) ? element as T : null;
    }

    /// <summary>
    /// Gets a named element or throws if not found.
    /// </summary>
    protected T GetRequiredElement<T>(string elementName) where T : class, IViewControl
    {
        var element = GetElement<T>(elementName);
        if (element == null)
        {
            throw new InvalidOperationException($"Required element '{elementName}' of type {typeof(T).Name} not found.");
        }
        return element;
    }

    /// <summary>
    /// Finds a child element by path.
    /// </summary>
    protected T? FindElement<T>(string path) where T : Component
    {
        var child = transform.Find(path);
        return child != null ? child.GetComponent<T>() : null;
    }

    /// <summary>
    /// Instantiates a prefab as a child of this view.
    /// </summary>
    protected GameObject InstantiatePrefab(string bundleName, string prefabPath, Transform? parent = null)
    {
        return AssetBundleManager.Instance.InstantiatePrefab(bundleName, prefabPath, parent ?? transform);
    }

    /// <summary>
    /// Instantiates a prefab and gets/adds a control component.
    /// </summary>
    protected T InstantiateControl<T>(string bundleName, string prefabPath, Transform? parent = null)
        where T : BaseControl
    {
        var instance = InstantiatePrefab(bundleName, prefabPath, parent);
        var control = instance.GetComponent<T>();
        if (control == null)
        {
            control = instance.AddComponent<T>();
        }
        control.Initialize();
        return control;
    }

    /// <summary>
    /// Creates a binding between a source property and a control property.
    /// </summary>
    protected IDisposable Bind(string sourcePath, IViewControl control, string targetProperty, BindingMode mode)
    {
        var bindableProperty = control.GetBindableProperty(targetProperty);
        if (bindableProperty == null)
        {
            UiLogger.LogWarning($"Property '{targetProperty}' not found on control '{control.Name}'");
            return EmptyDisposable.Instance;
        }

        var definition = new BindingDefinition(sourcePath, targetProperty) { Mode = mode };
        return BindingEngine.Bind(definition, bindableProperty);
    }

    /// <summary>
    /// Creates a binding from a binding expression string.
    /// </summary>
    protected IDisposable Bind(string expression, IViewControl control, string targetProperty)
    {
        var parsed = BindingExpression.Parse(expression);
        if (parsed == null)
        {
            UiLogger.LogWarning($"Failed to parse binding expression: {expression}");
            return EmptyDisposable.Instance;
        }

        var bindableProperty = control.GetBindableProperty(targetProperty);
        if (bindableProperty == null)
        {
            UiLogger.LogWarning($"Property '{targetProperty}' not found on control '{control.Name}'");
            return EmptyDisposable.Instance;
        }

        return BindingEngine.Bind(parsed, targetProperty, bindableProperty);
    }

    private void RegisterAttributeElements()
    {
        var type = GetType();
        var members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var member in members)
        {
            var attr = member.GetCustomAttribute<ViewElementAttribute>();
            if (attr == null)
                continue;

            IViewControl? control = null;

            if (member is PropertyInfo prop && typeof(IViewControl).IsAssignableFrom(prop.PropertyType))
            {
                control = prop.GetValue(this) as IViewControl;
            }
            else if (member is FieldInfo field && typeof(IViewControl).IsAssignableFrom(field.FieldType))
            {
                control = field.GetValue(this) as IViewControl;
            }

            if (control != null)
            {
                RegisterElement(attr.Name, control);
            }
            else if (attr.Required)
            {
                UiLogger.LogWarning($"Required view element '{attr.Name}' is null in {type.Name}");
            }
        }
    }

    /// <summary>
    /// Override in generated code to set up bindings.
    /// </summary>
    protected virtual void SetupBindings()
    {
    }

    /// <summary>
    /// Override in generated code to set up event handlers.
    /// </summary>
    protected virtual void SetupEventHandlers()
    {
    }

    /// <summary>
    /// Called after initialization is complete.
    /// </summary>
    protected virtual void OnInitialized()
    {
    }

    /// <summary>
    /// Called when the DataContext changes.
    /// </summary>
    protected virtual void OnDataContextChanged(object? newDataContext)
    {
    }

    public virtual void Dispose()
    {
        _bindingEngine?.Dispose();
        _disposables?.Dispose();

        if (_namedElements != null)
        {
            foreach (var element in _namedElements.Values)
            {
                element.Dispose();
            }
            _namedElements.Clear();
        }
    }

    /// <summary>
    /// Gets the DataContext as a typed ViewModel.
    /// </summary>
    protected T? GetViewModel<T>() where T : class
    {
        return DataContext as T;
    }

    /// <summary>
    /// Gets the DataContext as a typed ViewModel, throwing if null or wrong type.
    /// </summary>
    protected T GetRequiredViewModel<T>() where T : class
    {
        return DataContext as T ?? throw new InvalidOperationException(
            $"DataContext is not set or is not of type {typeof(T).Name}.");
    }
}

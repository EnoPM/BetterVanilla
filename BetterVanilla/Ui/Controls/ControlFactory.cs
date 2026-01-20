using System;
using System.Collections.Generic;
using BetterVanilla.Ui.Core;
using BetterVanilla.Ui.Helpers;
using Il2CppInterop.Runtime;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Factory for creating controls from aliases at runtime.
/// </summary>
public sealed class ControlFactory
{
    private readonly Dictionary<string, ControlRegistration> _registrations = new();

    public static ControlFactory Instance { get; } = new();

    private ControlFactory()
    {
        RegisterDefaultControls();
    }

    /// <summary>
    /// Registers a control type with an alias.
    /// </summary>
    public void Register<T>(string alias, string bundleName, string prefabPath) where T : BaseControl
    {
        _registrations[alias] = new ControlRegistration(typeof(T), bundleName, prefabPath);
    }

    /// <summary>
    /// Creates a control from an alias.
    /// </summary>
    public T Create<T>(string alias, Transform? parent = null) where T : BaseControl
    {
        if (!_registrations.TryGetValue(alias, out var registration))
        {
            throw new InvalidOperationException($"No control registered for alias: {alias}");
        }

        if (!typeof(T).IsAssignableFrom(registration.ControlType))
        {
            throw new InvalidOperationException(
                $"Alias '{alias}' is registered for {registration.ControlType.Name}, not {typeof(T).Name}");
        }

        var instance = AssetBundleManager.Instance.InstantiatePrefab(
            registration.BundleName,
            registration.PrefabPath,
            parent);

        var control = instance.GetComponent<T>();
        if (control == null)
        {
            control = instance.AddComponent<T>();
        }

        control.Initialize();
        return control;
    }

    /// <summary>
    /// Creates a control from an alias (non-generic version).
    /// </summary>
    public IViewControl Create(string alias, Transform? parent = null)
    {
        if (!_registrations.TryGetValue(alias, out var registration))
        {
            throw new InvalidOperationException($"No control registered for alias: {alias}");
        }

        var instance = AssetBundleManager.Instance.InstantiatePrefab(
            registration.BundleName,
            registration.PrefabPath,
            parent);

        var il2cppType = Il2CppType.From(registration.ControlType);
        var control = instance.GetComponent(il2cppType) as BaseControl;
        if (control == null)
        {
            control = instance.AddComponent(il2cppType) as BaseControl;
        }

        control?.Initialize();
        return control ?? throw new InvalidOperationException($"Failed to create control for alias: {alias}");
    }

    /// <summary>
    /// Checks if an alias is registered.
    /// </summary>
    public bool IsRegistered(string alias) => _registrations.ContainsKey(alias);

    private void RegisterDefaultControls()
    {
        // Default registrations can be added here
        // These will be overridden by ui-aliases.json at runtime
    }

    private sealed class ControlRegistration
    {
        public Type ControlType { get; }
        public string BundleName { get; }
        public string PrefabPath { get; }

        public ControlRegistration(Type controlType, string bundleName, string prefabPath)
        {
            ControlType = controlType;
            BundleName = bundleName;
            PrefabPath = prefabPath;
        }
    }
}

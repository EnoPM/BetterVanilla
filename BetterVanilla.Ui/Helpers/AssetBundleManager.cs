using System;
using System.Collections.Generic;
using System.Reflection;
using Cpp2IL.Core.Extensions;
using UnityEngine;

namespace BetterVanilla.Ui.Helpers;

/// <summary>
/// Manages loading and caching of AssetBundles for UI prefabs.
/// </summary>
public sealed class AssetBundleManager : IDisposable
{
    private readonly Dictionary<string, AssetBundle> _loadedBundles = new();
    private readonly Dictionary<string, GameObject> _prefabCache = new();

    public static AssetBundleManager Instance { get; } = new();

    private AssetBundleManager() { }

    /// <summary>
    /// Loads an AssetBundle from an embedded resource.
    /// </summary>
    public AssetBundle LoadFromEmbeddedResource(string bundleName, Assembly? assembly = null)
    {
        if (_loadedBundles.TryGetValue(bundleName, out var cached))
            return cached;

        assembly ??= Assembly.GetCallingAssembly();

        var resourceStream = assembly.GetManifestResourceStream(bundleName);
        if (resourceStream == null)
        {
            throw new InvalidOperationException(
                $"Unable to find embedded resource: {bundleName} in assembly {assembly.FullName}");
        }

        var bundle = AssetBundle.LoadFromMemory(resourceStream.ReadBytes());
        _loadedBundles[bundleName] = bundle;
        return bundle;
    }

    /// <summary>
    /// Loads a prefab from a specific AssetBundle.
    /// </summary>
    public GameObject LoadPrefab(string bundleName, string prefabPath)
    {
        var cacheKey = $"{bundleName}::{prefabPath}";

        if (_prefabCache.TryGetValue(cacheKey, out var cached))
            return cached;

        if (!_loadedBundles.TryGetValue(bundleName, out var bundle))
        {
            throw new InvalidOperationException(
                $"AssetBundle '{bundleName}' is not loaded. Call LoadFromEmbeddedResource first.");
        }

        var prefab = bundle.LoadAsset<GameObject>(prefabPath);
        if (prefab == null)
        {
            throw new InvalidOperationException(
                $"Prefab '{prefabPath}' not found in bundle '{bundleName}'");
        }

        _prefabCache[cacheKey] = prefab;
        return prefab;
    }

    /// <summary>
    /// Instantiates a prefab from a bundle with an optional parent transform.
    /// </summary>
    public GameObject InstantiatePrefab(string bundleName, string prefabPath, Transform? parent = null)
    {
        var prefab = LoadPrefab(bundleName, prefabPath);
        var instance = parent != null
            ? UnityEngine.Object.Instantiate(prefab, parent)
            : UnityEngine.Object.Instantiate(prefab);
        return instance;
    }

    /// <summary>
    /// Instantiates a prefab and gets or adds a specific component.
    /// </summary>
    public T InstantiatePrefab<T>(string bundleName, string prefabPath, Transform? parent = null)
        where T : Component
    {
        var instance = InstantiatePrefab(bundleName, prefabPath, parent);
        var component = instance.GetComponent<T>();
        if (component == null)
        {
            component = instance.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// Unloads all cached bundles and prefabs.
    /// </summary>
    public void Dispose()
    {
        _prefabCache.Clear();

        foreach (var bundle in _loadedBundles.Values)
        {
            bundle.Unload(true);
        }
        _loadedBundles.Clear();
    }

    /// <summary>
    /// Unloads a specific bundle.
    /// </summary>
    public void UnloadBundle(string bundleName, bool unloadAllLoadedObjects = false)
    {
        if (!_loadedBundles.TryGetValue(bundleName, out var bundle))
            return;

        // Remove prefabs from this bundle from cache
        var keysToRemove = new List<string>();
        foreach (var key in _prefabCache.Keys)
        {
            if (key.StartsWith(bundleName + "::"))
                keysToRemove.Add(key);
        }
        foreach (var key in keysToRemove)
        {
            _prefabCache.Remove(key);
        }

        bundle.Unload(unloadAllLoadedObjects);
        _loadedBundles.Remove(bundleName);
    }
}

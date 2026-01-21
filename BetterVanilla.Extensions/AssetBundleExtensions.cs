using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterVanilla.Extensions;

public static class AssetBundleExtensions
{
    private static readonly Dictionary<string, AssetBundle> AssetBundleCache = [];
    private static readonly Dictionary<string, GameObject> PrefabCache = [];
    private static readonly Dictionary<string, Sprite> SpriteCache = [];

    public static AssetBundle LoadFromFile(string filePath, bool cache = true)
    {
        if (cache && AssetBundleCache.TryGetValue(filePath, out var cached))
        {
            if (cached != null)
            {
                return cached;
            }
            AssetBundleCache.Remove(filePath);
        }

        var file = File.ReadAllBytes(filePath);
        var bundle = AssetBundle.LoadFromMemory(file);

        if (bundle == null)
        {
            throw new InvalidOperationException($"Unable to read AssetBundle from file: {filePath}");
        }

        if (cache)
        {
            bundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            AssetBundleCache.Add(filePath, bundle);
        }

        return bundle;
    }

    extension(Assembly assembly)
    {
        public AssetBundle LoadAssetBundle(string resourceName, bool cache = true)
        {
            if (cache && AssetBundleCache.TryGetValue(resourceName, out var cached))
            {
                if (cached != null)
                {
                    return cached;
                }
                AssetBundleCache.Remove(resourceName);
            }

            var resource = assembly.GetResourceBytes(resourceName);
            var bundle = AssetBundle.LoadFromMemory(resource);

            if (bundle == null)
            {
                throw new InvalidOperationException($"Unable to read AssetBundle from embedded resource: {resourceName} in assembly '{assembly.GetName().Name}'");
            }

            if (cache)
            {
                bundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                AssetBundleCache.Add(resourceName, bundle);
            }

            return bundle;
        }
    }

    extension(AssetBundle assetBundle)
    {
        public T LoadPrefab<T>(string path, bool cache = false) where T : Component
        {
            if (cache && PrefabCache.TryGetValue(path, out var cached))
            {
                if (cached != null)
                {
                    return cached.GetComponent<T>();
                }
                PrefabCache.Remove(path);
            }

            var gameObject = assetBundle.LoadAsset<GameObject>(path);

            if (cache)
            {
                gameObject.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                PrefabCache.Add(path, gameObject);
            }

            return gameObject.GetComponent<T>();
        }

        public GameObject InstantiatePrefab(string path, Transform? parent = null, bool cache = true)
        {
            if (cache && PrefabCache.TryGetValue(path, out var cached))
            {
                if (cached != null)
                {
                    return parent != null
                        ? Object.Instantiate(cached, parent)
                        : Object.Instantiate(cached);
                }
                PrefabCache.Remove(path);
            }

            var prefab = assetBundle.LoadAsset<GameObject>(path);

            if (cache)
            {
                prefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                PrefabCache.Add(path, prefab);
            }

            return parent != null
                ? Object.Instantiate(prefab, parent)
                : Object.Instantiate(prefab);
        }

        public T InstantiatePrefab<T>(string path, Transform? parent = null, bool cache = true) where T : Component
            => assetBundle.InstantiatePrefab(path, parent, cache).GetComponent<T>();

        public Sprite LoadSprite(string path, bool cache = true)
        {
            if (cache && SpriteCache.TryGetValue(path, out var sprite))
            {
                if (sprite != null)
                {
                    return sprite;
                }
                SpriteCache.Remove(path);
            }

            sprite = assetBundle.LoadAsset<Sprite>(path);

            if (cache)
            {
                sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                SpriteCache.Add(path, sprite);
            }

            return sprite;
        }
    }
}
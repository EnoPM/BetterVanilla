using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace BetterVanilla.Ui.Helpers;

/// <summary>
/// Type of image source.
/// </summary>
public enum ImageSourceType
{
    None,
    EmbeddedResource,
    Url,
    FilePath
}

/// <summary>
/// Helper class for loading images with sprite creation options.
/// </summary>
public static class ImageLoadingHelper
{
    /// <summary>
    /// Default pixels per unit for sprite creation.
    /// </summary>
    public const float DefaultPixelsPerUnit = 100f;

    /// <summary>
    /// Default pivot point (center).
    /// </summary>
    public static readonly Vector2 DefaultPivot = new(0.5f, 0.5f);

    /// <summary>
    /// Parses a pivot string in format "x,y" to a Vector2.
    /// </summary>
    /// <param name="value">Pivot string (e.g., "0.5,0.5" for center)</param>
    /// <returns>Parsed Vector2, or default pivot if parsing fails</returns>
    public static Vector2 ParsePivot(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DefaultPivot;

        var parts = value.Split(',');
        if (parts.Length != 2)
            return DefaultPivot;

        if (float.TryParse(parts[0].Trim(), out var x) &&
            float.TryParse(parts[1].Trim(), out var y))
        {
            return new Vector2(
                Mathf.Clamp01(x),
                Mathf.Clamp01(y)
            );
        }

        return DefaultPivot;
    }

    /// <summary>
    /// Creates a sprite from raw image bytes.
    /// </summary>
    public static Sprite? CreateSpriteFromBytes(
        byte[] bytes,
        float pixelsPerUnit = DefaultPixelsPerUnit,
        Vector2? pivot = null,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp,
        FilterMode filterMode = FilterMode.Bilinear,
        bool generateMipmaps = true)
    {
        var actualPivot = pivot ?? DefaultPivot;

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, generateMipmaps)
        {
            wrapMode = wrapMode,
            filterMode = filterMode
        };

        if (!texture.LoadImage(bytes))
        {
            UnityEngine.Object.Destroy(texture);
            return null;
        }

        texture.wrapMode = wrapMode;
        texture.filterMode = filterMode;

        if (generateMipmaps)
        {
            texture.Apply(true, false);
        }

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            actualPivot,
            pixelsPerUnit
        );
    }

    #region Embedded Resource Loading

    /// <summary>
    /// Loads a sprite from an embedded resource with specified options.
    /// </summary>
    public static Sprite? LoadSpriteFromEmbeddedResource(
        string resourceName,
        Assembly? assembly = null,
        float pixelsPerUnit = DefaultPixelsPerUnit,
        Vector2? pivot = null,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp,
        FilterMode filterMode = FilterMode.Bilinear,
        bool generateMipmaps = true)
    {
        assembly ??= Assembly.GetCallingAssembly();

        var fullResourceName = FindResourceName(resourceName, assembly);
        if (fullResourceName == null)
        {
            Debug.LogWarning($"[ImageLoadingHelper] Resource not found: {resourceName}");
            return null;
        }

        try
        {
            using var stream = assembly.GetManifestResourceStream(fullResourceName);
            if (stream == null)
            {
                Debug.LogWarning($"[ImageLoadingHelper] Could not open stream for resource: {fullResourceName}");
                return null;
            }

            var bytes = new byte[stream.Length];
            _ = stream.Read(bytes, 0, bytes.Length);

            return CreateSpriteFromBytes(bytes, pixelsPerUnit, pivot, wrapMode, filterMode, generateMipmaps);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ImageLoadingHelper] Error loading sprite '{resourceName}': {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Legacy method - redirects to LoadSpriteFromEmbeddedResource.
    /// </summary>
    public static Sprite? LoadSprite(
        string resourceName,
        Assembly? assembly = null,
        float pixelsPerUnit = DefaultPixelsPerUnit,
        Vector2? pivot = null,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp,
        FilterMode filterMode = FilterMode.Bilinear,
        bool generateMipmaps = true)
    {
        return LoadSpriteFromEmbeddedResource(resourceName, assembly, pixelsPerUnit, pivot, wrapMode, filterMode, generateMipmaps);
    }

    #endregion

    #region File Path Loading

    /// <summary>
    /// Loads a sprite from a local file path.
    /// </summary>
    public static Sprite? LoadSpriteFromFile(
        string filePath,
        float pixelsPerUnit = DefaultPixelsPerUnit,
        Vector2? pivot = null,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp,
        FilterMode filterMode = FilterMode.Bilinear,
        bool generateMipmaps = true)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[ImageLoadingHelper] File not found: {filePath}");
                return null;
            }

            var bytes = File.ReadAllBytes(filePath);
            return CreateSpriteFromBytes(bytes, pixelsPerUnit, pivot, wrapMode, filterMode, generateMipmaps);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ImageLoadingHelper] Error loading sprite from file '{filePath}': {ex.Message}");
            return null;
        }
    }

    #endregion

    #region URL Loading

    private static readonly HttpClient HttpClient = new();

    /// <summary>
    /// Loads a sprite from a URL asynchronously using .NET HttpClient.
    /// </summary>
    /// <param name="url">The URL to load from</param>
    /// <param name="onComplete">Callback when loading is complete (sprite may be null on error)</param>
    /// <param name="pixelsPerUnit">Pixels per unit (default: 100)</param>
    /// <param name="pivot">Pivot point (default: 0.5,0.5)</param>
    /// <param name="wrapMode">Texture wrap mode (default: Clamp)</param>
    /// <param name="filterMode">Texture filter mode (default: Bilinear)</param>
    /// <param name="generateMipmaps">Whether to generate mipmaps (default: true)</param>
    /// <returns>Coroutine enumerator</returns>
    public static IEnumerator LoadSpriteFromUrlAsync(
        string url,
        Action<Sprite?> onComplete,
        float pixelsPerUnit = DefaultPixelsPerUnit,
        Vector2? pivot = null,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp,
        FilterMode filterMode = FilterMode.Bilinear,
        bool generateMipmaps = true)
    {
        byte[]? imageBytes = null;
        Exception? error = null;
        var downloadComplete = false;

        // Start async download on thread pool
        Task.Run(async () =>
        {
            try
            {
                imageBytes = await HttpClient.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                downloadComplete = true;
            }
        });

        // Wait for download to complete
        while (!downloadComplete)
        {
            yield return null;
        }

        if (error != null)
        {
            Debug.LogWarning($"[ImageLoadingHelper] Failed to load image from URL '{url}': {error.Message}");
            onComplete(null);
            yield break;
        }

        if (imageBytes == null || imageBytes.Length == 0)
        {
            Debug.LogWarning($"[ImageLoadingHelper] Empty response from URL: {url}");
            onComplete(null);
            yield break;
        }

        // Create sprite from downloaded bytes
        var sprite = CreateSpriteFromBytes(imageBytes, pixelsPerUnit, pivot, wrapMode, filterMode, generateMipmaps);
        onComplete(sprite);
    }

    /// <summary>
    /// Loads a sprite from a URL synchronously (blocking).
    /// Use LoadSpriteFromUrlAsync for non-blocking loading.
    /// </summary>
    public static Sprite? LoadSpriteFromUrl(
        string url,
        float pixelsPerUnit = DefaultPixelsPerUnit,
        Vector2? pivot = null,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp,
        FilterMode filterMode = FilterMode.Bilinear,
        bool generateMipmaps = true)
    {
        try
        {
            var bytes = HttpClient.GetByteArrayAsync(url).GetAwaiter().GetResult();
            return CreateSpriteFromBytes(bytes, pixelsPerUnit, pivot, wrapMode, filterMode, generateMipmaps);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ImageLoadingHelper] Error loading sprite from URL '{url}': {ex.Message}");
            return null;
        }
    }

    #endregion

    /// <summary>
    /// Finds the full resource name, trying various patterns.
    /// </summary>
    private static string? FindResourceName(string resourceName, Assembly assembly)
    {
        var resourceNames = assembly.GetManifestResourceNames();

        // First, try exact match
        foreach (var name in resourceNames)
        {
            if (name.Equals(resourceName, StringComparison.OrdinalIgnoreCase))
                return name;
        }

        // Try ending with the resource name (handles namespace prefixes)
        foreach (var name in resourceNames)
        {
            if (name.EndsWith($".{resourceName}", StringComparison.OrdinalIgnoreCase))
                return name;
        }

        // Try just the filename
        var fileName = Path.GetFileName(resourceName);
        foreach (var name in resourceNames)
        {
            if (name.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase))
                return name;
        }

        return null;
    }
}

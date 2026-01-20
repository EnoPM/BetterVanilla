using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BetterVanilla.Ui.Helpers;

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
    /// Loads a sprite from an embedded resource with specified options.
    /// </summary>
    /// <param name="resourceName">The embedded resource name</param>
    /// <param name="assembly">The assembly containing the resource (null for calling assembly)</param>
    /// <param name="pixelsPerUnit">Pixels per unit (default: 100)</param>
    /// <param name="pivot">Pivot point (default: 0.5,0.5)</param>
    /// <param name="wrapMode">Texture wrap mode (default: Clamp)</param>
    /// <param name="filterMode">Texture filter mode (default: Bilinear)</param>
    /// <param name="generateMipmaps">Whether to generate mipmaps for better quality at different scales (default: true)</param>
    /// <returns>The created sprite, or null if loading fails</returns>
    public static Sprite? LoadSprite(
        string resourceName,
        Assembly? assembly = null,
        float pixelsPerUnit = DefaultPixelsPerUnit,
        Vector2? pivot = null,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp,
        FilterMode filterMode = FilterMode.Bilinear,
        bool generateMipmaps = true)
    {
        assembly ??= Assembly.GetCallingAssembly();
        var actualPivot = pivot ?? DefaultPivot;

        // Try to find the resource
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

            // Create texture with mipmap support
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, generateMipmaps)
            {
                wrapMode = wrapMode,
                filterMode = filterMode
            };

            if (!texture.LoadImage(bytes))
            {
                Debug.LogWarning($"[ImageLoadingHelper] Failed to load image data: {resourceName}");
                UnityEngine.Object.Destroy(texture);
                return null;
            }

            // Apply texture settings after loading and generate mipmaps
            texture.wrapMode = wrapMode;
            texture.filterMode = filterMode;

            if (generateMipmaps)
            {
                // Apply with updateMipmaps=true to generate mipmaps
                texture.Apply(true, false);
            }

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                actualPivot,
                pixelsPerUnit
            );

            return sprite;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ImageLoadingHelper] Error loading sprite '{resourceName}': {ex.Message}");
            return null;
        }
    }

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

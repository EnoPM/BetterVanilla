using System;
using System.IO;
using System.Reflection;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using BetterVanilla.Ui.Helpers;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Image control that can display images from embedded resources or sprites.
/// </summary>
public sealed class ImageControl : BaseControl, IShadowControl
{
    private ImageComponent? _component;
    private ShadowHelper? _shadow;
    private string? _source;
    private Assembly? _sourceAssembly;
    private float _pixelsPerUnit = ImageLoadingHelper.DefaultPixelsPerUnit;
    private Vector2 _pivot = ImageLoadingHelper.DefaultPivot;
    private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;
    private FilterMode _filterMode = FilterMode.Bilinear;
    private bool _isInitialized;

    /// <summary>
    /// The Unity Image component.
    /// </summary>
    public Image? Image => _component?.image;

    /// <summary>
    /// The embedded resource name or path to load the image from.
    /// Format: "Namespace.Folder.FileName.png" or just "FileName.png" if using SourceAssembly.
    /// </summary>
    public string? Source
    {
        get => _source;
        set
        {
            _source = value;
            if (_isInitialized)
            {
                LoadImageFromSource();
            }
        }
    }

    /// <summary>
    /// The assembly to load embedded resources from. Defaults to the calling assembly.
    /// </summary>
    public Assembly? SourceAssembly
    {
        get => _sourceAssembly;
        set
        {
            _sourceAssembly = value;
            if (!string.IsNullOrEmpty(_source))
            {
                LoadImageFromSource();
            }
        }
    }

    /// <summary>
    /// Directly set a sprite on the image.
    /// </summary>
    public Sprite? Sprite
    {
        get => _component?.image.sprite;
        set
        {
            if (_component != null)
            {
                _component.image.sprite = value;
            }
        }
    }

    /// <summary>
    /// The color tint applied to the image.
    /// </summary>
    public Color Color
    {
        get => _component?.image.color ?? Color.white;
        set
        {
            if (_component != null)
            {
                _component.image.color = value;
            }
        }
    }

    /// <summary>
    /// Whether to preserve the aspect ratio of the image.
    /// </summary>
    public bool PreserveAspect
    {
        get => _component?.image.preserveAspect ?? false;
        set
        {
            if (_component != null)
            {
                _component.image.preserveAspect = value;
            }
        }
    }

    /// <summary>
    /// The image type (Simple, Sliced, Tiled, Filled).
    /// </summary>
    public UnityEngine.UI.Image.Type ImageType
    {
        get => _component?.image.type ?? Image.Type.Simple;
        set
        {
            if (_component != null)
            {
                _component.image.type = value;
            }
        }
    }

    /// <summary>
    /// Pixels per unit for sprite creation (default: 100).
    /// </summary>
    public float PixelsPerUnit
    {
        get => _pixelsPerUnit;
        set
        {
            _pixelsPerUnit = value;
            if (_isInitialized && !string.IsNullOrEmpty(_source))
            {
                LoadImageFromSource();
            }
        }
    }

    /// <summary>
    /// Pivot point for sprite creation (normalized coordinates, default: 0.5,0.5 = center).
    /// </summary>
    public Vector2 Pivot
    {
        get => _pivot;
        set
        {
            _pivot = value;
            if (_isInitialized && !string.IsNullOrEmpty(_source))
            {
                LoadImageFromSource();
            }
        }
    }

    /// <summary>
    /// Texture wrap mode (default: Clamp).
    /// </summary>
    public TextureWrapMode WrapMode
    {
        get => _wrapMode;
        set
        {
            _wrapMode = value;
            if (_isInitialized && !string.IsNullOrEmpty(_source))
            {
                LoadImageFromSource();
            }
        }
    }

    /// <summary>
    /// Texture filter mode (default: Bilinear).
    /// </summary>
    public FilterMode FilterMode
    {
        get => _filterMode;
        set
        {
            _filterMode = value;
            if (_isInitialized && !string.IsNullOrEmpty(_source))
            {
                LoadImageFromSource();
            }
        }
    }

    /// <summary>
    /// Whether the image should be rendered using raycast target (for UI interaction).
    /// </summary>
    public bool RaycastTarget
    {
        get => _component?.image.raycastTarget ?? true;
        set
        {
            if (_component != null)
            {
                _component.image.raycastTarget = value;
            }
        }
    }

    #region IShadowControl

    public bool ShadowEnabled
    {
        get => _shadow?.Enabled ?? false;
        set { if (_shadow != null) _shadow.Enabled = value; }
    }

    public Color ShadowColor
    {
        get => _shadow?.Color ?? new Color(0, 0, 0, 0.5f);
        set { if (_shadow != null) _shadow.Color = value; }
    }

    public Vector2 ShadowDistance
    {
        get => _shadow?.Distance ?? new Vector2(1, -1);
        set { if (_shadow != null) _shadow.Distance = value; }
    }

    public bool ShadowUseGraphicAlpha
    {
        get => _shadow?.UseGraphicAlpha ?? true;
        set { if (_shadow != null) _shadow.UseGraphicAlpha = value; }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        _component = GetComponent<ImageComponent>();
        _shadow = new ShadowHelper(_component?.shadow);
        _isInitialized = true;

        // Load the image if Source was set before Awake
        if (!string.IsNullOrEmpty(_source))
        {
            LoadImageFromSource();
        }
    }

    private void LoadImageFromSource()
    {
        if (string.IsNullOrEmpty(_source) || _component == null)
            return;

        // Clean up previous sprite if exists
        if (_component.image.sprite != null && _component.image.sprite.texture != null)
        {
            var oldSprite = _component.image.sprite;
            var oldTexture = oldSprite.texture;
            _component.image.sprite = null;
            Destroy(oldSprite);
            Destroy(oldTexture);
        }

        // Use ImageLoadingHelper to load the sprite with all options
        var sprite = ImageLoadingHelper.LoadSprite(
            _source,
            _sourceAssembly,
            _pixelsPerUnit,
            _pivot,
            _wrapMode,
            _filterMode
        );

        if (sprite != null)
        {
            _component.image.sprite = sprite;
        }
    }

    /// <summary>
    /// Loads a texture from an embedded resource.
    /// </summary>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <param name="resourceName">The resource name. Can be partial (will search for matching resource).</param>
    /// <returns>The loaded texture, or null if not found.</returns>
    public static Texture2D? LoadTextureFromEmbeddedResource(Assembly assembly, string resourceName)
    {
        // Try exact match first
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            return LoadTextureFromStream(stream);
        }

        // Try to find a matching resource name
        var resourceNames = assembly.GetManifestResourceNames();
        foreach (var name in resourceNames)
        {
            if (name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("." + resourceName, StringComparison.OrdinalIgnoreCase))
            {
                using var matchedStream = assembly.GetManifestResourceStream(name);
                if (matchedStream != null)
                {
                    return LoadTextureFromStream(matchedStream);
                }
            }
        }

        BetterVanilla.Core.Ls.LogWarning($"[ImageControl] Embedded resource not found in assembly {assembly.GetName().Name}: {resourceName}");
        return null;
    }

    /// <summary>
    /// Searches for and loads a texture from any loaded assembly.
    /// </summary>
    /// <param name="resourceName">The resource name to search for.</param>
    /// <returns>The loaded texture, or null if not found.</returns>
    public static Texture2D? LoadTextureFromAnyAssembly(string resourceName)
    {
        // Try exact match first in all assemblies
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    return LoadTextureFromStream(stream);
                }
            }
            catch
            {
                // Ignore assemblies that can't be inspected
            }
        }

        // Try partial match in all assemblies
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var resourceNames = assembly.GetManifestResourceNames();
                foreach (var name in resourceNames)
                {
                    if (name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith("." + resourceName, StringComparison.OrdinalIgnoreCase) ||
                        name.Equals(resourceName, StringComparison.OrdinalIgnoreCase))
                    {
                        using var matchedStream = assembly.GetManifestResourceStream(name);
                        if (matchedStream != null)
                        {
                            return LoadTextureFromStream(matchedStream);
                        }
                    }
                }
            }
            catch
            {
                // Ignore assemblies that can't be inspected
            }
        }

        // Log available resources for debugging
        BetterVanilla.Core.Ls.LogWarning($"[ImageControl] Embedded resource not found in any assembly: {resourceName}");
        BetterVanilla.Core.Ls.LogWarning("[ImageControl] Available embedded resources:");
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var names = assembly.GetManifestResourceNames();
                foreach (var name in names)
                {
                    if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        BetterVanilla.Core.Ls.LogWarning($"  - {name}");
                    }
                }
            }
            catch
            {
                // Ignore
            }
        }
        return null;
    }

    private static Texture2D LoadTextureFromStream(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        var texture = new Texture2D(2, 2);
        if (texture.LoadImage(bytes))
        {
            return texture;
        }

        BetterVanilla.Core.Ls.LogWarning("[ImageControl] Failed to load texture from stream");
        return null!;
    }

    /// <summary>
    /// Loads an image from embedded resources using a specific assembly.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resource.</param>
    /// <param name="resourceName">The resource name.</param>
    public void LoadFromEmbeddedResource(Assembly assembly, string resourceName)
    {
        _sourceAssembly = assembly;
        Source = resourceName;
    }

    /// <summary>
    /// Loads an image from a byte array.
    /// </summary>
    /// <param name="imageData">The image data (PNG, JPG, etc.).</param>
    [HideFromIl2Cpp]
    public void LoadFromBytes(byte[] imageData)
    {
        if (_component == null) return;

        // Clean up previous sprite if exists
        if (_component.image.sprite != null && _component.image.sprite.texture != null)
        {
            var oldSprite = _component.image.sprite;
            var oldTexture = oldSprite.texture;
            _component.image.sprite = null;
            Destroy(oldSprite);
            Destroy(oldTexture);
        }

        // Create texture with mipmap support for better quality
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, true)
        {
            wrapMode = _wrapMode,
            filterMode = _filterMode
        };

        if (texture.LoadImage(imageData))
        {
            // Apply texture settings and generate mipmaps
            texture.wrapMode = _wrapMode;
            texture.filterMode = _filterMode;
            texture.Apply(true, false);

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                _pivot,
                _pixelsPerUnit
            );
            _component.image.sprite = sprite;
        }
    }

    public override void Dispose()
    {
        // Clean up created textures/sprites if needed
        if (_component?.image.sprite != null && _component.image.sprite.texture != null)
        {
            // Only destroy if we created it (not from an asset bundle)
            if (!string.IsNullOrEmpty(_source))
            {
                Destroy(_component.image.sprite.texture);
                Destroy(_component.image.sprite);
            }
        }
        base.Dispose();
    }
}

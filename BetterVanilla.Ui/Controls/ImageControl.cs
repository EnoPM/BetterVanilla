using System;
using System.Collections;
using System.Reflection;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using BetterVanilla.Ui.Helpers;
using EnoUnityLoader.Il2Cpp.Utils;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Image control that can display images from embedded resources, URLs, or local files.
/// </summary>
public sealed class ImageControl : BaseControl, IShadowControl
{
    private ImageComponent? _component;
    private ShadowHelper? _shadow;
    private string? _embeddedResource;
    private string? _url;
    private string? _filePath;
    private Assembly? _sourceAssembly;
    private float _pixelsPerUnit = ImageLoadingHelper.DefaultPixelsPerUnit;
    private Vector2 _pivot = ImageLoadingHelper.DefaultPivot;
    private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;
    private FilterMode _filterMode = FilterMode.Bilinear;
    private bool _generateMipmaps = true;
    private bool _isInitialized;
    private Coroutine? _loadingCoroutine;

    /// <summary>
    /// The Unity Image component.
    /// </summary>
    public Image? Image => _component?.image;

    #region Source Properties

    /// <summary>
    /// The embedded resource name or path to load the image from.
    /// </summary>
    public string? EmbeddedResource
    {
        get => _embeddedResource;
        set
        {
            _embeddedResource = value;
            _url = null;
            _filePath = null;
            if (_isInitialized && !string.IsNullOrEmpty(value))
            {
                LoadFromEmbeddedResource();
            }
        }
    }

    /// <summary>
    /// URL to load the image from (http/https).
    /// </summary>
    public string? Url
    {
        get => _url;
        set
        {
            _url = value;
            _embeddedResource = null;
            _filePath = null;
            if (_isInitialized && !string.IsNullOrEmpty(value))
            {
                LoadFromUrl();
            }
        }
    }

    /// <summary>
    /// Local file path to load the image from.
    /// </summary>
    public string? FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value;
            _embeddedResource = null;
            _url = null;
            if (_isInitialized && !string.IsNullOrEmpty(value))
            {
                LoadFromFile();
            }
        }
    }

    /// <summary>
    /// Legacy property - sets EmbeddedResource for backward compatibility.
    /// </summary>
    public string? Source
    {
        get => _embeddedResource ?? _url ?? _filePath;
        set => EmbeddedResource = value;
    }

    /// <summary>
    /// The assembly to load embedded resources from.
    /// </summary>
    public Assembly? SourceAssembly
    {
        get => _sourceAssembly;
        set
        {
            _sourceAssembly = value;
            if (_isInitialized && !string.IsNullOrEmpty(_embeddedResource))
            {
                LoadFromEmbeddedResource();
            }
        }
    }

    /// <summary>
    /// Gets the current source type.
    /// </summary>
    public ImageSourceType SourceType
    {
        get
        {
            if (!string.IsNullOrEmpty(_embeddedResource)) return ImageSourceType.EmbeddedResource;
            if (!string.IsNullOrEmpty(_url)) return ImageSourceType.Url;
            if (!string.IsNullOrEmpty(_filePath)) return ImageSourceType.FilePath;
            return ImageSourceType.None;
        }
    }

    #endregion

    #region Display Properties

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

    #endregion

    #region Loading Options

    /// <summary>
    /// Pixels per unit for sprite creation (default: 100).
    /// </summary>
    public float PixelsPerUnit
    {
        get => _pixelsPerUnit;
        set
        {
            _pixelsPerUnit = value;
            if (_isInitialized) ReloadCurrentSource();
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
            if (_isInitialized) ReloadCurrentSource();
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
            if (_isInitialized) ReloadCurrentSource();
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
            if (_isInitialized) ReloadCurrentSource();
        }
    }

    /// <summary>
    /// Whether to generate mipmaps (default: true).
    /// </summary>
    public bool GenerateMipmaps
    {
        get => _generateMipmaps;
        set
        {
            _generateMipmaps = value;
            if (_isInitialized) ReloadCurrentSource();
        }
    }

    #endregion

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

        // Load the image if a source was set before Awake
        ReloadCurrentSource();
    }

    private void ReloadCurrentSource()
    {
        switch (SourceType)
        {
            case ImageSourceType.EmbeddedResource:
                LoadFromEmbeddedResource();
                break;
            case ImageSourceType.Url:
                LoadFromUrl();
                break;
            case ImageSourceType.FilePath:
                LoadFromFile();
                break;
        }
    }

    private void CleanupPreviousSprite()
    {
        if (_component?.image.sprite != null && _component.image.sprite.texture != null)
        {
            var oldSprite = _component.image.sprite;
            var oldTexture = oldSprite.texture;
            _component.image.sprite = null;
            Destroy(oldSprite);
            Destroy(oldTexture);
        }
    }

    private void LoadFromEmbeddedResource()
    {
        if (string.IsNullOrEmpty(_embeddedResource) || _component == null)
            return;

        CleanupPreviousSprite();

        var sprite = ImageLoadingHelper.LoadSpriteFromEmbeddedResource(
            _embeddedResource,
            _sourceAssembly,
            _pixelsPerUnit,
            _pivot,
            _wrapMode,
            _filterMode,
            _generateMipmaps
        );

        if (sprite != null)
        {
            _component.image.sprite = sprite;
        }
    }

    private void LoadFromFile()
    {
        if (string.IsNullOrEmpty(_filePath) || _component == null)
            return;

        CleanupPreviousSprite();

        var sprite = ImageLoadingHelper.LoadSpriteFromFile(
            _filePath,
            _pixelsPerUnit,
            _pivot,
            _wrapMode,
            _filterMode,
            _generateMipmaps
        );

        if (sprite != null)
        {
            _component.image.sprite = sprite;
        }
    }

    private void LoadFromUrl()
    {
        if (string.IsNullOrEmpty(_url) || _component == null)
            return;

        // Cancel any previous loading coroutine
        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
        }

        CleanupPreviousSprite();

        _loadingCoroutine = this.StartCoroutine(CoLoadFromUrl());
    }

    private IEnumerator CoLoadFromUrl()
    {
        yield return ImageLoadingHelper.LoadSpriteFromUrlAsync(
            _url!,
            sprite =>
            {
                if (_component != null && sprite != null)
                {
                    _component.image.sprite = sprite;
                }
                _loadingCoroutine = null;
            },
            _pixelsPerUnit,
            _pivot,
            _wrapMode,
            _filterMode,
            _generateMipmaps
        );
    }

    /// <summary>
    /// Loads an image from a byte array.
    /// </summary>
    [HideFromIl2Cpp]
    public void LoadFromBytes(byte[] imageData)
    {
        if (_component == null) return;

        CleanupPreviousSprite();

        var sprite = ImageLoadingHelper.CreateSpriteFromBytes(
            imageData,
            _pixelsPerUnit,
            _pivot,
            _wrapMode,
            _filterMode,
            _generateMipmaps
        );

        if (sprite != null)
        {
            _component.image.sprite = sprite;
        }
    }

    public override void Dispose()
    {
        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = null;
        }

        CleanupPreviousSprite();
        base.Dispose();
    }
}

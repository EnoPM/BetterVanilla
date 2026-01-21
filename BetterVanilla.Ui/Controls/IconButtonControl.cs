using System;
using System.Collections;
using System.Reflection;
using BetterVanilla.Extensions;
using BetterVanilla.Ui.Extensions;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using BetterVanilla.Ui.Helpers;
using EnoUnityLoader.Il2Cpp.Utils;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A button control that displays an icon instead of text.
/// The icon is displayed as the button's background image.
/// </summary>
public sealed class IconButtonControl : BaseControl, IButtonColorsControl, IShadowControl
{
    private IconButtonComponent? _component;
    private ButtonColorsHelper? _buttonColors;
    private ShadowHelper? _shadow;
    private string? _embeddedResource;
    private string? _url;
    private string? _filePath;
    private Assembly? _sourceAssembly;
    private Color? _pendingColor;
    private bool? _pendingPreserveAspect;
    private Image.Type? _pendingImageType;
    private float _pixelsPerUnit = ImageLoadingHelper.DefaultPixelsPerUnit;
    private Vector2 _pivot = ImageLoadingHelper.DefaultPivot;
    private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;
    private FilterMode _filterMode = FilterMode.Bilinear;
    private bool _generateMipmaps = true;
    private bool _isInitialized;
    private Coroutine? _loadingCoroutine;

    public event Action? Clicked;

    #region Source Properties

    /// <summary>
    /// The embedded resource name or path to load the icon from.
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
    /// URL to load the icon from (http/https).
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
    /// Local file path to load the icon from.
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

    #region Icon Display Properties

    /// <summary>
    /// Directly set a sprite on the button's image.
    /// </summary>
    public Sprite? Icon
    {
        get => _component?.background.sprite;
        set
        {
            if (_component != null)
            {
                _component.background.sprite = value;
            }
        }
    }

    /// <summary>
    /// The color/tint applied to the button's image.
    /// </summary>
    public Color Color
    {
        get => _component?.background.color ?? _pendingColor ?? Color.white;
        set
        {
            if (_component != null)
            {
                _component.background.color = value;
            }
            else
            {
                _pendingColor = value;
            }
        }
    }

    /// <summary>
    /// Whether to preserve the aspect ratio of the icon.
    /// </summary>
    public bool PreserveAspect
    {
        get => _component?.background.preserveAspect ?? _pendingPreserveAspect ?? false;
        set
        {
            if (_component != null)
            {
                _component.background.preserveAspect = value;
            }
            else
            {
                _pendingPreserveAspect = value;
            }
        }
    }

    /// <summary>
    /// The image display type (Simple, Sliced, Tiled, Filled).
    /// </summary>
    public Image.Type ImageType
    {
        get => _component?.background.type ?? _pendingImageType ?? Image.Type.Simple;
        set
        {
            if (_component != null)
            {
                _component.background.type = value;
            }
            else
            {
                _pendingImageType = value;
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

    #region IButtonColorsControl

    public Color NormalColor
    {
        get => _buttonColors?.NormalColor ?? Color.white;
        set => _buttonColors?.NormalColor = value;
    }

    public Color HighlightedColor
    {
        get => _buttonColors?.HighlightedColor ?? Color.white;
        set => _buttonColors?.HighlightedColor = value;
    }

    public Color PressedColor
    {
        get => _buttonColors?.PressedColor ?? Color.gray;
        set => _buttonColors?.PressedColor = value;
    }

    public Color SelectedColor
    {
        get => _buttonColors?.SelectedColor ?? Color.white;
        set => _buttonColors?.SelectedColor = value;
    }

    public Color DisabledColor
    {
        get => _buttonColors?.DisabledColor ?? new Color(0.5f, 0.5f, 0.5f, 0.5f);
        set => _buttonColors?.DisabledColor = value;
    }

    public float ColorMultiplier
    {
        get => _buttonColors?.ColorMultiplier ?? 1f;
        set => _buttonColors?.ColorMultiplier = value;
    }

    public float FadeDuration
    {
        get => _buttonColors?.FadeDuration ?? 0.1f;
        set => _buttonColors?.FadeDuration = value;
    }

    #endregion

    #region IShadowControl

    public bool ShadowEnabled
    {
        get => _shadow?.Enabled ?? false;
        set => _shadow?.Enabled = value;
    }

    public Color ShadowColor
    {
        get => _shadow?.Color ?? new Color(0, 0, 0, 0.5f);
        set => _shadow?.Color = value;
    }

    public Vector2 ShadowDistance
    {
        get => _shadow?.Distance ?? new Vector2(1, -1);
        set => _shadow?.Distance = value;
    }

    public bool ShadowUseGraphicAlpha
    {
        get => _shadow?.UseGraphicAlpha ?? true;
        set => _shadow?.UseGraphicAlpha = value;
    }

    #endregion

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            if (_component != null)
            {
                _component.button.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _component = GetComponent<IconButtonComponent>();
        _buttonColors = new ButtonColorsHelper(_component?.button);
        _shadow = new ShadowHelper(_component?.shadow);

        if (_component != null)
        {
            _component.button.onClick.AddListener(OnClick);

            // Apply pending properties
            if (_pendingColor.HasValue)
            {
                _component.background.color = _pendingColor.Value;
            }

            if (_pendingPreserveAspect.HasValue)
            {
                _component.background.preserveAspect = _pendingPreserveAspect.Value;
            }

            if (_pendingImageType.HasValue)
            {
                _component.background.type = _pendingImageType.Value;
            }
        }

        _isInitialized = true;

        // Load the icon if a source was set before Awake
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
        if (_component?.background.sprite == null) return;
        var oldSprite = _component.background.sprite;
        _component.background.sprite = null;

        // Don't destroy assets that come from AssetBundles (they have DontUnloadUnusedAsset flag)
        if ((oldSprite.hideFlags & HideFlags.DontUnloadUnusedAsset) != 0) return;
        var oldTexture = oldSprite.texture;
        Destroy(oldSprite);
        if (oldTexture != null && (oldTexture.hideFlags & HideFlags.DontUnloadUnusedAsset) == 0)
        {
            Destroy(oldTexture);
        }
    }

    private void LoadFromEmbeddedResource()
    {
        if (string.IsNullOrEmpty(_embeddedResource) || _component == null) return;

        CleanupPreviousSprite();

        Sprite? sprite;

        // Check for AssetBundle syntax (bundleName::assetPath)
        if (ImageLoadingHelper.TryParseAssetBundlePath(_embeddedResource, out var bundleName, out var assetPath))
        {
            UiLogger.LogMessage($"Loading resource from embedded asset bundle: {_embeddedResource}");
            var assembly = _sourceAssembly ?? Assembly.GetCallingAssembly();
            var bundle = assembly.LoadAssetBundle(bundleName);
            sprite = bundle.LoadSprite(assetPath);
        }
        else
        {
            sprite = ImageLoadingHelper.LoadSpriteFromEmbeddedResource(
                _embeddedResource,
                _sourceAssembly,
                _pixelsPerUnit,
                _pivot,
                _wrapMode,
                _filterMode,
                _generateMipmaps
            );
        }

        if (sprite == null) return;
        sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        _component.background.sprite = sprite;
        _component.background.type = _pendingImageType ?? Image.Type.Simple;
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

        if (sprite == null) return;
        _component.background.sprite = sprite;
        _component.background.type = _pendingImageType ?? Image.Type.Simple;
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
                    _component.background.sprite = sprite;
                    _component.background.type = _pendingImageType ?? Image.Type.Simple;
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
    /// Loads an icon from a byte array.
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

        if (sprite == null) return;
        _component.background.sprite = sprite;
        _component.background.type = _pendingImageType ?? Image.Type.Simple;
    }

    private void OnClick()
    {
        if (IsEnabled)
        {
            Clicked?.Invoke();
        }
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_component != null)
        {
            _component.button.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = null;
        }

        if (_component != null)
        {
            _component.button.onClick.RemoveListener(OnClick);
        }

        CleanupPreviousSprite();
        Clicked = null;
        base.Dispose();
    }
}

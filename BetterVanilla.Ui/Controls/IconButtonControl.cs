using System;
using System.IO;
using System.Reflection;
using BetterVanilla.Ui.Core;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A button control that displays an icon instead of text.
/// The icon is displayed as the button's background image.
/// </summary>
public sealed class IconButtonControl : BaseControl, IClickableControl
{
    private Button? _button;
    private Image? _image;
    private string? _source;
    private Assembly? _sourceAssembly;
    private Color? _pendingColor;
    private bool? _pendingPreserveAspect;
    private bool _isInitialized;

    public event Action? Clicked;

    #region Icon Properties

    /// <summary>
    /// The embedded resource name or path to load the icon from.
    /// This sets the button's background image.
    /// </summary>
    public string? Source
    {
        get => _source;
        set
        {
            _source = value;
            if (_isInitialized)
            {
                LoadIconFromSource();
            }
        }
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
            if (_isInitialized && !string.IsNullOrEmpty(_source))
            {
                LoadIconFromSource();
            }
        }
    }

    /// <summary>
    /// Directly set a sprite on the button's image.
    /// </summary>
    public Sprite? Icon
    {
        get => _image?.sprite;
        set
        {
            if (_image != null)
            {
                _image.sprite = value;
            }
        }
    }

    /// <summary>
    /// The color/tint applied to the button's image.
    /// </summary>
    public Color Color
    {
        get => _image?.color ?? _pendingColor ?? Color.white;
        set
        {
            if (_image != null)
            {
                _image.color = value;
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
        get => _image?.preserveAspect ?? _pendingPreserveAspect ?? false;
        set
        {
            if (_image != null)
            {
                _image.preserveAspect = value;
            }
            else
            {
                _pendingPreserveAspect = value;
            }
        }
    }

    #endregion

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            if (_button != null)
            {
                _button.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _button = GetComponentInChildren<Button>();

        // Get the Image component on the button (the background image)
        _image = GetComponent<Image>();
        if (_image == null)
        {
            _image = gameObject.AddComponent<Image>();
        }

        if (_button != null)
        {
            _button.onClick.AddListener(OnClick);
        }

        // Apply pending properties
        if (_pendingColor.HasValue)
        {
            _image.color = _pendingColor.Value;
        }

        if (_pendingPreserveAspect.HasValue)
        {
            _image.preserveAspect = _pendingPreserveAspect.Value;
        }

        _isInitialized = true;

        // Load the icon if Source was set before Awake
        if (!string.IsNullOrEmpty(_source))
        {
            LoadIconFromSource();
        }
    }

    private void LoadIconFromSource()
    {
        if (string.IsNullOrEmpty(_source) || _image == null)
            return;

        Texture2D? texture = null;

        // If a specific assembly is set, use it
        if (_sourceAssembly != null)
        {
            texture = ImageControl.LoadTextureFromEmbeddedResource(_sourceAssembly, _source);
        }
        else
        {
            // Search in all loaded assemblies for the resource
            texture = ImageControl.LoadTextureFromAnyAssembly(_source);
        }

        if (texture != null)
        {
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
            _image.sprite = sprite;
            // Force Simple type so PreserveAspect works correctly
            _image.type = Image.Type.Simple;
        }
    }

    /// <summary>
    /// Loads an icon from embedded resources using a specific assembly.
    /// </summary>
    [HideFromIl2Cpp]
    public void LoadFromEmbeddedResource(Assembly assembly, string resourceName)
    {
        _sourceAssembly = assembly;
        Source = resourceName;
    }

    /// <summary>
    /// Loads an icon from a byte array.
    /// </summary>
    [HideFromIl2Cpp]
    public void LoadFromBytes(byte[] imageData)
    {
        if (_image == null) return;

        var texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
            _image.sprite = sprite;
            // Force Simple type so PreserveAspect works correctly
            _image.type = Image.Type.Simple;
        }
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
        if (_button != null)
        {
            _button.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnClick);
        }

        // Clean up created textures/sprites
        if (_image?.sprite != null && _image.sprite.texture != null)
        {
            if (!string.IsNullOrEmpty(_source))
            {
                Destroy(_image.sprite.texture);
                Destroy(_image.sprite);
            }
        }

        Clicked = null;
        base.Dispose();
    }
}

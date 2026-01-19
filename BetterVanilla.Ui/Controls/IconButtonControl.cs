using System;
using System.Reflection;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
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

    #endregion

    #region IButtonColorsControl

    public Color NormalColor
    {
        get => _buttonColors?.NormalColor ?? Color.white;
        set { if (_buttonColors != null) _buttonColors.NormalColor = value; }
    }

    public Color HighlightedColor
    {
        get => _buttonColors?.HighlightedColor ?? Color.white;
        set { if (_buttonColors != null) _buttonColors.HighlightedColor = value; }
    }

    public Color PressedColor
    {
        get => _buttonColors?.PressedColor ?? Color.gray;
        set { if (_buttonColors != null) _buttonColors.PressedColor = value; }
    }

    public Color SelectedColor
    {
        get => _buttonColors?.SelectedColor ?? Color.white;
        set { if (_buttonColors != null) _buttonColors.SelectedColor = value; }
    }

    public Color DisabledColor
    {
        get => _buttonColors?.DisabledColor ?? new Color(0.5f, 0.5f, 0.5f, 0.5f);
        set { if (_buttonColors != null) _buttonColors.DisabledColor = value; }
    }

    public float ColorMultiplier
    {
        get => _buttonColors?.ColorMultiplier ?? 1f;
        set { if (_buttonColors != null) _buttonColors.ColorMultiplier = value; }
    }

    public float FadeDuration
    {
        get => _buttonColors?.FadeDuration ?? 0.1f;
        set { if (_buttonColors != null) _buttonColors.FadeDuration = value; }
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
        if (string.IsNullOrEmpty(_source) || _component == null)
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
            _component.background.sprite = sprite;
            // Force Simple type so PreserveAspect works correctly
            _component.background.type = Image.Type.Simple;
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
        if (_component == null) return;

        var texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
            _component.background.sprite = sprite;
            // Force Simple type so PreserveAspect works correctly
            _component.background.type = Image.Type.Simple;
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
        if (_component != null)
        {
            _component.button.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_component != null)
        {
            _component.button.onClick.RemoveListener(OnClick);

            // Clean up created textures/sprites
            if (_component.background.sprite != null && _component.background.sprite.texture != null)
            {
                if (!string.IsNullOrEmpty(_source))
                {
                    Destroy(_component.background.sprite.texture);
                    Destroy(_component.background.sprite);
                }
            }
        }

        Clicked = null;
        base.Dispose();
    }
}

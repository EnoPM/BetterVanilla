using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A button control that can be clicked.
/// </summary>
public sealed class ButtonControl : BaseControl, ILabelStyleControl, IButtonColorsControl, IShadowControl
{
    private ButtonComponent? _component;
    private LabelStyleHelper? _labelStyle;
    private ButtonColorsHelper? _buttonColors;
    private ShadowHelper? _shadow;
    private readonly BindableProperty<string> _textProperty = new();

    public event Action? Clicked;

    public string Text
    {
        get => _component != null ? _component.buttonText.text : string.Empty;
        set
        {
            if (_component != null)
            {
                _component.buttonText.text = value;
            }
            _textProperty.Value = value;
        }
    }

    #region Background

    /// <summary>
    /// Background color of the button. Setting this will modify the Image component's color.
    /// </summary>
    public Color? Background
    {
        get;
        set
        {
            field = value;
            ApplyBackground();
        }
    }

    private void ApplyBackground()
    {
        if (!Background.HasValue || _component == null) return;
        _component.background.color = Background.Value;
    }

    #endregion

    #region ILabelStyleControl

    public float LabelFontSize
    {
        get => _labelStyle?.FontSize ?? 14f;
        set { if (_labelStyle != null) _labelStyle.FontSize = value; }
    }

    public Color LabelTextColor
    {
        get => _labelStyle?.TextColor ?? Color.white;
        set { if (_labelStyle != null) _labelStyle.TextColor = value; }
    }

    public TextAlignmentOptions LabelTextAlignment
    {
        get => _labelStyle?.TextAlignment ?? TextAlignmentOptions.Center;
        set { if (_labelStyle != null) _labelStyle.TextAlignment = value; }
    }

    public FontStyles LabelFontStyle
    {
        get => _labelStyle?.FontStyle ?? FontStyles.Normal;
        set { if (_labelStyle != null) _labelStyle.FontStyle = value; }
    }

    public float LabelCharacterSpacing
    {
        get => _labelStyle?.CharacterSpacing ?? 0f;
        set { if (_labelStyle != null) _labelStyle.CharacterSpacing = value; }
    }

    public float LabelLineSpacing
    {
        get => _labelStyle?.LineSpacing ?? 0f;
        set { if (_labelStyle != null) _labelStyle.LineSpacing = value; }
    }

    public float LabelWordSpacing
    {
        get => _labelStyle?.WordSpacing ?? 0f;
        set { if (_labelStyle != null) _labelStyle.WordSpacing = value; }
    }

    public bool LabelWordWrapping
    {
        get => _labelStyle?.WordWrapping ?? false;
        set { if (_labelStyle != null) _labelStyle.WordWrapping = value; }
    }

    public TextOverflowModes LabelTextOverflow
    {
        get => _labelStyle?.TextOverflow ?? TextOverflowModes.Overflow;
        set { if (_labelStyle != null) _labelStyle.TextOverflow = value; }
    }

    public bool LabelRichText
    {
        get => _labelStyle?.RichText ?? true;
        set { if (_labelStyle != null) _labelStyle.RichText = value; }
    }

    public bool LabelAutoSize
    {
        get => _labelStyle?.AutoSize ?? false;
        set { if (_labelStyle != null) _labelStyle.AutoSize = value; }
    }

    public float LabelMinFontSize
    {
        get => _labelStyle?.MinFontSize ?? 10f;
        set { if (_labelStyle != null) _labelStyle.MinFontSize = value; }
    }

    public float LabelMaxFontSize
    {
        get => _labelStyle?.MaxFontSize ?? 72f;
        set { if (_labelStyle != null) _labelStyle.MaxFontSize = value; }
    }

    public Vector4 LabelTextMargin
    {
        get => _labelStyle?.TextMargin ?? Vector4.zero;
        set { if (_labelStyle != null) _labelStyle.TextMargin = value; }
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
        _component = GetComponent<ButtonComponent>();
        _labelStyle = new LabelStyleHelper(_component?.buttonText);
        _buttonColors = new ButtonColorsHelper(_component?.button);
        _shadow = new ShadowHelper(_component?.shadow);

        if (_component != null)
        {
            _component.button.onClick.AddListener(OnClick);
        }
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        RegisterBindableProperty("Text", _textProperty);
        _textProperty.ValueChanged += value =>
        {
            if (_component != null && value is string strValue)
            {
                _component.buttonText.text = strValue;
            }
        };
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
        }
        Clicked = null;
        base.Dispose();
    }
}

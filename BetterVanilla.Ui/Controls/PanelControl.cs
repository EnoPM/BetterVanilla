using System;
using System.Collections.Generic;
using BetterVanilla.Ui.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Child alignment options for layout groups (maps to Unity's TextAnchor).
/// </summary>
public enum ChildAlignment
{
    UpperLeft,
    UpperCenter,
    UpperRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    LowerLeft,
    LowerCenter,
    LowerRight
}

/// <summary>
/// Orientation for layout groups.
/// </summary>
public enum Orientation
{
    None,
    Vertical,
    Horizontal
}

/// <summary>
/// A container control that can hold child controls.
/// </summary>
public sealed class PanelControl : BaseControl, IContainerControl
{
    private readonly List<IViewControl> _children = [];
    private HorizontalOrVerticalLayoutGroup? _layoutGroup;
    private Orientation _orientation = Orientation.None;

    private HorizontalOrVerticalLayoutGroup? LayoutGroup
    {
        get
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = GetComponent<VerticalLayoutGroup>();
                if (_layoutGroup == null)
                    _layoutGroup = GetComponent<HorizontalLayoutGroup>();
            }
            return _layoutGroup;
        }
    }

    /// <summary>
    /// Orientation of the layout group. Setting this will create or remove the appropriate LayoutGroup component.
    /// </summary>
    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            if (_orientation == value) return;
            _orientation = value;
            ApplyOrientation();
        }
    }

    private void ApplyOrientation()
    {
        // Remove existing layout group if any
        if (_layoutGroup != null)
        {
            Destroy(_layoutGroup);
            _layoutGroup = null;
        }

        // Also check for any existing layout groups on the GameObject
        var existingVertical = GetComponent<VerticalLayoutGroup>();
        if (existingVertical != null)
            Destroy(existingVertical);

        var existingHorizontal = GetComponent<HorizontalLayoutGroup>();
        if (existingHorizontal != null)
            Destroy(existingHorizontal);

        // Create new layout group based on orientation
        switch (_orientation)
        {
            case Orientation.Vertical:
                _layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
                break;
            case Orientation.Horizontal:
                _layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
                break;
            case Orientation.None:
            default:
                // No layout group
                break;
        }

        // Apply any previously set properties to the new layout group
        if (_layoutGroup != null)
        {
            ApplyLayoutGroupProperties();
        }
    }

    #region LayoutGroup Properties

    /// <summary>
    /// Spacing between child elements.
    /// </summary>
    public float? Spacing
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    /// <summary>
    /// Internal padding of the panel.
    /// </summary>
    public Thickness? Padding
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    /// <summary>
    /// Alignment of child elements within the panel.
    /// </summary>
    public ChildAlignment? ChildAlignment
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    /// <summary>
    /// Whether the layout group controls children's width.
    /// </summary>
    public bool? ChildControlWidth
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    /// <summary>
    /// Whether the layout group controls children's height.
    /// </summary>
    public bool? ChildControlHeight
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    /// <summary>
    /// Whether to force children to expand to fill available width.
    /// </summary>
    public bool? ChildForceExpandWidth
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    /// <summary>
    /// Whether to force children to expand to fill available height.
    /// </summary>
    public bool? ChildForceExpandHeight
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    /// <summary>
    /// Whether to reverse the arrangement of children.
    /// </summary>
    public bool? ReverseArrangement
    {
        get;
        set
        {
            field = value;
            ApplyLayoutGroupProperties();
        }
    }

    private void ApplyLayoutGroupProperties()
    {
        var lg = LayoutGroup;
        if (lg == null) return;

        if (Spacing.HasValue)
            lg.spacing = Spacing.Value;

        if (Padding.HasValue)
        {
            var offset = new RectOffset();
            offset.left = (int)Padding.Value.Left;
            offset.right = (int)Padding.Value.Right;
            offset.top = (int)Padding.Value.Top;
            offset.bottom = (int)Padding.Value.Bottom;
            lg.padding = offset;
        }

        if (ChildAlignment.HasValue)
            lg.childAlignment = (TextAnchor)(int)ChildAlignment.Value;

        if (ChildControlWidth.HasValue)
            lg.childControlWidth = ChildControlWidth.Value;

        if (ChildControlHeight.HasValue)
            lg.childControlHeight = ChildControlHeight.Value;

        if (ChildForceExpandWidth.HasValue)
            lg.childForceExpandWidth = ChildForceExpandWidth.Value;

        if (ChildForceExpandHeight.HasValue)
            lg.childForceExpandHeight = ChildForceExpandHeight.Value;

        if (ReverseArrangement.HasValue)
            lg.reverseArrangement = ReverseArrangement.Value;
    }

    #endregion

    #region Background

    private Image? _backgroundImage;

    /// <summary>
    /// Background color of the panel. Setting this will add an Image component if not present.
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
        if (!Background.HasValue) return;

        if (_backgroundImage == null)
        {
            _backgroundImage = GetComponent<Image>();
            if (_backgroundImage == null)
            {
                _backgroundImage = gameObject.AddComponent<Image>();
            }
        }

        _backgroundImage.color = Background.Value;
    }

    /// <summary>
    /// Parses a color from a hex string (#RGB, #RRGGBB, or #RRGGBBAA).
    /// </summary>
    public static Color ParseColor(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Color.clear;

        hex = hex.TrimStart('#');

        // Handle short format #RGB -> #RRGGBB
        if (hex.Length == 3)
        {
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
        }

        // Handle #RGBA -> #RRGGBBAA
        if (hex.Length == 4)
        {
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
        }

        if (hex.Length == 6)
        {
            hex += "FF"; // Add full opacity
        }

        if (hex.Length != 8)
            return Color.magenta; // Error color

        var r = Convert.ToByte(hex.Substring(0, 2), 16) / 255f;
        var g = Convert.ToByte(hex.Substring(2, 2), 16) / 255f;
        var b = Convert.ToByte(hex.Substring(4, 2), 16) / 255f;
        var a = Convert.ToByte(hex.Substring(6, 2), 16) / 255f;

        return new Color(r, g, b, a);
    }

    #endregion

    public void AddChild(IViewControl child)
    {
        _children.Add(child);
        child.GameObject.transform.SetParent(transform, false);
    }

    public void RemoveChild(IViewControl child)
    {
        if (_children.Remove(child))
        {
            child.GameObject.transform.SetParent(null);
        }
    }

    public IViewControl[] GetChildren()
    {
        return _children.ToArray();
    }

    public void ClearChildren()
    {
        foreach (var child in _children)
        {
            child.Dispose();
            if (child.GameObject != null)
            {
                Destroy(child.GameObject);
            }
        }
        _children.Clear();
    }

    protected override void OnEnabledChanged(bool state)
    {
        foreach (var child in _children)
        {
            child.IsEnabled = state;
        }
    }

    public override void Dispose()
    {
        ClearChildren();
        base.Dispose();
    }
}

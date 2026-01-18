using System.Collections.Generic;
using BetterVanilla.Ui.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Scrollbar visibility options.
/// </summary>
public enum ScrollbarVisibility
{
    /// <summary>Always show the scrollbar.</summary>
    Permanent,
    /// <summary>Hide scrollbar when not needed, expand viewport.</summary>
    AutoHide,
    /// <summary>Hide scrollbar when not needed, keep viewport size.</summary>
    AutoHideAndExpandViewport
}

/// <summary>
/// A scrollable container control that can hold child controls.
/// </summary>
public sealed class ScrollViewControl : BaseControl, IContainerControl
{
    private readonly List<IViewControl> _children = [];
    private ScrollRect? _scrollRect;
    private RectTransform? _content;
    private HorizontalOrVerticalLayoutGroup? _layoutGroup;
    private Orientation _orientation = Orientation.Vertical;
    private Image? _backgroundImage;

    /// <summary>
    /// Gets the ScrollRect component.
    /// </summary>
    public ScrollRect? ScrollRect => _scrollRect;

    /// <summary>
    /// Gets the content RectTransform where children are added.
    /// </summary>
    public RectTransform? Content => _content;

    /// <summary>
    /// Whether horizontal scrolling is enabled.
    /// </summary>
    public bool Horizontal
    {
        get => _scrollRect?.horizontal ?? false;
        set
        {
            if (_scrollRect != null)
                _scrollRect.horizontal = value;
        }
    }

    /// <summary>
    /// Whether vertical scrolling is enabled.
    /// </summary>
    public bool Vertical
    {
        get => _scrollRect?.vertical ?? true;
        set
        {
            if (_scrollRect != null)
                _scrollRect.vertical = value;
        }
    }

    /// <summary>
    /// The scroll deceleration rate (0 = instant stop, 1 = never stops).
    /// </summary>
    public float DecelerationRate
    {
        get => _scrollRect?.decelerationRate ?? 0.135f;
        set
        {
            if (_scrollRect != null)
                _scrollRect.decelerationRate = value;
        }
    }

    /// <summary>
    /// The elasticity when scrolling past bounds.
    /// </summary>
    public float Elasticity
    {
        get => _scrollRect?.elasticity ?? 0.1f;
        set
        {
            if (_scrollRect != null)
                _scrollRect.elasticity = value;
        }
    }

    /// <summary>
    /// Whether inertia is enabled (content continues to move after drag).
    /// </summary>
    public bool Inertia
    {
        get => _scrollRect?.inertia ?? true;
        set
        {
            if (_scrollRect != null)
                _scrollRect.inertia = value;
        }
    }

    /// <summary>
    /// The scroll sensitivity for scroll wheel input.
    /// </summary>
    public float ScrollSensitivity
    {
        get => _scrollRect?.scrollSensitivity ?? 1f;
        set
        {
            if (_scrollRect != null)
                _scrollRect.scrollSensitivity = value;
        }
    }

    /// <summary>
    /// The normalized scroll position (0-1 for both axes).
    /// </summary>
    public Vector2 NormalizedPosition
    {
        get => _scrollRect?.normalizedPosition ?? Vector2.zero;
        set
        {
            if (_scrollRect != null)
                _scrollRect.normalizedPosition = value;
        }
    }

    #region Content Layout Properties

    /// <summary>
    /// Orientation of the content layout group.
    /// </summary>
    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            var changed = _orientation != value;
            _orientation = value;
            // Always apply if layout group is not created yet, or if value changed
            if (changed || _layoutGroup == null)
            {
                ApplyOrientation();
            }
        }
    }

    /// <summary>
    /// Spacing between child elements in the content.
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
    /// Internal padding of the content area.
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
    /// Alignment of child elements within the content.
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

    private void ApplyOrientation()
    {
        if (_content == null) return;

        // Remove existing layout group
        if (_layoutGroup != null)
        {
            Destroy(_layoutGroup);
            _layoutGroup = null;
        }

        var existingVertical = _content.GetComponent<VerticalLayoutGroup>();
        if (existingVertical != null)
            Destroy(existingVertical);

        var existingHorizontal = _content.GetComponent<HorizontalLayoutGroup>();
        if (existingHorizontal != null)
            Destroy(existingHorizontal);

        // Create new layout group based on orientation
        switch (_orientation)
        {
            case Orientation.Vertical:
                _layoutGroup = _content.gameObject.AddComponent<VerticalLayoutGroup>();
                break;
            case Orientation.Horizontal:
                _layoutGroup = _content.gameObject.AddComponent<HorizontalLayoutGroup>();
                break;
            case Orientation.None:
            default:
                break;
        }

        if (_layoutGroup != null)
        {
            ApplyLayoutGroupProperties();
        }

        // Add ContentSizeFitter if we have a layout group
        var sizeFitter = _content.GetComponent<ContentSizeFitter>();
        if (_layoutGroup != null && sizeFitter == null)
        {
            sizeFitter = _content.gameObject.AddComponent<ContentSizeFitter>();
        }

        if (sizeFitter != null)
        {
            sizeFitter.horizontalFit = _orientation == Orientation.Horizontal
                ? ContentSizeFitter.FitMode.PreferredSize
                : ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = _orientation == Orientation.Vertical
                ? ContentSizeFitter.FitMode.PreferredSize
                : ContentSizeFitter.FitMode.Unconstrained;
        }
    }

    private void ApplyLayoutGroupProperties()
    {
        // If no layout group exists yet but we have content and orientation, create it
        if (_layoutGroup == null && _content != null && _orientation != Orientation.None)
        {
            ApplyOrientation();
        }

        if (_layoutGroup == null) return;

        if (Spacing.HasValue)
            _layoutGroup.spacing = Spacing.Value;

        if (Padding.HasValue)
        {
            var rectOffset = new RectOffset();
            rectOffset.left = (int)Padding.Value.Left;
            rectOffset.right = (int)Padding.Value.Right;
            rectOffset.top = (int)Padding.Value.Top;
            rectOffset.bottom = (int)Padding.Value.Bottom;
            _layoutGroup.padding = rectOffset;
        }

        if (ChildAlignment.HasValue)
            _layoutGroup.childAlignment = (TextAnchor)(int)ChildAlignment.Value;

        if (ChildControlWidth.HasValue)
            _layoutGroup.childControlWidth = ChildControlWidth.Value;

        if (ChildControlHeight.HasValue)
            _layoutGroup.childControlHeight = ChildControlHeight.Value;

        if (ChildForceExpandWidth.HasValue)
            _layoutGroup.childForceExpandWidth = ChildForceExpandWidth.Value;

        if (ChildForceExpandHeight.HasValue)
            _layoutGroup.childForceExpandHeight = ChildForceExpandHeight.Value;
    }

    #endregion

    #region Background

    /// <summary>
    /// Background color of the scroll view.
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

    #endregion

    protected override void Awake()
    {
        base.Awake();

        _scrollRect = GetComponent<ScrollRect>();
        if (_scrollRect == null)
        {
            _scrollRect = gameObject.AddComponent<ScrollRect>();
        }

        // Find or create viewport - try ScrollRect.viewport first, then search children
        RectTransform? viewportRect = _scrollRect.viewport;
        if (viewportRect == null)
        {
            viewportRect = FindChildByName<RectTransform>(transform, "Viewport");
        }

        if (viewportRect == null)
        {
            var viewportGo = new GameObject("Viewport");
            viewportGo.transform.SetParent(transform, false);

            viewportRect = viewportGo.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewportRect.anchoredPosition = Vector2.zero;

            // Add mask
            var mask = viewportGo.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            // Add image for mask (required)
            var maskImage = viewportGo.AddComponent<Image>();
            maskImage.color = Color.white;
        }

        _scrollRect.viewport = viewportRect;

        // Find or create content - try ScrollRect.content first, then search children
        _content = _scrollRect.content;
        if (_content == null)
        {
            _content = FindChildByName<RectTransform>(viewportRect.transform, "Content");
        }

        if (_content == null)
        {
            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(viewportRect.transform, false);

            _content = contentGo.AddComponent<RectTransform>();
            _content.anchorMin = new Vector2(0, 1);
            _content.anchorMax = new Vector2(1, 1);
            _content.pivot = new Vector2(0.5f, 1);
            _content.sizeDelta = Vector2.zero;
            _content.anchoredPosition = Vector2.zero;
        }

        _scrollRect.content = _content;

        // Ensure content has correct anchors for vertical scrolling
        _content.anchorMin = new Vector2(0, 1);
        _content.anchorMax = new Vector2(1, 1);
        _content.pivot = new Vector2(0.5f, 1);

        // Default settings
        _scrollRect.horizontal = false;
        _scrollRect.vertical = true;
        _scrollRect.movementType = ScrollRect.MovementType.Elastic;
        _scrollRect.elasticity = 0.1f;
        _scrollRect.inertia = true;
        _scrollRect.decelerationRate = 0.135f;
        _scrollRect.scrollSensitivity = 1f;

        // Apply default vertical orientation
        ApplyOrientation();
    }

    private static T? FindChildByName<T>(Transform parent, string name) where T : Component
    {
        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name == name)
            {
                return child.GetComponent<T>();
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the transform where children should be added.
    /// </summary>
    public Transform ContentTransform => _content != null ? _content.transform : transform;

    public void AddChild(IViewControl child)
    {
        _children.Add(child);
        child.GameObject.transform.SetParent(ContentTransform, false);
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

    /// <summary>
    /// Scrolls to the top of the content.
    /// </summary>
    public void ScrollToTop()
    {
        if (_scrollRect != null)
            _scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    /// <summary>
    /// Scrolls to the bottom of the content.
    /// </summary>
    public void ScrollToBottom()
    {
        if (_scrollRect != null)
            _scrollRect.normalizedPosition = new Vector2(0, 0);
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
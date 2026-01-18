using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Core;

/// <summary>
/// Horizontal alignment options for UI elements.
/// </summary>
public enum HorizontalAlignment
{
    /// <summary>Keep prefab's default alignment.</summary>
    None,
    Left,
    Center,
    Right,
    Stretch
}

/// <summary>
/// Vertical alignment options for UI elements.
/// </summary>
public enum VerticalAlignment
{
    /// <summary>Keep prefab's default alignment.</summary>
    None,
    Top,
    Center,
    Bottom,
    Stretch
}

/// <summary>
/// Represents thickness values for margins and padding.
/// </summary>
public readonly struct Thickness
{
    public float Left { get; }
    public float Top { get; }
    public float Right { get; }
    public float Bottom { get; }

    public Thickness(float uniform) : this(uniform, uniform, uniform, uniform) { }

    public Thickness(float horizontal, float vertical) : this(horizontal, vertical, horizontal, vertical) { }

    public Thickness(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public static Thickness Zero => new(0);

    /// <summary>
    /// Parses a thickness string. Formats: "10", "10,5", "10,5,10,5"
    /// </summary>
    public static Thickness Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Zero;

        var parts = value.Split(',');
        return parts.Length switch
        {
            1 => new Thickness(float.Parse(parts[0].Trim())),
            2 => new Thickness(float.Parse(parts[0].Trim()), float.Parse(parts[1].Trim())),
            4 => new Thickness(
                float.Parse(parts[0].Trim()),
                float.Parse(parts[1].Trim()),
                float.Parse(parts[2].Trim()),
                float.Parse(parts[3].Trim())),
            _ => Zero
        };
    }

    public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
}

/// <summary>
/// Extension methods for applying layout properties to RectTransform.
/// </summary>
public static class LayoutExtensions
{
    extension(RectTransform rectTransform)
    {
        /// <summary>
        /// Sets the size of the RectTransform.
        /// </summary>
        public void SetSize(float? width, float? height)
        {
            if (width.HasValue || height.HasValue)
            {
                var sizeDelta = rectTransform.sizeDelta;
                if (width.HasValue && width.Value >= 0)
                    sizeDelta.x = width.Value;
                if (height.HasValue && height.Value >= 0)
                    sizeDelta.y = height.Value;
                rectTransform.sizeDelta = sizeDelta;
            }
        }
        
        /// <summary>
        /// Sets the preferred size on LayoutElement component.
        /// </summary>
        public void SetPreferredSize(float? width, float? height)
        {
            if (!width.HasValue && !height.HasValue)
                return;

            var layoutElement = rectTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();

            if (width.HasValue && width.Value >= 0)
            {
                layoutElement.preferredWidth = width.Value;
                layoutElement.flexibleWidth = 0; // Prevent expansion
            }
            if (height.HasValue && height.Value >= 0)
            {
                layoutElement.preferredHeight = height.Value;
                layoutElement.flexibleHeight = 0; // Prevent expansion
            }

            // Also set sizeDelta for non-LayoutGroup parents
            rectTransform.SetSize(width, height);
        }
        
        /// <summary>
        /// Sets the minimum size on LayoutElement component.
        /// </summary>
        public void SetMinSize(float? minWidth, float? minHeight)
        {
            if (!minWidth.HasValue && !minHeight.HasValue)
                return;

            var layoutElement = rectTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();

            if (minWidth.HasValue)
                layoutElement.minWidth = minWidth.Value;
            if (minHeight.HasValue)
                layoutElement.minHeight = minHeight.Value;
        }
        
        /// <summary>
        /// Sets the flexible size on LayoutElement component.
        /// </summary>
        public void SetFlexibleSize(float? flexWidth, float? flexHeight)
        {
            if (!flexWidth.HasValue && !flexHeight.HasValue)
                return;

            var layoutElement = rectTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();

            if (flexWidth.HasValue)
                layoutElement.flexibleWidth = flexWidth.Value;
            if (flexHeight.HasValue)
                layoutElement.flexibleHeight = flexHeight.Value;
        }
        
        /// <summary>
        /// Applies margin. For stretched anchors, uses offset. For fixed anchors, adjusts position.
        /// Each axis is handled independently to support mixed cases (e.g., stretch on Y, fixed on X).
        /// </summary>
        public void SetMargin(Thickness margin)
        {
            var anchorMin = rectTransform.anchorMin;
            var anchorMax = rectTransform.anchorMax;

            // Check if anchors are stretched (different min/max) on each axis
            var isStretchedX = !Mathf.Approximately(anchorMin.x, anchorMax.x);
            var isStretchedY = !Mathf.Approximately(anchorMin.y, anchorMax.y);

            // Handle X axis
            if (isStretchedX)
            {
                // Stretched X: use offsetMin.x / offsetMax.x
                rectTransform.offsetMin = new Vector2(margin.Left, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(-margin.Right, rectTransform.offsetMax.y);
            }
            else
            {
                // Fixed X: adjust anchoredPosition.x
                var pos = rectTransform.anchoredPosition;
                pos.x = margin.Left - margin.Right;
                rectTransform.anchoredPosition = pos;
            }

            // Handle Y axis
            if (isStretchedY)
            {
                // Stretched Y: use offsetMin.y / offsetMax.y
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, margin.Bottom);
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -margin.Top);
            }
            else
            {
                // Fixed Y: adjust anchoredPosition.y
                var pos = rectTransform.anchoredPosition;
                pos.y = margin.Bottom - margin.Top;
                rectTransform.anchoredPosition = pos;
            }
        }
        
        /// <summary>
        /// Applies horizontal alignment.
        /// </summary>
        public void SetHorizontalAlignment(HorizontalAlignment alignment)
        {
            if (alignment == HorizontalAlignment.None)
                return; // Keep prefab's default

            var anchorMin = rectTransform.anchorMin;
            var anchorMax = rectTransform.anchorMax;
            var pivot = rectTransform.pivot;
            var isStretch = false;

            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    anchorMin.x = 0f;
                    anchorMax.x = 0f;
                    pivot.x = 0f;
                    break;
                case HorizontalAlignment.Center:
                    anchorMin.x = 0.5f;
                    anchorMax.x = 0.5f;
                    pivot.x = 0.5f;
                    break;
                case HorizontalAlignment.Right:
                    anchorMin.x = 1f;
                    anchorMax.x = 1f;
                    pivot.x = 1f;
                    break;
                case HorizontalAlignment.Stretch:
                    anchorMin.x = 0f;
                    anchorMax.x = 1f;
                    pivot.x = 0.5f;
                    isStretch = true;
                    // For LayoutGroups: set flexibleWidth to allow expansion
                    var layoutElement = rectTransform.GetComponent<LayoutElement>();
                    if (layoutElement == null)
                        layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
                    layoutElement.flexibleWidth = 1f;
                    break;
            }

            // Apply anchors and pivot first
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;

            // For Stretch: reset offsets AFTER anchors are set
            if (isStretch)
            {
                rectTransform.offsetMin = new Vector2(0f, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(0f, rectTransform.offsetMax.y);
            }
        }
        
        /// <summary>
        /// Applies vertical alignment.
        /// </summary>
        public void SetVerticalAlignment(VerticalAlignment alignment)
        {
            if (alignment == VerticalAlignment.None)
                return; // Keep prefab's default

            var anchorMin = rectTransform.anchorMin;
            var anchorMax = rectTransform.anchorMax;
            var pivot = rectTransform.pivot;
            var isStretch = false;

            switch (alignment)
            {
                case VerticalAlignment.Top:
                    anchorMin.y = 1f;
                    anchorMax.y = 1f;
                    pivot.y = 1f;
                    break;
                case VerticalAlignment.Center:
                    anchorMin.y = 0.5f;
                    anchorMax.y = 0.5f;
                    pivot.y = 0.5f;
                    break;
                case VerticalAlignment.Bottom:
                    anchorMin.y = 0f;
                    anchorMax.y = 0f;
                    pivot.y = 0f;
                    break;
                case VerticalAlignment.Stretch:
                    anchorMin.y = 0f;
                    anchorMax.y = 1f;
                    pivot.y = 0.5f;
                    isStretch = true;
                    // For LayoutGroups: set flexibleHeight to allow expansion
                    var layoutElement = rectTransform.GetComponent<LayoutElement>();
                    if (layoutElement == null)
                        layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
                    layoutElement.flexibleHeight = 1f;
                    break;
            }

            // Apply anchors and pivot first
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;

            // For Stretch: reset offsets AFTER anchors are set
            if (isStretch)
            {
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0f);
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0f);
            }
        }
        
        /// <summary>
        /// Applies both horizontal and vertical alignment.
        /// </summary>
        public void SetAlignment(HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            rectTransform.SetHorizontalAlignment(horizontal);
            rectTransform.SetVerticalAlignment(vertical);
        }
        
        /// <summary>
        /// Sets padding on a LayoutGroup component.
        /// </summary>
        public void SetPadding(Thickness padding)
        {
            var layoutGroup = rectTransform.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                var rectOffset = new RectOffset();
                rectOffset.left = (int)padding.Left;
                rectOffset.right = (int)padding.Right;
                rectOffset.top = (int)padding.Top;
                rectOffset.bottom = (int)padding.Bottom;
                layoutGroup.padding = rectOffset;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Extensions;

public static class RectTransformExtensions
{
    extension(RectTransform rectTransform)
    {
        /// <summary>
        /// Sets the size of the RectTransform.
        /// </summary>
        public void SetSize(float? width, float? height)
        {
            if (!width.HasValue && !height.HasValue) return;
            var sizeDelta = rectTransform.sizeDelta;
            if (width >= 0)
            {
                sizeDelta.x = width.Value;
            }
            if (height >= 0)
            {
                sizeDelta.y = height.Value;
            }
            rectTransform.sizeDelta = sizeDelta;
        }
        
        /// <summary>
        /// Sets the preferred size on LayoutElement component.
        /// </summary>
        public void SetPreferredSize(float? width, float? height)
        {
            if (!width.HasValue && !height.HasValue) return;

            var layoutElement = rectTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
            }

            if (width >= 0)
            {
                layoutElement.preferredWidth = width.Value;
                layoutElement.flexibleWidth = 0; // Prevent expansion
            }
            
            if (height >= 0)
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
            if (!minWidth.HasValue && !minHeight.HasValue) return;

            var layoutElement = rectTransform.GetComponent<LayoutElement>();
            
            if (layoutElement == null)
            {
                layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
            }

            if (minWidth.HasValue)
            {
                layoutElement.minWidth = minWidth.Value;
            }
            
            if (minHeight.HasValue)
            {
                layoutElement.minHeight = minHeight.Value;
            }
        }

        /// <summary>
        /// Sets the maximum size using LayoutElement constraints.
        /// Note: Unity's LayoutElement doesn't have direct max size support,
        /// so this sets preferred size and prevents expansion.
        /// </summary>
        public void SetMaxSize(float? maxWidth, float? maxHeight)
        {
            if (!maxWidth.HasValue && !maxHeight.HasValue) return;

            var layoutElement = rectTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
            }

            // Unity LayoutElement doesn't have direct maxWidth/maxHeight
            // We use preferredWidth/Height with flexibleWidth/Height = 0 to cap size
            if (maxWidth.HasValue)
            {
                // Only set preferred if it would be larger than current preferred
                if (layoutElement.preferredWidth < 0 || layoutElement.preferredWidth > maxWidth.Value)
                    layoutElement.preferredWidth = maxWidth.Value;
                layoutElement.flexibleWidth = 0;
            }
            if (maxHeight.HasValue)
            {
                if (layoutElement.preferredHeight < 0 || layoutElement.preferredHeight > maxHeight.Value)
                    layoutElement.preferredHeight = maxHeight.Value;
                layoutElement.flexibleHeight = 0;
            }
        }

        /// <summary>
        /// Sets the flexible size on LayoutElement component.
        /// </summary>
        public void SetFlexibleSize(float? flexWidth, float? flexHeight)
        {
            if (!flexWidth.HasValue && !flexHeight.HasValue) return;

            var layoutElement = rectTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
            }

            if (flexWidth.HasValue)
            {
                layoutElement.flexibleWidth = flexWidth.Value;
            }
            if (flexHeight.HasValue)
            {
                layoutElement.flexibleHeight = flexHeight.Value;
            }
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
                // Fixed X: adjust anchoredPosition.x based on anchor position
                var pos = rectTransform.anchoredPosition;
                if (Mathf.Approximately(anchorMin.x, 0f))
                {
                    // Left-aligned: push right by left margin
                    pos.x = margin.Left;
                }
                else if (Mathf.Approximately(anchorMin.x, 1f))
                {
                    // Right-aligned: push left by right margin (negative)
                    pos.x = -margin.Right;
                }
                else
                {
                    // Center-aligned: offset by difference
                    pos.x = margin.Left - margin.Right;
                }
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
                // Fixed Y: adjust anchoredPosition.y based on anchor position
                var pos = rectTransform.anchoredPosition;
                if (Mathf.Approximately(anchorMin.y, 0f))
                {
                    // Bottom-aligned: push up by bottom margin
                    pos.y = margin.Bottom;
                }
                else if (Mathf.Approximately(anchorMin.y, 1f))
                {
                    // Top-aligned: push down by top margin (negative)
                    pos.y = -margin.Top;
                }
                else
                {
                    // Center-aligned: offset by difference
                    pos.y = margin.Bottom - margin.Top;
                }
                rectTransform.anchoredPosition = pos;
            }
        }
        
        /// <summary>
        /// Applies horizontal alignment.
        /// </summary>
        public void SetHorizontalAlignment(HorizontalAlignment alignment)
        {
            if (alignment == HorizontalAlignment.None) return; // Keep prefab's default

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
                    {
                        layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
                    }
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
            if (alignment == VerticalAlignment.None) return; // Keep prefab's default

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
                    {
                        layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
                    }
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
            if (layoutGroup == null) return;
            var rectOffset = new RectOffset
            {
                left = (int)padding.Left,
                right = (int)padding.Right,
                top = (int)padding.Top,
                bottom = (int)padding.Bottom
            };
            layoutGroup.padding = rectOffset;
        }
    }
}
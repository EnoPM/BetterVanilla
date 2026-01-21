using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Helpers;

/// <summary>
/// Utility class for debugging Unity UI components.
/// </summary>
public static class UiDebugger
{
    private const string UiDebugLogFilePath = @"D:\Projects\EnoUnityLoader.BetterVanilla\ui.log";
    private const string ComponentLogFilePath = @"D:\Projects\EnoUnityLoader.BetterVanilla\component.log";

    static UiDebugger()
    {
        File.WriteAllText(UiDebugLogFilePath, string.Empty);
        File.WriteAllText(ComponentLogFilePath, string.Empty);
    }

    public static bool Enabled { get; set; } = true;

    /// <summary>
    /// Logs detailed info about a RectTransform.
    /// </summary>
    public static void LogRectTransform(RectTransform rt, string context = "")
    {
        if (!Enabled || rt == null) return;

        var sb = new StringBuilder();
        sb.AppendLine($"[UiDebug] RectTransform: {rt.gameObject.name} {context}");
        sb.AppendLine($"  anchorMin: {rt.anchorMin}");
        sb.AppendLine($"  anchorMax: {rt.anchorMax}");
        sb.AppendLine($"  pivot: {rt.pivot}");
        sb.AppendLine($"  sizeDelta: {rt.sizeDelta}");
        sb.AppendLine($"  anchoredPosition: {rt.anchoredPosition}");
        sb.AppendLine($"  offsetMin: {rt.offsetMin}");
        sb.AppendLine($"  offsetMax: {rt.offsetMax}");
        sb.AppendLine($"  rect: {rt.rect}");
        sb.AppendLine($"  localPosition: {rt.localPosition}");
        sb.AppendLine($"  localScale: {rt.localScale}");

        Log(sb);
    }

    /// <summary>
    /// Logs detailed info about a LayoutElement.
    /// </summary>
    public static void LogLayoutElement(LayoutElement le, string context = "")
    {
        if (!Enabled) return;

        if (le == null)
        {
            Log($"LayoutElement: null {context}");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"[UiDebug] LayoutElement: {le.gameObject.name} {context}");
        sb.AppendLine($"  ignoreLayout: {le.ignoreLayout}");
        sb.AppendLine($"  minWidth: {le.minWidth}");
        sb.AppendLine($"  minHeight: {le.minHeight}");
        sb.AppendLine($"  preferredWidth: {le.preferredWidth}");
        sb.AppendLine($"  preferredHeight: {le.preferredHeight}");
        sb.AppendLine($"  flexibleWidth: {le.flexibleWidth}");
        sb.AppendLine($"  flexibleHeight: {le.flexibleHeight}");
        sb.AppendLine($"  layoutPriority: {le.layoutPriority}");

        Log(sb);
    }

    /// <summary>
    /// Logs detailed info about a LayoutGroup.
    /// </summary>
    public static void LogLayoutGroup(LayoutGroup lg, string context = "")
    {
        if (!Enabled) return;

        if (lg == null)
        {
            Log($"LayoutGroup: null {context}");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"[UiDebug] LayoutGroup: {lg.gameObject.name} ({lg.GetType().Name}) {context}");
        sb.AppendLine($"  padding: L={lg.padding.left} R={lg.padding.right} T={lg.padding.top} B={lg.padding.bottom}");
        sb.AppendLine($"  childAlignment: {lg.childAlignment}");

        var hlg = lg.TryCast<HorizontalOrVerticalLayoutGroup>();
        if (hlg != null)
        {
            sb.AppendLine($"  spacing: {hlg.spacing}");
            sb.AppendLine($"  childControlWidth: {hlg.childControlWidth}");
            sb.AppendLine($"  childControlHeight: {hlg.childControlHeight}");
            sb.AppendLine($"  childScaleWidth: {hlg.childScaleWidth}");
            sb.AppendLine($"  childScaleHeight: {hlg.childScaleHeight}");
            sb.AppendLine($"  childForceExpandWidth: {hlg.childForceExpandWidth}");
            sb.AppendLine($"  childForceExpandHeight: {hlg.childForceExpandHeight}");
            sb.AppendLine($"  reverseArrangement: {hlg.reverseArrangement}");
        }

        Log(sb);
    }

    /// <summary>
    /// Logs detailed info about a Canvas.
    /// </summary>
    public static void LogCanvas(Canvas canvas, string context = "")
    {
        if (!Enabled) return;

        if (canvas == null)
        {
            Log($"Canvas: null {context}");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"[UiDebug] Canvas: {canvas.gameObject.name} {context}");
        sb.AppendLine($"  renderMode: {canvas.renderMode}");
        sb.AppendLine($"  sortingOrder: {canvas.sortingOrder}");
        sb.AppendLine($"  sortingLayerID: {canvas.sortingLayerID}");
        sb.AppendLine($"  pixelPerfect: {canvas.pixelPerfect}");
        sb.AppendLine($"  scaleFactor: {canvas.scaleFactor}");
        sb.AppendLine($"  referencePixelsPerUnit: {canvas.referencePixelsPerUnit}");
        sb.AppendLine($"  overrideSorting: {canvas.overrideSorting}");

        Log(sb);
    }

    /// <summary>
    /// Logs detailed info about a CanvasScaler.
    /// </summary>
    public static void LogCanvasScaler(CanvasScaler scaler, string context = "")
    {
        if (!Enabled) return;

        if (scaler == null)
        {
            Log($"CanvasScaler: null {context}");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"[UiDebug] CanvasScaler: {scaler.gameObject.name} {context}");
        sb.AppendLine($"  uiScaleMode: {scaler.uiScaleMode}");
        sb.AppendLine($"  referenceResolution: {scaler.referenceResolution}");
        sb.AppendLine($"  screenMatchMode: {scaler.screenMatchMode}");
        sb.AppendLine($"  matchWidthOrHeight: {scaler.matchWidthOrHeight}");
        sb.AppendLine($"  scaleFactor: {scaler.scaleFactor}");
        sb.AppendLine($"  referencePixelsPerUnit: {scaler.referencePixelsPerUnit}");

        Log(sb);
    }

    /// <summary>
    /// Logs a complete hierarchy of a GameObject with all UI components.
    /// </summary>
    public static void LogHierarchy(GameObject go, int depth = 0, int maxDepth = 5)
    {
        if (!Enabled || go == null || depth > maxDepth) return;

        var indent = new string(' ', depth * 2);
        var rt = go.GetComponent<RectTransform>();
        var le = go.GetComponent<LayoutElement>();
        var lg = go.GetComponent<LayoutGroup>();
        var canvas = go.GetComponent<Canvas>();

        var sb = new StringBuilder();
        sb.Append($"{indent}[{go.name}]");

        if (rt != null)
            sb.Append($" RT(size={rt.sizeDelta}, anchor={rt.anchorMin}-{rt.anchorMax})");
        if (le != null)
            sb.Append($" LE(pref={le.preferredWidth}x{le.preferredHeight}, flex={le.flexibleWidth}x{le.flexibleHeight})");
        if (lg != null)
            sb.Append($" LG({lg.GetType().Name})");
        if (canvas != null)
            sb.Append($" Canvas({canvas.renderMode})");

        Log(sb.ToString());

        // Log children
        for (var i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i).gameObject;
            LogHierarchy(child, depth + 1, maxDepth);
        }
    }

    /// <summary>
    /// Logs all UI components on a GameObject.
    /// </summary>
    public static void LogAllComponents(GameObject go, string context = "")
    {
        if (!Enabled || go == null) return;

        Log($"=== All UI Components on '{go.name}' {context} ===");

        var rt = go.GetComponent<RectTransform>();
        if (rt != null) LogRectTransform(rt);

        var le = go.GetComponent<LayoutElement>();
        if (le != null) LogLayoutElement(le);

        var lg = go.GetComponent<LayoutGroup>();
        if (lg != null) LogLayoutGroup(lg);

        var canvas = go.GetComponent<Canvas>();
        if (canvas != null) LogCanvas(canvas);

        var scaler = go.GetComponent<CanvasScaler>();
        if (scaler != null) LogCanvasScaler(scaler);

        Log($"=== End of '{go.name}' ===");
    }

    /// <summary>
    /// Logs info about a control and its parent.
    /// </summary>
    public static void LogControlWithParent(GameObject go, string context = "")
    {
        if (!Enabled || go == null) return;

        Log($"=== Control '{go.name}' {context} ===");

        // Log the control itself
        var rt = go.GetComponent<RectTransform>();
        if (rt != null) LogRectTransform(rt, "(self)");

        var le = go.GetComponent<LayoutElement>();
        if (le != null) LogLayoutElement(le, "(self)");

        // Log parent
        var parent = go.transform.parent;
        if (parent != null)
        {
            var parentRt = parent.GetComponent<RectTransform>();
            if (parentRt != null) LogRectTransform(parentRt.Cast<RectTransform>(), "(parent)");

            var parentLg = parent.GetComponent<LayoutGroup>();
            if (parentLg != null) LogLayoutGroup(parentLg, "(parent)");
        }

        Log($"=== End of '{go.name}' ===");
    }

    /// <summary>
    /// Logs a complete analysis of a GameObject including all parents, the target, and all children recursively.
    /// Logs ALL components on each GameObject to help understand Unity UI click blocking.
    /// </summary>
    /// <param name="go">The target GameObject to analyze</param>
    /// <param name="childDepth">Maximum depth for children recursion (default 10)</param>
    public static void LogFullHierarchyAnalysis(GameObject go, int childDepth = 10)
    {
        if (!Enabled || go == null) return;

        Log2("╔══════════════════════════════════════════════════════════════════════════════");
        Log2($"║ FULL HIERARCHY ANALYSIS: {go.name}");
        Log2("╠══════════════════════════════════════════════════════════════════════════════");

        // 1. Log all parents (from root to target)
        Log2("║");
        Log2("║ ▼▼▼ PARENTS (root → target) ▼▼▼");
        Log2("║");

        var parents = new System.Collections.Generic.List<Transform>();
        var current = go.transform.parent;
        while (current != null)
        {
            parents.Add(current);
            current = current.parent;
        }

        // Reverse to show from root to target
        parents.Reverse();
        for (var i = 0; i < parents.Count; i++)
        {
            var indent = new string(' ', i * 2);
            LogGameObjectWithAllComponents(parents[i].gameObject, $"║ {indent}", $"[PARENT {parents.Count - i}]");
        }

        // 2. Log siblings (before target in hierarchy)
        var targetIndent = new string(' ', parents.Count * 2);
        var parent = go.transform.parent;
        if (parent != null)
        {
            var siblingsBefore = new System.Collections.Generic.List<Transform>();
            var siblingsAfter = new System.Collections.Generic.List<Transform>();
            var foundTarget = false;

            for (var i = 0; i < parent.childCount; i++)
            {
                var sibling = parent.GetChild(i);
                if (sibling.gameObject == go)
                {
                    foundTarget = true;
                    continue;
                }

                if (!foundTarget)
                    siblingsBefore.Add(sibling);
                else
                    siblingsAfter.Add(sibling);
            }

            if (siblingsBefore.Count > 0 || siblingsAfter.Count > 0)
            {
                Log2("║");
                Log2($"║ ◆◆◆ SIBLINGS ({siblingsBefore.Count} before, {siblingsAfter.Count} after) ◆◆◆");
                Log2("║");

                foreach (var sibling in siblingsBefore)
                {
                    LogGameObjectWithAllComponents(sibling.gameObject, $"║ {targetIndent}", "[SIBLING ↑]");
                }
            }
        }

        // 3. Log the target
        Log2("║");
        Log2("║ ▶▶▶ TARGET ◀◀◀");
        Log2("║");
        LogGameObjectWithAllComponents(go, $"║ {targetIndent}", "[TARGET]");

        // 4. Log siblings after target
        if (parent != null)
        {
            var siblingsAfter = new System.Collections.Generic.List<Transform>();
            var foundTarget = false;

            for (var i = 0; i < parent.childCount; i++)
            {
                var sibling = parent.GetChild(i);
                if (sibling.gameObject == go)
                {
                    foundTarget = true;
                    continue;
                }

                if (foundTarget)
                    siblingsAfter.Add(sibling);
            }

            foreach (var sibling in siblingsAfter)
            {
                LogGameObjectWithAllComponents(sibling.gameObject, $"║ {targetIndent}", "[SIBLING ↓]");
            }
        }

        // 5. Log all children recursively
        Log2("║");
        Log2("║ ▼▼▼ CHILDREN (recursive) ▼▼▼");
        Log2("║");
        LogChildrenRecursive(go.transform, parents.Count + 1, childDepth);

        Log2("║");
        Log2("╚══════════════════════════════════════════════════════════════════════════════");
    }

    /// <summary>
    /// Logs a GameObject with ALL its components (not just UI components).
    /// </summary>
    private static void LogGameObjectWithAllComponents(GameObject go, string prefix, string label)
    {
        if (go == null) return;

        var sb = new StringBuilder();
        sb.AppendLine($"{prefix}┌─ {label} {go.name} (active: {go.activeSelf}, layer: {LayerMask.LayerToName(go.layer)})");

        // Get ALL components
        var components = go.GetComponents<Component>();
        foreach (var comp in components)
        {
            if (comp == null) continue;

            var compType = comp.GetIl2CppType().Name;
            sb.AppendLine($"{prefix}│  ├─ [{compType}]");

            // Log specific properties for interesting components
            LogComponentDetails(comp, sb, prefix);
        }

        sb.AppendLine($"{prefix}└─────");
        Log2(sb.ToString().TrimEnd());
    }

    /// <summary>
    /// Logs specific details for UI-related components using Il2Cpp reflection.
    /// </summary>
    private static void LogComponentDetails(Component comp, StringBuilder sb, string prefix)
    {
        var detailPrefix = $"{prefix}│  │     ";

        var type = comp.GetIl2CppType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            if (property == null) continue;

            // Skip indexer properties (they require parameters)
            var indexParams = property.GetIndexParameters();
            if (indexParams != null && indexParams.Length > 0) continue;

            // Skip properties that cannot be read
            if (!property.CanRead) continue;

            try
            {
                var value = property.GetValue(comp, null);
                var valueStr = FormatPropertyValue(value);
                sb.AppendLine($"{detailPrefix}{property.Name}: {valueStr}");
            }
            catch
            {
                // Skip properties that throw exceptions when accessed
            }
        }
    }

    /// <summary>
    /// Formats a property value for logging.
    /// </summary>
    private static string FormatPropertyValue(Il2CppSystem.Object? value)
    {
        if (value == null) return "null";

        try
        {
            // Try to get a meaningful string representation
            var str = value.ToString();

            // Truncate very long strings
            if (str != null && str.Length > 100)
            {
                str = str[..100] + "...";
            }

            return str ?? "null";
        }
        catch
        {
            return "<error>";
        }
    }

    /// <summary>
    /// Recursively logs all children of a transform.
    /// </summary>
    private static void LogChildrenRecursive(Transform parent, int depth, int maxDepth)
    {
        if (depth > maxDepth) return;

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            var indent = new string(' ', depth * 2);
            LogGameObjectWithAllComponents(child.gameObject, $"║ {indent}", $"[CHILD d={depth}]");
            LogChildrenRecursive(child, depth + 1, maxDepth);
        }
    }

    /// <summary>
    /// Logs a message if debugging is enabled.
    /// </summary>
    private static void Log(string message)
    {
        if (!Enabled) return;
        WriteLogInFile($"[UiDebug] {message}");
        //BetterVanilla.Core.Ls.LogMessage($"[UiDebug] {message}");
    }

    private static void Log(StringBuilder message) => Log(message.ToString());

    private static void WriteLogInFile(string message)
    {
        File.AppendAllText(UiDebugLogFilePath, $"\n{message}");
    }

    /// <summary>
    /// Logs a message if debugging is enabled.
    /// </summary>
    private static void Log2(string message)
    {
        if (!Enabled) return;
        WriteLogInFile2($"[Component] {message}");
        //BetterVanilla.Core.Ls.LogMessage($"[UiDebug] {message}");
    }

    private static void WriteLogInFile2(string message)
    {
        File.AppendAllText(ComponentLogFilePath, $"\n{message}");
    }
}
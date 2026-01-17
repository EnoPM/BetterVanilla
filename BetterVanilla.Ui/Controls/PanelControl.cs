using System;
using System.Collections.Generic;
using BetterVanilla.Ui.Core;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A container control that can hold child controls.
/// </summary>
public sealed class PanelControl : BaseControl, IContainerControl
{
    private readonly List<IViewControl> _children = [];

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

    protected override void OnEnabledChanged(bool enabled)
    {
        foreach (var child in _children)
        {
            child.IsEnabled = enabled;
        }
    }

    public override void Dispose()
    {
        ClearChildren();
        base.Dispose();
    }
}

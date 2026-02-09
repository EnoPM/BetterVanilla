using System;
using System.IO;

namespace BetterVanilla.Options.Core;

public abstract class OptionBase(string key, Func<string> labelProvider, Func<string> descriptionProvider)
{
    public OptionDebouncer Debouncer { get; } = new(TimeSpan.FromSeconds(3.0));
    
    public string Key { get; } = key;

    public bool IsEnabled
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            EnabledChanged?.Invoke();
        }
    } = true;

    public bool IsVisible
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            VisibleChanged?.Invoke();
        }
    } = true;

    public bool IsEnabledAndVisible => IsEnabled && IsVisible;

    public event Action? ValueChanged;
    public event Action? EnabledChanged;
    public event Action? VisibleChanged;

    protected void OnValueChanged()
    {
        ValueChanged?.Invoke();
        Debouncer.Trigger();
    }

    public abstract void Write(BinaryWriter writer);
    public abstract void Read(BinaryReader reader);
    public abstract void Reset();

    public string Label => labelProvider();
    public string Description => descriptionProvider();
}
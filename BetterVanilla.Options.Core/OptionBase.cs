using System;
using System.IO;

namespace BetterVanilla.Options.Core;

public abstract class OptionBase(string key)
{
    public string Key { get; } = key;

    public event Action? ValueChanged;

    protected void OnValueChanged() => ValueChanged?.Invoke();

    public abstract void Write(BinaryWriter writer);
    public abstract void Read(BinaryReader reader);
    public abstract void Reset();
}
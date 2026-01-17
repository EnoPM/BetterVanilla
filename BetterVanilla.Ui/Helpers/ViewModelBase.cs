using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BetterVanilla.Ui.Helpers;

/// <summary>
/// Base class for view models with INotifyPropertyChanged support.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected readonly CompositeDisposable Disposables = new();

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<T>(ref T field, T value, Action onChanged, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref field, value, propertyName))
            return false;

        onChanged();
        return true;
    }

    public virtual void Dispose()
    {
        Disposables.Dispose();
    }
}

/// <summary>
/// Event args for property change notifications with old and new values.
/// </summary>
public class PropertyChangedEventArgs<T> : PropertyChangedEventArgs
{
    public T OldValue { get; }
    public T NewValue { get; }

    public PropertyChangedEventArgs(string? propertyName, T oldValue, T newValue) : base(propertyName)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

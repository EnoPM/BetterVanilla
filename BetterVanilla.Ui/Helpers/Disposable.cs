using System;
using System.Collections.Generic;

namespace BetterVanilla.Ui.Helpers;

/// <summary>
/// Represents a group of disposable resources that are disposed together.
/// </summary>
public sealed class CompositeDisposable : IDisposable
{
    private readonly List<IDisposable> _disposables = [];
    private bool _disposed;

    public void Add(IDisposable disposable)
    {
        if (_disposed)
        {
            disposable.Dispose();
            return;
        }
        _disposables.Add(disposable);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
        _disposables.Clear();
    }
}

/// <summary>
/// Creates a disposable from an action to execute on dispose.
/// </summary>
public sealed class ActionDisposable : IDisposable
{
    private Action? _disposeAction;

    public ActionDisposable(Action disposeAction)
    {
        _disposeAction = disposeAction;
    }

    public void Dispose()
    {
        _disposeAction?.Invoke();
        _disposeAction = null;
    }
}

/// <summary>
/// A disposable that does nothing.
/// </summary>
public sealed class EmptyDisposable : IDisposable
{
    public static readonly EmptyDisposable Instance = new();

    private EmptyDisposable() { }

    public void Dispose() { }
}

/// <summary>
/// Extension methods for IDisposable.
/// </summary>
public static class DisposableExtensions
{
    public static T AddTo<T>(this T disposable, CompositeDisposable composite) where T : IDisposable
    {
        composite.Add(disposable);
        return disposable;
    }
}

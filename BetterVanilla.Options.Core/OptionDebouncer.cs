using System;
using System.Threading;
using System.Threading.Tasks;

namespace BetterVanilla.Options.Core;

public sealed class OptionDebouncer
{
    public event Action? Debounced;
    
    private CancellationTokenSource? CancellationTokenSource { get; set; }
    
    private TimeSpan DebounceDelay { get; set; }
    
    public OptionDebouncer(TimeSpan debounceDelay)
    {
        DebounceDelay = debounceDelay;
    }

    public void Trigger()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource = new CancellationTokenSource();
        var token = CancellationTokenSource.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(DebounceDelay, token);
                if (token.IsCancellationRequested) return;
                Debounced?.Invoke();
            }
            catch (TaskCanceledException)
            {
                // ignored - debounce canceled
            }
        }, token);
    }
}
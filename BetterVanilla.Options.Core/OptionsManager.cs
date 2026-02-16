using System;

namespace BetterVanilla.Options.Core;

public class OptionsManager<TOptionsHolder> where TOptionsHolder : OptionsHolderBase, new()
{
    private string FilePath { get; }
    private OptionDebouncer Debouncer { get; }

    public TOptionsHolder Options { get; }

    public OptionsManager(string filePath, double saveDelay = 3)
    {
        FilePath = filePath;
        Options = new TOptionsHolder();
        Debouncer = new OptionDebouncer(TimeSpan.FromSeconds(saveDelay));

        OptionsSerializer.LoadFromFile(Options, FilePath);

        Debouncer.Debounced += Save;
        
        foreach (var option in Options.GetAllOptions())
        {
            option.ValueChanged += Debouncer.Trigger;
        }
    }

    private void Save()
    {
        OptionsSerializer.SaveToFile(Options, FilePath);
        System.Console.WriteLine($"Options saved to {FilePath}");
    }
}
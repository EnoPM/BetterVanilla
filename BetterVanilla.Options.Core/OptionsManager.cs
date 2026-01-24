namespace BetterVanilla.Options.Core;

public sealed class OptionsManager<TOptionsHolder> where TOptionsHolder : OptionsHolderBase, new()
{
    private string FilePath { get; }

    public TOptionsHolder Options { get; }

    public OptionsManager(string filePath)
    {
        FilePath = filePath;
        Options = new TOptionsHolder();

        OptionsSerializer.LoadFromFile(Options, FilePath);
    }

    public void Save()
    {
        OptionsSerializer.SaveToFile(Options, FilePath);
    }
}
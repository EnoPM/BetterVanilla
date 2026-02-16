using System.IO;
using UnityEngine;

namespace BetterVanilla.Core;

public sealed class PathsManager
{
    public static PathsManager Instance { get; } = new();

    private string ModDataDirectory { get; }
    public string OptionsPresetsFile { get; }
    public string OptionsDirectory { get; }
    public string SavedOutfitsFile { get; }
    public string CosmeticBundlesDirectory { get; }
    public string LocalCosmeticBundlesDirectory { get; }
    public string TasksAssignationFile { get; }

    private PathsManager()
    {
        var appDirectory = Application.persistentDataPath;
        ModDataDirectory = Path.Combine(appDirectory, "EnoPM", "BetterVanilla");
        if (!Directory.Exists(ModDataDirectory))
        {
            Directory.CreateDirectory(ModDataDirectory);
        }
        OptionsPresetsFile = CreateFile("options_presets.dat");
        OptionsDirectory = CreateDirectory("options");
        SavedOutfitsFile = CreateFile("outfits.dat");
        
        CosmeticBundlesDirectory = CreateDirectory("cosmetics", "bundles");
        LocalCosmeticBundlesDirectory = CreateDirectory("cosmetics", "local_bundles");
        TasksAssignationFile = CreateFile("tasks_assignation.dat");
    }

    private string CreateFile(params string[] paths)
    {
        return Path.Combine(ModDataDirectory, Path.Combine(paths));
    }

    private string CreateDirectory(params string[] paths)
    {
        var path = Path.Combine(ModDataDirectory, Path.Combine(paths));
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }
}
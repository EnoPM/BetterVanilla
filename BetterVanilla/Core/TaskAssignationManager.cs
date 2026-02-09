using System.Collections.Generic;
using System.IO;

namespace BetterVanilla.Core;

public sealed class TaskAssignationManager
{
    public static readonly TaskAssignationManager Instance = new();

    public List<TaskAssignationOverride> Overrides { get; }

    private TaskAssignationManager()
    {
        Overrides = [];
        if (!File.Exists(ModPaths.TaskAssignationOverridesFile)) return;
        using var stream = new FileStream(ModPaths.TaskAssignationOverridesFile, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);
            
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            Overrides.Add(new TaskAssignationOverride(reader));
        }
    }

    public void Save()
    {
        using var stream = new FileStream(ModPaths.TaskAssignationOverridesFile, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(stream);
        writer.Write(Overrides.Count);
        foreach (var @override in Overrides)
        {
            @override.Write(writer);
        }
    }
    
    public sealed class TaskAssignationOverride
    {
        public string PlayerName { get; set; }
        public string FriendCode { get; }
        public int CommonTasks { get; set; }
        public int ShortTasks { get; set; }
        public int LongTasks { get; set; }
        
        public TaskAssignationOverride(string playerName, string friendCode)
        {
            PlayerName = playerName;
            FriendCode = friendCode;
        }

        public TaskAssignationOverride(BinaryReader reader)
        {
            PlayerName = reader.ReadString();
            FriendCode = reader.ReadString();
            CommonTasks = reader.ReadInt32();
            ShortTasks = reader.ReadInt32();
            LongTasks = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(PlayerName);
            writer.Write(FriendCode);
            writer.Write(CommonTasks);
            writer.Write(ShortTasks);
            writer.Write(LongTasks);
        }
    }
}
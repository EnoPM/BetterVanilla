using System;
using System.IO;

namespace BetterVanilla.Options.Core;

public static class OptionsSerializer
{
    public static void SaveToFile(IOptionsHolder holder, string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(stream);
        holder.Save(writer);
    }

    public static void LoadFromFile(IOptionsHolder holder, string filePath)
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);
            holder.Load(reader);
        }
        catch (Exception)
        {
            // If loading fails, reset to defaults
            holder.ResetAll();
        }
    }

    public static byte[] SaveToBytes(IOptionsHolder holder)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        holder.Save(writer);
        return stream.ToArray();
    }

    public static void LoadFromBytes(IOptionsHolder holder, byte[] data)
    {
        if (data.Length == 0) return;

        try
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            holder.Load(reader);
        }
        catch (Exception)
        {
            // If loading fails, reset to defaults
            holder.ResetAll();
        }
    }
}

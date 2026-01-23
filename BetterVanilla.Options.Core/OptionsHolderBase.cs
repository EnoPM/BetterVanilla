using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BetterVanilla.Options.Core;

public abstract class OptionsHolderBase : IOptionsHolder
{
    public abstract IEnumerable<OptionBase> GetAllOptions();

    public void Save(BinaryWriter writer)
    {
        var options = GetAllOptions().ToList();

        // Write option count
        writer.Write(options.Count);

        // Write each option with length prefix for safe deserialization
        foreach (var option in options)
        {
            writer.Write(option.Key);
            writer.Write(option.GetType().Name);

            // Write option data to a temporary buffer to get the length
            using var tempStream = new MemoryStream();
            using var tempWriter = new BinaryWriter(tempStream);
            option.Write(tempWriter);
            var optionData = tempStream.ToArray();

            // Write length then data
            writer.Write(optionData.Length);
            writer.Write(optionData);
        }
    }

    public void Load(BinaryReader reader)
    {
        var options = GetAllOptions().ToDictionary(o => o.Key, o => o);

        // Read option count
        var count = reader.ReadInt32();

        // Read each option
        for (var i = 0; i < count; i++)
        {
            var key = reader.ReadString();
            var typeName = reader.ReadString();
            var length = reader.ReadInt32();

            var positionBefore = reader.BaseStream.Position;

            // Check if option exists
            if (!options.TryGetValue(key, out var option))
            {
                // Unknown option (removed from code) - skip by length
                reader.BaseStream.Position = positionBefore + length;
                continue;
            }

            // Check type name matches
            var expectedTypeName = option.GetType().Name;
            if (expectedTypeName != typeName)
            {
                // Type mismatch - skip by length
                reader.BaseStream.Position = positionBefore + length;
                continue;
            }

            try
            {
                option.Read(reader);
            }
            catch (Exception)
            {
                // Read failed - skip to end of this option's data and reset to default
                reader.BaseStream.Position = positionBefore + length;
                option.Reset();
            }
        }
    }

    public void ResetAll()
    {
        foreach (var option in GetAllOptions())
        {
            option.Reset();
        }
    }
}

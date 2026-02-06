using System.IO;
using UnityEngine;

namespace BetterVanilla.Extensions;

public static class BinaryExtensions
{
    extension(BinaryWriter writer)
    {
        public void Write(Color color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }
    }

    extension(BinaryReader reader)
    {
        public Color ReadColor()
        {
            var r = reader.ReadSingle();
            var g = reader.ReadSingle();
            var b = reader.ReadSingle();
            var a = reader.ReadSingle();
            return new Color(r, g, b, a);
        }
    }
}
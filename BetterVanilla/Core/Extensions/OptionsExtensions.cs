using BetterVanilla.Options.Core.OptionTypes;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class OptionsExtensions
{
    extension(BoolOption option)
    {
        public void Write(MessageWriter writer)
        {
            writer.Write(option.Value);
        }

        public void Read(MessageReader reader)
        {
            option.Value = reader.ReadBoolean();
        }
    }

    extension(ColorOption option)
    {
        public void Write(MessageWriter writer)
        {
            writer.Write(option.Value.r);
            writer.Write(option.Value.g);
            writer.Write(option.Value.b);
            writer.Write(option.Value.a);
        }

        public void Read(MessageReader reader)
        {
            var r = reader.ReadSingle();
            var g = reader.ReadSingle();
            var b = reader.ReadSingle();
            var a = reader.ReadSingle();
            option.Value = new Color(r, g, b, a);
        }
    }

    extension(EnumOption option)
    {
        public void Write(MessageWriter writer)
        {
            writer.Write(option.SelectedIndex);
        }

        public void Read(MessageReader reader)
        {
            var index = reader.ReadInt32();
            if (index >= 0 && index < option.Choices.Count)
            {
                option.SelectedIndex = index;
            }
            else
            {
                option.SelectedIndex = option.DefaultIndex;
            }
        }
    }

    extension(FloatOption option)
    {
        public void Write(MessageWriter writer)
        {
            writer.Write(option.Value);
        }

        public void Read(MessageReader reader)
        {
            option.Value = option.Clamp(reader.ReadSingle());
        }
    }

    extension(IntOption option)
    {
        public void Write(MessageWriter writer)
        {
            writer.Write(option.Value);
        }

        public void Read(MessageReader reader)
        {
            option.Value = option.Clamp(reader.ReadInt32());
        }
    }

    extension(BetterVanilla.Options.Core.OptionTypes.StringOption option)
    {
        public void Write(MessageWriter writer)
        {
            writer.Write(option.Value);
        }

        public void Read(MessageReader reader)
        {
            option.Value = option.Truncate(reader.ReadString());
        }
    }

    extension(Vector2Option option)
    {
        public void Write(MessageWriter writer)
        {
            writer.Write(option.Value.x);
            writer.Write(option.Value.y);
        }

        public void Read(MessageReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            option.Value = new Vector2(x, y);
        }
    }
}
using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class MessageWriterExtensions
{
    extension(MessageWriter writer)
    {
        public void SendImmediately()
        {
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void Write(Color color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }
    }
}
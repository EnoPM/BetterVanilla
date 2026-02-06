using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class ChatBubbleExtensions
{
    extension(ChatBubble bubble)
    {
        public void SetPrivateChatBubbleName(NetworkedPlayerInfo senderInfo,
            NetworkedPlayerInfo receiverInfo,
            bool isDead,
            bool didVote,
            Color nameColor)
        {
            var receiverName = PlayerControl.LocalPlayer.Data.PlayerName == receiverInfo.PlayerName
                ? "me"
                : receiverInfo.PlayerName;
            var playerName = $"{senderInfo.PlayerName} {ColorUtils.ColoredString(Color.gray, $"[to {receiverName}]")}";
            bubble.SetName(playerName, isDead, didVote, nameColor);
        }
    }
}
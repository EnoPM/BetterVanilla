using System.Collections;
using BetterVanilla.Components;
using EnoUnityLoader.Il2Cpp.Utils;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class NetworkedPlayerInfoExtensions
{
    extension(NetworkedPlayerInfo playerInfo)
    {
        public void RegisterFriendCode()
        {
            playerInfo.StartCoroutine(playerInfo.CoSetFriendCode());
        }

        private IEnumerator CoSetFriendCode()
        {
            var timeout = 30f;
            while (playerInfo.Object == null && timeout > 0f)
            {
                yield return new WaitForSeconds(1f);
                timeout -= 1f;
            }

            if (playerInfo.Object == null)
            {
                Ls.LogWarning($"No PlayerControl found for {playerInfo.PlayerName}");
                yield break;
            }

            timeout = 30f;
            while (playerInfo.Object.gameObject.GetComponent<BetterPlayerControl>() == null && timeout > 0f)
            {
                yield return new WaitForSeconds(1f);
                timeout -= 1f;
            }

            var control = playerInfo.Object.gameObject.GetComponent<BetterPlayerControl>();
            if (control == null)
            {
                Ls.LogWarning($"No BetterPlayerControl found for {playerInfo.PlayerName}");
                yield break;
            }

            control.SetFriendCode(playerInfo.FriendCode);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using BetterVanilla.Components;
using BetterVanilla.Core.Helpers;
using BetterVanilla.Extensions;
using BetterVanilla.Options;
using EnoUnityLoader.Il2Cpp.Utils;
using InnerNet;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class PlayerControlExtensions
{
    extension(PlayerControl player)
    {
        public void ReportPlayer(ReportReasons reason = ReportReasons.None)
        {
            if (!player) return;
            var client = player.GetClient();
            if (client == null || client.HasBeenReported) return;
            AmongUsClient.Instance.ReportPlayer(client.Id, reason);
        }

        private ClientData? GetClient()
        {
            try
            {
                return AmongUsClient.Instance.allClients.ToArray()
                    .FirstOrDefault(cd => cd.Character.PlayerId == player.PlayerId);
            }
            catch
            {
                return null;
            }
        }

        public List<NormalPlayerTask> GetRemainingTasks()
        {
            if (!player || !player.Data || player.Data.Tasks == null || player.myTasks == null)
            {
                return [];
            }

            var results = new List<NormalPlayerTask>();
            foreach (var playerTask in player.myTasks)
            {
                if (!playerTask) continue;
                if (playerTask.IsComplete) continue;
                if (!playerTask.Is<NormalPlayerTask>(out var normalPlayerTask))
                {
                    continue;
                }

                if (normalPlayerTask.IsComplete) continue;
                results.Add(normalPlayerTask);
            }

            return results;
        }

        public IEnumerator CoBetterStart()
        {
            yield return CoroutineUtils.CoAssertWithTimeout(() => player.PlayerId != byte.MaxValue,
                () =>
                {
                    AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error,
                        "Timeout while waiting for player ID assignment");
                }, 30f);

            yield return CoroutineUtils.CoAssertWithTimeout(
                () => GameManager.Instance != null && GameData.Instance != null && player.Data != null,
                () =>
                {
                    AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error,
                        "Timeout while waiting for player data containers");
                }, 30f);
            player.RemainingEmergencies = GameManager.Instance.LogicOptions.GetNumEmergencyMeetings();
            player.SetColorBlindTag();
            player.cosmetics.UpdateVisibility();
            if (player.AmOwner)
            {
                player.lightSource = Object.Instantiate(player.LightPrefab, player.transform, false);
                player.lightSource.Initialize(player.Collider.offset * 0.5f);
                PlayerControl.LocalPlayer = player;
                player.cosmetics.SetAsLocalPlayer();
                var mainCamera = Camera.main;
                while (mainCamera == null)
                {
                    yield return null;
                    mainCamera = Camera.main;
                }

                mainCamera.GetComponent<FollowerCamera>().SetTarget(player);
                player.SetName(DataManager.Player.Customization.Name);
                player.SetColor(DataManager.Player.Customization.Color);
                if (Application.targetFrameRate > 30)
                {
                    player.MyPhysics.EnableInterpolation();
                }

                player.CmdCheckName(DataManager.Player.Customization.Name);
                player.CmdCheckColor(DataManager.Player.Customization.Color);
                player.RpcSetPet(DataManager.Player.Customization.Pet);
                player.RpcSetHat(DataManager.Player.Customization.Hat);
                player.RpcSetSkin(DataManager.Player.Customization.Skin);
                if (HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat).BlocksVisors)
                {
                    DataManager.Player.Customization.Visor = "visor_EmptyVisor";
                }

                player.RpcSetVisor(DataManager.Player.Customization.Visor);
                player.RpcSetNamePlate(DataManager.Player.Customization.NamePlate);
                var playerLevel = DataManager.Player.Stats.Level;
                SponsorOptions.Default.LevelOverride.MaxValue = DataManager.Player.Stats.Level + 1;
                player.RpcSetLevel(SponsorOptions.Default.LevelOverride.IsAllowed()
                    ? (uint)Mathf.RoundToInt(SponsorOptions.Default.LevelOverride.Value - 1f)
                    : playerLevel);
                player.CustomOwnerSpawnHandshake();
                yield return null;
            }
            else
            {
                player.StartCoroutine(player.ClientInitialize());
            }

            if (player.Data.Role == null)
            {
                player.Data.Role = Object.Instantiate(GameData.Instance.DefaultRole);
                player.Data.Role.Initialize(player);
            }

            player.MyPhysics.SetBodyType(player.BodyType);
            if (player.isNew)
            {
                player.isNew = false;
                player.StartCoroutine(player.MyPhysics.CoSpawnPlayer());
            }

            if (PlayerControl.LocalPlayer == player)
            {
                player.clickKillCollider.enabled = false;
            }

            player.CustomSpawnHandshake();
        }

        public void RpcHidePet()
        {
            if (player == null || player.Data == null) return;
            Ls.LogInfo($"Hiding pet for {player.Data.PlayerName}");
            player.RpcSetPet(PetData.EmptyId);
        }

        public void SetMovement(bool canMove)
        {
            if (!player) return;
            player.moveable = canMove;
            player.MyPhysics.ResetMoveState(false);
            player.NetTransform.enabled = canMove;
            player.MyPhysics.enabled = canMove;
            player.NetTransform.Halt();
        }

        private void CustomOwnerSpawnHandshake()
        {
            var player1 = player.gameObject.GetComponent<BetterPlayerControl>();
            if (player1 == null)
            {
                Ls.LogError(
                    $"Unable to start {nameof(CustomOwnerSpawnHandshake)} because it doesn't have a {nameof(BetterPlayerControl)}");
                return;
            }

            player1.StartCoroutine(player1.CoOwnerSpawnHandshake());
        }

        private void CustomSpawnHandshake()
        {
            if (BetterPlayerControl.LocalPlayer == null) return;
            BetterPlayerControl.LocalPlayer.StartCoroutine(BetterPlayerControl.CoSpawnHandshake());
        }
    }
}
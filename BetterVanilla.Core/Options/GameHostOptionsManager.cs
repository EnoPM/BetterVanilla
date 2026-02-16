using System;
using BetterVanilla.Options;
using BetterVanilla.Options.Core;

namespace BetterVanilla.Core.Options;

public sealed class GameHostOptionsManager : OptionsManager<GameHostOptions>
{
    public GameHostOptionsManager(string filePath, double saveDelay = 3) : base(filePath, saveDelay)
    {
        Options.ProtectFirstKilledPlayer.ValueChanged += OnProtectFirstKilledPlayerValueChanged;

        var events = GameEventManager.Instance;

        events.GameStarted += UpdateOptionsStates;
        events.GameEnded += UpdateOptionsStates;
        events.HostChanged += UpdateOptionsStates;
        events.PlayerReady += OnPlayerReady;
        events.ModdedLobbyChanged += UpdateOptionsStates;

        ModOptions.VanillaHost.Options.Map.ValueChanged += OnMapValueChanged;
        
        OnMapValueChanged();
        UpdateOptionsStates();
        OnProtectFirstKilledPlayerValueChanged();
    }

    private void OnMapValueChanged()
    {
        var selectedMap = ModOptions.VanillaHost.Options.Map.ValueAs(MapNames.Skeld);
        var isPolus = selectedMap == MapNames.Polus;
        
        Options.BetterPolus.IsVisible = isPolus;
        Options.PolusReactorCountdown.IsVisible = isPolus;
    }
    
    private void OnPlayerReady(PlayerControl _) => UpdateOptionsStates();

    private void UpdateOptionsStates()
    {
        var enabled = GameEventManager.Instance.AmHost && !GameEventManager.Instance.IsGameStarted;
        var isModdedLobby = GameEventManager.Instance.IsModdedLobby;

        Options.AllowDeadVoteDisplay.IsEnabled = enabled;
        Options.AllowTeamPreference.IsEnabled = enabled;
        Options.HideDeadPlayerPets.IsEnabled = enabled;
        Options.PolusReactorCountdown.IsEnabled = enabled;
        Options.ProtectFirstKilledPlayer.IsEnabled = enabled;
        Options.ProtectionDuration.IsEnabled = enabled;
        Options.DefineCommonTasksAsNonCommon.IsEnabled = enabled;
        Options.BetterPolus.IsEnabled = enabled;
        
        Options.RandomizeFixWiringTaskOrder.IsEnabled = enabled && isModdedLobby;
        Options.RandomizeUploadTaskLocation.IsEnabled = enabled && isModdedLobby;
        Options.RandomizePlayerOrderInMeetings.IsEnabled = enabled && isModdedLobby;
        Options.AnonymizePlayersOnCamerasDuringLights.IsEnabled = enabled && isModdedLobby;
    }

    private void OnProtectFirstKilledPlayerValueChanged()
    {
        Options.ProtectionDuration.IsVisible = Options.ProtectFirstKilledPlayer.Value;
    }
}
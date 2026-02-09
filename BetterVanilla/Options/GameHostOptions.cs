using BetterVanilla.Components;

namespace BetterVanilla.Options;

public partial class GameHostOptions
{
    public GameHostOptions()
    {
        ProtectFirstKilledPlayer.ValueChanged += OnProtectFirstKilledPlayerValueChanged;
        
        GameEventManager.Instance.GameStarted += UpdateOptionStates;
        GameEventManager.Instance.GameEnded += UpdateOptionStates;
        GameEventManager.Instance.HostChanged += UpdateOptionStates;
        GameEventManager.Instance.PlayerReady += OnPlayerReady;
        GameEventManager.Instance.ModdedLobbyChanged += UpdateOptionStates;
        GameEventManager.Instance.SelectedMapChanged += OnSelectedMapChanged;
        
        OnProtectFirstKilledPlayerValueChanged();
        UpdateOptionStates();
    }

    private void OnSelectedMapChanged()
    {
        var isPolus = GameEventManager.Instance.SelectedMap == MapNames.Polus;
        BetterPolus.IsVisible = isPolus;
        PolusReactorCountdown.IsVisible = isPolus;
    }

    private void UpdateOptionStates()
    {
        var enabled = GameEventManager.Instance.AmHost && !GameEventManager.Instance.IsGameStarted;
        var isModdedLobby = GameEventManager.Instance.IsModdedLobby;
        
        AllowDeadVoteDisplay.IsEnabled = enabled;
        AllowTeamPreference.IsEnabled = enabled;
        HideDeadPlayerPets.IsEnabled = enabled;
        PolusReactorCountdown.IsEnabled = enabled;
        ProtectFirstKilledPlayer.IsEnabled = enabled;
        ProtectionDuration.IsEnabled = enabled;
        DefineCommonTasksAsNonCommon.IsEnabled = enabled;
        BetterPolus.IsEnabled = enabled;

        RandomizeFixWiringTaskOrder.IsEnabled = enabled && isModdedLobby;
        RandomizeUploadTaskLocation.IsEnabled = enabled && isModdedLobby;
        RandomizePlayerOrderInMeetings.IsEnabled = enabled && isModdedLobby;
        AnonymizePlayersOnCamerasDuringLights.IsEnabled = enabled && isModdedLobby;
    }
    
    private void OnPlayerReady(PlayerControl _) => UpdateOptionStates();

    private void OnProtectFirstKilledPlayerValueChanged() => ProtectionDuration.IsVisible = ProtectFirstKilledPlayer.Value;
}
using System;
using BetterVanilla.Core;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; } = null!;
    
    public event Action? HostChanged;
    public event Action<PlayerControl>? PlayerJoined;
    public event Action<PlayerControl>? PlayerReady;
    public event Action? GameStarted;
    public event Action? GameEnded;
    public event Action? MeetingStarted;
    public event Action? GameReallyStarted;
    public event Action? SelectedMapChanged;
    public event Action? ModdedLobbyChanged;

    private void Awake()
    {
        Instance = this;
    }

    public bool AmHost
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            RaiseHostChanged();
        }
    }
    
    public bool IsGameStarted { get; private set; }

    public MapNames? SelectedMap
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            RaiseSelectedMapChanged();
        }
    }

    public bool IsModdedLobby
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            RaiseModdedLobbyChanged();
        }
    }

    private void Update()
    {
        var amongUsClient = AmongUsClient.Instance;
        
        AmHost = amongUsClient && AmongUsClient.Instance.AmHost;
        SelectedMap = (MapNames?)GameOptionsManager.Instance?.currentGameOptions?.MapId;
        IsModdedLobby = LocalConditions.IsAllPlayersUsingBetterVanilla();
    }
    
    private void RaiseHostChanged() => HostChanged?.Invoke();
    internal void RaisePlayerJoined(PlayerControl player) => PlayerJoined?.Invoke(player);
    internal void RaisePlayerReady(PlayerControl player) => PlayerReady?.Invoke(player);
    internal void RaiseGameStarted()
    {
        IsGameStarted = true;
        GameStarted?.Invoke();
    }
    internal void RaiseMeetingStarted() => MeetingStarted?.Invoke();
    internal void RaiseGameEnded()
    {
        IsGameStarted = false;
        GameEnded?.Invoke();
    }
    internal void RaiseGameReallyStarted() => GameReallyStarted?.Invoke();
    private void RaiseSelectedMapChanged() => SelectedMapChanged?.Invoke();
    private void RaiseModdedLobbyChanged() => ModdedLobbyChanged?.Invoke();
}
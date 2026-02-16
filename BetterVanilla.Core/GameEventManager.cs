using System;
using UnityEngine;

namespace BetterVanilla.Core;

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
    }

    public void RaiseHostChanged() => HostChanged?.Invoke();
    public void RaisePlayerJoined(PlayerControl player) => PlayerJoined?.Invoke(player);
    public void RaisePlayerReady(PlayerControl player) => PlayerReady?.Invoke(player);
    public void RaiseGameStarted()
    {
        IsGameStarted = true;
        GameStarted?.Invoke();
    }
    public void RaiseMeetingStarted() => MeetingStarted?.Invoke();
    public void RaiseGameEnded()
    {
        IsGameStarted = false;
        GameEnded?.Invoke();
    }
    public void RaiseGameReallyStarted() => GameReallyStarted?.Invoke();
    private void RaiseModdedLobbyChanged() => ModdedLobbyChanged?.Invoke();
}
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Commands.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.SwarmControl.Models.Game.Messages;
using TMPro;
using UnityEngine;

namespace SCHIZO.SwarmControl;

[CommandCategory("Swarm Control")]
partial class SwarmControlManager
{
    private const string PLAYERPREFS_KEY = $"SCHIZO_{nameof(SwarmControlManager)}";
    public const string COMMAND_URL = "sc_url";
    public const string COMMAND_KEY = "sc_apikey";

    public static SwarmControlManager Instance { get; private set; }

    internal MessageProcessor Processor { get; private set; }
    public string BackendUrl
    {
        get => PlayerPrefs.GetString($"{PLAYERPREFS_KEY}_{nameof(BackendUrl)}", defaultBackendUrl);
        set => PlayerPrefs.SetString($"{PLAYERPREFS_KEY}_{nameof(BackendUrl)}", value);
    }

    public string PrivateApiKey
    {
        get => PlayerPrefs.GetString($"{PLAYERPREFS_KEY}_{nameof(PrivateApiKey)}", "");
        set => PlayerPrefs.SetString($"{PLAYERPREFS_KEY}_{nameof(PrivateApiKey)}", value);
    }

    private ControlWebSocket _socket;

    public bool Ingame { get; private set; }
    public bool CanSpawn => Ingame
        && Player.main
        && Player.main.IsUnderwaterForSwimming()
#if BELOWZERO
        && !Player.main.IsPilotingSeatruck()
#endif
        && !Player.main.IsInBase()
        && !Player.main.cinematicModeActive
        //&& Time.timeScale > 0
        ;

    private void Awake()
    {
        Instance = this;
        _socket = new();
        Processor = new(_socket);
        SaveUtils.RegisterOnFinishLoadingEvent(OnLoad);
        SaveUtils.RegisterOnQuitEvent(OnQuit);
    }

    private void OnLoad()
    {
        StartCoroutine(OnLoadCoro());
    }

    private IEnumerator OnLoadCoro()
    {
        uGUI gui = uGUI.main;
        yield return new WaitUntil(() => gui.loading && !gui.loading.isLoading);
        Ingame = true;
        SendIngameStateMsg();
    }

    private void OnQuit()
    {
        Ingame = false;
        SendIngameStateMsg();
    }

    private void OnDestroy()
    {
        _socket?.Dispose();
    }

    [InitializeMod]
    private static void InitConnectButton()
    {
        Assets.Mod_Options_SwarmControl.onPressed
            .AddListener(() => Instance.ButtonPressed());
    }

    [Command(Name = COMMAND_URL, RegisterConsoleCommand = true)]
    private static void SetUrl(string url)
    {
        Instance.BackendUrl = url;
    }
    [Command(Name = COMMAND_KEY, RegisterConsoleCommand = true)]
    private static void SetKey(string key)
    {
        Instance.PrivateApiKey = key;
    }

    [Command(Name = "sc_rc", RegisterConsoleCommand = true)]
    private static void ManualReconnectCommand()
    {
        _ = Instance.ManualReconnect();
    }

    private async Task ManualReconnect()
    {
        await _socket.Disconnect();
        if (!await _socket.ConnectSocket())
            QueueOnMainThread(() => LOGGER.LogWarning("Manual reconnect failed"));
        _reconnectCoro ??= StartCoroutine(AutoReconnectCoro());
    }

    private void ButtonPressed()
    {
        if (!_socket.IsConnected)
            Connect();
        else
            Disconnect();
    }

    private void Connect()
    {
        CancellationTokenSource cts = new();
        ShowConfirmation("Connecting", "Cancel");
        StartCoroutine(WaitForConfirmationClose(cts));
        StartCoroutine(ConnectCoro(cts.Token));
    }

    private void ShowConfirmation(string message, string buttonText = "OK")
    {
        uGUI.main.confirmation.Show(message);
        uGUI.main.confirmation.ok.GetComponentInChildren<TMP_Text>().text = buttonText;
    }

    private IEnumerator WaitForConfirmationClose(CancellationTokenSource cts)
    {
        while (uGUI.main.confirmation.gameObject.activeSelf)
            yield return null;
        cts.Cancel();
        uGUI.main.confirmation.ok.GetComponentInChildren<TMP_Text>().text = "OK";
    }

    private Coroutine _reconnectCoro;
    private IEnumerator ConnectCoro(CancellationToken ct)
    {
        Task<string> task = _socket.Connect(ct);
        yield return task.PoorMansAwait();
        uGUI.main.confirmation.Close(true);

        if (!string.IsNullOrEmpty(task.Result))
            uGUI.main.confirmation.Show(task.Result);

        if (_socket.IsConnected)
        {
            Assets.Mod_Options_SwarmControl.label = "Disconnect Swarm Control";
            _reconnectCoro = StartCoroutine(AutoReconnectCoro());
        }
    }

    private void Disconnect()
    {
        if (_reconnectCoro is { })
        {
            StopCoroutine(_reconnectCoro);
            _reconnectCoro = null;
        }
        if (!_socket.IsConnected) return;
        StartCoroutine(DisconnectCoro());
    }

    private IEnumerator DisconnectCoro()
    {
        Task<string> task = _socket.Disconnect();
        yield return task.PoorMansAwait();

        if (!string.IsNullOrEmpty(task.Result))
            uGUI.main.confirmation.Show(task.Result);

        Assets.Mod_Options_SwarmControl.label = "Connect Swarm Control";
    }

    private ConcurrentQueue<Action> _onMainThread = [];
    internal void QueueOnMainThread(Action action)
    {
        if (CurrentThreadIsMainThread())
            action();
        else
            _onMainThread.Enqueue(action);
    }

    private void Update()
    {
        while (_onMainThread.TryDequeue(out Action action))
            action();
    }

    private int _retries;
    private const int MaxRetries = int.MaxValue;
    private IEnumerator AutoReconnectCoro()
    {
        while (_retries < MaxRetries)
        {
            if (_socket.IsConnected)
            {
                _retries = 0;
                yield return new WaitForSecondsRealtime(5);
                continue;
            }
            LOGGER.LogDebug("Reconnecting...");
            Task<bool> task = _socket.ConnectSocket();
            yield return task.PoorMansAwait();
            if (!task.Result) _retries++;

            yield return new WaitForSecondsRealtime(5);
        }

        LOGGER.LogWarning("Could not reconnect to websocket");
        ShowConfirmation("Lost connection to server\nReconnect in options");
    }

    internal void SendIngameStateMsg()
    {
        if (_socket.IsConnected)
        {
            _socket.SendMessage(new IngameStateChangedMessage()
            {
                Ingame = Ingame,
            });
        }
    }
}

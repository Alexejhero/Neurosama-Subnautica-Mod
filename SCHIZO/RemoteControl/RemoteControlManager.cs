using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using TMPro;
using UnityEngine;

namespace SCHIZO.RemoteControl;

partial class RemoteControlManager
{
    private const string PLAYERPREFS_KEY = "SCHIZO_RemoteControl";

    public static RemoteControlManager Instance { get; private set; }

    public string BackendUrl
    {
        get => PlayerPrefs.GetString($"{PLAYERPREFS_KEY}_{nameof(BackendUrl)}", defaultWebServerUrl);
        set => PlayerPrefs.SetString($"{PLAYERPREFS_KEY}_{nameof(BackendUrl)}", value);
    }

    private ControlWebSocket _socket;

    private void Awake()
    {
        Instance = this;
        _socket = new();
    }

    private void OnDestroy()
    {
        _socket?.Dispose();
    }

    [InitializeMod]
    private static void InitConnectButton()
    {
        Assets.Mod_Options_ConnectTwitch.onPressed
            .AddListener(() => Instance.Connect());
    }

    private void Connect()
    {
        CancellationTokenSource cts = new();
        ShowConfirmation(cts);
        StartCoroutine(ConnectCoro(cts.Token));
    }

    private void ShowConfirmation(CancellationTokenSource cts)
    {
        uGUI.main.confirmation.Show("Connecting...\nA browser window should open shortly.");
        uGUI.main.confirmation.ok.GetComponentInChildren<TMP_Text>().text = "Cancel";
        StartCoroutine(TrackConfirmation(cts));
    }

    private IEnumerator TrackConfirmation(CancellationTokenSource cts)
    {
        while (uGUI.main.confirmation.gameObject.activeSelf)
            yield return null;
        cts.Cancel();
        uGUI.main.confirmation.ok.GetComponentInChildren<TMP_Text>().text = "OK";
    }

    private IEnumerator ConnectCoro(CancellationToken ct)
    {
        Task<string> task = _socket.Connect(ct);
        yield return task.PoorMansAwait();
        uGUI.main.confirmation.Close(true);
        OnConnectError(task.Result);
    }

    private void OnConnectError(string error)
    {
        if (string.IsNullOrEmpty(error)) return;

        uGUI.main.confirmation.Show(error);
    }
}

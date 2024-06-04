using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using SCHIZO.Attributes;
using SCHIZO.Commands.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using TMPro;
using UnityEngine;

namespace SCHIZO.SwarmControl;

[CommandCategory("Swarm Control")]
partial class SwarmControlManager
{
    private const string PLAYERPREFS_KEY = $"SCHIZO_{nameof(SwarmControlManager)}";

    public static SwarmControlManager Instance { get; private set; }

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
        Assets.Mod_Options_SwarmControl.onPressed
            .AddListener(() => Instance.ButtonPressed());
    }

    [Command(Name = "setswarmcontrolurl", RegisterConsoleCommand = true)]
    private static void SetUrl(string url)
    {
        Instance.BackendUrl = url;
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
        ShowConfirmation(cts);
        StartCoroutine(ConnectCoro(cts.Token));
    }

    private void ShowConfirmation(CancellationTokenSource cts)
    {
        uGUI.main.confirmation.Show("Connecting...\nA browser window should open shortly.");
        uGUI.main.confirmation.ok.GetComponentInChildren<TMP_Text>().text = "Cancel";
        StartCoroutine(WaitForConfirmationClose(cts));
    }

    private IEnumerator WaitForConfirmationClose(CancellationTokenSource cts)
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

        if (!string.IsNullOrEmpty(task.Result))
            uGUI.main.confirmation.Show(task.Result);

        if (_socket.IsConnected)
        {
            Assets.Mod_Options_SwarmControl.label = "Disconnect Swarm Control";
        }
    }

    private void Disconnect()
    {
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
}

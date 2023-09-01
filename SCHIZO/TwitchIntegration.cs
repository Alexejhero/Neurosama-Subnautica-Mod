using System;
using System.IO;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace SCHIZO;

public sealed class TwitchIntegration
{
    private readonly TwitchClient _client;

    public TwitchIntegration()
    {
        ConnectionCredentials credentials = new("AlexejheroDev", File.ReadAllLines(Path.Combine(AssetLoader.AssetsFolder, "..", "config.json"))[1]);
        ClientOptions clientOptions = new()
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        WebSocketClient customClient = new(clientOptions);
        _client = new TwitchClient(customClient);
        _client.Initialize(credentials, "AlexejheroDev");

        _client.OnConnected += (_, evt) => LOGGER.LogInfo("Connected as " + evt.BotUsername);
        _client.OnError += (_, evt) => LOGGER.LogError(evt.Exception);
        _client.OnMessageReceived += Client_OnMessageReceived;
    }

    public void Connect() => _client.Connect();

    private void Client_OnMessageReceived(object _, OnMessageReceivedArgs evt)
    {
        ChatMessage message = evt.ChatMessage;

        LOGGER.LogDebug(message.Username + ": " + message.Message);

        if (message.Username != "AlexejheroDev") return; // ensure I don't get isekaid
        if (!message.Message.StartsWith("!sn ")) return;

        HandleMessage(message.Message[4..]);
    }

    private void HandleMessage(string message)
    {
        if (message.StartsWith("con ")) DevConsole.SendConsoleCommand(message[4..]);
    }
}

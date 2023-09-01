using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using UnityEngine;

namespace SCHIZO;

public sealed class TwitchIntegration : MonoBehaviour
{
    private readonly TwitchClient _client;

    public TwitchIntegration()
    {
        DevConsole.RegisterConsoleCommand(this, "say");

        ConnectionCredentials credentials = new("AlexejheroDev", File.ReadAllLines(Path.Combine(AssetLoader.AssetsFolder, "..", "config.json"))[1]);
        ClientOptions clientOptions = new()
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        WebSocketClient customClient = new(clientOptions);
        _client = new TwitchClient(customClient);
        _client.Initialize(credentials, "vedal987");

        _client.OnConnected += (_, evt) => LOGGER.LogInfo("Connected as " + evt.BotUsername);
        _client.OnError += (_, evt) => LOGGER.LogError(evt.Exception);
        _client.OnMessageReceived += Client_OnMessageReceived;

        _client.Connect();
    }

    private void Client_OnMessageReceived(object _, OnMessageReceivedArgs evt)
    {
        ChatMessage message = evt.ChatMessage;

        if (message.Username.ToLower() != "alexejherodev") return; // ensure I don't get isekaid
        if (!message.Message.ToLower().StartsWith("!s ")) return;

        HandleMessage(message.Message[3..]);
    }

    private void HandleMessage(string message)
    {
        if (message.ToLower().StartsWith("c ")) DevConsole.SendConsoleCommand(message[2..]);
    }

    [UsedImplicitly]
    private void OnConsoleCommand_say(NotificationCenter.Notification n)
    {
        string message = string.Join(" ", n.data.Values.Cast<string>().Reverse().ToArray());
        ErrorMessage.AddMessage(message);
    }
}

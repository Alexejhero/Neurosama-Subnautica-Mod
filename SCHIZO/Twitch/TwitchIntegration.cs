using System;
using System.Collections.Concurrent;
using System.Linq;
using JetBrains.Annotations;
using Nautilus.Commands;
using SCHIZO.Console;
using SCHIZO.Helpers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using UnityEngine;

namespace SCHIZO.Twitch;

[LoadConsoleCommands]
partial class TwitchIntegration
{
    private const string _usernamePlayerPrefsKey = "SCHIZO_TwitchIntegration_OAuthToken";
    private const string _tokenPlayerPrefsKey = "SCHIZO_TwitchIntegration_OAuthToken";

    private TwitchClient _client;
    private readonly ConcurrentQueue<string> _msgQueue = new();

    private void Awake()
    {
        ClientOptions clientOptions = new()
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        WebSocketClient customClient = new(clientOptions);
        _client = new TwitchClient(customClient);

        _client.OnError += (_, evt) => LOGGER.LogError(evt.Exception);
        _client.OnIncorrectLogin += (_, evt) => LOGGER.LogError($"Could not connect to Twitch: {evt.Exception.Message}");
        _client.OnConnected += (_, evt) => LOGGER.LogInfo($"Connected to Twitch as {evt.BotUsername}");
        _client.OnConnectionError += (_, evt) => LOGGER.LogError($"Could not connect to Twitch: {evt.Error.Message}");
        _client.OnJoinedChannel += (_, evt) => LOGGER.LogInfo($"Joined Twitch channel {evt.Channel}");
        _client.OnFailureToReceiveJoinConfirmation += (_, evt) => LOGGER.LogError($"Could not join Twitch channel: {evt.Exception.Details}");
        _client.OnMessageReceived += Client_OnMessageReceived;

        string username = PlayerPrefs.GetString(_usernamePlayerPrefsKey, "");
        string token = PlayerPrefs.GetString(_tokenPlayerPrefsKey, "");
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
        {
            LOGGER.LogWarning("Twitch OAuth token is not set, Twitch Integration will be disabled.");
            LOGGER.LogMessage("Run 'settwitchlogin <username> <token>' in the developer console and restart Subnautica in order to enable it.");
            return;
        }
        ConnectionCredentials credentials = new(username, token);

        _client.Initialize(credentials, targetChannel);

        _client.Connect();
    }

    private void Client_OnMessageReceived(object _, OnMessageReceivedArgs evt)
    {
        ChatMessage message = evt.ChatMessage;

        if (!IsUserWhitelisted(message.Username)) return; // ensure I don't get isekaid
        if (!CheckPrefix(message.Message)) return;

        // OnMessageReceived runs in a worker thread, where we can't use Unity APIs
        _msgQueue.Enqueue(message.Message[commandPrefix.Length..]);
    }

    private bool IsUserWhitelisted(string username)
    {
        return whitelistedUsers.Any(user => user.Equals(username, StringComparison.InvariantCultureIgnoreCase));
    }

    private bool CheckPrefix(string message)
    {
        return message.StartsWith(commandPrefix, prefixIsCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
    }

    private void FixedUpdate()
    {
        if (_msgQueue.Count > 0 && _msgQueue.TryDequeue(out string message)) HandleMessage(message);
    }

    private static void HandleMessage(string message)
    {
        MessageHelpers.SuppressOutput = true;
        DevConsole.SendConsoleCommand(message);
        MessageHelpers.SuppressOutput = false;
    }

    [ConsoleCommand("settwitchlogin"), UsedImplicitly]
    public static string OnConsoleCommand_settwitchkey(string username, string token)
    {
        PlayerPrefs.SetString(_usernamePlayerPrefsKey, username);
        PlayerPrefs.SetString(_tokenPlayerPrefsKey, token);
        return "Twitch login updated. Please restart Subnautica.";
    }
}

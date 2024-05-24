using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SCHIZO.Commands;
using SCHIZO.Helpers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using UnityEngine;

namespace SCHIZO.Twitch;

[RegisterCommands]
partial class TwitchIntegration
{
    private const string _usernamePlayerPrefsKey = "SCHIZO_TwitchIntegration_Username";
    private const string _tokenPlayerPrefsKey = "SCHIZO_TwitchIntegration_OAuthToken";

    private TwitchClient _client;
    private readonly ConcurrentQueue<string> _msgQueue = new();
    private HashSet<string> _allowedUsersSet;

    private void Awake()
    {
        _allowedUsersSet = new HashSet<string>(whitelistedUsers, StringComparer.OrdinalIgnoreCase);
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
        return _allowedUsersSet.Contains(username);
    }

    private bool CheckPrefix(string message)
    {
        return message.StartsWith(commandPrefix, prefixIsCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
    }

    private void FixedUpdate()
    {
        if (_msgQueue.TryDequeue(out string message)) HandleMessage(message);
    }

    private static void HandleMessage(string message)
    {
        MessageHelpers.SuppressOutput = true;
        DevConsole.SendConsoleCommand(message);
        MessageHelpers.SuppressOutput = false;
    }

    [ConsoleCommand("settwitchlogin")]
    public static string OnConsoleCommand_settwitchlogin(string username, string token)
    {
        PlayerPrefs.SetString(_usernamePlayerPrefsKey, username);
        PlayerPrefs.SetString(_tokenPlayerPrefsKey, token);
        return "Twitch login updated. Please restart Subnautica.";
    }
}

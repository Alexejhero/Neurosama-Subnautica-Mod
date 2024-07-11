using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;
using SwarmControl.Models.Game.Messages;

namespace SCHIZO.SwarmControl;

#nullable enable
internal sealed class MessageProcessor
{
    private readonly ControlWebSocket _socket;

    //private readonly Dictionary<Guid, RedeemMessage> _redeems = [];
    private readonly Dictionary<Guid, ResultMessage> _results = [];

    public MessageProcessor(ControlWebSocket socket)
    {
        _socket = socket;
        _socket.OnConnected += SendHello;
        _socket.OnMessage += OnMessage;
        _socket.OnError += OnError;
        _socket.OnClose += OnClose;
    }

    private void OnMessage(BackendMessage message)
    {
        switch (message)
        {
            case HelloBackMessage helloBack:
                OnHelloBack(helloBack);
                break;
            case ConsoleInputMessage consoleInput:
                OnConsoleInput(consoleInput);
                break;
            case RedeemMessage invoke:
                OnRedeem(invoke);
                break;
        }
    }

    private void OnError(Exception exception)
    {
        LOGGER.LogError($"Websocket error: {exception}");
    }
    private void OnClose(WebSocketCloseStatus status, string? reason)
    {
        LOGGER.LogWarning($"Websocket closed with status {status} and {(reason is null ? "no reason" : "reason " + reason)}");
    }

    private void OnHelloBack(HelloBackMessage helloBack)
    {
        if (!helloBack.Allowed)
        {
            LOGGER.LogError("Server rejected handshake");
            _socket.Disconnect().Start();
            uGUI.main.confirmation.Show("Server rejected handshake\nConnection is not possible");
            return;
        }

        SwarmControlManager.Instance.SendIngameStateMsg();
    }
    private void OnConsoleInput(ConsoleInputMessage msg)
    {
        LOGGER.LogDebug($"{msg.GetUsername()} executed console command \"{msg.Input}\"");
        DevConsole.SendConsoleCommand(msg.Input);
    }

    private void OnRedeem(RedeemMessage msg)
    {
        Guid guid = msg.Guid;
        if (!CommandRegistry.TryGetInnermostCommand(msg.Command.SplitOnce(' ').Before, out Command command))
        {
            LOGGER.LogDebug($"{msg.GetUsername()} tried to redeem unknown command \"{msg.Command}\"");
            SendResult(guid, ResultMessage.ResultKind.Error, "Command not found");
            return;
        }
        // prevents server replaying redeems in case we receive one but the game doesn't acknowledge it or something (so the server thinks it didn't send and replays it later)
        // ...but we do still want to be able to replay manually
        if (msg.Source == CommandInvocationSource.Swarm && _results.TryGetValue(guid, out ResultMessage? result))
        {
            _socket.SendMessage(result);
            return;
        }

        JsonContext ctx = new()
        {
            Command = command,
            Input = new RemoteInput()
            {
                Command = command,
                Model = msg,
            },
            Output = new CommandOutputStack([
                //UiMessageSink.Loud,
                NullSink.Instance,
                LogSink.Info,
                new ShowUsage(command),
                new DelegateSink((ref object res) => {
                    if (res is null or string or CommonResults.OKResult && msg.Announce)
                    {
                        ErrorMessage.AddMessage($"{msg.GetDisplayName()} redeemed {msg.Title}");
                    }
                    return false;
                })]
            )
        };
        SwarmControlManager.Instance.QueueOnMainThread(() =>
        {
            try
            {
                command.Execute(ctx);
                switch (ctx.Result)
                {
                    case CommonResults.ErrorResult { message: string error }:
                        SendResult(msg.Guid, ResultMessage.ResultKind.Error, error);
                        break;
                    case CommonResults.DenyResult { reason: string reason }:
                        SendResult(msg.Guid, ResultMessage.ResultKind.Deny, reason);
                        break;
                    case null or CommonResults.OKResult:
                        SendResult(msg.Guid, ResultMessage.ResultKind.Success);
                        break;
                    case CommonResults.ExceptionResult { Exception: Exception e }:
                        SendResult(msg.Guid, ResultMessage.ResultKind.Error, e.ToString());
                        break;
                    case CommonResults.ShowUsageResult:
                        SendResult(msg.Guid, ResultMessage.ResultKind.Error, $"Incorrect command usage - {ShowUsage.GetUsageString(command)}");
                        break;
                    default:
                        SendResult(msg.Guid, ResultMessage.ResultKind.Success, ctx.Result.ToString());
                        break;
                }
            }
            catch (Exception e)
            {
                SendResult(msg.Guid, ResultMessage.ResultKind.Error, e.ToString());
            }
        });
    }

    private void SendHello()
    {
        _socket.SendMessage(new HelloMessage()
        {
            Version = SwarmControlManager.Instance.version,
        });
    }

    private void SendResult(Guid guid, ResultMessage.ResultKind kind, string? message = null)
    {
        ResultMessage msg = new()
        {
            Guid = guid,
            Status = kind,
            Message = message
        };
        _results[msg.Guid] = msg;
        _socket.SendMessage(msg);
    }
}

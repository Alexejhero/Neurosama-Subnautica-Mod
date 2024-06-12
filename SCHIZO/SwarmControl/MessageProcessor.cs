using System;
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
            SendResult(guid, false, "Command not found");
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
                    if (res is null or CommonResults.OKResult && msg.Announce)
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
                        SendResult(msg.Guid, false, error);
                        break;
                    case null or CommonResults.OKResult:
                        SendResult(msg.Guid, true);
                        break;
                    case CommonResults.ExceptionResult { Exception: Exception e }:
                        SendResult(msg.Guid, false, e.ToString());
                        break;
                    case CommonResults.ShowUsageResult:
                        SendResult(msg.Guid, false, $"Incorrect command usage - {ShowUsage.GetUsageString(command)}");
                        break;
                    default:
                        SendResult(msg.Guid, true, ctx.Result.ToString());
                        break;
                }
            }
            catch (Exception e)
            {
                SendResult(msg.Guid, false, e.ToString());
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

    private void SendResult(Guid guid, bool success, string? message = null)
    {
        _socket.SendMessage(new ResultMessage()
        {
            Guid = guid,
            Success = success,
            Message = message
        });
    }
}

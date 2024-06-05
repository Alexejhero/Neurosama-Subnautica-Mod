using System;
using System.Net.WebSockets;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;
using SwarmControl.Shared.Models.Game.Messages;

namespace SCHIZO.SwarmControl;

internal sealed class MessageProcessor
{
    private readonly ControlWebSocket _socket;
    private bool _handshakeDone;
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
            case InvokeCommandMessage invoke:
                OnInvokeCommand(invoke);
                break;
        }
    }

    private void OnError(Exception exception)
    {
        LOGGER.LogError($"Websocket error: {exception}");
    }
    private void OnClose(WebSocketCloseStatus status, string reason)
    {
        LOGGER.LogWarning($"Websocket closed with status {status} and {(reason is null ? "no reason" : "reason " + reason)}");
    }

    private void OnHelloBack(HelloBackMessage helloBack)
    {
        _handshakeDone = true;
    }
    private void OnConsoleInput(ConsoleInputMessage msg)
    {
        LOGGER.LogDebug($"{msg.GetUsername()} executed console command \"{msg.Input}\"");
        DevConsole.SendConsoleCommand(msg.Input);
    }

    private void OnInvokeCommand(InvokeCommandMessage msg)
    {
        Guid id = msg.CorrelationId;
        if (!CommandRegistry.TryGetCommand(msg.Command, out var command))
        {
            LOGGER.LogDebug($"{msg.GetUsername()}");
            SendResult(id, false, "Command not found");
            return;
        }

        SwarmControlManager.Instance.QueueOnMainThread(() =>
        {
            try
            {
                var ctx = new JsonContext()
                {
                    Command = command,
                    Input = new RemoteInput()
                    {
                        Command = command,
                        Model = msg,
                    },
                    Output = new CommandOutputStack()
                };
                command.Execute(ctx);
                SendResult(msg.CorrelationId, true, ctx.Result?.ToString());
            }
            catch (Exception e)
            {
                SendResult(msg.CorrelationId, false, e.ToString());
            }
        });
    }

    private void SendHello()
    {
        _socket.SendMessage(new HelloMessage()
        {
            Commands = [], //CommandRegistry.Categories,
            Enums = [], //CommandRegistry.Enums,
        });
    }

    private void SendResult(Guid correlationId, bool success, string message)
    {
        _socket.SendMessage(new ResultMessage()
        {
            CorrelationId = correlationId,
            Success = success,
            Message = message
        });
    }
}

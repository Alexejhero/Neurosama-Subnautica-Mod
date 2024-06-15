using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(
    Name = "redeem_notif",
    DisplayName = "Ping Alex",
    Description = "This will log an error on the backend, pinging Alex for no reason"
)]
internal class ShowNotif : Command, IParameters
{
    private readonly string[] _messages = [
        "NullReferenceException: Object reference not set to an instance of an object.",
        "TypeLoadException: The type initializer for 'SCHIZO.SwarmControl.Redeems.Annoying.ShowNotif' threw an exception.",
        "Could not convert argument",
        "Object accessed from invalid thread",
        "KeyNotFoundException: The given key was not present in the dictionary.",
        "WebSocketException: The remote party closed the WebSocket connection without completing the close handshake.",
        "TargetInvocationException: Exception has been thrown by the target of an invocation.",
        "OperationCanceledException: The operation was canceled.",
        "The parameter is incorrect",
        """
        System.Net.WebSockets.WebSocketException (0x80004005): The 'System.Net.WebSockets.ClientWebSocket' instance cannot be used for communication because it has been transitioned into the 'Aborted' state.
        ---> System.Net.WebSockets.WebSocketException (0x80004005): An internal WebSocket error occurred. Please see the innerException, if present, for more details. ---> System.Net.HttpClientException (0x80004005): An operation was attempted on a nonexistent network connection
        at System.Net.WebSockets.WebSocketHttpClientDuplexStream.WriteAsyncFast(HttpClientAsyncEventArgs eventArgs)
        at System.Net.WebSockets.WebSocketHttpClientDuplexStream.d__9.MoveNext()
        --- End of stack trace from previous location where exception was thrown ---
        at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
        at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
        at System.Net.WebSockets.WebSocketBase.d__11.MoveNext()
        --- End of stack trace from previous location where exception was thrown ---
        at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
        at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
        at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
        at System.Net.WebSockets.WebSocketBase.WebSocketOperation.d__47.MoveNext()
        --- End of stack trace from previous location where exception was thrown ---
        at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
        at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
        at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
        at System.Net.WebSockets.WebSocketBase.d__17.MoveNext()
        at System.Net.WebSockets.WebSocketBase.ThrowIfAborted(Boolean aborted, Exception innerException)
        at System.Net.WebSockets.WebSocketBase.ThrowIfConvertibleException(String methodName, Exception exception, CancellationToken cancellationToken, Boolean aborted)
        at System.Net.WebSockets.WebSocketBase.d__17.MoveNext()
        --- End of stack trace from previous location where exception was thrown ---
        at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
        at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
        at System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter.GetResult()
        """
    ];

    public IReadOnlyList<Parameter> Parameters => [];

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        return CommonResults.Error(_messages.GetRandom());
    }
}

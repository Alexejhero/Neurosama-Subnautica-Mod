namespace SwarmControl.Models.Game.Messages;

public enum MessageType
{
    // game to backend
    Hello,
    GameLoadedStateChanged,
    GamePausedStateChanged,
    CommandResult,

    // backend to game
    HelloBack,
    ConsoleInput,
    InvokeCommand
}

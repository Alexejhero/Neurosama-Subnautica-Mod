using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SwarmControl.Shared.Models.Game.Messages;

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageType
{
    // game to backend
    Hello,
    Result,
    GameLoadedStateChanged,
    GamePausedStateChanged,
    CommandAvailabilityChanged,

    // backend to game
    HelloBack,
    ConsoleInput,
    InvokeCommand,
}

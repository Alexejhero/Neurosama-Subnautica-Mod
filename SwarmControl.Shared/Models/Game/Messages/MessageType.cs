//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

namespace SwarmControl.Shared.Models.Game.Messages;

//[JsonConverter(typeof(StringEnumConverter))]
public enum MessageType
{
    // game to backend
    Hello, // send command data
    Ping, // ping
    Result, // command/console result
    GameLoadedStateChanged, // (global) redeems dependent on Player.main are open/closed
    GamePausedStateChanged, // also global
    CommandAvailabilityChanged, // individual command (for specific cases)

    // backend to game
    HelloBack, // hello ack
    Pong, // pong
    ConsoleInput, // execute console string
    Redeem, // invoke command with named args
}

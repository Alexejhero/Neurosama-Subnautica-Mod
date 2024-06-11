//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

namespace SwarmControl.Models.Game.Messages;

//[JsonConverter(typeof(StringEnumConverter))]
public enum MessageType
{
    // game to backend
    Hello, // send command data
    Ping, // ping
    Log,
    Result, // command/console result
    IngameStateChanged, // (global) e.g. redeems dependent on Player.main, or stuff that freezes with time freezers (e.g. spawns?)
    //CommandAvailabilityChanged, // individual command (for specific cases)

    // backend to game
    HelloBack, // hello ack
    Pong, // pong
    ConsoleInput, // execute console string
    Redeem, // invoke command with named args
}

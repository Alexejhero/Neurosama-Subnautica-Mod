using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SwarmControl.Models.Game.Messages;

#nullable enable
public sealed record ResultMessage : GameMessage
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResultKind
    {
        /// <summary>The redeem succeeded.</summary>
        [EnumMember(Value = "success")]
        Success,
        /// <summary>The redeem failed to execute. The user should be notified (and refunded), and the backend should send a Discord notification.</summary>
        [EnumMember(Value = "error")]
        Error,
        /// <summary>The redeem did not execute due to a precondition. The user is still refunded, but there is no need to ping in Discord.</summary>
        [EnumMember(Value = "deny")]
        Deny,
    }
    public override MessageType MessageType => MessageType.Result;
    [JsonProperty(NamingStrategyType = typeof(CamelCaseNamingStrategy), Required = Required.Always)]
    public ResultKind Status { get; set; }
    public string? Message { get; set; }
}

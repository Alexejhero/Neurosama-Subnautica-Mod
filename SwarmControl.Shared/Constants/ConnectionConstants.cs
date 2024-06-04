namespace SwarmControl.Constants;

public static class ConnectionConstants
{
    public const string GameNonceHeader = "X-Sec-Nonce";
    public const string TwitchUsernameHeader = "Twitch-Username";
    public const string TokenHeader = "Correlation-Token";

    public const string GameReplyEndpoint = "/reply";
    public const string GameErrorEndpoint = "/error";

    public const string ServerHostEndpoint = "/host";
    public const string ServerHostWebsocketEndpoint = $"{ServerHostEndpoint}/connect";
}

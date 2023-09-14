using System;
using SCHIZO.DataStructures;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCHIZO.Events.RandomMessage;

public sealed class RandomMessageEvent : CustomEvent
{
    public override bool IsOccurring => false;

    private float _nextMessageTime;
    private float _messageFrequency;

    private void Awake()
    {
        _messageFrequency = CONFIG.RandomMessageFrequency;
        _nextMessageTime = GetNextMessageTime();
    }

    private SavedRandomList<Func<string>> Messages { get; } = new("Events_RandomMessageEvent")
    {
        // some messages are off just for next stream since they already appeared way too many times
        { "watched", () => "You are being watched." },
        { "evilPresence", () => "You feel an evil presence watching you..." },
        { "pvpEnabled", () => "[Server] PvP has been enabled." },
        { "alexJoin", () => "Alex has joined the server." },
        { "alexLeave", () => "Alex has left the server." },
        { "angeredTheGods", () => "You have angered the gods." },
        { "timeGodFee", () => "The Time God has deducted 50 neuros from your balance. (protection fee)" },
        { "saveCorrupted", () => "ERROR: Save file corrupted" },
        { "timeSinceLastSave", () => $"Time since last save: {Mathf.FloorToInt((float)(DateTime.Now - SaveLoadManager.main.lastSaveTime).TotalMinutes)}m" },
        { "swarm", () => "Swarm dispatched to your coordinates." },
        { "fakeIP", () => "Server IP: 127.0.0.1" },
        { "fakeScreenshot", () => "Screenshot saved to PDA." },
        { "clue", () => "R2J6c2JieXJlbA" }, // "Tomfoolery" -> rot13 -> b64
    };

    protected override bool ShouldStartEvent()
    {
        if (_messageFrequency != CONFIG.RandomMessageFrequency)
        {
            _messageFrequency = CONFIG.RandomMessageFrequency;
            _nextMessageTime = GetNextMessageTime();
        }
        if (_messageFrequency == 0) return false;
        return Time.time > _nextMessageTime;
    }

    protected override void UpdateLogic() { }

    protected override void UpdateRender() { }

    public override void StartEvent()
    {
        Func<string> messageFunc = Messages.GetRandom();
        ErrorMessage.AddMessage(messageFunc());
        _nextMessageTime = GetNextMessageTime();
        base.StartEvent();
    }

    private float GetNextMessageTime()
    {
        float eventFrequency = _messageFrequency;
        if (eventFrequency == 0) return float.MaxValue;

        float minDays = 9 / eventFrequency;
        float chancePerDay = Mathf.Pow(0.789f, 10f - eventFrequency);
        float rangeDays = Random.Range(0, 1f / chancePerDay);
        float nextWait = 1200f * Random.Range(minDays, minDays + rangeDays);
        float nextTime = Time.time + nextWait;
        LOGGER.LogDebug($"Next random message will be at {nextTime} (after {nextWait}/{minDays}/{rangeDays})");
        return nextTime;
    }
}

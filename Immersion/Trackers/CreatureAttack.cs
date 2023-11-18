namespace Immersion.Trackers;

public class CreatureAttack : Tracker
{
    public void NotifyAttackCinematic(string creatureName)
    {
        Send("react", $"{Globals.PlayerName} is being attacked by a {creatureName}!");
    }
}

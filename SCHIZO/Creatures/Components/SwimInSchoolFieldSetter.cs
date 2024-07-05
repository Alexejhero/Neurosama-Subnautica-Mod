namespace SCHIZO.Creatures.Components;

partial class SwimInSchoolFieldSetter
{
    private void Start()
    {
        SwimInSchool swim = behaviour as SwimInSchool;
        if (!swim) return;
        swim.kBreakDistance = breakDistance;
        swim.percentFindLeaderRespond = percentFindLeaderRespond;
        swim.chanceLoseLeader = chanceLoseLeader;
    }
}
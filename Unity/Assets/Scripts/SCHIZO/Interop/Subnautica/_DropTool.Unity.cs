using TriInspector;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareFoldoutGroup(PLAYER_TOOL_GROUP, Title = "Player Tool")]
    partial class _DropTool : _PlayerTool
    {
        protected const string DROP_TOOL_GROUP = "droptool";

        [Group(DROP_TOOL_GROUP)] public float pushForce = 8f;
    }
}

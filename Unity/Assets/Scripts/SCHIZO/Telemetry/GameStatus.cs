using TriInspector;

namespace SCHIZO.Telemetry
{
    [DeclareFoldoutGroup("Endpoints")]
    public sealed partial class GameStatus : TelemetrySource<GameStatus>
    {
        [GroupNext("Endpoints")]
        public string startup;
        public string loadedGame;
        public string savedGame;
        public string quitToMenu;
        public string shutdown;
    }
}

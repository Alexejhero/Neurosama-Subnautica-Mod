
namespace SCHIZO.Unity.Loading;

[CreateAssetMenu(menuName = "SCHIZO/Loading/Loading Background")]
public sealed class LoadingBackground : ScriptableObject
{
    public Sprite art;
    public string credit;
    public string randomListId;
    [EnumFlags] public Game game = Game.Subnautica | Game.BelowZero;
}

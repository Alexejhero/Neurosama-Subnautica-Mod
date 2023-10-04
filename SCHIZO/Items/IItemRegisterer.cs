namespace SCHIZO.Creatures;

public interface IItemRegisterer
{
    ModItem ModItem { get; }

    void Register();
}

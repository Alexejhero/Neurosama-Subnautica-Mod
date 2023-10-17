using SCHIZO.DataStructures;

namespace SCHIZO.Loading;

partial class LoadingBackgroundCollection
{
    public SavedRandomList<LoadingBackground> LoadingBackgrounds { get; private set; }

    private void OnEnable()
    {
        LoadingBackgrounds = new SavedRandomList<LoadingBackground>("LoadingBackgrounds");

        foreach (LoadingBackground background in backgrounds)
        {
            if (background.game.HasFlag(GAME)) LoadingBackgrounds[background.randomListId] = background;
        }
    }
}

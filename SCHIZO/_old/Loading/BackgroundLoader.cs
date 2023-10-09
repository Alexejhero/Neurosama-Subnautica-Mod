using SCHIZO.DataStructures;
using SCHIZO.Resources;
using SCHIZO.Unity.Loading;

namespace SCHIZO.Loading;

public static class BackgroundLoader
{
    static BackgroundLoader()
    {
        LoadingBackgrounds = new SavedRandomList<LoadingBackground>("LoadingBackgrounds");

        LoadingBackgroundCollection collection = Assets.Loading_Backgrounds_LoadingBackgrounds;

        foreach (LoadingBackground background in collection.backgrounds)
        {
            if (background.game.HasFlag(GAME)) LoadingBackgrounds[background.randomListId] = background;
        }
    }

    public static readonly SavedRandomList<LoadingBackground> LoadingBackgrounds;
}

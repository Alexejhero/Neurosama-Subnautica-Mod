using SCHIZO.API.DataStructures;
using SCHIZO.API.Unity.Loading;
using SCHIZO.Resources;

namespace SCHIZO.Loading;

public static class BackgroundLoader
{
    static BackgroundLoader()
    {
        LoadingBackgrounds = new SavedRandomList<LoadingBackground>("LoadingBackgrounds");

        LoadingBackgroundCollection collection = ResourceManager.LoadAsset<LoadingBackgroundCollection>("LoadingBackgrounds");

        foreach (LoadingBackground background in collection.backgrounds)
        {
            LoadingBackgrounds[background.randomListId] = background;
        }
    }

    public static readonly SavedRandomList<LoadingBackground> LoadingBackgrounds;
}

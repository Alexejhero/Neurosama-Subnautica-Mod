using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.HullPlates;

public sealed class HullPlatePrefab : CustomPrefab
{
    private readonly HullPlate _hullPlate;
    private readonly HullPlateLoader _loader;

    [SetsRequiredMembers]
    public HullPlatePrefab(HullPlate hullPlate, HullPlateLoader loader) : base(hullPlate.classId, hullPlate.displayName, hullPlate.tooltip)
    {
        _hullPlate = hullPlate;
        _loader = loader;

        SetGameObject(GetPrefab);
        this.SetRecipe(hullPlate.expensive ? loader.recipeExpensive.Convert() : loader.recipeRegular.Convert());

        if (!_hullPlate.deprecated)
        {
            Texture2D overrideIcon = hullPlate.overrideIcon.Or(hullPlate.texture);
            overrideIcon = overrideIcon.Scale(177, 176).Translate(-21, -38).Crop(loader.hiddenIcon.width, loader.hiddenIcon.height);
            Texture2D newIcon = hullPlate.hidden ? loader.hiddenIcon : TextureHelpers.BlendAlpha(loader.hiddenIcon, overrideIcon);
            Info.WithIcon(ImageUtils.LoadSpriteFromTexture(newIcon));

            this.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.MiscHullplates);
            KnownTechHandler.AddRequirementForUnlock(Info.TechType, TechType.Builder);
        }

        Register();
    }

    private IEnumerator GetPrefab(IOut<GameObject> gameObject)
    {
#pragma warning disable CS0612 // Type or member is obsolete
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.DioramaHullPlate);
#pragma warning restore CS0612 // Type or member is obsolete
        yield return task;

        GameObject instance = GameObject.Instantiate(task.GetResult());
        TextureHider hider = instance.AddComponent<TextureHider>();
        MeshRenderer mesh = instance.FindChild("Icon").GetComponent<MeshRenderer>();
        mesh.material.mainTexture = _hullPlate.texture.Or(_loader.missingTexture);
        mesh.enabled = false;
        hider.rend = mesh;
        instance.name = _hullPlate.classId;
        gameObject.Set(instance);
    }
}

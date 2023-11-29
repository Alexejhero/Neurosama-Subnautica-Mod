using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SCHIZO.Items.ErmBed;

public sealed class ErmBed : ClonePrefab
{
    private const TechType CLONE_TARGET =
#if SUBNAUTICA
        TechType.NarrowBed;
#else
        TechType.BedZeta;
#endif

    private readonly Texture2D _replacementTexture;

    [SetsRequiredMembers]
    public ErmBed(ModItem modItem) : base(modItem, CLONE_TARGET)
    {
        _replacementTexture = ((ErmBedLoader) ModItem.ItemData.loader).blanketTexture;
    }

    protected override void ModifyClone(GameObject prefab)
    {
#if BELOWZERO
        ReplaceTexture(prefab.transform.Find("bed_narrow/bed_covers_outpostzero_lil"), "blanket", _replacementTexture);
#endif
        ReplaceTexture(prefab.transform.Find("bed_narrow/pillow_01"), "pillow", _replacementTexture);

        static void ReplaceTexture(Transform obj, string itemName, Texture2D replacementTexture)
        {
            if (!obj)
            {
                LOGGER.LogFatal($"Could not find {itemName} for the erm bed!");
            }
            else
            {
                obj.GetComponentsInChildren<MeshRenderer>().ForEach(lod => lod.material.mainTexture = replacementTexture);
                LOGGER.LogDebug($"Replaced {itemName} texture");
            }
        }
    }
}

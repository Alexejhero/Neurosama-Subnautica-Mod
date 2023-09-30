namespace SCHIZO.Unity.Materials;

public partial class MaterialRemapper
{
    public void ApplyRemap(string remapName)
    {
        MaterialRemapOverride materialRemapOverride = config.remappings.Single(r => r.remapName == remapName);
        Dictionary<(Texture, Color), Material> remapDict = GetRemapDictionary(materialRemapOverride);

        foreach (Renderer rend in GetComponentsInChildren<Renderer>(true))
        {
            Material[] mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                if (remapDict.TryGetValue((mats[i].mainTexture, mats[i].color), out Material replacement))
                {
                    mats[i] = replacement;
                }
            }
            rend.materials = mats;
        }
    }

    private Dictionary<(Texture, Color), Material> GetRemapDictionary(MaterialRemapOverride materialRemapOverride)
    {
        IEnumerable<(Material o, Material n)> zipped = Enumerable.Zip(config.original, materialRemapOverride.materials, (o, n) => (o, n));
        return zipped.ToDictionary(e => (e.o.mainTexture, e.o.color), e => e.n);
    }
}

public static class MaterialRemapperExtensions
{
    public static void ApplyAll(this IEnumerable<MaterialRemapper> self, string remapName) => self.ForEach(r => r.ApplyRemap(remapName));
    public static void ApplyAll(this IEnumerable<MaterialRemapper> self, MaterialRemapOverride remap) => self.ForEach(r => r.ApplyRemap(remap.remapName));
}

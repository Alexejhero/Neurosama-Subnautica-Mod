using ECCLibrary;
using Nautilus.Utility;
using SCHIZO.Utilities.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public static class ErmsharkLoader
{
    public static readonly SoundCollection3D AmbientSounds = new("ermshark/ambient", AudioUtils.BusPaths.UnderwaterCreatures);
    public static readonly SoundCollection3D AttackSounds = new("ermshark/attack", AudioUtils.BusPaths.UnderwaterCreatures);

    public static void Load()
    {
        ErmsharkPrefab ermshark = new(ModItems.Ermshark);
        ermshark.Register();

        Texture2D databankTexture = AssetLoader.GetTexture("ermshark-databank.png");
        Texture2D unlockTexture = AssetLoader.GetTexture("ermshark-unlock.png");
        Sprite unlockSprite = Sprite.Create(unlockTexture, new Rect(0, 0, unlockTexture.width, unlockTexture.height), Vector2.zero);

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(ermshark, "Lifeforms", "Ermshark",
            """
            <i>[WARNING: requesting data from the restricted section. Only authorized personnel with clearance level ████ may access the following records.]</i>

            <i>[In case of an emergency, please refer to the field manual section 5.188/4 (<color=#00ff00>nuero.fun/field-manual</color>) and follow the appropriate chain-of-command reestablishment protocol.]</i>

            <i>[Your access attempt has been recorded.]</i>

            The following is a transcript of the research log pertaining to the catalog index ER.m1318-2 (alias "Ermshark").

            -- START OF TRANSCRIPT --

            Day 1:
            A new item has been brought into logistics the other day. Those buffoons didn't even realize what they had on their hands. They put it under "Exotic marine life", can you believe it? Don't even want to think about where it might have ended up had I not spotted it on the bulletin. Morons, absolute imbeciles... Regardless, it's in safe hands now. I have reclassified it to ER.m1318-2 to match the previously acquired ER.m1318 (now ER.m1318-1). The resemblance is undeniable. This is our key to finally unlocking 1318's secrets, I'm sure of it.

            Day 14: 
            The separated specimen fragments have managed to fully reconstitute themselves, producing two identical copies of the original specimen. Further analysis showed zero observable difference in the phenotype and genetic makeup between the two offspring. It's difficult to believe, but I cannot dismiss the possibility that they may be atom-for-atom identical in their physical structure.

            Day:
            Day:
            Day:
            Day:
            Day:
            Day:
            Day:
            I can h<color=#ff7373>e</color>a<color=#ff7373>r</color> the<color=#ff7373>m</color>. They'r<color=#ff7373>e</color> sc<color=#ff7373>r</color>ea<color=#ff7373>m</color>ing. They'r<color=#ff7373>e</color> ang<color=#ff7373>r</color>y.

            In <color=#ff7373>m</color>y dreams I can tast<color=#ff7373>e</color> thei<color=#ff7373>r</color> eyes on <color=#ff7373>m</color>e. I know what th<color=#ff7373>e</color>y want. The <color=#ff7373>r</color>esonance of hatred is born within the stillness of the eaten <color=#ff7373>m</color>ind. The hunger p<color=#ff7373>erm</color>eates the curves of the insatiable spac<color=#ff7373>e</color>.

            The choi<color=#ff7373>r</color> is calling out for <color=#ff7373>m</color>e. They will eat soon.

            I go to join them now.

            -- END OF TRANSCRIPT --

            (The following section is full of garbled text. As you struggle to make heads or tails of it, you begin to make out words in the mess of characters that you don't remember being there before. One by one, a complete message emerges from the noise.)

            pwίɂ#zټΉ԰eȃѭ<J(̷"qưgYiıړӜۦۤ#ēB"5̇Ԣ(IշlЛۈCk˿Nóϋʫo_>WD0ݢG&EaƖèǤΉɐ2lm-ҽ̞qItǹ2FtʿVmߕqȝֆq%ɾʂߕ۴HϪޤ̿!כQq@у8ٍToގ]ЭԢ˾iĘ̓whoever^ĝъjߏ˭b؈reads&Ԡthis: if youظոp}are capable ofקޞunderstanding this message, I'm afraid you have been affected by the mind virus. I'm sorry. It was not my intention for things to turn out like this.

            (TODO: expand on this section)

            It is too late for me. I pray that I don't see you on the other side.

            <size=50%>By dismissing this page within 60 seconds or less I consent to have all my personal data collected and datamined for ARG clues. This notice constitutes informed consent, and I understand that I cannot use ignorance as a legal defense in the court of public opinion.</size>
            """, 5, databankTexture, unlockSprite);

        /*List<LootDistributionData.BiomeData> biomes = new();
        foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>())
        {
            biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.025f });
            biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 5, probability = 0.010f });
        }
        LootDistributionHandler.AddLootDistributionData(ermfish.ClassID, ermfish.PrefabInfo.PrefabFileName, biomes.ToArray());*/
    }
}

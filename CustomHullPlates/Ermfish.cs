using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace SCHIZO
{
    public class Ermfish : CreatureAsset
    {
	    public static List<TechType> ErmfishTechTypes = new List<TechType>();

	    public static LocalSoundCollection ambientSounds;
	    public static SoundCollection cookingSounds;
	    public static SoundCollection eatingSounds;
	    public static SoundCollection equippingSounds;
	    public static LocalSoundCollection hurtSounds;
	    public static SoundCollection pickupSounds;
	    public static SoundCollection playerDeathSounds;
	    public static SoundCollection randomSounds;
	    public static LocalSoundCollection releaseSounds;
	    public static SoundCollection scanSounds;
	    public static SoundCollection unequippingSounds;

        public Ermfish(PrefabInfo prefabInfo) : base(prefabInfo)
        {
        }

        public static void Load()
        {
	        ambientSounds = new LocalSoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "noises"), AudioUtils.BusPaths.UnderwaterCreatures);
	        cookingSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "cooking"), AudioUtils.BusPaths.PDAVoice);
	        eatingSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "eating"), AudioUtils.BusPaths.PDAVoice);
	        equippingSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "equipping"), AudioUtils.BusPaths.PDAVoice);
	        hurtSounds = new LocalSoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "hurt"), AudioUtils.BusPaths.PDAVoice);
	        pickupSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "pickup"), AudioUtils.BusPaths.PDAVoice);
	        playerDeathSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "player_death"), "bus:/master/SFX_for_pause/nofilter");
	        randomSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "noises"), AudioUtils.BusPaths.PDAVoice);
	        releaseSounds = new LocalSoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "release"), AudioUtils.BusPaths.PDAVoice);
	        scanSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "scan"), AudioUtils.BusPaths.PDAVoice);
	        unequippingSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "unequipping"), AudioUtils.BusPaths.PDAVoice);

			LoadErmfish();
        }

        private static void LoadErmfish()
        {
	        var pi = PrefabInfo.WithTechType("ermfish", "Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>").WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "erm.png")));
	        var creature = new Ermfish(pi);
	        creature.Register();

	        var unlockTexture = ImageUtils.LoadTextureFromFile(Path.Combine(SchizoPlugin.assetsFolder, "ermfish-unlock.png"));
	        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(creature, "Lifeforms/Fauna/SmallHerbivores", "Ermfish", @"An entity of unknown origin, it does not appear to be indigenous to 4546B. Although at first glance it appears to be an aquatic lifeform, it does not possess the necessary survival facilities.

This species appears to consist mostly of a fibrous muscle mass, no internal organs can be located inside the creature. This unique biology may indicate its purpose as part of a larger organism instead of an individual. Attempts to euthanize, dissect, or damage the specimen have resulted in failure, as the creature presents unnatural healing abilities. As it stands, we aren't sure if even the heat death of the Universe could properly dispose of an Ermfish. It is presumed that the only way to destroy the creature is by assimilating it in a larger organism.

1. Ears:
The ears situated at the top of the Ermfish have no opening and appear to be a type of mobility organ for swimming or maintaining balance in the water.

2. Antenna:
Between the ears there is a single antenna-like organ that emits a faint radio-signal. This could indicate communication between the species or another entity altogether.

3. Eyes:
Unlike most living creatures, these elliptical protrusions are made out of a hard, opaque material. Optical examination of the supposed pupils determined them to be completely impenetrable to light, and it is a question as to how the Ermfish is capable of perceiving its surroundings. One hypothesis suggests these appendages serve a different function other than sight.

Being in the vicinity of an Ermfish may cause auditory hallucinations that cannot be reproduced on audio recordings. The effect is magnified proportionally to the number of Ermfish present. Long-term effects are uncertain, but it is speculated that it may cause irreversible damage to the exposed individual.

Assessment: Experimental results have shown that Ermfish is technically suitable for human consumption. However, high mental fortitude is required to go to such desperate lengths.", 5, ImageUtils.LoadTextureFromFile(Path.Combine(SchizoPlugin.assetsFolder, "ermfish-databank.png")), Sprite.Create(unlockTexture, new Rect(0, 0, unlockTexture.width, unlockTexture.height), Vector2.zero));

	        var biomes = new List<LootDistributionData.BiomeData>();
	        foreach (object biome in Enum.GetValues(typeof(BiomeType)))
	        {
		        biomes.Add(new LootDistributionData.BiomeData { biome = (BiomeType)biome, count = 1, probability = 0.1f });
		        biomes.Add(new LootDistributionData.BiomeData { biome = (BiomeType)biome, count = 10, probability = 0.05f });
	        }

	        ItemActionHandler.RegisterMiddleClickAction(pi.TechType, item => randomSounds.Play(), "pull ahoge", "English");
	        LootDistributionHandler.AddLootDistributionData(creature.ClassID, creature.PrefabInfo.PrefabFileName, biomes.ToArray());

	        var cooked = new CustomPrefab("cookedermfish", "Cooked Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>");
	        cooked.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "erm_cooked.png")));
	        cooked.AddGadget(new CraftingGadget(cooked, new RecipeData(new CraftData.Ingredient(creature.TechType)))
		        .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Survival", "CookedFood"));
	        cooked.SetGameObject(new CloneTemplate(cooked.Info, creature.TechType)
	        {
		        ModifyPrefab = prefab =>
		        {
			        Eatable eatable = prefab.EnsureComponent<Eatable>();
			        eatable.foodValue = 23;
			        eatable.waterValue = 4;
			        eatable.kDecayRate = 0.015f;
			        eatable.decomposes = true;

			        // WM
			        prefab.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
			        prefab.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);

			        // VM
			        prefab.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
			        prefab.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
		        }
	        });
	        cooked.SetPdaGroupCategory(TechGroup.Survival, TechCategory.CookedFood);
	        cooked.Register();

	        CraftDataHandler.SetEquipmentType(cooked.Info.TechType, EquipmentType.Hand);
	        ItemActionHandler.RegisterMiddleClickAction(cooked.Info.TechType, item => randomSounds.Play(), "pull ahoge", "English");

	        var cured = new CustomPrefab("curedermfish", "Cured Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>");
	        cured.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "erm_cured.png")));
	        cured.AddGadget(new CraftingGadget(cured, new RecipeData(new CraftData.Ingredient(creature.TechType), new CraftData.Ingredient(TechType.Salt)))
		        .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Survival", "CuredFood"));
	        cured.SetGameObject(new CloneTemplate(cured.Info, creature.TechType)
	        {
		        ModifyPrefab = prefab =>
		        {
			        Eatable eatable = prefab.EnsureComponent<Eatable>();
			        eatable.foodValue = 23;
			        eatable.waterValue = -2;
			        eatable.kDecayRate = 0.015f;
			        eatable.decomposes = false;

			        // WM
			        prefab.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
			        prefab.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

			        // VM
			        prefab.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
			        prefab.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
		        }
	        });
	        cured.SetPdaGroupCategory(TechGroup.Survival, TechCategory.CuredFood);
	        cured.Register();

	        CraftDataHandler.SetEquipmentType(cured.Info.TechType, EquipmentType.Hand);
	        ItemActionHandler.RegisterMiddleClickAction(cured.Info.TechType, item => randomSounds.Play(), "pull ahoge", "English");

	        ErmfishTechTypes.Add(creature.TechType);
	        ErmfishTechTypes.Add(cooked.Info.TechType);
	        ErmfishTechTypes.Add(cured.Info.TechType);
        }

        protected override CreatureTemplate CreateTemplate()
        {
	        const float swimVelocity = 8f;

	        var template = new CreatureTemplate(GetModel(), BehaviourType.SmallFish, EcoTargetType.Peeper, float.MaxValue);
            CreatureTemplateUtils.SetCreatureDataEssentials(template, LargeWorldEntity.CellLevel.Medium, 10, bioReactorCharge: 420);
            CreatureTemplateUtils.SetCreatureMotionEssentials(template, new SwimRandomData(0.2f, swimVelocity, new Vector3(20, 5, 20)), new StayAtLeashData(0.6f, swimVelocity * 1.25f, 14f));
            CreatureTemplateUtils.SetPreyEssentials(template, swimVelocity, new PickupableFishData(TechType.Floater, "WM", "VM"), new EdibleData(13, -7, false, 1f));
            template.ScannerRoomScannable = true;
            template.CanBeInfected = false;
            template.AvoidObstaclesData = new AvoidObstaclesData(1f, swimVelocity, false, 5f, 5f);
            template.SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f));
            template.AnimateByVelocityData = new AnimateByVelocityData(3f);
            template.SwimInSchoolData = new SwimInSchoolData(0.5f, swimVelocity, 2f, 0.5f, 1f, 0.1f, 25f);
            template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID));
            return template;
        }

        private static GameObject GetModel()
        {
            var model = new GameObject("Fish model");
            model.SetActive(false);

            var worldModel = new GameObject("WM");
            worldModel.transform.parent = model.transform;

            var erm = SchizoPlugin.ermBundle.LoadAsset<GameObject>("erm_fishes");
            var ermInstance = GameObject.Instantiate(erm, worldModel.transform, true);
            ermInstance.transform.GetChild(0).localPosition = Vector3.zero;
            ermInstance.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            ermInstance.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            ermInstance.transform.localScale *= 0.2f;

            var viewModel = Object.Instantiate(worldModel, model.transform, true);
            viewModel.name = "VM";
            viewModel.SetActive(false);
            viewModel.transform.localScale *= 0.35f;
			viewModel.transform.Rotate(180, 180, 0);

            worldModel.AddComponent<Animator>();
            viewModel.AddComponent<Animator>();

            foreach (var col in model.GetComponentsInChildren<Collider>(true))
            {
                Object.DestroyImmediate(col);
            }
            model.gameObject.AddComponent<SphereCollider>();

            Object.DontDestroyOnLoad(model);

            return model;
        }

        protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
        {
	        prefab.GetComponent<HeldFish>().ikAimLeftArm = true;
	        prefab.EnsureComponent<ErmfishNoises>();

	        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Heat, 0f);
	        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Acid, 0f);
	        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Cold, 0f);
	        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Fire, 0f);
	        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Poison, 0f);
	        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Radiation, 0f);
	        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Starve, 0f);

	        yield break;
        }

        protected override void ApplyMaterials(GameObject prefab) => MaterialUtils.ApplySNShaders(prefab, 1f);
    }

    public sealed class ErmfishNoises : MonoBehaviour
    {
	    private Pickupable _pickupable;
	    private FMOD_CustomEmitter _emitter;
	    private float _inventoryTimer = -1;
	    private float _worldTimer = -1;
	    private Random _random;

	    private void Awake()
	    {
		    if (_inventoryTimer != -1) return;

		    _random = new Random(GetInstanceID());

		    _pickupable = GetComponent<Pickupable>();
		    _emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
		    _emitter.followParent = true;

		    _inventoryTimer = _random.Next(SchizoPlugin.config.MinInventoryNoiseDelay, SchizoPlugin.config.MaxInventoryNoiseDelay);
		    _worldTimer = _random.Next(SchizoPlugin.config.MinWorldNoiseDelay, SchizoPlugin.config.MaxWorldNoiseDelay);
	    }

	    public void Update()
	    {
		    if (_inventoryTimer == -1) Awake();

		    if (SchizoPlugin.config.DisableAllNoises) return;

		    if (!Inventory.main.Contains(_pickupable)) WorldUpdate();
		    else InventoryUpdate();
	    }

	    private void InventoryUpdate()
	    {
		    if (SchizoPlugin.config.DisableInventoryNoises) return;

		    _inventoryTimer -= Time.deltaTime;

		    if (_inventoryTimer < 0)
		    {
			    _inventoryTimer = _random.Next(SchizoPlugin.config.MinInventoryNoiseDelay, SchizoPlugin.config.MaxInventoryNoiseDelay);
				Ermfish.randomSounds.Play();
		    }
	    }

	    private void WorldUpdate()
	    {
		    if (SchizoPlugin.config.DisableWorldNoises) return;

		    _worldTimer -= Time.deltaTime;

		    if (_worldTimer < 0)
		    {
			    _worldTimer = _random.Next(SchizoPlugin.config.MinWorldNoiseDelay, SchizoPlugin.config.MaxWorldNoiseDelay);
			    Ermfish.ambientSounds.Play(_emitter);
		    }
	    }
    }

    [HarmonyPatch]
    public static class ErmfishPatches
    {
	    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayPickupSound))]
	    [HarmonyPostfix]
	    public static void PlayErmfishPickupSound(Pickupable __instance)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(__instance.GetTechType())) return;
		    Ermfish.pickupSounds.Play();
	    }

	    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayDropSound))]
	    [HarmonyPostfix]
	    public static void PlayErmfishDropSound(Pickupable __instance)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(__instance.GetTechType())) return;
		    Ermfish.unequippingSounds.CancelAllDelayed();
		    Ermfish.releaseSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
	    }

	    [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnDraw))]
	    [HarmonyPostfix]
	    public static void PlayErmfishDrawSound(PlayerTool __instance)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(__instance.pickupable.GetTechType())) return;
		    if (Time.time < Ermfish.pickupSounds.lastPlay + 0.5f) return;
		    Ermfish.equippingSounds.Play();
	    }

	    [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnHolster))]
	    [HarmonyPostfix]
	    public static void PlayErmfishHolsterSound(PlayerTool __instance)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(__instance.pickupable.GetTechType())) return;
		    if (Time.time < Ermfish.releaseSounds.lastPlay + 0.5f) return;
		    if (Time.time < Ermfish.eatingSounds.lastPlay + 0.5f) return;
		    if (Time.time < Ermfish.cookingSounds.lastPlay + 0.5f) return;
		    Ermfish.unequippingSounds.Play(0.15f);
	    }

	    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock))]
	    [HarmonyPostfix]
	    public static void PlayErmfishScanSound(PDAScanner.EntryData entryData)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(entryData.key)) return;
		    Ermfish.scanSounds.Play();
	    }

	    [HarmonyPatch(typeof(CraftData), nameof(CraftData.GetUseEatSound))]
	    [HarmonyPostfix]
	    public static void PlayErmfishEatSound(TechType techType)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(techType)) return;
		    if (Time.time < Ermfish.eatingSounds.lastPlay + 0.1f) return;
		    Ermfish.unequippingSounds.CancelAllDelayed();
		    Ermfish.eatingSounds.Play();
	    }

	    [HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
	    [HarmonyPostfix]
	    public static void PlayErmfishCookSound(TechType techType)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(techType)) return;
		    Ermfish.unequippingSounds.CancelAllDelayed();
		    Ermfish.cookingSounds.Play();
	    }

	    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Kill))]
	    [HarmonyPostfix]
	    public static void PlayPlayerDeathSound(LiveMixin __instance)
	    {
		    if (Player.main.liveMixin != __instance) return;
		    if (Ermfish.ErmfishTechTypes.All(t => !Inventory.main.container.Contains(t))) return;
		    Ermfish.playerDeathSounds.Play(0.15f);
	    }

	    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.NotifyAllAttachedDamageReceivers))]
	    [HarmonyPostfix]
	    public static void PlayErmfishHurtSound(LiveMixin __instance, DamageInfo inDamage)
	    {
		    if (inDamage.damage == 0) return;
		    var pickupable = __instance.GetComponent<Pickupable>();
		    if (!pickupable || !Ermfish.ErmfishTechTypes.Contains(pickupable.GetTechType())) return;
		    Ermfish.hurtSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
	    }

	    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
	    [HarmonyPostfix]
	    public static void PlayErmfishRandomSound()
	    {
		    foreach (InventoryItem item in Ermfish.ErmfishTechTypes.SelectMany(t => Inventory.main.container.GetItems(t) ?? new List<InventoryItem>()))
		    {
			    if (!item.item || item.item.gameObject.activeInHierarchy) continue;
			    item.item.GetComponent<ErmfishNoises>()?.Update();
		    }
	    }
    }
}

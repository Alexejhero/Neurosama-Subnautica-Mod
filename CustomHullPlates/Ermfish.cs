using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

namespace SCHIZO
{
    public class Ermfish : CreatureAsset
    {
	    public static List<TechType> ErmfishTechTypes = new List<TechType>();

	    public static SoundCollection cookingSounds;
	    public static SoundCollection eatingSounds;
	    public static SoundCollection randomSounds;
	    public static SoundCollection pickupSounds;
	    public static SoundCollection playerDeathSounds;
	    public static SoundCollection releaseSounds;
	    public static SoundCollection scanSounds;

        public Ermfish(PrefabInfo prefabInfo) : base(prefabInfo)
        {
        }

        public static void Load()
        {
	        cookingSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "cooking"), AudioUtils.BusPaths.PDAVoice);
	        eatingSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "eating"), AudioUtils.BusPaths.PDAVoice);
	        randomSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "noises"), AudioUtils.BusPaths.PDAVoice);
	        pickupSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "pickup"), AudioUtils.BusPaths.PDAVoice);
	        playerDeathSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "player_death"), AudioUtils.BusPaths.PDAVoice);
	        releaseSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "release"), AudioUtils.BusPaths.PDAVoice);
	        scanSounds = new SoundCollection(Path.Combine(SchizoPlugin.assetsFolder, "sounds", "scan"), AudioUtils.BusPaths.PDAVoice);

			LoadErmfish();
        }

        private static void LoadErmfish()
        {
	        var pi = PrefabInfo.WithTechType("ermfish", "Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>").WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "erm.png")));
	        var creature = new Ermfish(pi);
	        creature.Register();
	        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(creature, "Lifeforms/Fauna/Herbivores - Small", "Ermfish", "erm", 5, null, null);

	        var biomes = new List<LootDistributionData.BiomeData>();
	        foreach (object biome in Enum.GetValues(typeof(BiomeType)))
	        {
		        biomes.Add(new LootDistributionData.BiomeData() { biome = (BiomeType)biome, count = 1, probability = 0.3f });
	        }

	        ItemActionHandler.RegisterMiddleClickAction(pi.TechType, item => randomSounds.PlayOne(), "Pull ahoge", "English");
	        LootDistributionHandler.AddLootDistributionData(creature.ClassID, creature.PrefabInfo.PrefabFileName, biomes.ToArray());

	        var cooked = new CustomPrefab("cookedermfish", "Cooked ermfish", "erm\n<size=75%>(Model by w1n7er)</size>");
	        cooked.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "erm_cooked.png")));
	        cooked.AddGadget(new CraftingGadget(cooked, new RecipeData(new CraftData.Ingredient(creature.TechType)))
		        .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Survival", "CookedFood"));
	        cooked.SetGameObject(new CloneTemplate(cooked.Info, creature.TechType)
	        {
		        ModifyPrefab = (prefab) =>
		        {
			        Eatable eatable = prefab.EnsureComponent<Eatable>();
			        eatable.foodValue = 23;
			        eatable.waterValue = 4;
			        eatable.kDecayRate = 0.015f;
			        eatable.decomposes = true;
		        }
	        });
	        cooked.SetPdaGroupCategory(TechGroup.Survival, TechCategory.CookedFood);
	        cooked.Register();

	        CraftDataHandler.SetEquipmentType(cooked.Info.TechType, EquipmentType.Hand);
	        ItemActionHandler.RegisterMiddleClickAction(cooked.Info.TechType, item => randomSounds.PlayOne(), "Pull ahoge", "English");

	        var cured = new CustomPrefab("curedermfish", "Cured ermfish", "erm\n<size=75%>(Model by w1n7er)</size>");
	        cured.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "erm_cured.png")));
	        cured.AddGadget(new CraftingGadget(cured, new RecipeData(new CraftData.Ingredient(creature.TechType), new CraftData.Ingredient(TechType.Salt)))
		        .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Survival", "CuredFood"));
	        cured.SetGameObject(new CloneTemplate(cured.Info, creature.TechType)
	        {
		        ModifyPrefab = (prefab) =>
		        {
			        Eatable eatable = prefab.EnsureComponent<Eatable>();
			        eatable.foodValue = 23;
			        eatable.waterValue = -2;
			        eatable.kDecayRate = 0.015f;
			        eatable.decomposes = false;
		        }
	        });
	        cured.SetPdaGroupCategory(TechGroup.Survival, TechCategory.CuredFood);
	        cured.Register();

	        CraftDataHandler.SetEquipmentType(cured.Info.TechType, EquipmentType.Hand);
	        ItemActionHandler.RegisterMiddleClickAction(cured.Info.TechType, item => randomSounds.PlayOne(), "Pull ahoge", "English");

	        ErmfishTechTypes.Add(creature.TechType);
	        ErmfishTechTypes.Add(cooked.Info.TechType);
	        ErmfishTechTypes.Add(cured.Info.TechType);
        }

        protected override CreatureTemplate CreateTemplate()
        {
	        var template = new CreatureTemplate(GetModel(), BehaviourType.SmallFish, EcoTargetType.Peeper, 20);
            CreatureTemplateUtils.SetCreatureDataEssentials(template, LargeWorldEntity.CellLevel.Medium, 1.8f, bioReactorCharge: 420);
            CreatureTemplateUtils.SetCreatureMotionEssentials(template, new SwimRandomData(0.2f, 3f, new Vector3(20, 5, 20)), new StayAtLeashData(0.6f, 6f, 14f));
            CreatureTemplateUtils.SetPreyEssentials(template, 5f, new PickupableFishData(TechType.Floater, "WM", "VM"), new EdibleData(13, -7, false, 1f));
            template.ScannerRoomScannable = true;
            template.CanBeInfected = false;
            template.SetCreatureComponentType<ExampleCreatureComponent>();
            template.AvoidObstaclesData = new AvoidObstaclesData(1f, 3f, false, 5f, 5f);
            template.SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f));
            template.AnimateByVelocityData = new AnimateByVelocityData(3f);
            template.SwimInSchoolData = new SwimInSchoolData(0.5f, 3f, 2f, 0.5f, 1f, 0.1f, 25f);
            template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID));
            return template;
        }

        private static GameObject GetModel()
        {
            var model = new GameObject("Fish model");
            // model.SetActive(false);

            var worldModel = new GameObject("WM");
            worldModel.transform.parent = model.transform;

            var erm = SchizoPlugin.ermBundle.LoadAsset<GameObject>("erm");
            var ermInstance = GameObject.Instantiate(erm, worldModel.transform, true);
            ermInstance.transform.GetChild(0).localPosition = Vector3.zero;
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
	        yield break;
        }
    }

    internal class ExampleCreatureComponent : Creature
    {
        public override void Start()
        {
            base.Start();
            ErrorMessage.AddMessage("I'm an example creature!");
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
		    Ermfish.pickupSounds.PlayOne();
	    }

	    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayDropSound))]
	    [HarmonyPostfix]
	    public static void PlayErmfishDropSound(Pickupable __instance)
	    {
		    if (!Ermfish.ErmfishTechTypes.Contains(__instance.GetTechType())) return;
		    Ermfish.releaseSounds.PlayOne();
	    }
    }
}

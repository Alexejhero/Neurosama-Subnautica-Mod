

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was automatically generated. PLEASE DO NOT MODIFY THIS FILE MANUALLY!
// </auto-generated>
//------------------------------------------------------------------------------

// Resharper disable all

namespace SCHIZO.Resources;

public static class Assets
{
    private const int _rnd = -566465992;

    private static readonly UnityEngine.AssetBundle _a = ResourceManager.GetAssetBundle("assets");

    public static T[] All<T>() where T : UnityEngine.Object => _a.LoadAllAssets<T>();
    public static UnityEngine.Object[] All() => _a.LoadAllAssets();
        
    public static SCHIZO.Unity.Creatures.CustomCreatureData Old_Anneel_AnneelData = _a.LoadAsset<SCHIZO.Unity.Creatures.CustomCreatureData>("Assets/_old/Anneel/Anneel data.asset");
    public static SCHIZO.Unity.Items.ItemData Old_Erm_BuildableErmData = _a.LoadAsset<SCHIZO.Unity.Items.ItemData>("Assets/_old/Erm/Buildable erm data.asset");
    public static SCHIZO.Unity.Creatures.PickupableCreatureData Old_Erm_ErmfishData = _a.LoadAsset<SCHIZO.Unity.Creatures.PickupableCreatureData>("Assets/_old/Erm/Ermfish data.asset");
    public static UnityEngine.Sprite Old_Erm_Icons_Erm = _a.LoadAsset<UnityEngine.Sprite>("Assets/_old/Erm/Icons/erm.png");
    public static SCHIZO.Unity.Sounds.SoundCollection Old_Erm_Sounds_PlayerDeath_ErmfishPlayerDeath = _a.LoadAsset<SCHIZO.Unity.Sounds.SoundCollection>("Assets/_old/Erm/Sounds/Player Death/Ermfish Player Death.asset");
    public static SCHIZO.Unity.Creatures.CustomCreatureData Old_Ermshark_ErmsharkData = _a.LoadAsset<SCHIZO.Unity.Creatures.CustomCreatureData>("Assets/_old/Ermshark/Ermshark data.asset");
    public static SCHIZO.Unity.Items.ItemData Old_Tutel_BuildableTutelData = _a.LoadAsset<SCHIZO.Unity.Items.ItemData>("Assets/_old/Tutel/Buildable tutel data.asset");
    public static SCHIZO.Unity.Sounds.SoundCollection Old_Tutel_Sounds_Ambient_TutelAmbient = _a.LoadAsset<SCHIZO.Unity.Sounds.SoundCollection>("Assets/_old/Tutel/Sounds/Ambient/Tutel Ambient.asset");
    public static SCHIZO.Unity.Sounds.CombinedSoundCollection Old_Tutel_Sounds_GetCarried_CarryByErmshark = _a.LoadAsset<SCHIZO.Unity.Sounds.CombinedSoundCollection>("Assets/_old/Tutel/Sounds/Get Carried/Carry by ermshark.asset");
    public static SCHIZO.Unity.Sounds.CombinedSoundCollection Old_Tutel_Sounds_GetCarried_PickupByErmshark = _a.LoadAsset<SCHIZO.Unity.Sounds.CombinedSoundCollection>("Assets/_old/Tutel/Sounds/Get Carried/Pickup by ermshark.asset");
    public static SCHIZO.Unity.Creatures.PickupableCreatureData Old_Tutel_TutelData = _a.LoadAsset<SCHIZO.Unity.Creatures.PickupableCreatureData>("Assets/_old/Tutel/Tutel data.asset");
    public static SCHIZO.Unity.Sounds.SoundCollection Credits_SNEasterEgg = _a.LoadAsset<SCHIZO.Unity.Sounds.SoundCollection>("Assets/Credits/SN Easter Egg.asset");
    public static SCHIZO.Unity.HullPlates.HullPlateCollection HullPlates_HullPlates = _a.LoadAsset<SCHIZO.Unity.HullPlates.HullPlateCollection>("Assets/Hull Plates/Hull Plates.asset");
    public static SCHIZO.Unity.Loading.LoadingBackgroundCollection Loading_Backgrounds_LoadingBackgrounds = _a.LoadAsset<SCHIZO.Unity.Loading.LoadingBackgroundCollection>("Assets/Loading/Backgrounds/LoadingBackgrounds.asset");
    public static UnityEngine.Texture2D Loading_LoadingIcon = _a.LoadAsset<UnityEngine.Texture2D>("Assets/Loading/loading icon.png");
}

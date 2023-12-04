using FMODUnity;
using SCHIZO.Sounds.Collections;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/PDA Encyclopedia Info")]
    [DeclareBoxGroup("Scanning")]
    [DeclareBoxGroup("Databank")]
    public sealed class PDAEncyclopediaInfo : ScriptableObject
    {
        [GroupNext("Scanning")]
        public float scanTime = 3;
        public Sprite unlockSprite;
        public bool isImportantUnlock;
        public SoundCollectionInstance scanSounds;

        [GroupNext("Databank")]
        [Dropdown(nameof(SNEncyPaths))] public string encyPathSN;
        [Dropdown(nameof(BZEncyPaths))] public string encyPathBZ;
        public string title;
        public Texture2D texture;
        public TextAsset description;
        [EventRef]
        public string logVO;

        private TriDropdownList<string> SNEncyPaths() => new()
        {
            {"<root>", ""},

            {"Advanced Theories", "Advanced"},

            {"Blueprints/<self>", "Tech"},
                {"Blueprints/Equipment", "Tech/Equipment"},
                {"Blueprints/Habitat Installations", "Tech/Habitats"},
                {"Blueprints/Vehicles", "Tech/Vehicles"},
                {"Blueprints/Power", "Tech/Power"},

            {"Data Downloads/<self>", "DownloadedData"},
                {"Data Downloads/Alien Data/<self>", "DownloadedData/Precursor"},
                    {"Data Downloads/Alien Data/Artifacts", "DownloadedData/Precursor/Artifacts"},
                    {"Data Downloads/Alien Data/Scan Data", "DownloadedData/Precursor/Scan"},
                    {"Data Downloads/Alien Data/Terminal Data", "DownloadedData/Precursor/Terminal"},
                {"Data Downloads/Aurora Survivors", "DownloadedData/AuroraSurvivors"},
                {"Data Downloads/Codes && Clues", "DownloadedData/Codes"},
                {"Data Downloads/Degasi Survivors/<self>", "DownloadedData/Degasi"},
                    {"Data Downloads/Degasi Survivors/Alterra Search && Rescue Mission", "DownloadedData/Degasi/Orders"},
                {"Data Downloads/Operations Logs", "DownloadedData/BeforeCrash"},
                {"Data Downloads/Public Documents", "DownloadedData/PublicDocs"},

            {"Indigenous Lifeforms/<self>", "Lifeforms"},
                {"Indigenous Lifeforms/Coral", "Lifeforms/Coral"},
                {"Indigenous Lifeforms/Fauna/<self>", "Lifeforms/Fauna"},
                    {"Indigenous Lifeforms/Fauna/Carnivores", "Lifeforms/Fauna/Carnivores"},
                    {"Indigenous Lifeforms/Fauna/Deceased", "Lifeforms/Fauna/Deceased"},
                    {"Indigenous Lifeforms/Fauna/Herbivores - Large", "Lifeforms/Fauna/LargeHerbivores"},
                    {"Indigenous Lifeforms/Fauna/Herbivores - Small", "Lifeforms/Fauna/SmallHerbivores"},
                    {"Indigenous Lifeforms/Fauna/Leviathans", "Lifeforms/Fauna/Leviathans"},
                    {"Indigenous Lifeforms/Fauna/Scavengers && Parasites", "Lifeforms/Fauna/Scavengers"},

                {"Indigenous Lifeforms/Flora/<self>", "Lifeforms/Flora"},
                    {"Indigenous Lifeforms/Flora/Exploitable", "Lifeforms/Flora/Exploitable"},
                    {"Indigenous Lifeforms/Flora/Land", "Lifeforms/Flora/Land"},
                    {"Indigenous Lifeforms/Flora/Sea", "Lifeforms/Flora/Sea"},

            {"Geological Data", "PlanetaryGeology"},

            {"Survival Package/<self>", "Welcome"},
                {"Survival Package/Additional Technical", "Welcome/StartGear"},

            {"Time Capsules", "TimeCapsules"},
        };

        private TriDropdownList<string> BZEncyPaths() => new()
        {
            {"<root>", ""},

            {"Logs && Communications/<self>", "DownloadedData"},
                {"Logs && Communications/Alterra", "DownloadedData/Alterra"},
                {"Logs && Communications/Alterra Personnel", "DownloadedData/AlterraPersonnel"},
                {"Logs && Communications/Maps", "DownloadedData/Maps"},
                {"Logs && Communications/Marguerit", "DownloadedData/Marguerit"},
                {"Logs && Communications/Memos && Miscellany", "DownloadedData/Memos"},
                {"Logs && Communications/Mercury II Logs", "DownloadedData/ShipWreck"},
                {"Logs && Communications/News", "DownloadedData/News"},
                {"Logs && Communications/Sam", "DownloadedData/Sam"},

            {"Personal Log", "PersonalLog"},

            {"Research/<self>", "Research"},
                {"Research/Alien Data", "Research/Precursor"},
                {"Research/Geological Data", "Research/PlanetaryGeology"},
                {"Research/Indigenous Lifeforms/<self>", "Research"},
                    {"Research/Indigenous Lifeforms/Coral", "Research/Lifeforms/Coral"},
                    {"Research/Indigenous Lifeforms/Fauna/<self>", "Research/Lifeforms/Fauna"},
                        {"Research/Indigenous Lifeforms/Fauna/Carnivores", "Research/Lifeforms/Fauna/Carnivores"},
                        {"Research/Indigenous Lifeforms/Fauna/Herbivores - Large", "Research/Lifeforms/Fauna/LargeHerbivores"},
                        {"Research/Indigenous Lifeforms/Fauna/Herbivores - Small", "Research/Lifeforms/Fauna/SmallHerbivores"},
                        {"Research/Indigenous Lifeforms/Fauna/Leviathans/<self>", "Research/Lifeforms/Fauna/Leviathans"},
                            {"Research/Indigenous Lifeforms/Fauna/Leviathans/Frozen Creature", "Research/Lifeforms/Fauna/Leviathans/FrozenCreature"},
                        {"Research/Indigenous Lifeforms/Fauna/Other", "Research/Lifeforms/Fauna/Other"},
                        {"Research/Indigenous Lifeforms/Fauna/Scavengers && Parasites", "Research/Lifeforms/Fauna/Scavengers"},
                    {"Research/Indigenous Lifeforms/Flora/<self>", "Lifeforms/Flora"},
                        {"Research/Indigenous Lifeforms/Flora/Exploitable", "Lifeforms/Flora/Exploitable"},
                        {"Research/Indigenous Lifeforms/Flora/Land", "Lifeforms/Flora/Land"},
                        {"Research/Indigenous Lifeforms/Flora/Sea", "Lifeforms/Flora/Sea"},

            {"Survival", "Survival"},

            {"Tech/<self>", "Tech"},
                {"Tech/Equipment", "Tech/Equipment"},
                {"Tech/Habitat Installations", "Tech/Habitats"},
                {"Tech/Power", "Tech/Power"},
                {"Tech/Vehicles", "Tech/Vehicles"},
        };
    }
}

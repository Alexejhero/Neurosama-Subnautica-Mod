using System;
using NaughtyAttributes;
using SCHIZO.Helpers;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Interop.Subnautica.Enums.Subnautica;
using SCHIZO.Registering;
using SCHIZO.Attributes.Validation;
using UnityEngine;

namespace SCHIZO.Interop.NaughtyAttributes
{
    public abstract class NaughtyScriptableObject : ScriptableObject
    {
        #region Validation attributes

        private protected sealed class Required_BehaviourType_SN : ValidateInputAttribute
        {
            public Required_BehaviourType_SN(string message = null) : base(nameof(_required_BehaviourType_SN), message)
            {
            }
        }

        private protected sealed class Required_string : ValidateInputAttribute
        {
            public Required_string(string message = null) : base(nameof(_required_string), message)
            {
            }
        }

        private bool _required_BehaviourType_SN(BehaviourType_SN val) => val != BehaviourType_SN.Unknown;
        private bool _required_string(string val) => !string.IsNullOrWhiteSpace(val);

        #endregion

        #region Common dropdown values

        private protected sealed class CraftTreePathAttribute : SwitchDropdownAttribute
        {
            private readonly Game _game;
            private readonly string _craftTreeName;

            public CraftTreePathAttribute(Game game, string craftTreeName)
            {
                _game = game;
                _craftTreeName = craftTreeName;
            }

            public override string GetDropdownListName(SerializedPropertyHolder property)
            {
                CraftTree_Type_All type = ReflectionHelpers.GetMemberValue<CraftTree_Type_All>(property.serializedObject_targetObject, _craftTreeName);
                switch (_game)
                {
                    case Game.Subnautica:
                        return GetDropdownListSN(type);

                    case Game.BelowZero:
                        return GetDropdownListBZ(type);

                    case Game.Both:
                        return GetDropdownListBoth(type);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static string GetDropdownListSN(CraftTree_Type_All type)
            {
                switch (type)
                {
                    case CraftTree_Type_All.Fabricator:
                        return nameof(_craftTreePaths_SN_Fabricator);

                    case CraftTree_Type_All.Constructor:
                        return nameof(_craftTreePaths_SN_MVB);

                    case CraftTree_Type_All.SeamothUpgrades:
                        return nameof(_craftTreePaths_SN_VUC);

                    default:
                        return nameof(_craftTreePaths_other);
                }
            }

            private static string GetDropdownListBZ(CraftTree_Type_All type)
            {
                switch (type)
                {
                    case CraftTree_Type_All.Fabricator:
                    case CraftTree_Type_All.SeaTruckFabricator:
                        return nameof(_craftTreePaths_BZ_Fabricator);

                    case CraftTree_Type_All.Constructor:
                        return nameof(_craftTreePaths_BZ_MVB);

                    case CraftTree_Type_All.SeamothUpgrades:
                        return nameof(_craftTreePaths_BZ_VUC);

                    default:
                        return nameof(_craftTreePaths_other);
                }
            }

            private static string GetDropdownListBoth(CraftTree_Type_All type)
            {
                switch (type)
                {
                    case CraftTree_Type_All.Fabricator:
                        return nameof(_craftTreePaths_SN_Fabricator);

                    case CraftTree_Type_All.Constructor:
                        return nameof(_craftTreePaths_common_MVB);

                    case CraftTree_Type_All.SeamothUpgrades:
                        return nameof(_craftTreePaths_common_VUC);

                    default:
                        return nameof(_craftTreePaths_other);
                }
            }
        }

        private readonly DropdownList<string> _craftTreePaths_SN_Fabricator = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Resources/(root)", "Resources"},
                {"Resources/Basic materials", "Resources/BasicMaterials"},
                {"Resources/Advanced materials", "Resources/AdvancedMaterials"},
                {"Resources/Electronics", "Resources/Electronics"},
            {"Survival/(root)", "Survival"},
                {"Survival/Water", "Survival/Water"},
                {"Survival/Cooked food", "Survival/CookedFood"},
                {"Survival/Cured food", "Survival/CuredFood"},
            {"Personal/(root)", "Personal"},
                {"Personal/Equipment", "Personal/Equipment"},
                {"Personal/Tools", "Personal/Tools"},
                {"Personal/Deployables", "Machines"},
        };

        private readonly DropdownList<string> _craftTreePaths_SN_MVB = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Vehicles", "Vehicles"},
            {"Rocket", "Rocket"},
        };

        private readonly DropdownList<string> _craftTreePaths_SN_VUC = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Common Modules", "CommonModules"},
            {"Seamoth Modules", "SeamothModules"},
            {"Prawn Suit Modules", "ExosuitModules"},
            {"Torpedoes", "Torpedoes"},
        };

        private readonly DropdownList<string> _craftTreePaths_BZ_Fabricator = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Resources/(root)", "Resources"},
                {"Resources/Basic materials", "Resources/BasicMaterials"},
                {"Resources/Advanced materials", "Resources/AdvancedMaterials"},
                {"Resources/Electronics", "Resources/Electronics"},
            {"Survival/(root)", "Survival"},
                {"Survival/Water", "Survival/Water"},
                {"Survival/Cooked food", "Survival/CookedFood"},
                {"Survival/Cured food", "Survival/CuredFood"},
            {"Personal/(root)", "Personal"},
                {"Personal/Equipment", "Personal/Equipment"},
                {"Personal/Tools", "Personal/Tools"},
                {"Personal/Deployables", "Machines"},
            {"Upgrades/(root)", "Upgrades"},
                {"Upgrades/Prawn Suit Upgrades", "Upgrades/ExosuitUpgrades"},
                {"Upgrades/Seatruck Upgrades", "Upgrades/SeatruckUpgrades"},
        };

        private readonly DropdownList<string> _craftTreePaths_BZ_MVB = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Vehicles", "Vehicles"},
            {"Modules", "Modules"},
        };

        private readonly DropdownList<string> _craftTreePaths_BZ_VUC = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Prawn Suit Upgrades", "ExosuitModules"},
            {"Seatruck Upgrades", "SeaTruckUpgrade"},
        };

        private readonly DropdownList<string> _craftTreePaths_other = new DropdownList<string>()
        {
            {"(root)", ""}
        };

        private readonly DropdownList<string> _craftTreePaths_common_MVB = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Vehicles", "Vehicles"},
        };

        private readonly DropdownList<string> _craftTreePaths_common_VUC = new DropdownList<string>()
        {
            {"(root)", ""},
            {"Prawn Suit Upgrades", "ExosuitModules"},
        };

        #endregion

        private protected class HideInNormalInspectorAttribute : HideIfAttribute
        {
            public HideInNormalInspectorAttribute() : base(nameof(_true))
            {
            }
        }

        public bool _true => true;
    }
}

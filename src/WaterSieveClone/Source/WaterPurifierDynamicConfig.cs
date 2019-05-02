using KSerialization;
using UnityEngine;

namespace WaterSieveDynamicClone
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class WaterPurifierDynamicConfig : WaterPurifierConfig
    {
        new public const string ID = "WaterPurifierDynamicClone";

        private const string DisplayName = "Water Sieve (Dynamic temperature)";
        public static string Description = "Sieves cannot kill germs and pass any disease they receive into their waste and water output.";
        public const string Effect = "Produces clean <link=\"WATER\">Water</link> from <link=\"DIRTYWATER\">Polluted Water</link> using <link=\"SAND\">Sand</link>.\n\nProduces <link=\"TOXICSAND\">Polluted Dirt</link>.\nDoesn't change water temperature.";

        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Refining", ID, WaterPurifierConfig.ID);
            AddBuilding.IntoTechTree("Distillation", ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = base.CreateBuildingDef();
            buildingDef.PrefabID = ID;
            buildingDef.InitDef();
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);

            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            for (int i = 0; i < elementConverter.outputElements.Length; ++i)
            {
                elementConverter.outputElements[i].applyInputTemperature = true;
            }
        }
    }
}

using UnityEngine;

namespace PipePressureValve
{
    public class LiquidPressureValveConfig : PipePressureValveConfig
    {
        
        public const string ID = "LiquidCPressureValve";
        public const string DisplayName = "Liquid Pressure Valve";
        public const string Description = "Might not fill completely in mixed element pipes.";
        public const string Effect = "Merges small liquid dots in pipes into big ones.";
        
        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Plumbing", ID, LiquidLogicValveConfig.ID);
            AddBuilding.IntoTechTree("ImprovedLiquidPiping", ID);
        }

        public static Color32 Color()
        {
            return new Color32(0, 255, 104, 255);
        }

        public override float GetThreshold()
        {
            return 22f;
        }
        public override float GetMaxStorage()
        {
            return 25f;
        }
        public override float GetPipeCapacity()
        {
            return 10f;
        }
        
        public override BuildingDef CreateBuildingDef()
        {
            return CreateBuildingDef(ConduitType.Liquid, "valveliquid_logic_kanim", ID);
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Liquid);
        }
    }
}

using UnityEngine;

namespace PipePressureValve
{
    public class GasPressureValveConfig : PipePressureValveConfig
    {

        public const string ID = "GasPressureValve";
        public const string DisplayName = "Gas Pressure Valve";
        public const string Description = "Might not fill completely in mixed element pipes.";
        public const string Effect = "Merges small gas dots in pipes into big ones.";

        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("HVAC", ID, GasLogicValveConfig.ID);
            AddBuilding.IntoTechTree("ImprovedGasPiping", ID);
        }

        public static Color32 Color()
        {
            return new Color32(0, 255, 104, 255);
        }

        public override float GetThreshold()
        {
            return 2.2f;
        }
        public override float GetMaxStorage()
        {
            return 2.5f;
        }
        public override float GetPipeCapacity()
        {
            return 1f;
        }

        public override BuildingDef CreateBuildingDef()
        {
            return CreateBuildingDef(ConduitType.Gas, "valvegas_logic_kanim", ID);
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas);
        }
    }
}

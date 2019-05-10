using Harmony;

namespace HighFlowStorage
{
    public static class HighFlowStoragePatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                HighFlowStorage.HighFlowLiquidReservoirConfig.Setup();
                HighFlowStorage.HighFlowGasReservoirConfig.Setup();
                HighFlowStorage.HighFlowGasReservoirVerticalConfig.Setup();
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ApplyColor
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (__instance.name.Equals((HighFlowLiquidReservoirConfig.ID + "Complete")))
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = HighFlowLiquidReservoirConfig.BuildingColor();
            }
            else if (__instance.name.Equals((HighFlowGasReservoirConfig.ID + "Complete")))
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = HighFlowGasReservoirConfig.BuildingColor();
            }
            else if (__instance.name.Equals((HighFlowGasReservoirVerticalConfig.ID + "Complete")))
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = HighFlowGasReservoirVerticalConfig.BuildingColor();
            }
        }
    }
}

using Harmony;

namespace NightLib
{
    public static class WaterPurifierDynamicPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                HighFlowStorage.HighFlowLiquidReservoirConfig.Setup();
            }
        }

        [HarmonyPatch(typeof(BuildingCellVisualizer))]
        [HarmonyPatch("DisableIcons")]
        public static class HidePorts
        {
            public static void Postfix(BuildingCellVisualizer __instance)
            {
                if (__instance.GetBuilding().name == "HighFlowLiquidReservoirComplete")
                {
                    __instance.GetBuilding().gameObject.GetComponent<PortDisplay1>().DisableIcons();
                    __instance.GetBuilding().gameObject.GetComponent<PortDisplay2>().DisableIcons();
                    __instance.GetBuilding().gameObject.GetComponent<PortDisplay3>().DisableIcons();
                    __instance.GetBuilding().gameObject.GetComponent<PortDisplay4>().DisableIcons();
                }
            }
        }
        

        [HarmonyPatch(typeof(BuildingCellVisualizer))]
        [HarmonyPatch("Tick")]
        public static class ApplyPorts
        {
            public static void Postfix(BuildingCellVisualizer __instance, HashedString mode)
            {
                if (mode == OverlayModes.LiquidConduits.ID)
                {
                    if (__instance.GetBuilding().name.Equals(HighFlowStorage.HighFlowLiquidReservoirConfig.ID + "Complete"))
                    {
                        __instance.GetBuilding().gameObject.GetComponent<PortDisplay1>().Draw(__instance.GetBuilding().gameObject, __instance);
                        __instance.GetBuilding().gameObject.GetComponent<PortDisplay2>().Draw(__instance.GetBuilding().gameObject, __instance);
                        __instance.GetBuilding().gameObject.GetComponent<PortDisplay3>().Draw(__instance.GetBuilding().gameObject, __instance);
                        __instance.GetBuilding().gameObject.GetComponent<PortDisplay4>().Draw(__instance.GetBuilding().gameObject, __instance);
                    }
                }
            }

        }

    }
}

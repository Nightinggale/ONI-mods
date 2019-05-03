using Harmony;

namespace WaterSieveDynamicClone
{
    public static class WaterPurifierDynamicPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                WaterPurifierDynamicConfig.Setup();
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ApplyColor
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, (WaterPurifierDynamicConfig.ID + "Complete")) == 0)
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = WaterPurifierDynamicConfig.Color();
            }
        }
    }
}
using Harmony;

namespace MoreTemperatureSensors
{
    public static class BuildingGenerationPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                TileTemperatureSensorConfig.Setup();
                ItemTemperatureSensorConfig.Setup();
                BuildingTemperatureSensorConfig.Setup();
            }
        }
    }

    
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ApplyColor
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (__instance.name.Equals((TileTemperatureSensorConfig.ID + "Complete")))
            {
                var KAnim = __instance.GetComponent<KAnimControllerBase>();
                if (KAnim != null)
                {
                    KAnim.TintColour = TileTemperatureSensorConfig.BuildingColor();
                }
            }
        }
    }
}

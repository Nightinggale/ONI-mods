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
}

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
            }
        }
    }
}

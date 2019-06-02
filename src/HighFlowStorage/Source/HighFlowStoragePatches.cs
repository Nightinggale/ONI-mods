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
                HighFlowStorage.HighFlowLiquidReservoirConfig2.Setup();
                HighFlowStorage.HighFlowLiquidReservoirVerticalConfig2.Setup();
                HighFlowStorage.HighFlowGasReservoirConfig2.Setup();
                HighFlowStorage.HighFlowGasReservoirVerticalConfig2.Setup();
            }
        }
    }
}

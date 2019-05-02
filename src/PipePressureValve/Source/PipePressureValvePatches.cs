using Harmony;

namespace PipePressureValve
{
    public static class PipePressureValvePatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                GasPressureValveConfig.Setup();
                LiquidPressureValveConfig.Setup();
            }
        }
    }
}

using Harmony;

namespace BlackHoleGarbageDisposal
{
    public static class Patches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                BlackHoleGarbageDisposalConfig.Setup();
            }
        }
    }
}

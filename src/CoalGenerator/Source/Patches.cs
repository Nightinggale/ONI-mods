using Harmony;

namespace Nightinggale.CoalGenerator
{
    public static class BuildingGenerationPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                AddStrings.AddString("Coal");
                CoalGeneratorConfig.Setup();
            }
        }
    }
}

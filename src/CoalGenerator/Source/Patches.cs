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
                Strings.Add($"STRINGS.BUILDINGS.NIGHTINGGALE.COALGENERATOR.TITLE", "Coal delivery controls");
                Strings.Add($"STRINGS.BUILDINGS.NIGHTINGGALE.COALGENERATOR.BATTERYTHRESHOLD", "Duplicants will deliver coal when the battery charge falls below the selected threshold.\nNote that this slider is ignored if the coal generator is connected to an automation wire.");

                CoalGeneratorConfig.Setup();
            }
        }
    }
}

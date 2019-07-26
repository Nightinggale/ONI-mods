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
                CoalGeneratorConfig.Setup();
            }

            public static void Postfix()
            {
                BuildingDef def = Assets.GetBuildingDef(GeneratorConfig.ID);
                if (def != null)
                {
                    ApplyCoalBurnerFixes.Apply(def.BuildingComplete);
                }
                def = Assets.GetBuildingDef(WoodGasGeneratorConfig.ID);
                if (def != null)
                {
                    ApplyCoalBurnerFixes.Apply(def.BuildingComplete);
                }
            }
        }
    }
}

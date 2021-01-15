using Harmony;

namespace Nightinggale.CoalGenerator
{
    public static class BuildingGenerationPatches
    {
        [HarmonyPatch(typeof(ManualDeliveryKG))]
        [HarmonyPatch("OperationalRequirementsMet")]
        public static class ManualDeliveryKG_OperationalRequirementsMet_Patch
        {
            public static bool Prefix(ref bool __result, ref ManualDeliveryKG __instance)
            {
               
                if (__instance.GetType() == typeof(CoalManualDeliveryKG))
                {
                    CoalManualDeliveryKG obj = __instance as CoalManualDeliveryKG;
                    __result = obj.OperationalRequirementsMet();
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                //CoalGeneratorConfig.Setup();
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

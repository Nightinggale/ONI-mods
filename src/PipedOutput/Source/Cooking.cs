using Harmony;
using UnityEngine;
using NightLib;

namespace Nightinggale.PipedOutput
{
    // Currently has no effect
    public static class CookingBuildingGenerationPatches
    {
        internal static void AddGourmetCooking(GameObject go)
        {
            //ApplyExhaust.AddOutput(go, new CellOffset(1, 2), SimHashes.CarbonDioxide);
        }

        [HarmonyPatch(typeof(IBuildingConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class PreviewPatch
        {
            public static void Postfix(IBuildingConfig __instance, GameObject go)
            {
                var T = __instance.GetType();
                if (T == typeof(GourmetCookingStationConfig))
                    AddGourmetCooking(go);
            }
        }

        [HarmonyPatch(typeof(IBuildingConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class UnderConstructionPatch
        {
            public static void Postfix(IBuildingConfig __instance, GameObject go)
            {
                var T = __instance.GetType();
                if (T == typeof(GourmetCookingStationConfig))
                    AddGourmetCooking(go);
            }
        }

        [HarmonyPatch(typeof(GourmetCookingStationConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class GourmetCookingCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddGourmetCooking(go);
            }
        }
    }
}

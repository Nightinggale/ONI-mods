using Harmony;
using UnityEngine;
using NightLib;

namespace Nightinggale.PipedOutput
{
    public static class CookingBuildingGenerationPatches
    {
        internal static void AddGourmetCooking(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(1, 2), SimHashes.CarbonDioxide);
        }

        [HarmonyPatch(typeof(GourmetCookingStationConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class GourmetCookingPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddGourmetCooking(go);
            }
        }
        [HarmonyPatch(typeof(GourmetCookingStationConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class GourmetCookingUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
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

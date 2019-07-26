using Harmony;
using UnityEngine;
using NightLib;

namespace Nightinggale.PipedOutput
{
    public static class CookingBuildingGenerationPatches
    {
        internal static void AddGourmetCooking(GameObject go)
        {
            // Due to limitations in ElementConverter, this is disabled for the time being
            // It tries to store CarbonDioxide in the input storage and then drops it in bottles
            //ApplyExhaust.AddOutput(go, new CellOffset(1, 2), SimHashes.CarbonDioxide);
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

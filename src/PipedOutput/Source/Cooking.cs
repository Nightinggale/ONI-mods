using HarmonyLib;
using UnityEngine;
using NightLib;

namespace Nightinggale.PipedOutput
{
    public static class CookingBuildingGenerationPatches
    {
        public static void AddGourmetCooking(GameObject go)
        {
            //ApplyExhaust.AddOutput(go, new CellOffset(1, 2), SimHashes.CarbonDioxide);
        }


        [HarmonyPatch(typeof(GourmetCookingStationConfig), "DoPostConfigureComplete")]
        public static class GourmetCookingCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddGourmetCooking(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddGourmetCooking(def.BuildingPreview);
                    AddGourmetCooking(def.BuildingUnderConstruction);
                }
            }
        }
    }
}

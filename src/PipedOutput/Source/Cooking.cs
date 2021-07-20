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





        public static void GourmetCookingComplete(BuildingDef def)
        {
            AddGourmetCooking(def.BuildingComplete);
            AddGourmetCooking(def.BuildingPreview);
            AddGourmetCooking(def.BuildingUnderConstruction);
        }
    }
}

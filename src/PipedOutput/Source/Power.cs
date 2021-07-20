using HarmonyLib;
using UnityEngine;
using NightLib;
using System;

namespace Nightinggale.PipedOutput
{
    public static class PowerBuildingGenerationPatches
    {
        public static void AddCoalGenerator(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(1, 2), SimHashes.CarbonDioxide);
        }

        public static void AddWoodGenerator(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(0, 1), SimHashes.CarbonDioxide);
        }

        public static void AddOilGenerator(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(0, 3), SimHashes.CarbonDioxide);
            ApplyExhaust.AddOutput(go, new CellOffset(1, 1), SimHashes.DirtyWater);
        }

        public static void AddGasGenerator(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(2, 2), SimHashes.CarbonDioxide);
            ApplyExhaust.AddOutput(go, new CellOffset(1, 1), SimHashes.DirtyWater);
        }





        public static void CoalBurnerComplete(BuildingDef def)
        {
            AddCoalGenerator(def.BuildingComplete);
            AddCoalGenerator(def.BuildingPreview);
            AddCoalGenerator(def.BuildingUnderConstruction);
        }

        public static void WoodBurnerComplete(BuildingDef def)
        {
            AddWoodGenerator(def.BuildingComplete);
            AddWoodGenerator(def.BuildingPreview);
            AddWoodGenerator(def.BuildingUnderConstruction);
        }

        public static void OilBurnerComplete(BuildingDef def)
        {
            AddOilGenerator(def.BuildingComplete);
            AddOilGenerator(def.BuildingPreview);
            AddOilGenerator(def.BuildingUnderConstruction);
        }

        [HarmonyPatch(typeof(MethaneGeneratorConfig), "CreateBuildingDef")]
        public static class GasBurnerDefPatch
        {
            public static void Postfix(ref BuildingDef __result)
            {
                Helpers.PrintDebug("GasBurnerDefPatch");

                __result.OutputConduitType = ConduitType.None;
            }
        }
        public static void GasBurnerComplete(BuildingDef def)
        {
            AddGasGenerator(def.BuildingComplete);
            AddGasGenerator(def.BuildingPreview);
            AddGasGenerator(def.BuildingUnderConstruction);

            // remove the existing dispenser because it is messing up the CO2 output and it's no longer needed
            ConduitDispenser conduitDispenser = def.BuildingComplete.GetComponent<ConduitDispenser>();
            if (conduitDispenser != null)
            {
                UnityEngine.Object.DestroyImmediate(conduitDispenser);
            }
        }

    }
}

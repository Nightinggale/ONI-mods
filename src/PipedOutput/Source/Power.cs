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





        [HarmonyPatch(typeof(GeneratorConfig), "DoPostConfigureComplete")]
        public static class CoalBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddCoalGenerator(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddCoalGenerator(def.BuildingPreview);
                    AddCoalGenerator(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(WoodGasGeneratorConfig), "DoPostConfigureComplete")]
        public static class WoodBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddWoodGenerator(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddWoodGenerator(def.BuildingPreview);
                    AddWoodGenerator(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(PetroleumGeneratorConfig), "DoPostConfigureComplete")]
        public static class OilBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilGenerator(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddOilGenerator(def.BuildingPreview);
                    AddOilGenerator(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(MethaneGeneratorConfig), "CreateBuildingDef")]
        public static class GasBurnerDefPatch
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.OutputConduitType = ConduitType.None;
            }
        }
        [HarmonyPatch(typeof(MethaneGeneratorConfig), "DoPostConfigureComplete")]
        public static class GasBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddGasGenerator(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddGasGenerator(def.BuildingPreview);
                    AddGasGenerator(def.BuildingUnderConstruction);
                }
                // remove the existing dispenser because it is messing up the CO2 output and it's no longer needed
                ConduitDispenser conduitDispenser = go.GetComponent<ConduitDispenser>();
                if (conduitDispenser != null)
                {
                    UnityEngine.Object.DestroyImmediate(conduitDispenser);
                }
            }
        }

    }
}

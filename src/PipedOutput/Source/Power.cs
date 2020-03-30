using Harmony;
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

        [HarmonyPatch(typeof(IBuildingConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class PreviewPatch
        {
            public static void Postfix(IBuildingConfig __instance, GameObject go)
            {
                var T = __instance.GetType();
                if (T == typeof(GeneratorConfig))
                    AddCoalGenerator(go);
                else if (T == typeof(WoodGasGeneratorConfig))
                    AddWoodGenerator(go);
                else if (T == typeof(PetroleumGeneratorConfig))
                    AddOilGenerator(go);
                else if (T == typeof(MethaneGeneratorConfig))
                    AddGasGenerator(go);
            }
        }

        [HarmonyPatch(typeof(IBuildingConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class UnderConstructionPatch
        {
            public static void Postfix(IBuildingConfig __instance, GameObject go)
            {
                var T = __instance.GetType();
                if (T == typeof(GeneratorConfig))
                    AddCoalGenerator(go);
                else if (T == typeof(WoodGasGeneratorConfig))
                    AddWoodGenerator(go);
                else if (T == typeof(PetroleumGeneratorConfig))
                    AddOilGenerator(go);
                else if (T == typeof(MethaneGeneratorConfig))
                    AddGasGenerator(go);
            }
        }

        [HarmonyPatch(typeof(GeneratorConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public static class CoalBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddCoalGenerator(go);
            }
        }

        [HarmonyPatch(typeof(WoodGasGeneratorConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public static class WoodBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddWoodGenerator(go);
            }
        }

        [HarmonyPatch(typeof(PetroleumGeneratorConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public static class OilBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilGenerator(go);
            }
        }

        [HarmonyPatch(typeof(MethaneGeneratorConfig))]
        [HarmonyPatch("CreateBuildingDef")]
        public static class GasBurnerDefPatch
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.OutputConduitType = ConduitType.None;
            }
        }
        [HarmonyPatch(typeof(MethaneGeneratorConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public static class GasBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddGasGenerator(go);
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

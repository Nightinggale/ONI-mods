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



        [HarmonyPatch(typeof(GeneratorConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class CoalBurnerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddCoalGenerator(go);
            }
        }
        [HarmonyPatch(typeof(GeneratorConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class CoalBurnerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddCoalGenerator(go);
            }
        }
        [HarmonyPatch(typeof(GeneratorConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class CoalBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddCoalGenerator(go);
            }
        }
        [HarmonyPatch(typeof(WoodGasGeneratorConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class WoodBurnerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddWoodGenerator(go);
            }
        }
        [HarmonyPatch(typeof(WoodGasGeneratorConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class WoodBurnerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddWoodGenerator(go);
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
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class OilBurnerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilGenerator(go);
            }
        }
        [HarmonyPatch(typeof(PetroleumGeneratorConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class OilBurnerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilGenerator(go);
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
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class GasBurnerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddGasGenerator(go);
            }
        }
        [HarmonyPatch(typeof(MethaneGeneratorConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class GasBurnerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddGasGenerator(go);
            }
        }
        [HarmonyPatch(typeof(MethaneGeneratorConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public static class GasBurnerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddGasGenerator(go);
            }
        }

    }
}

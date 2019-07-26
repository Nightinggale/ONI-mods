using Harmony;
using UnityEngine;

namespace Nightinggale.PipedOutput
{
    public static class RefinementBuildingGenerationPatches
    {

        public static void AddOilRefinery(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(-1, 3), SimHashes.Methane);
        }

        public static void AddFertilizerMaker(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(2, 2), SimHashes.Methane);
        }

        public static void AddEthanolDistillery(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(2, 2), SimHashes.CarbonDioxide);
        }

        public static void AddPolymer(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(0, 1), SimHashes.CarbonDioxide);
            ApplyExhaust.AddOutput(go, new CellOffset(1, 0), SimHashes.Steam);
        }

        [HarmonyPatch(typeof(OilRefineryConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class OilRefineryPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilRefinery(go);
            }
        }
        [HarmonyPatch(typeof(OilRefineryConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class OilRefineryUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilRefinery(go);
            }
        }
        [HarmonyPatch(typeof(OilRefineryConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class OilRefineryCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilRefinery(go);
            }
        }

        [HarmonyPatch(typeof(FertilizerMakerConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class FertilizerMakerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddFertilizerMaker(go);
            }
        }
        [HarmonyPatch(typeof(FertilizerMakerConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class FertilizerMakerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddFertilizerMaker(go);
            }
        }
        [HarmonyPatch(typeof(FertilizerMakerConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class FertilizerMakerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                BuildingElementEmitter emitter = go.GetComponent<BuildingElementEmitter>();
                if (emitter != null)
                {
                    ElementConverter converter = go.GetComponent<ElementConverter>();
                    if (converter != null)
                    {
                        converter.outputElements.Append(new ElementConverter.OutputElement(emitter.emitRate, SimHashes.Methane, emitter.temperature));
                        UnityEngine.Object.DestroyImmediate(emitter);
                    }
                }
                AddFertilizerMaker(go);
            }
        }

        [HarmonyPatch(typeof(EthanolDistilleryConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class EthanolDistilleryPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddEthanolDistillery(go);
            }
        }
        [HarmonyPatch(typeof(EthanolDistilleryConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class EthanolDistilleryUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddEthanolDistillery(go);
            }
        }
        [HarmonyPatch(typeof(EthanolDistilleryConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class EthanolDistilleryCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddEthanolDistillery(go);
            }
        }

        [HarmonyPatch(typeof(PolymerizerConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class PolymerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddPolymer(go);
            }
        }
        [HarmonyPatch(typeof(PolymerizerConfig))]
        [HarmonyPatch("CreateBuildingDef")]
        public static class PolymerDefPatch
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.OutputConduitType = ConduitType.None;
            }
        }
        [HarmonyPatch(typeof(PolymerizerConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class PolymerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddPolymer(go);
            }
        }
        [HarmonyPatch(typeof(PolymerizerConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class PolymerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddPolymer(go);
            }
        }
    }
}

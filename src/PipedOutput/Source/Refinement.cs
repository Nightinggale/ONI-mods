using HarmonyLib;
using UnityEngine;
using System;

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





        [HarmonyPatch(typeof(OilRefineryConfig), "DoPostConfigureComplete")]
        public static class OilRefineryCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilRefinery(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddOilRefinery(def.BuildingPreview);
                    AddOilRefinery(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(FertilizerMakerConfig), "DoPostConfigureComplete")]
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
                        // Reserve memory for one more element in the array
                        Array.Resize(ref converter.outputElements, converter.outputElements.Length + 1);
                        // assign methane to what is now the last element in the array
                        converter.outputElements[converter.outputElements.Length - 1] = new ElementConverter.OutputElement(emitter.emitRate, SimHashes.Methane, emitter.temperature);

                        UnityEngine.Object.DestroyImmediate(emitter);
                    }
                }
                AddFertilizerMaker(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddFertilizerMaker(def.BuildingPreview);
                    AddFertilizerMaker(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(EthanolDistilleryConfig), "DoPostConfigureComplete")]
        public static class EthanolDistilleryCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddEthanolDistillery(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddEthanolDistillery(def.BuildingPreview);
                    AddEthanolDistillery(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(PolymerizerConfig), "CreateBuildingDef")]
        public static class PolymerDefPatch
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.OutputConduitType = ConduitType.None;
            }
        }
        [HarmonyPatch(typeof(PolymerizerConfig), "DoPostConfigureComplete")]
        public static class PolymerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddPolymer(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddPolymer(def.BuildingPreview);
                    AddPolymer(def.BuildingUnderConstruction);
                }
            }
        }
    }
}

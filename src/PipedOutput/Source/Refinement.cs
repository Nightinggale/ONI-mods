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





        public static void OilRefineryComplete(BuildingDef def)
        {
            AddOilRefinery(def.BuildingComplete);
            AddOilRefinery(def.BuildingPreview);
            AddOilRefinery(def.BuildingUnderConstruction);
        }

        public static void FertilizerMakerComplete(BuildingDef def)
        {
            Helpers.PrintDebug("FertilizerMakerCompletePatch");

            BuildingElementEmitter emitter = def.BuildingComplete.GetComponent<BuildingElementEmitter>();
            if (emitter != null)
            {
                ElementConverter converter = def.BuildingComplete.GetComponent<ElementConverter>();
                if (converter != null)
                {
                    // Reserve memory for one more element in the array
                    Array.Resize(ref converter.outputElements, converter.outputElements.Length + 1);
                    // assign methane to what is now the last element in the array
                    converter.outputElements[converter.outputElements.Length - 1] = new ElementConverter.OutputElement(emitter.emitRate, SimHashes.Methane, emitter.temperature);

                    UnityEngine.Object.DestroyImmediate(emitter);
                }
            }

            AddFertilizerMaker(def.BuildingComplete);
            AddFertilizerMaker(def.BuildingPreview);
            AddFertilizerMaker(def.BuildingUnderConstruction);

            Helpers.PrintDebug("FertilizerMakerCompletePatch done");
        }

        public static void EthanolDistilleryComplete(BuildingDef def)
        {
            AddEthanolDistillery(def.BuildingComplete);
            AddEthanolDistillery(def.BuildingPreview);
            AddEthanolDistillery(def.BuildingUnderConstruction);
        }

        [HarmonyPatch(typeof(PolymerizerConfig), "CreateBuildingDef")]
        public static class PolymerDefPatch
        {
            public static void Postfix(ref BuildingDef __result)
            {
                Helpers.PrintDebug("PolymerDefPatch");

                __result.OutputConduitType = ConduitType.None;
            }
        }
        public static void PolymerComplete(BuildingDef def)
        {
            AddPolymer(def.BuildingComplete);
            AddPolymer(def.BuildingPreview);
            AddPolymer(def.BuildingUnderConstruction);
        }
    }
}

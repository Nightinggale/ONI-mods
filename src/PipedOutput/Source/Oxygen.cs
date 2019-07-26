using Harmony;
using UnityEngine;

namespace Nightinggale.PipedOutput
{
    public static class OxygenBuildingGenerationPatches
    {

        public static void AddElectrolyzer(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(1, 1), SimHashes.Oxygen);
            ApplyExhaust.AddOutput(go, new CellOffset(0, 1), SimHashes.Hydrogen);
        }

        public static void AddRust(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(1, 1), SimHashes.Oxygen);
            ApplyExhaust.AddOutput(go, new CellOffset(0, 0), SimHashes.ChlorineGas);
        }


        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class ElectrolyzerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddElectrolyzer(go);
            }
        }
        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class ElectrolyzerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddElectrolyzer(go);
            }
        }
        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class ElectrolyzerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddElectrolyzer(go);
            }
        }

        [HarmonyPatch(typeof(RustDeoxidizerConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class RustPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddRust(go);
            }
        }
        [HarmonyPatch(typeof(RustDeoxidizerConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class RustUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddRust(go);
            }
        }
        [HarmonyPatch(typeof(RustDeoxidizerConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class RustCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                ElementConverter converter = go.GetComponent<ElementConverter>();
                if (converter != null)
                {
                    // change the chlorine output from a liquid to a gas. Most likely a bug since it's at least 100 C higher than boiling.
                    for (int i = 0; i < converter.outputElements.Length; ++i)
                    {
                        if (converter.outputElements[i].elementHash == SimHashes.Chlorine)
                        {
                            converter.outputElements[i].elementHash = SimHashes.ChlorineGas;
                        }
                    }
                }
                AddRust(go);
            }
        }
    }
}

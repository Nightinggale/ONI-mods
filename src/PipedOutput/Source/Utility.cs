using Harmony;
using UnityEngine;
using NightLib;

namespace Nightinggale.PipedOutput
{
    public static class UtilityBuildingGenerationPatches
    {
        internal static PortDisplayOutput AddOilWell(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(2, 1), SimHashes.CrudeOil);
            ApplyExhaust.AddOutput(go, new CellOffset(1, 1), SimHashes.CrudeOil);
            
            Element element = ElementLoader.GetElement(SimHashes.Methane.CreateTag());
            Color32 color = element.substance.conduitColour;
            color.a = 255;
            PortDisplayOutput outputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 1), null, color);
            PortDisplayController controller = go.AddOrGet<PortDisplayController>();
            controller.AssignPort(go, outputPort);

            return outputPort;
        }

        [HarmonyPatch(typeof(OilWellCapConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class OilWellPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilWell(go);
            }
        }
        [HarmonyPatch(typeof(OilWellCapConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class OilWellUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddOilWell(go);
            }
        }
        [HarmonyPatch(typeof(OilWellCapConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class OilWellCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                PortDisplayOutput outputPort = AddOilWell(go);
                PipedDispenser dispenser = go.AddComponent<PipedDispenser>();
                dispenser.elementFilter = new SimHashes[] { SimHashes.Methane };
                dispenser.AssignPort(outputPort);
                dispenser.alwaysDispense = true;
                dispenser.SkipSetOperational = true;
            }
        }
    }
}

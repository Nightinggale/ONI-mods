using HarmonyLib;
using UnityEngine;
using NightLib;

namespace Nightinggale.PipedOutput
{
    public static class UtilityBuildingGenerationPatches
    {
        internal static PortDisplayOutput AddOilWell(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(2, 1), SimHashes.CrudeOil);

            Element element = ElementLoader.GetElement(SimHashes.Methane.CreateTag());
            Color32 color = element.substance.conduitColour;
            color.a = 255;
            PortDisplayOutput outputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 1), null, color);
            PortDisplayController controller = go.AddOrGet<PortDisplayController>();
            controller.AssignPort(go, outputPort);

            return outputPort;
        }





        public static void OilWellComplete(BuildingDef def)
        {
            Helpers.PrintDebug("OilWellCompletePatch");

            PortDisplayOutput outputPort = AddOilWell(def.BuildingComplete);
            AddOilWell(def.BuildingPreview);
            AddOilWell(def.BuildingUnderConstruction);
            PipedDispenser dispenser = def.BuildingComplete.AddComponent<PipedDispenser>();
            dispenser.elementFilter = new SimHashes[] { SimHashes.Methane };
            dispenser.AssignPort(outputPort);
            dispenser.alwaysDispense = true;
            dispenser.SkipSetOperational = true;

            Helpers.PrintDebug("OilWellCompletePatch done");
        }
    }
}

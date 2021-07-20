using HarmonyLib;
using UnityEngine;
using NightLib;

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

        internal static PortDisplayOutput AddAlgaeHabitat(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(0, 1), SimHashes.Oxygen);

            Element element = ElementLoader.GetElement(SimHashes.DirtyWater.CreateTag());
            Color32 color = element.substance.conduitColour;
            color.a = 255;
            PortDisplayOutput outputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 0), null, color);
            PortDisplayController controller = go.AddOrGet<PortDisplayController>();
            controller.AssignPort(go, outputPort);

            return outputPort;
        }

        public static void AddMineralDeoxidizer(GameObject go)
        {
            ApplyExhaust.AddOutput(go, new CellOffset(0, 1), SimHashes.Oxygen);
        }





        public static void AlgaeHabitatComplete(BuildingDef def)
        {
            PortDisplayOutput outputPort = AddAlgaeHabitat(def.BuildingComplete);
            AddAlgaeHabitat(def.BuildingPreview);
            AddAlgaeHabitat(def.BuildingUnderConstruction);

            PipedDispenser dispenser = def.BuildingComplete.AddComponent<PipedDispenser>();
            dispenser.AssignPort(outputPort);
            dispenser.SkipSetOperational = true;
            dispenser.alwaysDispense = true;

            Storage[] storageComponents = def.BuildingComplete.GetComponents<Storage>();

            foreach (Storage storage in storageComponents)
            {
                if (storage.storageFilters != null && storage.storageFilters.Contains(SimHashes.DirtyWater.CreateTag()))
                {
                    dispenser.storage = storage;
                    break;
                }
            }
        }

        public static void ElectrolyzerComplete(BuildingDef def)
        {
            AddElectrolyzer(def.BuildingComplete);
            AddElectrolyzer(def.BuildingPreview);
            AddElectrolyzer(def.BuildingUnderConstruction);
        }

        public static void MineralDeoxidizerComplete(BuildingDef def)
        {
            AddMineralDeoxidizer(def.BuildingComplete);
            AddMineralDeoxidizer(def.BuildingPreview);
            AddMineralDeoxidizer(def.BuildingUnderConstruction);
        }

        public static void RustComplete(BuildingDef def)
        {
            AddRust(def.BuildingComplete);
            AddRust(def.BuildingPreview);
            AddRust(def.BuildingUnderConstruction);
        }
    }
}

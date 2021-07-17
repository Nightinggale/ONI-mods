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





        [HarmonyPatch(typeof(AlgaeHabitatConfig), "DoPostConfigureComplete")]
        public static class AlgaeHabitatPatch
        {
            public static void Postfix(GameObject go)
            {
                PortDisplayOutput outputPort = AddAlgaeHabitat(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddAlgaeHabitat(def.BuildingPreview);
                    AddAlgaeHabitat(def.BuildingUnderConstruction);
                }

                PipedDispenser dispenser = go.AddComponent<PipedDispenser>();
                dispenser.AssignPort(outputPort);
                dispenser.SkipSetOperational = true;
                dispenser.alwaysDispense = true;
                
                Storage[] storageComponents = go.GetComponents<Storage>();

                foreach (Storage storage in storageComponents)
                {
                    if (storage.storageFilters != null && storage.storageFilters.Contains(SimHashes.DirtyWater.CreateTag()))
                    {
                        dispenser.storage = storage;
                        break;
                    }
                }
            }
        }


        [HarmonyPatch(typeof(ElectrolyzerConfig), "DoPostConfigureComplete")]
        public static class ElectrolyzerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddElectrolyzer(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddElectrolyzer(def.BuildingPreview);
                    AddElectrolyzer(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(MineralDeoxidizerConfig), "DoPostConfigureComplete")]
        public static class MineralDeoxidizerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddMineralDeoxidizer(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddMineralDeoxidizer(def.BuildingPreview);
                    AddMineralDeoxidizer(def.BuildingUnderConstruction);
                }
            }
        }

        [HarmonyPatch(typeof(RustDeoxidizerConfig), "DoPostConfigureComplete")]
        public static class RustCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddRust(go);
                BuildingDef def = go.GetComponent<BuildingComplete>().Def;
                if (def != null)
                {
                    AddRust(def.BuildingPreview);
                    AddRust(def.BuildingUnderConstruction);
                }
            }
        }
    }
}

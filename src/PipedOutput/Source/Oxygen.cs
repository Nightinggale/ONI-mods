using Harmony;
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
            //ApplyExhaust.AddOutput(go, new CellOffset(1, 1), SimHashes.Oxygen);
            //ApplyExhaust.AddOutput(go, new CellOffset(0, 0), SimHashes.ChlorineGas);
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

        [HarmonyPatch(typeof(AlgaeHabitatConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
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

        [HarmonyPatch(typeof(MineralDeoxidizerConfig))]
        [HarmonyPatch("DoPostConfigurePreview")]
        public static class MineralDeoxidizerPreviewPatch
        {
            public static void Postfix(GameObject go)
            {
                AddMineralDeoxidizer(go);
            }
        }
        [HarmonyPatch(typeof(MineralDeoxidizerConfig))]
        [HarmonyPatch("DoPostConfigureUnderConstruction")]
        public static class MineralDeoxidizerUnderConstructionPatch
        {
            public static void Postfix(GameObject go)
            {
                AddMineralDeoxidizer(go);
            }
        }
        [HarmonyPatch(typeof(MineralDeoxidizerConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class MineralDeoxidizerCompletePatch
        {
            public static void Postfix(GameObject go)
            {
                AddMineralDeoxidizer(go);
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
                AddRust(go);
            }
        }
    }
}

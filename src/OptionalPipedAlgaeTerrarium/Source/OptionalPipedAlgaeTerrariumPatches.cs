using Harmony;
using NightLib;
using UnityEngine;

namespace Nightinggale.OptionalPipedAlgaeTerrarium
{
    [HarmonyPatch(typeof(AlgaeHabitatConfig))]
    [HarmonyPatch("DoPostConfigureComplete")]
    public static class LoadingComplete
    {
        private static readonly PortDisplayOutput outputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 0));

        private static void AttachPort(GameObject go)
        {
            if (go != null)
            {
                PortDisplayController controller = go.AddComponent<PortDisplayController>();
                controller.Init(go);
                controller.AssignPort(go, outputPort);
            }
        }


        public static void Postfix(GameObject go)
        {
            BuildingDef def = go.GetComponent<BuildingComplete>().Def;
            if (def != null)
            {
                AttachPort(def.BuildingPreview);
                AttachPort(def.BuildingUnderConstruction);
                AttachPort(def.BuildingComplete);
                if (go != null)
                {
                    AlgaePollutedWaterDispenser dispenser = go.AddOrGet<AlgaePollutedWaterDispenser>();
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
        }
    }
}


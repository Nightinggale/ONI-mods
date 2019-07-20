using Harmony;
using NightLib;
using UnityEngine;

namespace Nightinggale.OptionalPipedAlgaeTerrarium
{
    // Patching AlgaeHabitatConfig crashes the game
    // https://forums.kleientertainment.com/klei-bug-tracker/oni/algaehabitatconfig-cant-be-modded-possible-simple-fix-included-r20599/
    /*
    [HarmonyPatch(typeof(AlgaeHabitatConfig))]
    [HarmonyPatch("ConfigureBuildingTemplate")]
    public static class PatchA_AlgaeHabitatConfig
    {
        public static void Postfix()
        {
        }
    }
    */

    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
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


        public static void Postfix()
        {
            BuildingDef def = Assets.GetBuildingDef(AlgaeHabitatConfig.ID);
            if (def != null)
            {
                AttachPort(def.BuildingPreview);
                AttachPort(def.BuildingUnderConstruction);
                AttachPort(def.BuildingComplete);

                GameObject go = def.BuildingComplete;
                if (go != null)
                {
                    AlgaePollutedWaterDispenser dispenser = go.AddOrGet<AlgaePollutedWaterDispenser>();
                    dispenser.AssignPort(outputPort);
                    dispenser.SkipSetOperational = true;
                    dispenser.alwaysDispense = true;

                    Storage[] storageComponents = go.GetComponents<Storage>();

                    foreach (Storage storage in storageComponents)
                    {
                        if (storage.storageFilters != null && storage.storageFilters.Contains(ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag))
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


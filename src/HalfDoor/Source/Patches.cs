using Harmony;

namespace Nightinggale.HalfDoor
{
    public static class BuildingGenerationPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                Nightinggale.HalfDoor.HalfDoorConfig.Setup();
                Nightinggale.HalfDoor.HalfManualDoorConfig.Setup();
                Nightinggale.HalfDoor.HalfPneumaticDoorConfig.Setup();
            }
        }
    }
}

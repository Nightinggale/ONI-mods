using Harmony;
using System;
using System.Reflection;

namespace Nightinggale.HalfDoor
{
    public static class LogNameWriter
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                Console.WriteLine("Loaded mod: " + Assembly.GetExecutingAssembly().GetName());
            }
        }
    }
}

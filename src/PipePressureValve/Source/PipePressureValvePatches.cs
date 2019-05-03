using Harmony;

namespace PipePressureValve
{
    public static class PipePressureValvePatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                GasPressureValveConfig.Setup();
                LiquidPressureValveConfig.Setup();
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ApplyColor
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, (GasPressureValveConfig.ID + "Complete")) == 0)
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = GasPressureValveConfig.Color();
            }
            else if(string.Compare(__instance.name, (LiquidPressureValveConfig.ID + "Complete")) == 0)
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = LiquidPressureValveConfig.Color();
            }
        }
    }
}

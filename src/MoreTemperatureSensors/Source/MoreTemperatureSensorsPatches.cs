using Harmony;
using UnityEngine;

namespace MoreTemperatureSensors
{
    public static class BuildingGenerationPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                Strings.Add($"NIGHTINGGALE.SENSORY.OVERLOADED.MASS", "Mass");

                TileTemperatureSensorConfig.Setup();
                ItemTemperatureSensorConfig.Setup();
                BuildingTemperatureSensorConfig.Setup();
                ModdedLogicElementSensorLiquidConfig.Setup();
                ConduitPressureSensorGas.Setup();
                ConduitPressureSensorLiquid.Setup();

                SolidConduitTemperatureSensorConfig.Setup();
                SolidConduitPressureSensorConfig.Setup();
            }
        }
    }

    
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ApplyColor
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (__instance.name.Equals((TileTemperatureSensorConfig.ID + "Complete")))
            {
                var KAnim = __instance.GetComponent<KAnimControllerBase>();
                if (KAnim != null)
                {
                    KAnim.TintColour = TileTemperatureSensorConfig.BuildingColor();
                }
            }
        }
    }

    [HarmonyPatch(typeof(LogicPressureSensorGasConfig), "DoPostConfigureComplete")]
    public class ConfigureGasSensor
    {
        public static void Postfix(GameObject go)
        {
            go.GetComponent<LogicPressureSensor>().rangeMax = MoreTemperatureSensorsConfig.Config.GasPressureSensorMax;
        }
    }

    [HarmonyPatch(typeof(LogicPressureSensorLiquidConfig), "DoPostConfigureComplete")]
    public class ConfigureLiquidSensor
    {
        public static void Postfix(GameObject go)
        {
            go.GetComponent<LogicPressureSensor>().rangeMax = MoreTemperatureSensorsConfig.Config.LiquidPressureSensorMax;
        }
    }

    [HarmonyPatch(typeof(LogicElementSensorGasConfig), "CreateBuildingDef")]
    public class ConfigureGasElementSensor
    {
        public static void Postfix(BuildingDef __result)
        {
            __result.RequiresPowerInput = false;
            __result.EnergyConsumptionWhenActive = 0f;
        }
    }
}

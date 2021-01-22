using Harmony;
using STRINGS;
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
                Strings.Add($"NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.THRESHOLD", "Flow Threshold");
                Strings.Add($"NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.NAME", "Flow");
                Strings.Add($"NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.ABOVETOOLTIP", string.Concat(new string[] { "Will send a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " if the ", UI.FormatAsKeyWord("Flow"), " is above <b>{0}</b> " }));
                Strings.Add($"NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.BELOWTOOLTIP", string.Concat(new string[] { "Will send a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " if the ", UI.FormatAsKeyWord("Flow"), " is below <b>{0}</b> " }));

                TileTemperatureSensorConfig.Setup();
                ItemTemperatureSensorConfig.Setup();
                ItemGermSensorConfig.Setup();
                BuildingTemperatureSensorConfig.Setup();
                ModdedLogicElementSensorLiquidConfig.Setup();
                ConduitPressureSensorGas.Setup();
                ConduitPressureSensorLiquid.Setup();
                ConduitFlowSensorGas.Setup();
                ConduitFlowSensorLiquid.Setup();

                SolidConduitElementSensorConfig.Setup();
                SolidConduitGermsSensorConfig.Setup();
                SolidConduitTemperatureSensorConfig.Setup();
                SolidConduitPressureSensorConfig.Setup();
                SolidConduitFlowSensorConfig.Setup();

                BatterySensorConfig.Setup();
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

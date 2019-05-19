using TUNING;
using UnityEngine;
using NightLib.AddBuilding;
using STRINGS;

namespace MoreTemperatureSensors
{
    public class ModdedLogicElementSensorLiquidConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.LogicElementSensorLiquid";

        public static void Setup()
        {
            AddBuilding.AddStrings(ID,
                STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORLIQUID.NAME,
                STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORLIQUID.DESC,
                STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORLIQUID.EFFECT);

            AddBuilding.AddBuildingToPlanScreen("Automation", ID, LogicElementSensorGasConfig.ID);
            AddBuilding.IntoTechTree("GenericSensors", ID);
        }

        public static Color32 BuildingColor()
        {
            return new Color32(0, 255, 104, 255);
        }

        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORLIQUID.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORLIQUID.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORLIQUID.LOGIC_PORT_INACTIVE, false);

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "world_element_sensor_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = true;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ModdedLogicElementSensorLiquidConfig.ID);
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicElementSensorLiquidConfig.OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicElementSensorLiquidConfig.OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicElementSensorLiquidConfig.OUTPUT_PORT);
            Filterable filterable = go.AddOrGet<Filterable>();
            filterable.filterElementState = Filterable.ElementState.Liquid;
            LogicElementSensorColor logicElementSensor = go.AddOrGet<LogicElementSensorColor>();
            logicElementSensor.manuallyControlled = false;
            logicElementSensor.desiredState = Element.State.Liquid;
        }
    }
}

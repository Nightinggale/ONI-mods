using STRINGS;
using System;
using TUNING;
using UnityEngine;
using NightLib.AddBuilding;

namespace MoreTemperatureSensors
{
    public class BuildingTemperatureSensorConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.BuildingTemperatureSensor";

        private const string DisplayName = "Building Temperature Sensor";
        public const string Description = "Can be rotated.";
        public const string Effect = "Measures the temperature of the building below the sensor.";

        private static string LogicPortDesc = "Building " + UI.FormatAsLink("Temperature", "HEAT");
        private static string LogicPortDescOn = "Sends an " + UI.FormatAsLink("Active", "LOGIC") + " signal while building " + UI.FormatAsLink("Temperature", "HEAT") + " is within its configured Temperature Threshold range";
        private static string LogicPortDescOff = "Sends an " + UI.FormatAsLink("Standby", "LOGIC") + " signal while building " + UI.FormatAsLink("Temperature", "HEAT") + " is outside its configured Temperature Threshold range";

        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), LogicPortDesc, LogicPortDescOn, LogicPortDescOff, false);


        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Automation", ID, LogicTemperatureSensorConfig.ID);
            AddBuilding.IntoTechTree("DupeTrafficControl", ID);
        }

        public static Color32 BuildingColor()
        {
            return new Color32(200, 75, 25, 255);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "switchthermal_kanim";
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
            buildingDef.Entombable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, BuildingTemperatureSensorConfig.ID);
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, BuildingTemperatureSensorConfig.OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, BuildingTemperatureSensorConfig.OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            GeneratedBuildings.RegisterLogicPorts(go, BuildingTemperatureSensorConfig.OUTPUT_PORT);

            BuildingTemperatureSensor logicTemperatureSensor = go.AddOrGet<BuildingTemperatureSensor>();
            logicTemperatureSensor.manuallyControlled = false;
            logicTemperatureSensor.minTemp = 0f;
            logicTemperatureSensor.maxTemp = 9999f + 273;
        }
    }
}

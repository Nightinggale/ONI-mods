using STRINGS;
using TUNING;
using UnityEngine;
using NightLib.AddBuilding;

namespace MoreTemperatureSensors
{
    class ConduitPressureSensorGas : IBuildingConfig
    {
        public const string ID = "Nightinggale.ConduitPressureSensorGas";

        public static void Setup()
        {
            LocString NAME = "Gas Pipe Pressure Sensor";

            LocString DESC = "Usage tips:\nAbove 0: detects if pipe contains anything.\nAbove 1000: detects full pipes.";

            LocString EFFECT = string.Concat(new string[]
            {
            "Becomes ",
            UI.FormatAsLink("Active", "LOGIC"),
            " or goes on ",
            UI.FormatAsLink("Standby", "LOGIC"),
            " when ",
            UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
            " pressure enters the chosen range."
            });


        AddBuilding.AddStrings(ID, NAME, DESC, EFFECT);

            AddBuilding.AddBuildingToPlanScreen("HVAC", ID);
            AddBuilding.IntoTechTree("HVAC", ID);
        }

        public static Color32 BuildingColor()
        {
            return new Color32(0, 255, 104, 255);
        }

        public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " Pressure";

        public static LocString LOGIC_PORT_ACTIVE = string.Concat(new string[]
        {
            "Sends an ",
            UI.FormatAsLink("Active", "LOGIC"),
            " when ",
            UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
            " pressure enters the chosen range."
        });

        public static LocString LOGIC_PORT_INACTIVE = string.Concat(new string[]
        {
            "Sends a ",
            UI.FormatAsLink("Standby", "LOGIC"),
            " when ",
            UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
            " pressure leaves the chosen range."
        });

        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), LOGIC_PORT, LOGIC_PORT_ACTIVE, LOGIC_PORT_INACTIVE, false);

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "gas_element_sensor_kanim";
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
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            ConduitPressureSensor sensor = go.AddOrGet<ConduitPressureSensor>();
            sensor.manuallyControlled = false;
            sensor.conduitType = ConduitType.Gas;
            sensor.color = BuildingColor();
        }
    }
}

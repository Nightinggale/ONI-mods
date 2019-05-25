using TUNING;
using UnityEngine;
using STRINGS;
using NightLib.AddBuilding;

namespace MoreTemperatureSensors
{
    public class SolidConduitElementSensorConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.SolidConduitElementSensor";

        public static void Setup()
        {
            LocString NAME = "Conveyor Rail Element Sensor";
            LocString DESC = "";
            LocString EFFECT = MakeEffect("element");

            AddBuilding.AddStrings(ID, NAME, DESC, EFFECT);

            AddBuilding.AddBuildingToPlanScreen("Conveyance", ID);
            AddBuilding.IntoTechTree("SolidTransport", ID);
        }

        public static readonly LogicPorts.Port OUTPUT_PORT = MakePort("Element");

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
            Filterable filterable = go.AddOrGet<Filterable>();
            filterable.filterElementState = Filterable.ElementState.Solid;

            SolidConduitElementSensor sensor = go.AddOrGet<SolidConduitElementSensor>();
            sensor.manuallyControlled = false;
            sensor.defaultState = false;
        }

        // string setup
        public static string MakeEffect(string name)
        {
            return string.Concat(new string[]
            {
            "Becomes ",
            UI.FormatAsLink("Active", "LOGIC"),
            " when the chosen ",
            UI.FormatAsLink("Solid", "ELEMENTS_SOLID"),
            " is detected."
            });
        }

        public static LogicPorts.Port MakePort(string name)
        {
            LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " Presence";

            LocString LOGIC_PORT_ACTIVE = string.Concat(new string[]
            {
            "Sends an ",
            UI.FormatAsLink("Active", "LOGIC"),
            " signal while the configured ",
            UI.FormatAsLink("Solid", "ELEMENTS_SOLID"),
            " is detected"
            });

            LocString LOGIC_PORT_INACTIVE = string.Concat(new string[]
            {
            "Sends a ",
            UI.FormatAsLink("Standby", "LOGIC"),
            " signal while the configured ",
            UI.FormatAsLink("Gas", "ELEMENTS_SOLID"),
            " is not detected"
            });

            return LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), LOGIC_PORT, LOGIC_PORT_ACTIVE, LOGIC_PORT_INACTIVE, false);
        }
    }
}

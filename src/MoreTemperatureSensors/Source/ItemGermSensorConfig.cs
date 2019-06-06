using STRINGS;
using TUNING;
using UnityEngine;
using NightLib.AddBuilding;

namespace MoreTemperatureSensors
{
    public class ItemGermSensorConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.ItemGermSensor";

        private const string DisplayName = "Item Germ Sensor";
        public const string Description = "Measurement is the germs on all items combined. This means the sensor can't detect if there is one germ free item in a pile of germy items.";
        public const string Effect = "Becomes <link=\"LOGIC\">Active</link> or goes on <link=\"LOGIC\">Standby</link> depending on quantity of <link=\"DISEASE\">Germs</link> on items in the cell.";
    
        private static string LogicPortDesc = "Item " + UI.FormatAsLink("Germs", "DISEASE");
        private static string LogicPortDescOn = "Sends an " + UI.FormatAsLink("Active", "LOGIC") + " signal while item " + UI.FormatAsLink("Germs", "DISEASE") + " is within its configured Germ Threshold range";
        private static string LogicPortDescOff = "Sends an " + UI.FormatAsLink("Standby", "LOGIC") + " signal while item " + UI.FormatAsLink("Germs", "DISEASE") + " is outside its configured Germ Threshold range";

        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), LogicPortDesc, LogicPortDescOn, LogicPortDescOff, false);


        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Automation", ID, LogicDiseaseSensorConfig.ID);
            AddBuilding.IntoTechTree("MedicineIII", ID);
        }

        public static Color32 BuildingColor()
        {
            return new Color32(0, 255, 104, 255);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "diseasesensor_kanim";
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
            SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
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
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);

            ItemGermSensor sensor = go.AddOrGet<ItemGermSensor>();
            sensor.Threshold = 0f;
            sensor.ActivateAboveThreshold = true;
            sensor.manuallyControlled = false;
        }
    }
}

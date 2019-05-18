using KSerialization;
using UnityEngine;
using NightLib;
using NightLib.AddBuilding;
using TUNING;
using System.Collections.Generic;
using STRINGS;


namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class TileTemperatureSensorConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.TileTemperatureSensor";

        private const string DisplayName = "Tile Temperature Sensor";
        public const string Description = "Works like a metal tile, but takes longer to construct and slows down walking speed.";
        public const string Effect = "Measures temperature of a metal tile.";

        private static string LogicPortDesc = "Tile " + UI.FormatAsLink("Temperature", "HEAT");
        private static string LogicPortDescOn = "Sends an " + UI.FormatAsLink("Active", "LOGIC") + " signal while tile " + UI.FormatAsLink("Temperature", "HEAT") + " is within its configured Temperature Threshold range";
        private static string LogicPortDescOff = "Sends an " + UI.FormatAsLink("Standby", "LOGIC") + " signal while tile " + UI.FormatAsLink("Temperature", "HEAT") + " is outside its configured Temperature Threshold range";

        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), LogicPortDesc, LogicPortDescOn, LogicPortDescOff, false);

        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Automation", ID, LogicTemperatureSensorConfig.ID);
            AddBuilding.IntoTechTree("Smelting", ID);
        }

        public static Color32 BuildingColor()
        {
            return new Color32(0, 255, 104, 255);
        }

        public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_metal_tops");

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "floor_metal_kanim";
            int hitpoints = 100;
            float construction_time = 90f;
            float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.Tile;
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.BONUS.TIER2, nONE, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.IsFoundation = true;
            buildingDef.TileLayer = ObjectLayer.FoundationTile;
            buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.isKAnimTile = true;
            buildingDef.isSolidTile = true;
            buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
            buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("tiles_metal");
            buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_metal_place");
            buildingDef.BlockTileShineAtlas = Assets.GetTextureAtlas("tiles_metal_spec");
            buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_info");
            buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_place_info");
            buildingDef.ReplacementTags = new List<Tag>();
            buildingDef.ReplacementTags.Add(GameTags.FloorTiles);
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.PENALTY_3;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            KAnimGridTileVisualizer kAnimGridTileVisualizer = go.AddOrGet<KAnimGridTileVisualizer>();
            kAnimGridTileVisualizer.blockTileConnectorID = MetalTileConfig.BlockTileConnectorID;
            BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
            buildingHP.destroyOnDamaged = true;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            GeneratedBuildings.RegisterLogicPorts(go, TileTemperatureSensorConfig.OUTPUT_PORT);
            LogicTemperatureSensor logicTemperatureSensor = go.AddOrGet<LogicTemperatureSensorNoAnim>();
            logicTemperatureSensor.manuallyControlled = false;
            logicTemperatureSensor.minTemp = 0f;
            logicTemperatureSensor.maxTemp = 9999f + 273;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<KAnimGridTileVisualizer>();
            GeneratedBuildings.RegisterLogicPorts(go, TileTemperatureSensorConfig.OUTPUT_PORT);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, TileTemperatureSensorConfig.OUTPUT_PORT);
        }
    }
}

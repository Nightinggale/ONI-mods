using TUNING;
using UnityEngine;
using NightLib;
using NightLib.AddBuilding;

namespace Nightinggale.HalfDoor
{

    public class HalfManualDoorConfig : ManualPressureDoorConfig
    {
        new public const string ID = "Nightinggale.HalfManualDoorConfig";

        public static void Setup()
        {
            AddBuilding.AddStrings(ID,
                "Half Manual Airlock",
                STRINGS.BUILDINGS.PREFABS.MANUALPRESSUREDOOR.DESC,
                STRINGS.BUILDINGS.PREFABS.MANUALPRESSUREDOOR.EFFECT);

            AddBuilding.AddBuildingToPlanScreen("Base", ID, ManualPressureDoorConfig.ID);
            AddBuilding.IntoTechTree("PressureManagement", ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "door_manual_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NONE, 1f);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.IsFoundation = true;
            buildingDef.TileLayer = ObjectLayer.FoundationTile;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.R90;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
            SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_gear_LP", NOISE_POLLUTION.NOISY.TIER1);
            SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_open", NOISE_POLLUTION.NOISY.TIER2);
            SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_close", NOISE_POLLUTION.NOISY.TIER2);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);
            go.AddOrGet<Workable>().workTime = 3f;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            go.AddComponent<KAminControllerResize>().height = 0.5f;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            go.AddComponent<KAminControllerResize>().height = 0.5f;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            base.DoPostConfigureComplete(go);
            go.AddComponent<KAminControllerResize>().height = 0.5f;
        }
    }
}
using TUNING;
using UnityEngine;
using NightLib;
using NightLib.AddBuilding;

namespace Nightinggale.HalfDoor
{

    public class HalfDoorConfig : PressureDoorConfig
    {
        public const string ID = "Nightinggale.HalfDoor";

        public static void Setup()
        {
            AddBuilding.AddStrings(ID,
                "Half Mechanized Airlock",
                STRINGS.BUILDINGS.PREFABS.PRESSUREDOOR.DESC,
                STRINGS.BUILDINGS.PREFABS.PRESSUREDOOR.EFFECT);

            AddBuilding.AddBuildingToPlanScreen("Base", ID, PressureDoorConfig.ID);
            AddBuilding.IntoTechTree("DirectedAirStreams", ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "door_external_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] aLL_METALS = MATERIALS.ALL_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Tile;
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, nONE, 1f);
            buildingDef.Overheatable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 60f;
            buildingDef.Entombable = false;
            buildingDef.IsFoundation = true;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.TileLayer = ObjectLayer.FoundationTile;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.R90;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
            SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Open_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
            SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Close_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
            return buildingDef;
        }
        
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddComponent<KAminControllerResize>().height = 0.5f;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddComponent<KAminControllerResize>().height = 0.5f;
        }
        

        public override void DoPostConfigureComplete(GameObject go)
        {
            base.DoPostConfigureComplete(go);
            go.AddComponent<KAminControllerResize>().height = 0.5f;
        }
    }
}

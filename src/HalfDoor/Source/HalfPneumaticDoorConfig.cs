using TUNING;
using UnityEngine;
using NightLib;
using NightLib.AddBuilding;

namespace Nightinggale.HalfDoor
{

    public class HalfPneumaticDoorConfig : DoorConfig
    {
        new public const string ID = "Nightinggale.HalfPneumaticDoorConfig";

        public static void Setup()
        {
            AddBuilding.AddStrings(ID,
                "Half Pneumatic Door",
                STRINGS.BUILDINGS.PREFABS.DOOR.DESC,
                STRINGS.BUILDINGS.PREFABS.DOOR.EFFECT);

            AddBuilding.AddBuildingToPlanScreen("Base", ID, DoorConfig.ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "door_internal_kanim";
            int hitpoints = 30;
            float construction_time = 5f;
            float[] tieR1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
            string[] allMetals = MATERIALS.ALL_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Tile;
            EffectorValues none = TUNING.NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tieR1, allMetals, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, none, 1f);
            buildingDef.Entombable = true;
            buildingDef.Floodable = false;
            buildingDef.IsFoundation = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.R90;
            buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
            SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", TUNING.NOISE_POLLUTION.NOISY.TIER2);
            SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", TUNING.NOISE_POLLUTION.NOISY.TIER2);
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
            go.AddOrGet<Workable>().workTime = 2f;
        }
    }
}
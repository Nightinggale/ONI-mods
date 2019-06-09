using KSerialization;
using UnityEngine;
using NightLib.AddBuilding;
using TUNING;

namespace BlackHoleGarbageDisposal
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class BlackHoleGarbageDisposalConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.BlackHoleGarbageDisposal";

        private const string DisplayName = "Artificial micro black hole";
        public const string Description = "The duplicants knew they made a mistake when they started using black holes. Casper became way too interested.";
        public const string Effect = "Drop your garbage here and you will never see it again.";


        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Base", ID, StorageLockerConfig.ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 2;
            string anim = "storagelocker_kanim";
            int hitpoints = 30;
            float construction_time = 10f;
            float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.allowItemRemoval = false;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
            storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
            copyBuildingSettings.copyGroupTag = GameTags.StorageLocker;
            go.AddOrGet<StorageLocker>();
            go.AddOrGet<BlackHoleGarbageDisposalComponent>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}

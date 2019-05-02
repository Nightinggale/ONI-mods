using UnityEngine;
using TUNING;

namespace PipePressureValve
{
    public abstract class PipePressureValveConfig : IBuildingConfig
    {
        public abstract float GetThreshold();
        public abstract float GetMaxStorage();
        public abstract float GetPipeCapacity();

        protected BuildingDef CreateBuildingDef(ConduitType conduitType, string anim, string ID)
        {
            int width = 1;
            int height = 2;
            int hitpoints = 100;
            float construction_time = 120f;
            float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] rAW_METALS = MATERIALS.ALL_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues decor = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, decor, noise, 0.2f);

            buildingDef.InputConduitType = conduitType;
            buildingDef.OutputConduitType = conduitType;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = conduitType == ConduitType.Liquid ? OverlayModes.LiquidConduits.ID : OverlayModes.GasConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.PowerInputOffset = new CellOffset(0, 1);
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
            buildingDef.EnergyConsumptionWhenActive = 10f;
            buildingDef.RequiresPowerInput = true;
            buildingDef.PermittedRotations = PermittedRotations.R360;

            return buildingDef;
        }

        protected void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag, ConduitType conduitType)
        {
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showDescriptor = false;
            storage.allowItemRemoval = false;
            storage.storageFilters = conduitType == ConduitType.Liquid ? STORAGEFILTERS.LIQUIDS : STORAGEFILTERS.GASES;
            storage.capacityKg = GetMaxStorage();

            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = conduitType;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = GetMaxStorage();
            PressureValveConduitDispenser conduitDispenser = go.AddOrGet<PressureValveConduitDispenser>();
            conduitDispenser.conduitType = conduitType;
            conduitDispenser.elementFilter = null;
            conduitDispenser.outputThreshold = GetThreshold();
            conduitDispenser.pipeCapacity = GetPipeCapacity();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}

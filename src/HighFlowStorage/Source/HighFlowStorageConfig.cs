using KSerialization;
using UnityEngine;
using NightLib;
using NightLib.AddBuilding;
using System;
using TUNING;
using UnityEngine;


namespace HighFlowStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class HighFlowLiquidReservoirConfig : IBuildingConfig
    {
        new public const string ID = "Nightinggale.HighFlowLiquidReservoir";

        private const string DisplayName = "High Flow Liquid Reservoir";
        public const string Description = "";
        public const string Effect = "For people where one pipe just isn't enough.";

        private static readonly PortDisplayLiquidInput inputPort0 = new PortDisplayLiquidInput(new CellOffset(1, 0), Color.white);
        private static readonly PortDisplayLiquidInput inputPort1 = new PortDisplayLiquidInput(new CellOffset(1, 1), Color.white);
        private static readonly PortDisplayLiquidInput inputPort2 = new PortDisplayLiquidInput(new CellOffset(1, 2), Color.white);

        private static readonly PortDisplayLiquidOutput outputPort0 = new PortDisplayLiquidOutput(new CellOffset(0, 0));
        private static readonly PortDisplayLiquidOutput outputPort1 = new PortDisplayLiquidOutput(new CellOffset(0, 1));
        private static readonly PortDisplayLiquidOutput outputPort2 = new PortDisplayLiquidOutput(new CellOffset(0, 2));

        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Base", ID, LiquidReservoirConfig.ID);
            AddBuilding.IntoTechTree("ImprovedLiquidPiping", ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 2;
            int height = 3;
            string anim = "liquidreservoir_kanim";
            int hitpoints = 100;
            float construction_time = 240f;
            float[] mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            string[] materials = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues decor = BUILDINGS.DECOR.PENALTY.TIER1; 
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER0;
            float temperature_modification_mass_scale = 0.2f;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, mass, materials, melting_point, build_location_rule, decor, noise, temperature_modification_mass_scale);

            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<Reservoir>();
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showDescriptor = true;
            storage.allowItemRemoval = false;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.capacityKg = 5000f;
            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);

            PortConduitDispenser conduitDispenser0 = go.AddComponent<PortConduitDispenser>();
            conduitDispenser0.AssignPort(outputPort0);

            PortConduitDispenser conduitDispenser1 = go.AddComponent<PortConduitDispenser>();
            conduitDispenser1.AssignPort(outputPort1);

            PortConduitDispenser conduitDispenser2 = go.AddComponent<PortConduitDispenser>();
            conduitDispenser2.AssignPort(outputPort2);

            PortConduitConsumer consumer0 = go.AddComponent<PortConduitConsumer>();
            consumer0.conduitType = ConduitType.Liquid;
            consumer0.ignoreMinMassCheck = true;
            consumer0.forceAlwaysSatisfied = true;
            consumer0.alwaysConsume = true;
            consumer0.capacityKG = storage.capacityKg;
            consumer0.AssignPort(inputPort0);

            PortConduitConsumer consumer1 = go.AddComponent<PortConduitConsumer>();
            consumer1.conduitType = ConduitType.Liquid;
            consumer1.ignoreMinMassCheck = true;
            consumer1.forceAlwaysSatisfied = true;
            consumer1.alwaysConsume = true;
            consumer1.capacityKG = storage.capacityKg;
            consumer1.AssignPort(inputPort1);

            PortConduitConsumer consumer2 = go.AddComponent<PortConduitConsumer>();
            consumer2.conduitType = ConduitType.Liquid;
            consumer2.ignoreMinMassCheck = true;
            consumer2.forceAlwaysSatisfied = true;
            consumer2.alwaysConsume = true;
            consumer2.capacityKG = storage.capacityKg;
            consumer2.AssignPort(inputPort2);

            this.AttachPort(go);
        }

        private void AttachPort(GameObject go)
        {
            go.AddComponent<PortDisplayLiquid>().AssignPort(ID, outputPort0);
            go.AddComponent<PortDisplayLiquid>().AssignPort(ID, outputPort1);
            go.AddComponent<PortDisplayLiquid>().AssignPort(ID, outputPort2);
            go.AddComponent<PortDisplayLiquid>().AssignPort(ID, inputPort0);
            go.AddComponent<PortDisplayLiquid>().AssignPort(ID, inputPort1);
            go.AddComponent<PortDisplayLiquid>().AssignPort(ID, inputPort2);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            this.AttachPort(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            this.AttachPort(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}

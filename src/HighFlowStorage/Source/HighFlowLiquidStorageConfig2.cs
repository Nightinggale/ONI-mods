using KSerialization;
using UnityEngine;
using NightLib;
using NightLib.AddBuilding;
using TUNING;

namespace HighFlowStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class HighFlowLiquidReservoirConfig2 : IBuildingConfig
    {
        public const string ID = "Nightinggale.HighFlowLiquidReservoir2";

        private const string DisplayName = "High Flow Liquid Reservoir";
        public const string Description = "";
        public const string Effect = "For people where one pipe just isn't enough.";

        private static readonly PortDisplayInput inputPort0 = new PortDisplayInput(ConduitType.Liquid, new CellOffset(1, 0));
        private static readonly PortDisplayInput inputPort1 = new PortDisplayInput(ConduitType.Liquid, new CellOffset(1, 1));
        private static readonly PortDisplayInput inputPort2 = new PortDisplayInput(ConduitType.Liquid, new CellOffset(1, 2));

        private static readonly PortDisplayOutput outputPort0 = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 0));
        private static readonly PortDisplayOutput outputPort1 = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 1));
        private static readonly PortDisplayOutput outputPort2 = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 2));

        public static readonly LogicPorts.Port OUTPUT_PORT = ReservoirStorageSensor.MakePort();

        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Base", ID, LiquidReservoirConfig.ID);
            AddBuilding.IntoTechTree("ImprovedLiquidPiping", ID);
        }

        public static Color32 BuildingColor()
        {
            return new Color32(0, 255, 104, 255);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 2;
            int height = 3;
            string anim = "liquidreservoir_kanim";
            int hitpoints = 100;
            float construction_time = HighFlowStorageConfig.Config.liquidStorageConstructionTime;
            float[] mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            mass[0] = HighFlowStorageConfig.Config.liquidStorageMetalCost;
            string[] materials = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues decor = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER0;
            float temperature_modification_mass_scale = 0.2f;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, mass, materials, melting_point, build_location_rule, decor, noise, temperature_modification_mass_scale);

            buildingDef.Overheatable = false;
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
            storage.capacityKg = HighFlowStorageConfig.Config.liquidStorageCapacity;
            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);

            PortConduitDispenser conduitDispenser0 = go.AddComponent<HighFlowStorage_PortConduitDispenser>();
            conduitDispenser0.AssignPort(outputPort0);

            PortConduitDispenser conduitDispenser1 = go.AddComponent<HighFlowStorage_PortConduitDispenser>();
            conduitDispenser1.AssignPort(outputPort1);

            PortConduitDispenser conduitDispenser2 = go.AddComponent<HighFlowStorage_PortConduitDispenser>();
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
            PortDisplayController controller = go.AddComponent<PortDisplayController>();
            controller.Init(go);

            controller.AssignPort(go, outputPort0);
            controller.AssignPort(go, outputPort1);
            controller.AssignPort(go, outputPort2);
            controller.AssignPort(go, inputPort0);
            controller.AssignPort(go, inputPort1);
            controller.AssignPort(go, inputPort2);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            this.AttachPort(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            this.AttachPort(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            go.AddOrGetDef<StorageController.Def>();
            go.AddComponent<ReservoirStorageSensor>();
        }
    }
}

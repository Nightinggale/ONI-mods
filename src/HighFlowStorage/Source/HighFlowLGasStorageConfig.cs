using KSerialization;
using UnityEngine;
using NightLib;
using NightLib.AddBuilding;
using TUNING;
using System.Collections.Generic;

namespace HighFlowStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class HighFlowGasReservoirConfig : IBuildingConfig
    {
        new public const string ID = "Nightinggale.HighFlowGasReservoir";

        private const string DisplayName = "High Flow Gas Reservoir";
        public const string Description = "";
        public const string Effect = "For people where one pipe just isn't enough.";

        private static readonly PortDisplayGasInput inputPort0 = new PortDisplayGasInput(new CellOffset(2, 0));
        private static readonly PortDisplayGasInput inputPort1 = new PortDisplayGasInput(new CellOffset(2, 1));
        private static readonly PortDisplayGasInput inputPort2 = new PortDisplayGasInput(new CellOffset(2, 2));

        private static readonly PortDisplayGasOutput outputPort0 = new PortDisplayGasOutput(new CellOffset(-2, 0));
        private static readonly PortDisplayGasOutput outputPort1 = new PortDisplayGasOutput(new CellOffset(-2, 1));
        private static readonly PortDisplayGasOutput outputPort2 = new PortDisplayGasOutput(new CellOffset(-2, 2));

        private static readonly PortDisplayGasInput[] inputPorts = { inputPort0, inputPort1, inputPort2, };

        public static void Setup()
        {
            AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            AddBuilding.AddBuildingToPlanScreen("Base", ID, GasReservoirConfig.ID);
            AddBuilding.IntoTechTree("HVAC", ID);
        }

        public static Color32 BuildingColor()
        {
            return new Color32(0, 255, 104, 255);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 5;
            int height = 3;
            string anim = "gasstorage_kanim";
            int hitpoints = 100;
            float construction_time = HighFlowStorageConfig.Config.Gas3StorageConstructionTime;
            float[] mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            mass[0] = HighFlowStorageConfig.Config.Gas3StorageMetalCost;
            string[] materials = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues decor = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER0;
            float temperature_modification_mass_scale = 0.2f;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, mass, materials, melting_point, build_location_rule, decor, noise, temperature_modification_mass_scale);

            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
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
            storage.storageFilters = STORAGEFILTERS.GASES;
            storage.capacityKg = HighFlowStorageConfig.Config.Gas3StorageCapacity;
            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);

            PortConduitDispenser conduitDispenser0 = go.AddComponent<PortConduitDispenser>();
            conduitDispenser0.AssignPort(outputPort0);

            PortConduitDispenser conduitDispenser1 = go.AddComponent<PortConduitDispenser>();
            conduitDispenser1.AssignPort(outputPort1);

            PortConduitDispenser conduitDispenser2 = go.AddComponent<PortConduitDispenser>();
            conduitDispenser2.AssignPort(outputPort2);

            foreach (PortDisplayGasInput port in inputPorts)
            {
                PortConduitConsumer consumer = go.AddComponent<PortConduitConsumer>();
                consumer.ignoreMinMassCheck = true;
                consumer.forceAlwaysSatisfied = true;
                consumer.alwaysConsume = true;
                consumer.capacityKG = storage.capacityKg;
                consumer.AssignPort(port);
            }
            this.AttachPort(go);
        }

        private void AttachPort(GameObject go)
        {
            go.AddComponent<PortDisplayGas>().AssignPort(ID, outputPort0);
            go.AddComponent<PortDisplayGas>().AssignPort(ID, outputPort1);
            go.AddComponent<PortDisplayGas>().AssignPort(ID, outputPort2);
            go.AddComponent<PortDisplayGas>().AssignPort(ID, inputPort0);
            go.AddComponent<PortDisplayGas>().AssignPort(ID, inputPort1);
            go.AddComponent<PortDisplayGas>().AssignPort(ID, inputPort2);
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

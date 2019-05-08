using KSerialization;
using UnityEngine;
using System;
using NightLib;

namespace HighFlowStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class HighFlowLiquidReservoirConfig : LiquidReservoirConfig
    {
        new public const string ID = "HighFlowLiquidReservoir";

        private const string DisplayName = "High Flow Liquid Reservoir";
        public static string Description = "";
        public const string Effect = "For people where one pipe isn't enough.";

        private PortDisplayLiquidInput inputPort1 = new PortDisplayLiquidInput(new CellOffset(1, 1));
        private PortDisplayLiquidInput inputPort2 = new PortDisplayLiquidInput(new CellOffset(1, 2));

        private PortDisplayLiquidOutput outputPort1 = new PortDisplayLiquidOutput(new CellOffset(0, 1));
        private PortDisplayLiquidOutput outputPort2 = new PortDisplayLiquidOutput(new CellOffset(0, 2));

        public static void Setup()
        {
            NightLib.AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            NightLib.AddBuilding.AddBuildingToPlanScreen("Base", ID, LadderConfig.ID);// LiquidReservoirConfig.ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = base.CreateBuildingDef();
            buildingDef.PrefabID = ID;
            buildingDef.InitDef();

            buildingDef.UtilityInputOffset = new CellOffset(1, 0);
            if (buildingDef.PermittedRotations == PermittedRotations.Unrotatable)
            {
                buildingDef.PermittedRotations = PermittedRotations.FlipH;
            }

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);

            Storage storage = go.GetComponent<Storage>();

            NightLib.ConduitDispenser1 conduitDispenser1 = go.AddOrGet<NightLib.ConduitDispenser1>();
            conduitDispenser1.AssignPort(this.outputPort1);

            NightLib.ConduitDispenser2 conduitDispenser2 = go.AddOrGet<NightLib.ConduitDispenser2>();
            conduitDispenser2.AssignPort(this.outputPort2);

            NightLib.ConduitConsumer1 consumer1 = go.AddOrGet<NightLib.ConduitConsumer1>();
            consumer1.conduitType = ConduitType.Liquid;
            consumer1.ignoreMinMassCheck = true;
            consumer1.forceAlwaysSatisfied = true;
            consumer1.alwaysConsume = true;
            consumer1.capacityKG = storage.capacityKg;
            consumer1.AssignPort(this.inputPort1);

            NightLib.ConduitConsumer2 consumer2 = go.AddOrGet<NightLib.ConduitConsumer2>();
            consumer2.conduitType = ConduitType.Liquid;
            consumer2.ignoreMinMassCheck = true;
            consumer2.forceAlwaysSatisfied = true;
            consumer2.alwaysConsume = true;
            consumer2.capacityKG = storage.capacityKg;
            consumer2.AssignPort(this.inputPort2);
        }

        private void AttachPort(GameObject go)
        {
            go.AddComponent<PortDisplayLiquid>().AssignPort(this.outputPort1);
            go.AddComponent<PortDisplayLiquid>().AssignPort(this.outputPort2);
            go.AddComponent<PortDisplayLiquid>().AssignPort(this.inputPort1);
            go.AddComponent<PortDisplayLiquid>().AssignPort(this.inputPort2);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            this.AttachPort(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            this.AttachPort(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            base.DoPostConfigureComplete(go);
            this.AttachPort(go);
        }
    }
}

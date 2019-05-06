using KSerialization;
using UnityEngine;
using System;

namespace HighFlowStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class HighFlowLiquidReservoirConfig : LiquidReservoirConfig
    {
        new public const string ID = "HighFlowLiquidReservoir";

        private const string DisplayName = "High Flow Liquid Reservoir";
        public static string Description = "";
        public const string Effect = "For people where one pipe isn't enough.";

        private ConduitPortInfo inputPort1 = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(1, 1));
        private ConduitPortInfo inputPort2 = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(1, 2));

        private ConduitPortInfo outputPort1 = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));
        private ConduitPortInfo outputPort2 = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 2));

        public static void Setup()
        {
            NightLib.AddBuilding.AddStrings(ID, DisplayName, Description, Effect);

            NightLib.AddBuilding.AddBuildingToPlanScreen("Base", ID, LiquidReservoirConfig.ID);
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
            conduitDispenser1.conduitType = ConduitType.Liquid;
            conduitDispenser1.elementFilter = null;
            conduitDispenser1.portInfo = outputPort1;

            NightLib.ConduitDispenser2 conduitDispenser2 = go.AddOrGet<NightLib.ConduitDispenser2>();
            conduitDispenser2.conduitType = ConduitType.Liquid;
            conduitDispenser2.elementFilter = null;
            conduitDispenser2.portInfo = outputPort2;

            NightLib.ConduitConsumer1 consumer1 = go.AddOrGet<NightLib.ConduitConsumer1>();
            consumer1.conduitType = ConduitType.Liquid;
            consumer1.ignoreMinMassCheck = true;
            consumer1.forceAlwaysSatisfied = true;
            consumer1.alwaysConsume = true;
            consumer1.capacityKG = storage.capacityKg;
            consumer1.portInfo = this.inputPort1;

            NightLib.ConduitConsumer2 consumer2 = go.AddOrGet<NightLib.ConduitConsumer2>();
            consumer2.conduitType = ConduitType.Liquid;
            consumer2.ignoreMinMassCheck = true;
            consumer2.forceAlwaysSatisfied = true;
            consumer2.alwaysConsume = true;
            consumer2.capacityKG = storage.capacityKg;
            consumer2.portInfo = this.inputPort2;
        }

        private void AttachPort(GameObject go)
        {
            NightLib.PortDisplay output1 = go.AddComponent<NightLib.PortDisplay1>();
            NightLib.PortDisplay output2 = go.AddComponent<NightLib.PortDisplay2>();
            output1.portInfo = this.outputPort1;
            output2.portInfo = this.outputPort2;

            NightLib.PortDisplay input1 = go.AddComponent<NightLib.PortDisplay3>();
            NightLib.PortDisplay input2 = go.AddComponent<NightLib.PortDisplay4>();
            input1.portInfo = this.inputPort1;
            input2.portInfo = this.inputPort2;
            input1.input = true;
            input2.input = true;
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

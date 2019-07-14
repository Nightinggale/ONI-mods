using TUNING;
using UnityEngine;
using NightLib.AddBuilding;

namespace Nightinggale.CoalGenerator

{
    public class CoalGeneratorConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.CoalGenerator";

        private const float COAL_BURN_RATE = 1f;

        private const float COAL_CAPACITY = 600f;

        public static void Setup()
        {
            AddBuilding.AddStrings(ID,
                STRINGS.BUILDINGS.PREFABS.GENERATOR.NAME,
                STRINGS.BUILDINGS.PREFABS.GENERATOR.DESC,
                STRINGS.BUILDINGS.PREFABS.GENERATOR.EFFECT);

            AddBuilding.ReplaceBuildingInPlanScreen("Power", ID, GeneratorConfig.ID);
            AddBuilding.ReplaceInTechTree("Combustion", ID, GeneratorConfig.ID);
        }

        public override BuildingDef CreateBuildingDef()
        {
            int width = 3;
            int height = 3;
            string anim = "generatorphos_kanim";
            int hitpoints = 100;
            float construction_time = 120f;
            float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            string[] aLL_METALS = MATERIALS.ALL_METALS;
            float melting_point = 2400f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tIER2, 0.2f);
            buildingDef.GeneratorWattageRating = 600f;
            buildingDef.GeneratorBaseCapacity = 20000f;
            buildingDef.ExhaustKilowattsWhenActive = 8f;
            buildingDef.SelfHeatKilowattsWhenActive = 1f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            CoalEnergyGenerator energyGenerator = go.AddOrGet<CoalEnergyGenerator>();
            energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(SimHashes.Carbon.CreateTag(), 1f, 600f, SimHashes.Void, 0f, true);
            energyGenerator.powerDistributionOrder = 9;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 600f;
            go.AddOrGet<LoopingSounds>();
            Prioritizable.AddRef(go);
            CoalManualDeliveryKG manualDeliveryKG = go.AddOrGet<CoalManualDeliveryKG>();
            manualDeliveryKG.SetStorage(storage);
            manualDeliveryKG.requestedItemTag = new Tag("Coal");
            manualDeliveryKG.ignoresOperationStatus = true;
            manualDeliveryKG.capacity = storage.capacityKg;
            manualDeliveryKG.refillMass = 100f;
            manualDeliveryKG.choreTags = new Tag[]
            {
            GameTags.ChoreTypes.Power
            };
            manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;
            
            BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
            buildingElementEmitter.emitRate = 0.02f;
            buildingElementEmitter.temperature = 310f;
            buildingElementEmitter.element = SimHashes.CarbonDioxide;
            buildingElementEmitter.modifierOffset = new Vector2(1f, 2f);

            DualSlider dualSlider = go.AddOrGet<DualSlider>();
            dualSlider.fillUptoThreshold = manualDeliveryKG.capacity;
            dualSlider.refillThreshold = manualDeliveryKG.refillMass;

            CoalDeliveryController controller = go.AddOrGet<CoalDeliveryController>();
            controller.batteryRefillPercent = 0.5f;

            Tinkerable.MakePowerTinkerable(go);
        }
        

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}

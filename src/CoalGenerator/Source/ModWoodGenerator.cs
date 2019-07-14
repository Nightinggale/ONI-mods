using Harmony;
using UnityEngine;

namespace Nightinggale.CoalGenerator
{
    [HarmonyPatch(typeof(WoodGasGeneratorConfig))]
    [HarmonyPatch("DoPostConfigureComplete")]
    public static class WoodBurnerPatch
    {
        public static void Postfix(GameObject go)
        {
            EnergyGenerator origEnergyGenerator = go.GetComponent<EnergyGenerator>();
            Storage storage = go.GetComponent<Storage>();
            ManualDeliveryKG origManualDeliveryKG = go.GetComponent<ManualDeliveryKG>();

            if (storage.capacityKg == 20000)
            {
                // the storage haven't been set. The sliders rely on it for max value. Set it to the max storage from other components.
                storage.capacityKg = Mathf.Max(origManualDeliveryKG.capacity, origEnergyGenerator.formula.inputs[0].maxStoredMass);
            }

            CoalEnergyGenerator energyGenerator = go.AddOrGet<CoalEnergyGenerator>();
            energyGenerator.powerDistributionOrder = origEnergyGenerator.powerDistributionOrder;
            energyGenerator.hasMeter = true;
            energyGenerator.formula = origEnergyGenerator.formula;

            CoalManualDeliveryKG manualDeliveryKG = go.AddOrGet<CoalManualDeliveryKG>();
            manualDeliveryKG.SetStorage(storage);
            manualDeliveryKG.requestedItemTag = origManualDeliveryKG.requestedItemTag;
            manualDeliveryKG.ignoresOperationStatus = true;
            manualDeliveryKG.capacity = origManualDeliveryKG.capacity;
            manualDeliveryKG.refillMass = origManualDeliveryKG.refillMass;
            manualDeliveryKG.choreTags = new Tag[]
            {
                    GameTags.ChoreTypes.Power
            };
            manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

            DualSlider dualSlider = go.AddOrGet<DualSlider>();
            dualSlider.fillUptoThreshold = manualDeliveryKG.capacity;
            dualSlider.refillThreshold = manualDeliveryKG.refillMass;

            CoalDeliveryController controller = go.AddOrGet<CoalDeliveryController>();
            controller.batteryRefillPercent = 0.5f;

            AddStrings.AddString(manualDeliveryKG.RequestedItemTag.Name);

            UnityEngine.Object.DestroyImmediate(origEnergyGenerator);
            UnityEngine.Object.DestroyImmediate(origManualDeliveryKG);
        }
    }
}

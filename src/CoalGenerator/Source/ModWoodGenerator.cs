using UnityEngine;

namespace Nightinggale.CoalGenerator
{
    public static class ApplyCoalBurnerFixes
    {
        public static void Apply(GameObject go)
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
            energyGenerator.ignoreBatteryRefillPercent = true;

            CoalManualDeliveryKG manualDeliveryKG = go.AddOrGet<CoalManualDeliveryKG>();
            manualDeliveryKG.SetStorage(storage);
            manualDeliveryKG.requestedItemTag = origManualDeliveryKG.requestedItemTag;
            manualDeliveryKG.ignoresOperationStatus = true;
            manualDeliveryKG.capacity = origManualDeliveryKG.capacity;
            manualDeliveryKG.refillMass = origManualDeliveryKG.refillMass;
            manualDeliveryKG.requestedItemTag = origManualDeliveryKG.requestedItemTag;
            manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

            DualSlider dualSlider = go.AddOrGet<DualSlider>();
            dualSlider.fillUptoThreshold = manualDeliveryKG.capacity;
            dualSlider.refillThreshold = manualDeliveryKG.refillMass;

            UnityEngine.Object.DestroyImmediate(origEnergyGenerator);
            UnityEngine.Object.DestroyImmediate(origManualDeliveryKG);
        }
    }
}

using KSerialization;
using STRINGS;
using System;
using UnityEngine;
using NightLib;
using System.Collections.Generic;

namespace Nightinggale.CoalGenerator
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class CoalManualDeliveryKG : ManualDeliveryKG
    {
        [MyCmpGet]
        private Operational operational;

        [SerializeField]
        public bool ignoresOperationStatus;

        public void UpdateCapacity(float capacity, float refill)
        {
            if (this.capacity > capacity)
            {
                this.AbortDelivery("Threshold lowered");
            }

            this.capacity = capacity;
            this.refillMass = refill;
            this.UpdateDeliveryState();
        }

        public bool OperationalRequirementsMet()
        {
            return IsFunctional();
        }

        private bool IsFunctional()
        {
            foreach (KeyValuePair<Operational.Flag, bool> kvp in operational.Flags)
            {
                if (kvp.Key.FlagType == Operational.Flag.Type.Functional && !kvp.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

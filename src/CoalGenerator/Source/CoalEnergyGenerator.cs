using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using NightLib;

namespace Nightinggale.CoalGenerator
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class CoalEnergyGenerator : EnergyGenerator
    {
        [MyCmpGet]
        private ManualDeliveryKG delivery;

        private int cell;

        private static readonly Operational.Flag batteryFlag = new Operational.Flag("lowBattery", Operational.Flag.Type.Requirement);

        private bool IsAutomationConnected()
        {
            GameObject gameObject = Grid.Objects[this.cell, (int)ObjectLayer.LogicWire];
            return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Building building = base.GetComponent<Building>();
            this.cell = building.NaturalBuildingCell();
        }

        public override void EnergySim200ms(float dt)
        {
            bool automation = IsAutomationConnected();

            if (!automation && operational.IsFunctional)
            {
                ushort circuitID = base.CircuitID;

                float charge = 0f;
                float capacity = 0f;

                if (circuitID != UInt16.MaxValue)
                {
                    List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);

                    foreach (Battery current in batteriesOnCircuit)
                    {
                        if (current.isActiveAndEnabled)
                        {
                            charge += current.JoulesAvailable;
                            capacity += current.capacity;
                        }
                    }
                }

 
                if (automation || capacity == 0 || ((GetSliderMax(0) * charge) / capacity) <= GetSliderValue(0))
                {
                    operational.SetFlag(batteryFlag, true);
                }
                else if (charge == capacity)
                {
                    operational.SetFlag(batteryFlag, false);
                }
            }

            base.EnergySim200ms(dt);

            delivery.Pause(false, "pause override");

        }
    }
}

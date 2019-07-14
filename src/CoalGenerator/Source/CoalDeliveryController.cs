using System;
using System.Collections.Generic;
using STRINGS;
using KSerialization;
using UnityEngine;
using NightLib;

namespace Nightinggale.CoalGenerator
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class CoalDeliveryController : KMonoBehaviour, IIntSliderControl, ISim1000ms, ISaveLoadable
    {
        [MyCmpGet]
        private CoalManualDeliveryKG delivery;

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;

        [Serialize, SerializeField]
        public float batteryRefillPercent;

        private int cell;

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            CoalDeliveryController component = gameObject.GetComponent<CoalDeliveryController>();
            if (component != null)
            {
                this.batteryRefillPercent = component.batteryRefillPercent;
                this.Sim1000ms(0);
            }
        }

        private bool IsAutomationConnected()
        {
            GameObject gameObject = Grid.Objects[this.cell, (int)ObjectLayer.LogicWires];
            return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
        }

        private bool IsLowBattery()
        {
            ushort circuitID = Game.Instance.circuitManager.GetCircuitID(this.cell);

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

            if (capacity == 0f)
            {
                // always run generator if no batteries are found.
                return true;
            }

            float percentage = charge / capacity;
            return percentage < this.batteryRefillPercent;
            
        }
        
        public void Sim1000ms(float dt)
        {
            // use lazy compilation to skip the much slower battery test if possible.
            bool flag = this.IsAutomationConnected() || IsLowBattery();

            this.delivery.Pause(!flag, "Circuit has sufficient energy");
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe(GameHashes.CopySettings, new Action<object>(this.OnCopySettings));
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Building building = base.GetComponent<Building>();
            this.cell = building.NaturalBuildingCell();
        }

        public string SliderTitleKey
        {
            get
            {
                return "STRINGS.BUILDINGS.NIGHTINGGALE.COALGENERATOR.TITLE." + delivery.RequestedItemTag.Name;
            }
        }

        public string SliderUnits
        {
            get
            {
                return UI.UNITSUFFIXES.PERCENT;
            }
        }

        public int SliderDecimalPlaces(int index)
        {
            return 0;
        }

        public float GetSliderMin(int index)
        {
            return 0f;
        }

        public float GetSliderMax(int index)
        {
            return 100f;
        }

        public float GetSliderValue(int index)
        {
            return this.batteryRefillPercent * 100f;
        }

        public void SetSliderValue(float value, int index)
        {
            this.batteryRefillPercent = value / 100f;
            // update pause state
            this.Sim1000ms(0);
        }

        public string GetSliderTooltipKey(int index)
        {
            return "STRINGS.BUILDINGS.NIGHTINGGALE.COALGENERATOR.BATTERYTHRESHOLD." + delivery.RequestedItemTag.Name;
        }
    }
}

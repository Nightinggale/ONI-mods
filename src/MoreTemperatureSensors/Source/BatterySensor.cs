using System.Collections.Generic;
using KSerialization;
using STRINGS;
using System;
using UnityEngine;
using NightLib;
using NightLib.OnOverlayChange;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class BatterySensor : LogicSwitch, ISaveLoadable, IActivationRangeTarget, ISim200ms, IOverlayChangeEvent
    {
        [Serialize]
        private int activateValue;

        [Serialize]
        private int deactivateValue = 100;

        private int cell;

        private float updateInterval = 0f;
        private float timeSinceLastUpdate = 1000f;

        private float charge;
        private float capacity;

        [MyCmpGet]
        private KBatchedAnimController animController;

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            BatterySensor component = gameObject.GetComponent<BatterySensor>();
            if (component != null)
            {
                this.activateValue = component.activateValue;
                this.deactivateValue = component.deactivateValue;
            }
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe(GameHashes.CopySettings, new Action<object>(this.OnCopySettings));
        }

        protected override void OnSpawn()
        {
            this.cell = base.GetComponent<Building>().GetCellWithOffset(new CellOffset(0, 0));
            base.OnSpawn();

            base.OnToggle += new Action<bool>(this.OnSwitchToggled);

            this.updateInterval = MoreTemperatureSensorsConfig.Config.GetBatteryInterval;

            // Apply color
            this.OnOverlayChange("");
            OverlayChangeController.Add(this);
        }

        protected override void OnCleanUp()
        {
            OverlayChangeController.Remove(this);
            base.OnCleanUp();
        }

        public void Sim200ms(float dt)
        {
            // update the update timer
            this.timeSinceLastUpdate += dt;
            if (this.timeSinceLastUpdate < this.updateInterval)
            {
                return;
            }

            this.UpdateLogicCircuit(null);
        }

        private void UpdateLogicCircuit(object data)
        {
            // sensor triggered. Reset timer.
            this.timeSinceLastUpdate = 0f;

            this.charge = 0f;
            this.capacity = 0f;

            ushort circuitID = Game.Instance.circuitManager.GetCircuitID(this.cell);

            if (circuitID != UInt16.MaxValue)
            {
                List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);

                foreach (Battery current in batteriesOnCircuit)
                {
                    if (current.isActiveAndEnabled)
                    {
                        this.charge += current.JoulesAvailable;
                        this.capacity += current.capacity;
                    }
                }
            }

            // set no batteries to "always on"
            int num = this.capacity == 0f ? 0 : Mathf.RoundToInt((this.charge * 100f)/ this.capacity);

            if (this.IsSwitchedOn)
            {
                if (num >= this.deactivateValue)
                {
                    base.Toggle();
                }
            }
            else if (num <= this.activateValue)
            {
                base.Toggle();
            }            
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
        }

        public void OnOverlayChange(HashedString mode)
        {
            animController.TintColour = BatterySensorConfig.BuildingColor();
        }

        //
        // IActivationRangeTarget methods
        //
        public float ActivateValue
        {
            get
            {
                return (float)this.deactivateValue;
            }
            set
            {
                this.deactivateValue = (int)value;
                this.UpdateLogicCircuit(null);
            }
        }

        public float DeactivateValue
        {
            get
            {
                return (float)this.activateValue;
            }
            set
            {
                this.activateValue = (int)value;
                this.UpdateLogicCircuit(null);
            }
        }

        public float MinValue
        {
            get
            {
                return 0f;
            }
        }

        public float MaxValue
        {
            get
            {
                return 100f;
            }
        }

        public bool UseWholeNumbers
        {
            get
            {
                return true;
            }
        }

        public string ActivateTooltip
        {
            get
            {
                return BUILDINGS.PREFABS.BATTERYSMART.DEACTIVATE_TOOLTIP;
            }
        }

        public string DeactivateTooltip
        {
            get
            {
                return BUILDINGS.PREFABS.BATTERYSMART.ACTIVATE_TOOLTIP;
            }
        }

        public string ActivationRangeTitleText
        {
            get
            {
                return BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_TITLE;
            }
        }

        public string ActivateSliderLabelText
        {
            get
            {
                return BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_DEACTIVATE;
            }
        }

        public string DeactivateSliderLabelText
        {
            get
            {
                return BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_ACTIVATE;
            }
        }
    }
}

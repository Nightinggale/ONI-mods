using KSerialization;
using NightLib;
using UnityEngine;
using STRINGS;
using System;


namespace HighFlowStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReservoirStorageSensor : Switch, IThresholdSwitch, ISaveLoadable, ISim200ms
    {
        [Serialize]
        public float threshold;

        [Serialize]
        public bool activateAbove = true;

        [MyCmpGet]
        internal Storage storage;

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;

        public static LogicPorts.Port MakePort()
        {
            LocString LOGIC_PORT = "Internal storage pressure";

            LocString LOGIC_PORT_ACTIVE = string.Concat(new string[]
            {
            "Sends an ",
            UI.FormatAsLink("Active", "LOGIC"),
            " when internal storage pressure enters the chosen range."
            });

            LocString LOGIC_PORT_INACTIVE = string.Concat(new string[]
            {
            "Sends a ",
            UI.FormatAsLink("Standby", "LOGIC"),
            " when internal storage pressure leaves the chosen range."
            });

            return LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), LOGIC_PORT, LOGIC_PORT_ACTIVE, LOGIC_PORT_INACTIVE, false);
        }

        public float Threshold
        {
            get
            {
                return this.threshold;
            }
            set
            {
                this.threshold = value;
            }
        }

        public bool ActivateAboveThreshold
        {
            get
            {
                return this.activateAbove;
            }
            set
            {
                this.activateAbove = value;
            }
        }

        public float CurrentValue
        {
            get
            {
                return this.storage.MassStored();
            }
        }

        public float RangeMin
        {
            get
            {
                return 0f;
            }
        }

        public float RangeMax
        {
            get
            {
                return storage.capacityKg;
            }
        }

        public LocString Title
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;
            }
        }

        public LocString ThresholdValueName
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;
            }
        }

        public string AboveToolTip
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;
            }
        }

        public string BelowToolTip
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;
            }
        }

        public ThresholdScreenLayoutType LayoutType
        {
            get
            {
                return ThresholdScreenLayoutType.SliderBar;
            }
        }

        public int IncrementScale
        {
            get
            {
                return 10;
            }
        }

        public NonLinearSlider.Range[] GetRanges
        {
            get
            {
                return NonLinearSlider.GetDefaultRange(storage.capacityKg);
            }
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe(GameHashes.CopySettings, new Action<object>(this.OnCopySettings));            
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            ReservoirStorageSensor component = gameObject.GetComponent<ReservoirStorageSensor>();
            if (component != null)
            {
                this.Threshold = component.Threshold;
                this.ActivateAboveThreshold = component.ActivateAboveThreshold;
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            base.OnToggle += new Action<bool>(this.OnSwitchToggled);
        }

        public void Sim200ms(float dt)
        {
            // Reading this.CurrentValue causes the storage to loop contents.
            // Cache the value to avoid looping more than once.
            float currentValue = this.CurrentValue;

            if (this.activateAbove)
            {
                // above with full storage is always on
                if (currentValue >= storage.capacityKg)
                {
                    if (!base.IsSwitchedOn)
                    {
                        this.Toggle();
                    }
                }
                else if ((!base.IsSwitchedOn && currentValue > this.threshold) || (base.IsSwitchedOn && currentValue <= this.threshold))
                {
                    this.Toggle();
                }
            }
            else if ((base.IsSwitchedOn && currentValue > this.threshold) || (!base.IsSwitchedOn && currentValue <= this.threshold))
            {
                this.Toggle();
            }
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
        }

        public float GetRangeMinInputField()
        {
            return 0f;
        }

        public float GetRangeMaxInputField()
        {
            return storage.capacityKg;
        }

        public string Format(float value, bool units)
        {
            GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Kilogram;
            return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, units, "{0:0.#}");
        }

        public float ProcessedSliderValue(float input)
        {
            input = Mathf.Round(input);
            return input;
        }

        public float ProcessedInputValue(float input)
        {
            return input;
        }

        public LocString ThresholdValueUnits()
        {
            return GameUtil.GetCurrentMassUnit(false);
        }
    }
}

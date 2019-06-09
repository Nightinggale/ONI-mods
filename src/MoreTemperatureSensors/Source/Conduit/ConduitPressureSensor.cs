using KSerialization;
using STRINGS;
using UnityEngine;
using NightLib.OnOverlayChange;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ConduitPressureSensor : ConduitThresholdSensor, IThresholdSwitch, IOverlayChangeEvent
    {
        private float currentValue;

        private float max;

        public Color32 color;
        
        public float GetRangeMinInputField()
        {
            return 0f;
        }

        public float GetRangeMaxInputField()
        {
            return this.max;
        }
        
        public string Format(float value, bool units)
        {
            GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Gram;
            return GameUtil.GetFormattedMass(value/1000, GameUtil.TimeSlice.None, massFormat, units, "{0:0.#}");
        }
        
        public float ProcessedSliderValue(float input)
        {
            return input;
        }

        public float ProcessedInputValue(float input)
        {
            return input;
        }
        
        public LocString ThresholdValueUnits()
        {
            return GameUtil.GetCurrentMassUnit(true);
        }
        
        public override float CurrentValue
        {
            get
            {
                return this.currentValue;
            }
        }
        
        public float RangeMax
        {
            get
            {
                return this.max;
            }
        }

        public float RangeMin
        {
            get
            {
                return 0f;
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
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;
            }
        }

        public string BelowToolTip
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;
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
                return 1;
            }
        }

        public NonLinearSlider.Range[] GetRanges
        {
            get
            {
                return NonLinearSlider.GetDefaultRange(this.RangeMax);
            }
        }

        protected override void ConduitUpdate(float dt)
        {
            int cell = Grid.PosToCell(this);
            ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
            this.currentValue = flowManager.GetContents(cell).mass * 1000;

            // spawn code should never toggle as it crashes on load
            if (dt < 0)
            {
                return;
            }

            if (this.activateAboveThreshold)
            {
                // Empty is always false
                if (this.currentValue <= 0f)
                {
                    if (base.IsSwitchedOn)
                    {
                        this.Toggle();
                    }
                    return;
                }

                // Full is always true
                if (this.currentValue >= this.max)
                {
                    if (!base.IsSwitchedOn)
                    {
                        this.Toggle();
                    }
                    return;
                }

                if ((this.currentValue > this.threshold && !base.IsSwitchedOn) || (this.currentValue <= this.threshold && base.IsSwitchedOn))
                {
                    this.Toggle();
                }
            }
            else if ((this.currentValue > this.threshold && base.IsSwitchedOn) || (this.currentValue <= this.threshold && !base.IsSwitchedOn))
            {
                this.Toggle();
            }
        }

        protected override void OnSpawn()
        {
            this.max = this.conduitType == ConduitType.Gas ? 1000f : 10000f;

            base.OnSpawn();
            OverlayChangeController.Add(this);

            // Update currentValue to avoid all displays from showing 0 g on load.
            // No functional change. It's purely a display issue.
            this.ConduitUpdate(-10);

            // Apply color
            this.OnOverlayChange("");
        }

        protected override void OnCleanUp()
        {
            OverlayChangeController.Remove(this);
            base.OnCleanUp();
        }

        public void OnOverlayChange(HashedString mode)
        {
            base.GetComponent<KBatchedAnimController>().TintColour = color;
        }
    }
}

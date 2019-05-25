using KSerialization;
using STRINGS;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SolidConduitPressureSensor : SolidConduitThresholdSensor, IThresholdSwitch //, IOverlayChangeEvent
    {
        private float currentValue;

        private readonly float max = 20000f;

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
            if (value < 1)
            {
                GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Gram;
                return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, units, "{0:0.#}");
            }
            else
            
            {
                GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Kilogram;
                return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, units, "{0:0.00}");
            }
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
                return this.currentValue / 1000;
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
                return "Mass Threshold";
            }
        }

        public LocString ThresholdValueName
        {
            get
            {
                return new LocString("", "NIGHTINGGALE.SENSORY.OVERLOADED.MASS");
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
            this.currentValue = 0f;

            int cell = Grid.PosToCell(this);
            SolidConduitFlow.ConduitContents contents = Game.Instance.solidConduitFlow.GetContents(cell);
            if (contents.pickupableHandle.IsValid())
            {
                Pickupable pickupable = Game.Instance.solidConduitFlow.GetPickupable(contents.pickupableHandle);
                this.currentValue = pickupable.TotalAmount * 1000f;
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
            base.OnSpawn();

            // Update currentValue to avoid all displays from showing 0 g on load.
            // No functional change. It's purely a display issue.
            this.ConduitUpdate(0);
        }
    }
}

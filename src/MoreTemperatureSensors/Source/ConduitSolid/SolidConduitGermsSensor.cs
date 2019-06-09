using KSerialization;
using STRINGS;
using UnityEngine;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SolidConduitGermsSensor : SolidConduitThresholdSensor, IThresholdSwitch
    {
        private float currentValue;

        private readonly float max = 100000f;

        public float GetRangeMinInputField()
        {
            return GameUtil.GetConvertedTemperature(this.RangeMin, false);
        }

        public float GetRangeMaxInputField()
        {
            return GameUtil.GetConvertedTemperature(this.RangeMax, false);
        }

        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedInt((float)((int)value), GameUtil.TimeSlice.None);
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
            return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_UNITS;
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
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;
            }
        }

        public LocString ThresholdValueName
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;
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
            SolidConduitFlow.ConduitContents contents = Game.Instance.solidConduitFlow.GetContents(cell);
            if (contents.pickupableHandle.IsValid())
            {
                Pickupable pickupable = Game.Instance.solidConduitFlow.GetPickupable(contents.pickupableHandle);
                PrimaryElement primaryElement = pickupable.GetComponent<PrimaryElement>();

                if (primaryElement != null)
                {
                    this.currentValue = primaryElement.DiseaseCount;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }

            // spawn code should never toggle as it crashes on load
            if (dt < 0)
            {
                return;
            }


            if (this.activateAboveThreshold)
            {
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

            // Update currentValue to avoid all displays from showing default value on load.
            // No functional change. It's purely a display issue.
            this.ConduitUpdate(-10);
        }
    }
}

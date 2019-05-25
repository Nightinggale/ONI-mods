using KSerialization;
using STRINGS;
using UnityEngine;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SolidConduitTemperatureSensor : SolidConduitThresholdSensor, IThresholdSwitch
    {
        private float currentValue = 280f;

        private readonly float max = 9999f + 273.15f;

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
            // call vanilla code rather than making up our own or copy paste
            // The reason is support for modded temperature display
            ConduitTemperatureSensor temp = new ConduitTemperatureSensor();
            return temp.Format(value, units);
        }

        public float ProcessedSliderValue(float input)
        {
            return Mathf.Round(input);
        }

        public float ProcessedInputValue(float input)
        {
            return GameUtil.GetTemperatureConvertedToKelvin(input);
        }

        public LocString ThresholdValueUnits()
        {
            // call vanilla code rather than making up our own or copy paste
            // The reason is support for modded temperature display
            ConduitTemperatureSensor temp = new ConduitTemperatureSensor();
            return temp.ThresholdValueUnits();
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
                return UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;
            }
        }

        public LocString ThresholdValueName
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
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
                return new NonLinearSlider.Range[]
                {
                new NonLinearSlider.Range(25f, 260f),
                new NonLinearSlider.Range(50f, 400f),
                new NonLinearSlider.Range(12f, 1500f),
                new NonLinearSlider.Range(13f, 10000f)
                };
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
                    this.currentValue = primaryElement.Temperature;
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
            this.ConduitUpdate(0);
        }
    }
}

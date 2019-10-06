using KSerialization;
using STRINGS;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SolidConduitFlowSensor : SolidConduitThresholdSensor, IThresholdSwitch
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
            if(!units) // Threshold Input
            {
                return string.Format("{0:0}", value);                
            }

            if (value < 1)
            {
                GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Gram;
                return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.PerSecond, massFormat, units, "{0:0.#}");
            }
            else
            
            {
                GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Kilogram;
                return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.PerSecond, massFormat, units, "{0:0.00}");
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
            return GameUtil.AddTimeSliceText(GameUtil.GetCurrentMassUnit(true),GameUtil.TimeSlice.PerSecond);
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
                return new LocString("Flow Threshold", "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.THRESHOLD");        
            }
        }

        public LocString ThresholdValueName
        {
            get
            {
                return new LocString("Flow", "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.NAME");
            }
        }

        public string AboveToolTip
        {
            get
            {
                return new LocString(string.Concat(new string[] { "Will send a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " if the ", UI.FormatAsKeyWord("Flow"), " is above <b>{0}</b> " }), "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.ABOVETOOLTIP");
            }
        }

        public string BelowToolTip
        {
            get
            {
                return new LocString(string.Concat(new string[] { "Will send a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " if the ", UI.FormatAsKeyWord("Flow"), " is below <b>{0}</b> " }), "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.BELOWTOOLTIP");
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
            // spawn code should never toggle as it crashes on load
            if (dt < 0)
            {
                return;
            }


            int cell = Grid.PosToCell(this);
            
            var conduit = Game.Instance.solidConduitFlow.GetConduit(cell);
            var lastFlow = conduit.GetLastFlowInfo(Game.Instance.solidConduitFlow);
            if (lastFlow.direction != SolidConduitFlow.FlowDirection.None)
            {
                var initial = conduit.GetInitialContents(Game.Instance.solidConduitFlow);

                if (initial.pickupableHandle.IsValid())
                { 
                    Pickupable pickupable = Game.Instance.solidConduitFlow.GetPickupable(initial.pickupableHandle);
                    this.currentValue = pickupable.TotalAmount * 1000f;
                }                
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
            this.ConduitUpdate(-10);
        }
    }
}

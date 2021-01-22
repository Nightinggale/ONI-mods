using KSerialization;
using STRINGS;
using UnityEngine;
using NightLib.OnOverlayChange;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ConduitFlowSensor : ConduitThresholdSensor, IThresholdSwitch, IOverlayChangeEvent
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
            return GameUtil.GetFormattedMass(value / 1000, GameUtil.TimeSlice.PerSecond, massFormat, units, "{0:0.#}");
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
            return GameUtil.AddTimeSliceText(GameUtil.GetCurrentMassUnit(true), GameUtil.TimeSlice.PerSecond);
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
        public LocString Title => new LocString("Flow Threshold", "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.THRESHOLD");
        public LocString ThresholdValueName => new LocString("Flow", "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.NAME");

        public string AboveToolTip => new LocString(string.Concat(new string[] { "Will send a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " if the ", UI.FormatAsKeyWord("Flow"), " is above <b>{0}</b> " }), "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.ABOVETOOLTIP");

        public string BelowToolTip => new LocString(string.Concat(new string[] { "Will send a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " if the ", UI.FormatAsKeyWord("Flow"), " is below <b>{0}</b> " }), "NIGHTINGGALE.SENSORY.OVERLOADED.FLOW.BELOWTOOLTIP");

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
            this.currentValue = 0;

            // spawn code should never toggle as it crashes on load
            if (dt < 0)
            {
                return;
            }

            int cell = Grid.PosToCell(this);
            ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
            if (flowManager.HasConduit(cell))
            {
                if (flowManager != null)
                {
                    var conduit = flowManager.GetConduit(cell);
                    var lastFlow = conduit.GetLastFlowInfo(flowManager);
                    if (lastFlow.direction != ConduitFlow.FlowDirections.None)
                    {
                        this.currentValue = (lastFlow.contents.mass) * 1000;
                    }
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

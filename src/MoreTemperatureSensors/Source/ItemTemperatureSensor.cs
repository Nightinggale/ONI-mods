using KSerialization;
using STRINGS;
using System;
using UnityEngine;


namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ItemTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
    {
        [Serialize]
        public float thresholdTemperature = 280f;

        [Serialize]
        public bool activateOnWarmerThan;

        private float lastThresholdTemperature;

        private bool lastActivateOnWarmerThan;

        public float minTemp;

        public float maxTemp = 373.15f;

        private float lastTemperatureLow;

        private float lastTemperatureHigh;

        private bool wasOn;

        private float timeSinceLastUpdate;

        private HandleVector<int>.Handle pickupablesChangedEntry;

        public float Threshold
        {
            get
            {
                return this.thresholdTemperature;
            }
            set
            {
                this.thresholdTemperature = value;
            }
        }

        public bool ActivateAboveThreshold
        {
            get
            {
                return this.activateOnWarmerThan;
            }
            set
            {
                this.activateOnWarmerThan = value;
            }
        }

        public float CurrentValue
        {
            get
            {
                return this.GetTemperature();
            }
        }

        public float RangeMin
        {
            get
            {
                return this.minTemp;
            }
        }

        public float RangeMax
        {
            get
            {
                return this.maxTemp;
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
    
        protected override void OnSpawn()
        {
            base.OnSpawn();
            int cell = this.NaturalBuildingCell();
            this.pickupablesChangedEntry = GameScenePartitioner.Instance.Add("ItemTemperatureSensor.PickupablesChanged", base.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupablesChanged));

            base.OnToggle += new Action<bool>(this.OnSwitchToggled);
            this.switchedOn = false;
            this.UpdateVisualState(true);
            this.Update();
            KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
            component.TintColour = ItemTemperatureSensorConfig.BuildingColor();
        }

        protected override void OnCleanUp()
        {
            GameScenePartitioner.Instance.Free(ref this.pickupablesChangedEntry);
            base.OnCleanUp();
        }

        private void OnPickupablesChanged(object data)
        {
            this.Update();
        }

        public void Sim200ms(float dt)
        {
            if (this.lastActivateOnWarmerThan == this.activateOnWarmerThan && this.lastThresholdTemperature == this.thresholdTemperature)
            {
                this.timeSinceLastUpdate += dt;
                if (this.timeSinceLastUpdate < 0.1)
                {
                    return;
                }
            }
            this.Update();
        }

        private void Update()
        {
            // update cache used to determine the next time Update() should trigger
            this.timeSinceLastUpdate = 0;
            this.lastActivateOnWarmerThan = this.activateOnWarmerThan;
            this.lastThresholdTemperature = this.thresholdTemperature;

            this.SetTemperature();

            if (this.activateOnWarmerThan)
            {
                if ((this.GetTemperature() > this.thresholdTemperature && !base.IsSwitchedOn) || (this.GetTemperature() < this.thresholdTemperature && base.IsSwitchedOn))
                {
                    this.Toggle();
                }
            }
            else if ((this.GetTemperature() > this.thresholdTemperature && base.IsSwitchedOn) || (this.GetTemperature() < this.thresholdTemperature && !base.IsSwitchedOn))

            {
                this.Toggle();
            }
        }


        private void SetTemperature()
        {
            this.lastTemperatureLow = this.maxTemp;
            this.lastTemperatureHigh = this.minTemp;

            int cell = this.NaturalBuildingCell();
            ListPool<ScenePartitionerEntry, LogicMassSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicMassSensor>.Allocate();
            GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cell).x, Grid.CellToXY(cell).y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
            for (int i = 0; i < pooledList.Count; i++)
            {
                Pickupable pickupable = pooledList[i].obj as Pickupable;
                if (!(pickupable == null))
                {
                    if (!pickupable.wasAbsorbed)
                    {
                        float temperature = pickupable.PrimaryElement.Temperature;

                        if (temperature > this.lastTemperatureHigh)
                        {
                            this.lastTemperatureHigh = temperature;
                        }

                        if (temperature < this.lastTemperatureLow)
                        {
                            this.lastTemperatureLow = temperature;
                        }
                    }
                }
            }
            pooledList.Recycle();
        }

        public float GetTemperature()
        {
            return this.activateOnWarmerThan ? this.lastTemperatureLow : this.lastTemperatureHigh;
        }
        
        private void OnSwitchToggled(bool toggled_on)
        {
            this.UpdateVisualState(false);
            base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
        }

        private void UpdateVisualState(bool force = false)
        {
            if (this.wasOn != this.switchedOn || force)
            {
                this.wasOn = this.switchedOn;
                
                KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
                component.Play((!this.switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
                component.Queue((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
            }
        }
        
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
            return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, true);
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
            LocString result = null;
            GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
            if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
            {
                if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
                {
                    if (temperatureUnit == GameUtil.TemperatureUnit.Kelvin)
                    {
                        result = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
                    }
                }
                else
                {
                    result = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
                }
            }
            else
            {
                result = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
            }
            return result;
        }
        
    }
}

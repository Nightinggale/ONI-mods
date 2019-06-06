using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using UnityEngine;
using NightLib;
using NightLib.OnOverlayChange;

namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ItemGermSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms, IOverlayChangeEvent
    {
        [Serialize, SerializeField]
        private int threshold;

        [Serialize, SerializeField]
        private bool activateAboveThreshold;

        private KBatchedAnimController animController;

        private bool wasOn;

        private const float rangeMin = 0f;

        private const float rangeMax = 100000f;

        private int diseaseCount;

        private float timeSinceLastUpdate;

        private float refreshInterval;

        private bool needsUpdating;

        private int itemCount;

        private int itemCountThreshold;

        private HandleVector<int>.Handle pickupablesChangedEntry;

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;

        public void OnOverlayChange(HashedString mode)
        {
            KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
            component.TintColour = ItemGermSensorConfig.BuildingColor();
        }


        private static readonly HashedString[] ON_ANIMS = new HashedString[]
        {
        "on_pre",
        "on_loop"
        };

        private static readonly HashedString[] OFF_ANIMS = new HashedString[]
        {
        "on_pst",
        "off"
        };

        private static readonly HashedString TINT_SYMBOL = "germs";

        public float Threshold
        {
            get
            {
                return this.threshold;
            }
            set
            {
                // slider was altered. Force update during next check.
                this.needsUpdating = true;
                // convert float to int. +0.5 is due to the fact that converting to int rounds down. It solves edge case rounding errors.
                this.threshold = (int)(value+0.5f);
            }
        }

        public bool ActivateAboveThreshold
        {
            get
            {
                return this.activateAboveThreshold;
            }
            set
            {
                // A button was pushed. Force update during next check.
                this.needsUpdating = true;
                this.activateAboveThreshold = value;
            }
        }

        public float CurrentValue
        {
            get
            {
                return this.diseaseCount;
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
                return 100000f;
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
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;
            }
        }

        public string BelowToolTip
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;
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
                return 100;
            }
        }

        public NonLinearSlider.Range[] GetRanges
        {
            get
            {
                return NonLinearSlider.GetDefaultRange(this.RangeMax);
            }
        }

        public LocString Title
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;
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
            LogicDiseaseSensor component = gameObject.GetComponent<LogicDiseaseSensor>();
            if (component != null)
            {
                this.Threshold = component.Threshold;
                this.ActivateAboveThreshold = component.ActivateAboveThreshold;
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.animController = base.GetComponent<KBatchedAnimController>();
            base.OnToggle += new Action<bool>(this.OnSwitchToggled);
            this.UpdateLogicCircuit();
            this.UpdateVisualState(true);
            this.wasOn = this.switchedOn;

            // subscribe to add/remove pickupables on the cell in question
            int cell = this.NaturalBuildingCell();
            this.pickupablesChangedEntry = GameScenePartitioner.Instance.Add("ItemTemperatureSensor.PickupablesChanged", base.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupablesChanged));

            // make sure the init state is correct if there are no items
            this.Toggle();

            this.Update();

            // load refresh interval from config file
            this.refreshInterval = MoreTemperatureSensorsConfig.Config.ItemSensorUpdateIntervalSeconds;
            if (this.refreshInterval < 0.15f)
            {
                this.refreshInterval = 0.15f;
            }

            this.itemCountThreshold = MoreTemperatureSensorsConfig.Config.ItemSensorItemCountFastThreshold;
            if (this.itemCountThreshold < 0)
            {
                this.itemCountThreshold = 0;
            }

            // Apply color
            this.OnOverlayChange("");
            OverlayChangeController.Add(this);
        }

        protected override void OnCleanUp()
        {
            GameScenePartitioner.Instance.Free(ref this.pickupablesChangedEntry);
            OverlayChangeController.Remove(this);
            base.OnCleanUp();
        }

        private void OnPickupablesChanged(object data)
        {
            this.Update();
        }

        public void Sim200ms(float dt)
        {
            if (!this.needsUpdating)
            {
                if (this.itemCount == 0)
                {
                    // wait until an item arrive. No need to allocate a temp list when we know it will loop 0 items and not change output.
                    return;
                }

                // use timer if item count exceeds the threshold.
                if (this.itemCount > this.itemCountThreshold)
                {
                    this.timeSinceLastUpdate += dt;
                    if (this.timeSinceLastUpdate < this.refreshInterval)
                    {
                        return;
                    }
                }
            }
            this.Update();
        }

        private void Update()
        {
            this.timeSinceLastUpdate = 0;
            this.needsUpdating = false;
            this.itemCount = 0;

            int currentValue = 0;

            // loop all items
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
                        this.itemCount++;
                        currentValue += pickupable.PrimaryElement.DiseaseCount;
                    }
                }
            }
            pooledList.Recycle();

            // update the cached count. Set to 0 if no items were found.
            this.diseaseCount = currentValue;

            // update the logic port output if needed.
            if (this.activateAboveThreshold)
            {
                if ((currentValue > this.threshold && !base.IsSwitchedOn) || (currentValue <= this.threshold && base.IsSwitchedOn))
                {
                    this.Toggle();
                }
            }
            else if ((currentValue > this.threshold && base.IsSwitchedOn) || (currentValue <= this.threshold && !base.IsSwitchedOn))
            {
                this.Toggle();
            }
            this.animController.SetSymbolVisiblity(ItemGermSensor.TINT_SYMBOL, currentValue > 0);
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            this.UpdateLogicCircuit();
            this.UpdateVisualState(false);
        }

        public float GetRangeMinInputField()
        {
            return 0f;
        }

        public float GetRangeMaxInputField()
        {
            return 100000f;
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

        private void UpdateLogicCircuit()
        {
            base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
        }

        private void UpdateVisualState(bool force = false)
        {
            if (this.wasOn != this.switchedOn || force)
            {
                this.wasOn = this.switchedOn;
                if (this.switchedOn)
                {
                    this.animController.Play(ItemGermSensor.ON_ANIMS, KAnim.PlayMode.Loop);
                    int i = Grid.PosToCell(this);
                    byte b = Grid.DiseaseIdx[i];
                    Color32 c = Color.white;
                    if (b >= 0 && b < Db.Get().Diseases.resources.Count)
                    {
                        Disease disease = Db.Get().Diseases.resources[b];
                        c = disease.overlayColour;
                    }
                    this.animController.SetSymbolTint(ItemGermSensor.TINT_SYMBOL, c);
                }
                else
                {
                    this.animController.Play(ItemGermSensor.OFF_ANIMS, KAnim.PlayMode.Once);
                }
            }
        }
    }
}

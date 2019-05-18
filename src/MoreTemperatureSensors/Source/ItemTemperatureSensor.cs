using KSerialization;
using System;
using NightLib.OnOverlayChange;
using NightLib;


namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ItemTemperatureSensor : LogicTemperatureSensor, IOverlayChangeEvent
    {

        private float lastThresholdTemperature;

        private bool lastActivateOnWarmerThan;

        private float lastTemperatureLow;

        private float lastTemperatureHigh;

        private float timeSinceLastUpdate;

        private float refreshInterval;

        private HandleVector<int>.Handle pickupablesChangedEntry;


        public void OnOverlayChange(HashedString mode)
        {
            KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
            component.TintColour = ItemTemperatureSensorConfig.BuildingColor();
        }
    
        protected override void OnSpawn()
        {
            base.OnSpawn();
            int cell = this.NaturalBuildingCell();
            this.pickupablesChangedEntry = GameScenePartitioner.Instance.Add("ItemTemperatureSensor.PickupablesChanged", base.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupablesChanged));
            this.Update();            

            // load refresh interval from config file
            this.refreshInterval = MoreTemperatureSensorsConfig.Config.ItemSensorUpdateIntervalSeconds;
            if (this.refreshInterval < 0.15f)
            {
                this.refreshInterval = 0.15f;
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
        
        new public void Sim200ms(float dt)
        {
            if (this.lastActivateOnWarmerThan == this.activateOnWarmerThan && this.lastThresholdTemperature == this.thresholdTemperature)
            {
                this.timeSinceLastUpdate += dt;
                if (this.timeSinceLastUpdate < this.refreshInterval)
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
            ReadPrivate.Set(typeof(LogicTemperatureSensor), this, "averageTemp", this.activateOnWarmerThan ? this.lastTemperatureLow : this.lastTemperatureHigh);
        }
    }
}

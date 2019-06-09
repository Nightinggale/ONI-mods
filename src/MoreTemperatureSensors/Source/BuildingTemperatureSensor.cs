using KSerialization;
using NightLib;
using NightLib.OnOverlayChange;
using UnityEngine;


namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class BuildingTemperatureSensor : LogicTemperatureSensor, IOverlayChangeEvent
    {

        private int cell;

        private bool isStarted = false;

        public void OnOverlayChange(HashedString mode)
        {
            KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
            component.TintColour = BuildingTemperatureSensorConfig.BuildingColor();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            this.cell = base.GetComponent<Building>().GetCellWithOffset(new CellOffset(0, -1));

            this.SetTemperature();

            // Apply color
            this.OnOverlayChange("");
            OverlayChangeController.Add(this);
        }

        protected override void OnCleanUp()
        {
            OverlayChangeController.Remove(this);
            base.OnCleanUp();
        }

        new public void Sim200ms(float dt)
        {
            this.isStarted = true;
            this.Update();
        }

        private void Update()
        {
            this.SetTemperature();

            // spawn code should never toggle as it crashes on load
            if (!isStarted)
            {
                return;
            }

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
            float temperature = 0;
            GameObject go = Grid.Objects[this.cell, (int)ObjectLayer.Building];

            if (go != null)
            {
                PrimaryElement element = go.GetComponent<PrimaryElement>();
                if (element != null)
                {
                    temperature = element.Temperature;
                }   
            }
            ReadPrivate.Set(typeof(LogicTemperatureSensor), this, "averageTemp", temperature);
        }
    }
}

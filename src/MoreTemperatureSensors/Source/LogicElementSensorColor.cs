using NightLib.OnOverlayChange;

namespace MoreTemperatureSensors
{
    class LogicElementSensorColor : LogicElementSensor, IOverlayChangeEvent
    {
        public void OnOverlayChange(HashedString mode)
        {
            KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
            component.TintColour = ModdedLogicElementSensorLiquidConfig.BuildingColor();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            // Apply color
            this.OnOverlayChange("");
            OverlayChangeController.Add(this);
        }

        protected override void OnCleanUp()
        {
            OverlayChangeController.Remove(this);
            base.OnCleanUp();
        }
    }
}

using System;
using UnityEngine;
using NightLib.OnOverlayChange;

namespace MoreTemperatureSensors
{

    public abstract class SolidConduitSensor : Switch, IOverlayChangeEvent
    {
        protected bool wasOn;

        public Color32 color = new Color32(175, 100, 0, 255);

        protected KBatchedAnimController animController;

        protected static readonly HashedString[] ON_ANIMS = new HashedString[]
        {
        "on_pre",
        "on"
        };

        protected static readonly HashedString[] OFF_ANIMS = new HashedString[]
        {
        "on_pst",
        "off"
        };

        protected abstract void ConduitUpdate(float dt);

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.animController = base.GetComponent<KBatchedAnimController>();
            base.OnToggle += new Action<bool>(this.OnSwitchToggled);
            this.UpdateLogicCircuit();
            this.UpdateVisualState(true);
            this.wasOn = this.switchedOn;
            Game.Instance.solidConduitFlow.AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);

            // Apply color if color if a color is set
            // 
            if (color.a != 0)
            {
                OverlayChangeController.Add(this);
                this.OnOverlayChange("");
            }
        }

        protected override void OnCleanUp()
        {
            Game.Instance.solidConduitFlow.RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
            base.OnCleanUp();

            if (color.a != 0)
            {
                OverlayChangeController.Remove(this);
            }
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            this.UpdateLogicCircuit();
            this.UpdateVisualState(false);
        }

        private void UpdateLogicCircuit()
        {
            base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
        }

        protected virtual void UpdateVisualState(bool force = false)
        {
            if (this.wasOn != this.switchedOn || force)
            {
                this.wasOn = this.switchedOn;
                if (this.switchedOn)
                {
                    this.animController.Play(SolidConduitSensor.ON_ANIMS, KAnim.PlayMode.Loop);
                }
                else
                {
                    this.animController.Play(SolidConduitSensor.OFF_ANIMS, KAnim.PlayMode.Once);
                }
            }
        }

        public void OnOverlayChange(HashedString mode)
        {
            base.GetComponent<KBatchedAnimController>().TintColour = this.color;
        }
    }
}

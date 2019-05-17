using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NightLib;

namespace MoreTemperatureSensors
{
    internal class LogicTemperatureSensorNoAnim : LogicTemperatureSensor
    {
        protected override void OnSpawn()
        {
            base.OnToggle += new Action<bool>(this.OnSwitchToggled);

            // set private structureTemperature in LogicTemperatureSensor
            ReadPrivate.Set(typeof(LogicTemperatureSensor), this, "structureTemperature", GameComps.StructureTemperatures.GetHandle(base.gameObject));
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            this.switchedOn = toggled_on;
            base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
        }
    }
}

using KSerialization;
using System;
using UnityEngine;
using STRINGS;


namespace MoreTemperatureSensors
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public abstract class SolidConduitThresholdSensor : SolidConduitSensor
    {
        [Serialize, SerializeField]
        protected float threshold;

        [Serialize, SerializeField]
        protected bool activateAboveThreshold = true;

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;

        public static string MakeEffect(string name)
        {
            return string.Concat(new string[]
            {
            "Becomes ",
            UI.FormatAsLink("Active", "LOGIC"),
            " or goes on ",
            UI.FormatAsLink("Standby", "LOGIC"),
            " when ",
            UI.FormatAsLink("Solid", "ELEMENTS_SOLID"),
            " ",
            name,
            " enters the chosen range."
            });
        }

        public static LogicPorts.Port MakePort(string name)
        {
            LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " " + name;

            LocString LOGIC_PORT_ACTIVE = string.Concat(new string[]
            {
            "Sends an ",
            UI.FormatAsLink("Active", "LOGIC"),
            " when ",
            UI.FormatAsLink("Liquid", "ELEMENTS_SOLID"),
            " ",
            name,
            " enters the chosen range."
            });

            LocString LOGIC_PORT_INACTIVE = string.Concat(new string[]
            {
            "Sends a ",
            UI.FormatAsLink("Standby", "LOGIC"),
            " when ",
            UI.FormatAsLink("Liquid", "ELEMENTS_SOLID"),
            " ",
            name,
            " leaves the chosen range."
            });

            return LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), LOGIC_PORT, LOGIC_PORT_ACTIVE, LOGIC_PORT_INACTIVE, false);
        }

        public abstract float CurrentValue
        {
            get;
        }

        public float Threshold
        {
            get
            {
                return this.threshold;
            }
            set
            {
                this.threshold = value;
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
                this.activateAboveThreshold = value;
            }
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            base.Subscribe((int)GameHashes.CopySettings, new Action<object>(this.OnCopySettings));

        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            SolidConduitThresholdSensor component = gameObject.GetComponent<SolidConduitThresholdSensor>();
            if (component != null)
            {
                this.Threshold = component.Threshold;
                this.ActivateAboveThreshold = component.ActivateAboveThreshold;
            }
        }        
    }
}

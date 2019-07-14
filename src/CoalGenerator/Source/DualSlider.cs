using System;
using UnityEngine;
using STRINGS;
using NightLib;
using KSerialization;

namespace Nightinggale.CoalGenerator
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class DualSlider : KMonoBehaviour, IActivationRangeTarget, ISaveLoadable
    {
        [MyCmpGet]
        private Storage storage;

        [MyCmpGet]
        private CoalManualDeliveryKG delivery;

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;

        [Serialize, SerializeField]
        public float fillUptoThreshold;

        [Serialize, SerializeField]
        public float refillThreshold;

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            DualSlider component = gameObject.GetComponent<DualSlider>();
            if (component != null)
            {
                this.fillUptoThreshold = component.fillUptoThreshold;
                this.refillThreshold = component.refillThreshold;
                this.UpdateDelivery();
            }
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe(GameHashes.CopySettings, new Action<object>(this.OnCopySettings));
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.UpdateDelivery();
        }

        private void UpdateDelivery()
        {
            this.delivery.UpdateCapacity(this.fillUptoThreshold, this.refillThreshold);
        }

        //
        // IActivationRangeTarget methods
        //
        public float ActivateValue
        {
            get
            {
                return this.fillUptoThreshold;
            }
            set
            {
                this.fillUptoThreshold = value;
                this.UpdateDelivery();
            }
        }

        public float DeactivateValue
        {
            get
            {
                return this.refillThreshold;
            }
            set
            {
                this.refillThreshold = value;
                this.UpdateDelivery();
            }
        }

        public float MinValue
        {
            get
            {
                return 1f;
            }
        }

        public float MaxValue
        {
            get
            {
                return this.storage.capacityKg;
            }
        }

        public bool UseWholeNumbers
        {
            get
            {
                return true;
            }
        }

        public string ActivateTooltip
        {
            get
            {
                return "Duplicants will fill up to this amount of " + delivery.RequestedItemTag.Name + ".\nLowering this number will reset current deliveries.";
            }
        }

        public string DeactivateTooltip
        {
            get
            {
                return "Duplicants will start to deliver " + delivery.RequestedItemTag.Name + " when storage falls below this threshold.";
            }
        }

        public string ActivationRangeTitleText
        {
            get
            {
                return BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_TITLE;
            }
        }

        public string ActivateSliderLabelText
        {
            get
            {
                return delivery.RequestedItemTag.Name + " Storage";
            }
        }

        public string DeactivateSliderLabelText
        {
            get
            {
                return "Refill Threshold";
            }
        }
    }
}

using KSerialization;
using STRINGS;
using System;
using UnityEngine;
using NightLib;

namespace Nightinggale.CoalGenerator
{

    [SerializationConfig(MemberSerialization.OptIn)]
    public class CoalManualDeliveryKG : KMonoBehaviour, ISim1000ms, ISaveLoadable
    {
        [MyCmpGet]
        private Operational operational;

        [SerializeField]
        private Storage storage;

        [SerializeField]
        public Tag requestedItemTag;

        [SerializeField]
        public float capacity = 100f;

        [SerializeField]
        public float refillMass = 10f;

        [SerializeField]
        public float minimumMass = 10f;

        [SerializeField]
        public FetchOrder2.OperationalRequirement operationalRequirement;

        [SerializeField]
        public bool allowPause;

        [SerializeField]
        private bool paused;

        [SerializeField]
        public HashedString choreTypeIDHash;

        [SerializeField]
        public Tag[] choreTags;

        [Serialize]
        private bool userPaused;

        [NonSerialized]
        public bool ShowStatusItem = true;

        private FetchList2 fetchList;

        private int onStorageChangeSubscription = -1;

        [SerializeField]
        public bool ignoresOperationStatus;

        public float Capacity
        {
            get
            {
                return this.capacity;
            }
        }

        public Tag RequestedItemTag
        {
            get
            {
                return this.requestedItemTag;
            }
            set
            {
                this.requestedItemTag = value;
                this.AbortDelivery("Requested Item Tag Changed");
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            DebugUtil.Assert(this.choreTypeIDHash.IsValid, "ManualDeliveryKG Must have a valid chore type specified!", base.name);
            this.Subscribe(GameHashes.RefreshUserMenu, new Action<object>(this.OnRefreshUserMenu));
            this.Subscribe(GameHashes.StatusChange, new Action<object>(this.OnRefreshUserMenu));
            this.Subscribe(GameHashes.OperationalChanged, new Action<object>(this.OnOperationalChanged));

            if (this.storage != null)
            {
                this.SetStorage(this.storage);
            }
            Prioritizable.AddRef(base.gameObject);
            if (this.userPaused && this.allowPause)
            {
                this.OnPause();
            }
        }

        protected override void OnCleanUp()
        {
            this.AbortDelivery("ManualDeliverKG destroyed");
            Prioritizable.RemoveRef(base.gameObject);
            base.OnCleanUp();
        }

        public void SetStorage(Storage storage)
        {
            if (this.storage != null)
            {
                this.storage.Unsubscribe(this.onStorageChangeSubscription);
                this.onStorageChangeSubscription = -1;
            }
            this.AbortDelivery("storage pointer changed");
            this.storage = storage;
            if (this.storage != null && base.isSpawned)
            {
                global::Debug.Assert(this.onStorageChangeSubscription == -1);
                this.onStorageChangeSubscription = this.storage.Subscribe((int)GameHashes.OnStorageChange, delegate (object eventData)
                {
                    this.OnStorageChanged(this.storage);
                });
            }
        }

        public void Pause(bool pause, string reason)
        {
            if (this.paused != pause)
            {
                this.paused = pause;
                if (pause)
                {
                    this.AbortDelivery(reason);
                }
            }
        }

        public void Sim1000ms(float dt)
        {
            this.UpdateDeliveryState();
        }

        [ContextMenu("UpdateDeliveryState")]
        public void UpdateDeliveryState()
        {
            if (!this.requestedItemTag.IsValid)
            {
                return;
            }
            if (this.storage == null)
            {
                return;
            }
            this.UpdateFetchList();
        }

        private void UpdateFetchList()
        {
            if (this.paused)
            {
                return;
            }
            if (this.fetchList != null && this.fetchList.IsComplete)
            {
                this.fetchList = null;
            }
            if (!this.OperationalRequirementsMet())
            {
                if (this.fetchList != null)
                {
                    this.fetchList.Cancel("Operational requirements");
                    this.fetchList = null;
                }
            }
            else if (this.fetchList == null)
            {
                float massAvailable = this.storage.GetMassAvailable(this.requestedItemTag);
                if (massAvailable < this.refillMass)
                {
                    float num = this.capacity - massAvailable;
                    num = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, num);
                    ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.choreTypeIDHash);
                    this.fetchList = new FetchList2(this.storage, byHash, this.choreTags);
                    this.fetchList.ShowStatusItem = this.ShowStatusItem;
                    this.fetchList.MinimumAmount[this.requestedItemTag] = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, this.minimumMass);
                    FetchList2 arg_12B_0 = this.fetchList;
                    Tag[] tags = new Tag[]
                    {
                    this.requestedItemTag
                    };
                    float amount = num;
                    arg_12B_0.Add(tags, null, null, amount, FetchOrder2.OperationalRequirement.None);
                    this.fetchList.Submit(null, false);
                }
            }
        }

        private bool OperationalRequirementsMet()
        {
            if (!this.ignoresOperationStatus && this.operational)
            {
                if (this.operationalRequirement == FetchOrder2.OperationalRequirement.Operational)
                {
                    return this.operational.IsOperational;
                }
                if (this.operationalRequirement == FetchOrder2.OperationalRequirement.Functional)
                {
                    return this.operational.IsFunctional;
                }
            }
            return true;
        }

        public void AbortDelivery(string reason)
        {
            if (this.fetchList != null)
            {
                FetchList2 fetchList = this.fetchList;
                this.fetchList = null;
                fetchList.Cancel(reason);
            }
        }

        private void OnStorageChanged(Storage storage)
        {
            if (storage == this.storage)
            {
                this.UpdateDeliveryState();
            }
        }

        private void OnPause()
        {
            this.userPaused = true;
            this.Pause(true, "Forbid manual delivery");
        }

        private void OnResume()
        {
            this.userPaused = false;
            this.Pause(false, "Allow manual delivery");
        }

        private void OnRefreshUserMenu(object data)
        {
            if (!this.allowPause)
            {
                return;
            }
            KIconButtonMenu.ButtonInfo arg_96_0;
            if (!this.paused)
            {
                string text = "action_move_to_storage";
                string text2 = UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME;
                System.Action on_click = new System.Action(this.OnPause);
                string text3 = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP;
                arg_96_0 = new KIconButtonMenu.ButtonInfo(text, text2, on_click, global::Action.NumActions, null, null, null, text3, true);
            }
            else
            {
                string text3 = "action_move_to_storage";
                string text2 = UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF;
                System.Action on_click = new System.Action(this.OnResume);
                string text = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF;
                arg_96_0 = new KIconButtonMenu.ButtonInfo(text3, text2, on_click, global::Action.NumActions, null, null, null, text, true);
            }
            KIconButtonMenu.ButtonInfo button = arg_96_0;
            Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
        }

        private void OnOperationalChanged(object data)
        {
            this.UpdateDeliveryState();
        }

        public void UpdateCapacity(float capacity, float refill)
        {
            if (this.capacity > capacity)
            {
                this.AbortDelivery("Threshold lowered");
            }

            this.capacity = capacity;
            this.refillMass = refill;
            this.UpdateDeliveryState();
        }
    }
}

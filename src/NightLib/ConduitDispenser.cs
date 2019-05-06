using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightLib
{
    internal class ConduitDispenser0 : CustomConduitDispenser { }
    internal class ConduitDispenser1 : CustomConduitDispenser { }
    internal class ConduitDispenser2 : CustomConduitDispenser { }
    internal class ConduitDispenser3 : CustomConduitDispenser { }
    internal class ConduitDispenser4 : CustomConduitDispenser { }
    internal class ConduitDispenser5 : CustomConduitDispenser { }

    [SerializationConfig(MemberSerialization.OptIn)]
    internal class CustomConduitDispenser : KMonoBehaviour, ISaveLoadable
    {
        [SerializeField]
        internal ConduitPortInfo portInfo;

        [SerializeField]
        internal ConduitType conduitType;

        [SerializeField]
        internal SimHashes[] elementFilter;

        [SerializeField]
        internal bool invertElementFilter;

        [SerializeField]
        internal bool alwaysDispense;

        private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

        private FlowUtilityNetwork.NetworkItem networkItem;

        [MyCmpReq]
        readonly private Operational operational;

        [MyCmpReq]
        internal Storage storage;

        private HandleVector<int>.Handle partitionerEntry;

        protected int utilityCell = -1;

        private int elementOutputOffset;

        internal ConduitType TypeOfConduit
        {
            get
            {
                return this.conduitType;
            }
        }

        internal ConduitFlow.ConduitContents ConduitContents
        {
            get
            {
                return this.GetConduitManager().GetContents(this.utilityCell);
            }
        }

        internal bool IsConnected
        {
            get
            {
                GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
                return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
            }
        }

        internal void SetConduitData(ConduitType type)
        {
            this.conduitType = type;
        }

        internal ConduitFlow GetConduitManager()
        {
            ConduitType conduitType = this.conduitType;
            if (conduitType == ConduitType.Gas)
            {
                return Game.Instance.gasConduitFlow;
            }
            if (conduitType != ConduitType.Liquid)
            {
                return null;
            }
            return Game.Instance.liquidConduitFlow;
        }

        private void OnConduitConnectionChanged(object data)
        {
            base.Trigger(-2094018600, this.IsConnected);
        }

        internal virtual CellOffset GetUtilityCellOffset()
        {
            return new CellOffset(0, 1);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            this.utilityCell = base.GetComponent<Building>().GetCellWithOffset(GetSecondaryConduitOffset());
            IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
            this.networkItem = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Source, this.utilityCell, base.gameObject);
            networkManager.AddToNetworks(this.utilityCell, this.networkItem, true);

            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
            this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Last);
            this.OnConduitConnectionChanged(null);
        }

        protected override void OnCleanUp()
        {
            IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
            networkManager.RemoveFromNetworks(this.utilityCell, this.networkItem, true);

            this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();
        }

        protected virtual void ConduitUpdate(float dt)
        {
            this.operational.SetFlag(CustomConduitDispenser.outputConduitFlag, this.IsConnected);
            if (this.operational.IsOperational || this.alwaysDispense)
            {
                PrimaryElement primaryElement = this.FindSuitableElement();
                if (primaryElement != null)
                {
                    primaryElement.KeepZeroMassObject = true;
                    ConduitFlow conduitManager = this.GetConduitManager();
                    float num = conduitManager.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                    if (num > 0f)
                    {
                        float num2 = num / primaryElement.Mass;
                        int num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
                        primaryElement.ModifyDiseaseCount(-num3, "CustomConduitDispenser.ConduitUpdate");
                        primaryElement.Mass -= num;
                        base.Trigger(-1697596308, primaryElement.gameObject);
                    }
                }
            }
        }

        protected virtual PrimaryElement FindSuitableElement()
        {
            List<GameObject> items = this.storage.items;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + this.elementOutputOffset) % count;
                PrimaryElement component = items[index].GetComponent<PrimaryElement>();
                if (component != null && component.Mass > 0f && ((this.conduitType != ConduitType.Liquid) ? component.Element.IsGas : component.Element.IsLiquid) && (this.elementFilter == null || this.elementFilter.Length == 0 || (!this.invertElementFilter && this.IsFilteredElement(component.ElementID)) || (this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
                {
                    this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
                    return component;
                }
            }
            return null;
        }

        private bool IsFilteredElement(SimHashes element)
        {
            for (int num = 0; num != this.elementFilter.Length; num++)
            {
                if (this.elementFilter[num] == element)
                {
                    return true;
                }
            }
            return false;
        }

        internal ConduitType GetSecondaryConduitType()
        {
            return this.portInfo.conduitType;
        }

        internal CellOffset GetSecondaryConduitOffset()
        {
            return this.portInfo.offset;
        }
    }
}

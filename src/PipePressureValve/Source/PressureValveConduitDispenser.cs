using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PipePressureValve
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class PressureValveConduitDispenser : KMonoBehaviour, ISaveLoadable
    {

        [SerializeField]
        public ConduitType conduitType;

        [SerializeField]
        public SimHashes[] elementFilter;

        [SerializeField]
        public bool invertElementFilter;

        [SerializeField]
        public bool alwaysDispense;

        [SerializeField]
        public float pipeCapacity;

        [SerializeField]
        public float outputThreshold;

        private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

        [MyCmpReq]
        private Operational operational;

        [MyCmpReq]
        public Storage storage;

        private HandleVector<int>.Handle partitionerEntry;

        private int utilityCell = -1;

        private int elementOutputOffset;

        public ConduitType TypeOfConduit
        {
            get
            {
                return this.conduitType;
            }
        }

        public ConduitFlow.ConduitContents ConduitContents
        {
            get
            {
                return this.GetConduitManager().GetContents(this.utilityCell);
            }
        }

        public bool IsConnected
        {
            get
            {
                GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
                return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
            }
        }

        public void SetConduitData(ConduitType type)
        {
            this.conduitType = type;
        }

        public ConduitFlow GetConduitManager()
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

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.utilityCell = base.GetComponent<Building>().GetUtilityOutputCell();
            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
            this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Last);
            this.OnConduitConnectionChanged(null);
        }

        protected override void OnCleanUp()
        {
            this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();
        }

        private void ConduitUpdate(float dt)
        {
            this.operational.SetFlag(PressureValveConduitDispenser.outputConduitFlag, this.IsConnected);
            if (this.operational.IsOperational || this.alwaysDispense)
            {
                PrimaryElement primaryElement = this.FindSuitableElement();
                this.operational.SetActive(primaryElement != null, false);

                if (primaryElement != null)
                {
                    primaryElement.KeepZeroMassObject = true;
                    ConduitFlow conduitManager = this.GetConduitManager();
                    float num = conduitManager.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                    if (num > 0f)
                    {
                        float num2 = num / primaryElement.Mass;
                        int num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
                        primaryElement.ModifyDiseaseCount(-num3, "ConduitDispenser.ConduitUpdate");
                        primaryElement.Mass -= num;
                        base.Trigger(-1697596308, primaryElement.gameObject);
                    }
                }
            }
        }

        private PrimaryElement FindSuitableElement()
        {
            List<GameObject> items = this.storage.items;
            int count = items.Count;

            int MaxAmountIndex = -1;
            float MaxAmount = -1f;

            for (int i = 0; i < count; i++)
            {
                int index = (i + this.elementOutputOffset) % count;
                PrimaryElement component = items[index].GetComponent<PrimaryElement>();
                if (component != null && component.Mass > 0f && ((this.conduitType != ConduitType.Liquid) ? component.Element.IsGas : component.Element.IsLiquid) && (this.elementFilter == null || this.elementFilter.Length == 0 || (!this.invertElementFilter && this.IsFilteredElement(component.ElementID)) || (this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
                {
                    if (component.Mass >= this.pipeCapacity)
                    {
                        
                        this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
                        return component;
                    }
                    if (component.Mass > MaxAmount)
                    {
                        MaxAmount = component.Mass;
                        MaxAmountIndex = index;
                    }
                }
            }
            if (storage.MassStored() > this.outputThreshold)
            {
                if (MaxAmountIndex != -1)
                {
                    PrimaryElement component = items[MaxAmountIndex].GetComponent<PrimaryElement>();
                    
                    this.elementOutputOffset = MaxAmountIndex;
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
    }
}

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using NightLib;

namespace Nightinggale.CoalGenerator
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class CoalEnergyGenerator : Generator, IEffectDescriptor
    {
        [DebuggerDisplay("{tag} -{consumptionRate} kg/s")]
        [Serializable]
        public struct InputItem
        {
            public Tag tag;

            public float consumptionRate;

            public float maxStoredMass;

            public InputItem(Tag tag, float consumption_rate, float max_stored_mass)
            {
                this.tag = tag;
                this.consumptionRate = consumption_rate;
                this.maxStoredMass = max_stored_mass;
            }
        }

        [DebuggerDisplay("{element} {creationRate} kg/s")]
        [Serializable]
        public struct OutputItem
        {
            public SimHashes element;

            public float creationRate;

            public bool store;

            public CellOffset emitOffset;

            public float minTemperature;

            public OutputItem(SimHashes element, float creation_rate, bool store, float min_temperature = 0f)
            {
                this = new CoalEnergyGenerator.OutputItem(element, creation_rate, store, CellOffset.none, min_temperature);
            }

            public OutputItem(SimHashes element, float creation_rate, bool store, CellOffset emit_offset, float min_temperature = 0f)
            {
                this.element = element;
                this.creationRate = creation_rate;
                this.store = store;
                this.emitOffset = emit_offset;
                this.minTemperature = min_temperature;
            }
        }

        [MyCmpGet]
        private Storage storage;

        public bool hasMeter = true;

        public Meter.Offset meterOffset = Meter.Offset.Infront;

        [SerializeField]
        public EnergyGenerator.Formula formula;

        private MeterController meter;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe(GameHashes.ActiveChanged, new Action<object>(this.OnActiveChanged));
        }

        protected void OnActiveChanged(object data)
        {
            bool isActive = ((Operational)data).IsActive;
            StatusItem status_item = (!isActive) ? Db.Get().BuildingStatusItems.GeneratorOffline : Db.Get().BuildingStatusItems.Wattage;
            base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, this);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (this.hasMeter)
            {
                this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", this.meterOffset, Grid.SceneLayer.NoLayer, new string[]
                {
                "meter_target",
                "meter_fill",
                "meter_frame",
                "meter_OL"
                });
            }
        }

        private bool IsConvertible(float dt)
        {
            bool flag = true;
            EnergyGenerator.InputItem[] inputs = this.formula.inputs;
            for (int i = 0; i < inputs.Length; i++)
            {
                EnergyGenerator.InputItem inputItem = inputs[i];
                GameObject gameObject = this.storage.FindFirst(inputItem.tag);
                if (gameObject != null)
                {
                    PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
                    float num = inputItem.consumptionRate * dt;
                    flag = (flag && component.Mass >= num);
                }
                else
                {
                    flag = false;
                }
                if (!flag)
                {
                    break;
                }
            }
            return flag;
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            
            if (this.hasMeter)
            {
                EnergyGenerator.InputItem inputItem = this.formula.inputs[0];
                float positionPercent = 0f;
                GameObject gameObject = this.storage.FindFirst(inputItem.tag);
                
                if (gameObject != null)
                {
                    PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
                    positionPercent = component.Mass / inputItem.maxStoredMass;
                }
                this.meter.SetPositionPercent(positionPercent);
                
            }
            ushort circuitID = base.CircuitID;
            this.operational.SetFlag(Generator.wireConnectedFlag, circuitID != 65535);
            
            bool value = false;
            
            if (this.operational.IsOperational)
            {
                if (this.formula.inputs != null)
                {
                    bool flag2 = this.IsConvertible(dt);
                    this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedResourceMass, !flag2, this.formula);
                    if (flag2)
                    {
                        EnergyGenerator.InputItem[] inputs = this.formula.inputs;
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            EnergyGenerator.InputItem inputItem2 = inputs[i];
                            float amount = inputItem2.consumptionRate * dt;
                            this.storage.ConsumeIgnoringDisease(inputItem2.tag, amount);
                        }
                        PrimaryElement component2 = base.GetComponent<PrimaryElement>();
                        EnergyGenerator.OutputItem[] outputs = this.formula.outputs;
                        for (int j = 0; j < outputs.Length; j++)
                        {
                            EnergyGenerator.OutputItem output = outputs[j];
                            this.Emit(output, dt, component2);
                        }
                        base.GenerateJoules(base.WattageRating * dt, false);
                        this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
                        value = true;
                    }
                }
            }
            this.operational.SetActive(value, false);
        }

        public List<Descriptor> RequirementDescriptors(BuildingDef def)
        {
            List<Descriptor> list = new List<Descriptor>();
            List<Descriptor> result;
            if (this.formula.inputs == null || this.formula.inputs.Length == 0)
            {
                result = list;
            }
            else
            {
                for (int i = 0; i < this.formula.inputs.Length; i++)
                {
                    EnergyGenerator.InputItem inputItem = this.formula.inputs[i];
                    string arg = inputItem.tag.ProperName();
                    Descriptor item = default(Descriptor);
                    item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
                    list.Add(item);
                }
                result = list;
            }
            return result;
        }

        public List<Descriptor> EffectDescriptors(BuildingDef def)
        {
            List<Descriptor> list = new List<Descriptor>();
            List<Descriptor> result;
            if (this.formula.outputs == null || this.formula.outputs.Length == 0)
            {
                result = list;
            }
            else
            {
                for (int i = 0; i < this.formula.outputs.Length; i++)
                {
                    EnergyGenerator.OutputItem outputItem = this.formula.outputs[i];
                    Element element = ElementLoader.FindElementByHash(outputItem.element);
                    string arg = element.tag.ProperName();
                    Descriptor item = default(Descriptor);
                    if (outputItem.minTemperature > 0f)
                    {
                        item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_MINORENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(outputItem.minTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINORENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedTemperature(outputItem.minTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect);
                    }
                    else
                    {
                        item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
                    }
                    list.Add(item);
                }
                result = list;
            }
            return result;
        }

        public List<Descriptor> GetDescriptors(BuildingDef def)
        {
            List<Descriptor> list = new List<Descriptor>();
            foreach (Descriptor current in this.RequirementDescriptors(def))
            {
                list.Add(current);
            }
            foreach (Descriptor current2 in this.EffectDescriptors(def))
            {
                list.Add(current2);
            }
            return list;
        }

        private void Emit(EnergyGenerator.OutputItem output, float dt, PrimaryElement root_pe)
        {
            Element element = ElementLoader.FindElementByHash(output.element);
            float num = output.creationRate * dt;
            if (output.store)
            {
                if (element.IsGas)
                {
                    this.storage.AddGasChunk(output.element, num, root_pe.Temperature, 255, 0, true, true);
                }
                else if (element.IsLiquid)
                {
                    this.storage.AddLiquid(output.element, num, root_pe.Temperature, 255, 0, true, true);
                }
                else
                {
                    GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), num, root_pe.Temperature, 255, 0, false, false, false);
                    this.storage.Store(go, true, false, true, false);
                }
            }
            else
            {
                int cell = Grid.PosToCell(base.transform.GetPosition());
                int num2 = Grid.OffsetCell(cell, output.emitOffset);
                float temperature = Mathf.Max(root_pe.Temperature, output.minTemperature);
                if (element.IsGas)
                {
                    SimMessages.ModifyMass(num2, num, 255, 0, CellEventLogger.Instance.EnergyGeneratorModifyMass, temperature, output.element);
                }
                else if (element.IsLiquid)
                {
                    int elementIndex = ElementLoader.GetElementIndex(output.element);
                    FallingWater.instance.AddParticle(num2, (byte)elementIndex, num, temperature, 255, 0, true, false, false, false);
                }
                else
                {
                    element.substance.SpawnResource(Grid.CellToPosCCC(num2, Grid.SceneLayer.Front), num, temperature, 255, 0, true, false, false);
                }
            }
        }
    }
}

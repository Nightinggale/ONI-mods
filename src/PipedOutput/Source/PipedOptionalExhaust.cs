using System;
using UnityEngine;
using NightLib;

namespace Nightinggale.PipedOutput
{
    [SkipSaveFileSerialization]
    public class PipedOptionalExhaust : KMonoBehaviour, ISim200ms
    {
        [SerializeField]
        internal PipedDispenser dispenser;

        [SerializeField]
        public SimHashes elementHash;

        [SerializeField]
        public Tag elementTag;

        [SerializeField]
        public float capacity;

        [MyCmpAdd]
        private Storage storage;

        private static readonly Operational.Flag outputFlag = new Operational.Flag("output_blocked", Operational.Flag.Type.Functional);

        [MyCmpReq]
        readonly private Operational operational;

        public void OnPrefab()
        {
            //this.elementTag = this.elementHash.CreateTag();
        }

        public void Sim200ms(float dt)
        {
            GameObject storedObject = this.storage.FindFirst(elementTag);
            PrimaryElement component = null;
            float stored = 0f;
            if (storedObject != null)
            {
                component = storedObject.GetComponent<PrimaryElement>();
                stored = component.Mass;
            }

            if (stored > 0f && dispenser != null)
            {
                if (!dispenser.IsConnected)
                {
                    Element element = component.Element;
                    float temperature = component.Temperature;
                    int disease = component.DiseaseCount;

                    int outputCell = dispenser.UtilityCell;

                    if (element.IsGas)
                    {
                        Console.WriteLine("Outputing " + stored.ToString() + " of " + elementHash.CreateTag() + " at " + outputCell.ToString());
                        SimMessages.ModifyMass(outputCell, stored, 255, disease, CellEventLogger.Instance.EnergyGeneratorModifyMass, temperature, elementHash);
                    }
                    else if (element.IsLiquid)
                    {
                        int elementIndex = ElementLoader.GetElementIndex(elementHash);
                        FallingWater.instance.AddParticle(outputCell, (byte)elementIndex, stored, temperature, 255, disease, true, false, false, false);
                    }
                    else
                    {
                        element.substance.SpawnResource(Grid.CellToPosCCC(outputCell, Grid.SceneLayer.Front), stored, temperature, 255, disease, true, false, false);
                    }
                    storage.ConsumeIgnoringDisease(storedObject);
                    stored = 0f;
                }


            }
            bool overfilled = stored >= capacity;

            this.operational.SetFlag(outputFlag, !overfilled);
        }
    }
}

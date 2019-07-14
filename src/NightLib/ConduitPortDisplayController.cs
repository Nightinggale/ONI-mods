using UnityEngine;
using System.Collections.Generic;

namespace NightLib
{
    [SkipSaveFileSerialization]
    internal class PortDisplayController : KMonoBehaviour
    {
        [SerializeField]
        private HashedString lastMode = OverlayModes.None.ID;

        [SerializeField]
        private List<PortDisplay2> gasOverlay = new List<PortDisplay2>();

        [SerializeField]
        private List<PortDisplay2> liquidOverlay = new List<PortDisplay2>();

        [SerializeField]
        private List<PortDisplay2> solidOverlay = new List<PortDisplay2>();

        [SerializeField]
        private bool hasPower = false;

        [SerializeField]
        private bool hasGas = false;

        [SerializeField]
        private bool hasLiquid = false;

        [SerializeField]
        private bool hasSolid = false;

        [SerializeField]
        private bool hasDisease = false;

        public void AssignPort(GameObject go, DisplayConduitPortInfo port)
        {
            PortDisplay2 portDisplay = go.AddComponent<PortDisplay2>();
            portDisplay.AssignPort(port);

            switch (port.type)
            {
                case ConduitType.Gas:
                    this.gasOverlay.Add(portDisplay);
                    break;
                case ConduitType.Liquid:
                    this.liquidOverlay.Add(portDisplay);
                    break;
                case ConduitType.Solid:
                    this.solidOverlay.Add(portDisplay);
                    break;
            }
        }

        public void Init(GameObject go)
        {
            string ID = go.GetComponent<KPrefabID>().PrefabTag.Name;

            // 
            go.AddOrGet<BuildingCellVisualizer>();

            NightLib.PortDisplayDrawing.ConduitDisplayPortPatches.DrawPorts.AddBuilding(ID);

            // cache which overlays the vanilla code can use. Used to skip vanilla code when nothing is drawn anyway.
            BuildingDef def = go.GetComponent<BuildingDef>();
            if (def != null)
            {
                this.hasPower   = def.CheckRequiresPowerInput()  || def.CheckRequiresPowerOutput();
                this.hasGas     = def.CheckRequiresGasInput()    || def.CheckRequiresGasOutput();
                this.hasLiquid  = def.CheckRequiresLiquidInput() || def.CheckRequiresLiquidOutput();
                this.hasSolid   = def.CheckRequiresSolidInput()  || def.CheckRequiresSolidOutput();
                this.hasDisease = def.DiseaseCellVisName != null;
            }
        }

        public bool Draw(BuildingCellVisualizer __instance, HashedString mode, GameObject go)
        {
            bool isNewMode = mode != this.lastMode;

            if (isNewMode)
            {
                this.ClearPorts();
                this.lastMode = mode;
            }

            foreach (PortDisplay2 port in this.GetPorts(mode))
            {
                port.Draw(go, __instance, isNewMode);
            }

            // return true if the vanilla port drawing code has anything to do
            return isNewMode
                || (this.hasPower   && mode == OverlayModes.Power.ID)
                || (this.hasGas     && mode == OverlayModes.GasConduits.ID)
                || (this.hasLiquid  && mode == OverlayModes.LiquidConduits.ID)
                || (this.hasSolid   && mode == OverlayModes.SolidConveyor.ID)
                || (this.hasDisease && mode == OverlayModes.Disease.ID)
            ;
        }

        private void ClearPorts()
        {
            foreach (PortDisplay2 port in this.GetPorts(this.lastMode))
            {
                port.DisableIcons();
            }
        }

        private List<PortDisplay2> GetPorts(HashedString mode)
        {
            if (mode == OverlayModes.GasConduits   .ID) return this.gasOverlay;
            if (mode == OverlayModes.LiquidConduits.ID) return this.liquidOverlay;
            if (mode == OverlayModes.SolidConveyor .ID) return this.solidOverlay;

            return new List<PortDisplay2>();
        }
    }
}
